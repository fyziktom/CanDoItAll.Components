using System.Diagnostics;
using System.Text.Json;
using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;

namespace CanDoItAll.Components.WebGlRunLib.Tests;

public sealed class WebGlRunPerformanceBudgetTests
{
    private const int ObjectCount = 500;
    private const int FrameCount = 120;
    private const int StagesPerFrame = 4;
    private const int MotionsPerStage = 8;
    private const int RecreateIterations = 2;

    private static readonly JsonSerializerOptions MetricsJsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    [Fact]
    public async Task Long_run_playback_budget_outputs_machine_readable_metrics_and_enforces_thresholds()
    {
        WebGlRunDocument document = CreateStressRunDocument();
        var applier = new BudgetRecordingFrameApplier();
        var runner = new WebGlRunDocumentRunner(applier);

        long allocatedBefore = GC.GetTotalAllocatedBytes(precise: true);
        var stopwatch = Stopwatch.StartNew();
        for (int iteration = 0; iteration < RecreateIterations; iteration++)
        {
            WebGlRunExecutionResult load = await runner.LoadAsync(document);
            Assert.True(load.Succeeded, string.Join(Environment.NewLine, load.Errors));

            WebGlRunExecutionResult first = await runner.ApplyCurrentFrameAsync();
            Assert.True(first.Succeeded, string.Join(Environment.NewLine, first.Errors));

            for (int frameIndex = 1; frameIndex < FrameCount; frameIndex++)
            {
                WebGlRunExecutionResult step = await runner.StepForwardAsync();
                Assert.True(step.Succeeded, string.Join(Environment.NewLine, step.Errors));
            }
        }

        stopwatch.Stop();
        long allocatedBytes = Math.Max(0, GC.GetTotalAllocatedBytes(precise: true) - allocatedBefore);
        WebGlRunBudgetMetrics metrics = BuildMetrics(stopwatch.Elapsed, allocatedBytes, applier);
        WriteMetrics(metrics);

        Assert.All(metrics.Budgets, budget =>
            Assert.True(budget.Passed, $"{budget.Name} exceeded threshold. Metrics: {JsonSerializer.Serialize(metrics, MetricsJsonOptions)}"));
    }

    private static WebGlRunBudgetMetrics BuildMetrics(
        TimeSpan elapsed,
        long allocatedBytes,
        BudgetRecordingFrameApplier applier)
    {
        const int maxElapsedMs = 20_000;
        const double maxAverageFrameApplyMs = 75;
        const long maxAllocatedBytes = 256L * 1024 * 1024;
        int expectedAppliedFrames = RecreateIterations * FrameCount;
        int expectedStageCount = expectedAppliedFrames * StagesPerFrame;
        int expectedMotionCount = expectedStageCount * MotionsPerStage;
        double averageFrameApplyMs = applier.AppliedFrameCount == 0
            ? 0
            : elapsed.TotalMilliseconds / applier.AppliedFrameCount;

        return new()
        {
            SchemaVersion = "webglrun-performance-budget/v1",
            Scenario = "generic-webglrun-large-playback",
            ObjectCount = ObjectCount,
            FrameCount = FrameCount,
            StagesPerFrame = StagesPerFrame,
            MotionsPerStage = MotionsPerStage,
            RecreateIterations = RecreateIterations,
            InitialSceneImportCount = applier.InitialSceneImportCount,
            AppliedFrameCount = applier.AppliedFrameCount,
            AppliedStageCount = applier.AppliedStageCount,
            AppliedMotionCount = applier.AppliedMotionCount,
            AppliedPatchCount = applier.AppliedPatchCount,
            MaxStagesPerFrame = applier.MaxStagesPerFrame,
            MaxMotionsPerFrame = applier.MaxMotionsPerFrame,
            MaxPatchesPerFrame = applier.MaxPatchesPerFrame,
            InteropCallsAvoided = applier.InteropCallsAvoided,
            DroppedDuplicateMotionCount = applier.DroppedDuplicateMotionCount,
            PreservedOrderedDuplicateMotionCount = applier.PreservedOrderedDuplicateMotionCount,
            ElapsedMs = Math.Round(elapsed.TotalMilliseconds, 2),
            AverageFrameApplyMs = Math.Round(averageFrameApplyMs, 4),
            AllocatedBytes = allocatedBytes,
            MetricsPath = ResolveMetricsPath(),
            Budgets =
            [
                new("elapsedMs", elapsed.TotalMilliseconds, maxElapsedMs, elapsed.TotalMilliseconds <= maxElapsedMs),
                new("averageFrameApplyMs", averageFrameApplyMs, maxAverageFrameApplyMs, averageFrameApplyMs <= maxAverageFrameApplyMs),
                new("allocatedBytes", allocatedBytes, maxAllocatedBytes, allocatedBytes <= maxAllocatedBytes),
                new("appliedFrameCount", applier.AppliedFrameCount, expectedAppliedFrames, applier.AppliedFrameCount == expectedAppliedFrames),
                new("appliedStageCount", applier.AppliedStageCount, expectedStageCount, applier.AppliedStageCount == expectedStageCount),
                new("appliedMotionCount", applier.AppliedMotionCount, expectedMotionCount, applier.AppliedMotionCount == expectedMotionCount),
                new("initialSceneImportCount", applier.InitialSceneImportCount, RecreateIterations, applier.InitialSceneImportCount >= RecreateIterations),
                new("interopCallsAvoided", applier.InteropCallsAvoided, expectedMotionCount, applier.InteropCallsAvoided >= expectedMotionCount)
            ]
        };
    }

    private static WebGlRunDocument CreateStressRunDocument()
    {
        var document = new WebGlRunDocument
        {
            RunId = new("run.generic.performance-budget"),
            InitialScene = new WebGlSceneDocument
            {
                Scene = new()
                {
                    SceneId = "scene.generic.performance-budget",
                    Revision = 1,
                    UiState = new() { Revision = 1 },
                    Objects =
                    [
                        .. Enumerable.Range(0, ObjectCount).Select(index => new WebGlSceneObject
                        {
                            Id = ObjectId(index),
                            Kind = "generic",
                            Family = "budget",
                            Position = new WebGlVector3(index % 50, 0, index / 50),
                            Color = "#38bdf8"
                        })
                    ]
                }
            },
            Timeline = { FrameRate = 30 }
        };

        for (int frameIndex = 0; frameIndex < FrameCount; frameIndex++)
        {
            var frame = new WebGlRunFrame
            {
                Index = frameIndex,
                TimeSeconds = frameIndex / 30d,
                Metadata =
                {
                    ["sourceFrameId"] = $"source.frame.{frameIndex:000}",
                    ["batchingPolicy"] = WebGlSceneBatchingPolicies.Parallel,
                    ["orderingMode"] = BatchOrderingMode.CoalesceIndependent.ToString()
                }
            };

            for (int stageIndex = 0; stageIndex < StagesPerFrame; stageIndex++)
            {
                frame.Stages.Add(CreateStressStage(frameIndex, stageIndex));
            }

            document.Timeline.Frames.Add(frame);
        }

        return document;
    }

    private static WebGlRunActionStage CreateStressStage(int frameIndex, int stageIndex)
    {
        int patchedObjectIndex = ObjectIndex(frameIndex, stageIndex, 0);
        var stage = new WebGlRunActionStage
        {
            StageId = $"frame.{frameIndex:000}.stage.{stageIndex:00}",
            StageIndex = stageIndex,
            OrderIndex = stageIndex,
            SequenceId = $"sequence.{frameIndex:000}.{stageIndex:00}",
            ExecutionPolicy = WebGlRunStageExecutionPolicies.Parallel,
            CoalescingScope = WebGlRunCoalescingScopes.Frame,
            BarrierPolicy = WebGlSceneStageBarrierPolicies.WaitForObjectMotions,
            BarrierObjectIds = { ObjectId(patchedObjectIndex) },
            Metadata =
            {
                ["sourceStageId"] = $"source.frame.{frameIndex:000}.stage.{stageIndex:00}",
                ["orderingMode"] = BatchOrderingMode.CoalesceIndependent.ToString()
            },
            ScenePatches =
            {
                new()
                {
                    Id = $"patch.{frameIndex:000}.{stageIndex:00}",
                    Patch = new()
                    {
                        SceneId = "scene.generic.performance-budget",
                        ObjectPatches =
                        {
                            new()
                            {
                                ObjectId = ObjectId(patchedObjectIndex),
                                Color = stageIndex % 2 == 0 ? "#22c55e" : "#0ea5e9",
                                Metadata = new Dictionary<string, string>
                                {
                                    ["budgetFrame"] = frameIndex.ToString(System.Globalization.CultureInfo.InvariantCulture),
                                    ["budgetStage"] = stageIndex.ToString(System.Globalization.CultureInfo.InvariantCulture)
                                }
                            }
                        }
                    }
                }
            }
        };

        for (int motionIndex = 0; motionIndex < MotionsPerStage; motionIndex++)
        {
            int objectIndex = ObjectIndex(frameIndex, stageIndex, motionIndex);
            stage.Motions.Add(new()
            {
                MotionId = $"motion.{frameIndex:000}.{stageIndex:00}.{motionIndex:00}",
                ObjectId = ObjectId(objectIndex),
                TargetPosition = new WebGlVector3(
                    objectIndex % 50,
                    (frameIndex % 12) * 0.05,
                    objectIndex / 50 + stageIndex * 0.02),
                DurationSeconds = 0.05,
                QueueMode = WebGlMotionQueueModes.Append
            });
        }

        return stage;
    }

    private static int ObjectIndex(int frameIndex, int stageIndex, int motionIndex)
        => (frameIndex * 17 + stageIndex * MotionsPerStage + motionIndex) % ObjectCount;

    private static string ObjectId(int index)
        => $"object.{index:0000}";

    private static void WriteMetrics(WebGlRunBudgetMetrics metrics)
    {
        string metricsPath = ResolveMetricsPath();
        Directory.CreateDirectory(Path.GetDirectoryName(metricsPath) ?? AppContext.BaseDirectory);
        File.WriteAllText(metricsPath, JsonSerializer.Serialize(metrics, MetricsJsonOptions));
    }

    private static string ResolveMetricsPath()
        => Environment.GetEnvironmentVariable("CDA_WEBGLRUN_BUDGET_METRICS_PATH") is { Length: > 0 } configured
            ? configured
            : Path.Combine(AppContext.BaseDirectory, "webglrun-performance-budget-metrics.json");

    private sealed class BudgetRecordingFrameApplier : IWebGlRunFrameApplier, IWebGlRunInitialSceneApplier
    {
        public int InitialSceneImportCount { get; private set; }

        public int AppliedFrameCount { get; private set; }

        public int AppliedStageCount { get; private set; }

        public int AppliedMotionCount { get; private set; }

        public int AppliedPatchCount { get; private set; }

        public int MaxStagesPerFrame { get; private set; }

        public int MaxMotionsPerFrame { get; private set; }

        public int MaxPatchesPerFrame { get; private set; }

        public int InteropCallsAvoided { get; private set; }

        public int DroppedDuplicateMotionCount { get; private set; }

        public int PreservedOrderedDuplicateMotionCount { get; private set; }

        public ValueTask ApplyInitialSceneAsync(WebGlSceneDocument sceneDocument, CancellationToken cancellationToken = default)
        {
            InitialSceneImportCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask ApplyAsync(WebGlRunFrameApplyResult frame, CancellationToken cancellationToken = default)
        {
            AppliedFrameCount++;
            AppliedStageCount += frame.CommandBatch.Stages.Count;
            int framePatchCount = frame.CommandBatch.Patches.Count + frame.CommandBatch.Stages.Sum(static stage => stage.Patches.Count);
            int frameMotionCount = frame.CommandBatch.Motions.Count + frame.CommandBatch.Stages.Sum(static stage => stage.Motions.Count);
            AppliedPatchCount += framePatchCount;
            AppliedMotionCount += frameMotionCount;
            MaxStagesPerFrame = Math.Max(MaxStagesPerFrame, frame.CommandBatch.Stages.Count);
            MaxMotionsPerFrame = Math.Max(MaxMotionsPerFrame, frameMotionCount);
            MaxPatchesPerFrame = Math.Max(MaxPatchesPerFrame, framePatchCount);
            InteropCallsAvoided += ReadMetric(frame.CommandBatch.Metadata, "interopCallsAvoided");
            DroppedDuplicateMotionCount += ReadMetric(frame.CommandBatch.Metadata, "droppedDuplicateMotionCount");
            PreservedOrderedDuplicateMotionCount += ReadMetric(frame.CommandBatch.Metadata, "preservedOrderedDuplicateMotionCount");
            return ValueTask.CompletedTask;
        }

        private static int ReadMetric(IReadOnlyDictionary<string, string> metadata, string key)
            => metadata.TryGetValue(key, out string? value) &&
               int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out int parsed)
                ? parsed
                : 0;
    }

    private sealed class WebGlRunBudgetMetrics
    {
        public string SchemaVersion { get; set; } = string.Empty;

        public string Scenario { get; set; } = string.Empty;

        public int ObjectCount { get; set; }

        public int FrameCount { get; set; }

        public int StagesPerFrame { get; set; }

        public int MotionsPerStage { get; set; }

        public int RecreateIterations { get; set; }

        public int InitialSceneImportCount { get; set; }

        public int AppliedFrameCount { get; set; }

        public int AppliedStageCount { get; set; }

        public int AppliedMotionCount { get; set; }

        public int AppliedPatchCount { get; set; }

        public int MaxStagesPerFrame { get; set; }

        public int MaxMotionsPerFrame { get; set; }

        public int MaxPatchesPerFrame { get; set; }

        public int InteropCallsAvoided { get; set; }

        public int DroppedDuplicateMotionCount { get; set; }

        public int PreservedOrderedDuplicateMotionCount { get; set; }

        public double ElapsedMs { get; set; }

        public double AverageFrameApplyMs { get; set; }

        public long AllocatedBytes { get; set; }

        public string MetricsPath { get; set; } = string.Empty;

        public List<WebGlRunBudgetAssertion> Budgets { get; set; } = [];
    }

    private sealed record WebGlRunBudgetAssertion(string Name, double Actual, double Threshold, bool Passed);
}
