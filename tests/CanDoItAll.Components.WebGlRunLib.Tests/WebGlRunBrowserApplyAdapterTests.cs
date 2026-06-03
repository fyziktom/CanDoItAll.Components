using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;

namespace CanDoItAll.Components.WebGlRunLib.Tests;

public sealed class WebGlRunBrowserApplyAdapterTests
{
    [Fact]
    public async Task Adapter_applies_frame_to_runtime_and_returns_counts_and_snapshot()
    {
        var runtime = new RecordingBrowserRuntime
        {
            Diagnostics = new()
            {
                CurrentCommandBatchId = "run-frame:7",
                CurrentCommandStageId = "stage.symbol",
                QueuedCommandStageCount = 1,
                CommandStageBarrierPolicy = WebGlSceneStageBarrierPolicies.WaitForObjectMotions,
                CommandStageBarrierTarget = "actor",
                CommandStageBarrierBlockers = ["active:actor"],
                CommandStageBarrierObjectIds = ["actor"],
                ActiveMotionCount = 1,
                ActiveMotionIds = ["motion.actor"],
                QueuedMotionCount = 1,
                QueuedMotionIds = ["motion.actor.next"],
                CommandStageRecentJournalEntries =
                [
                    new()
                    {
                        Sequence = 9,
                        BatchId = "run-frame:7",
                        StageId = "stage.move",
                        EventKind = "stage-apply",
                        Status = "applied",
                        BarrierPolicy = WebGlSceneStageBarrierPolicies.WaitForObjectMotions
                    }
                ]
            }
        };
        var initialScene = new WebGlSceneDocument
        {
            Scene = new WebGlSceneModel { SceneId = "scene.initial" }
        };
        var adapter = new WebGlRunBrowserApplyAdapter(runtime, initialScene);
        var frame = new WebGlRunFrameApplyResult
        {
            FrameIndex = 7,
            RequiresSceneReset = true,
            CommandBatch = new()
            {
                BatchId = "run-frame:7",
                Patches = { new WebGlScenePatch { SceneId = "scene.initial" } },
                Motions = { Motion("motion.top", "actor") },
                Stages =
                {
                    new()
                    {
                        StageId = "stage.symbol",
                        BarrierPolicy = WebGlSceneStageBarrierPolicies.WaitForObjectMotions,
                        BarrierObjectIds = { "actor" },
                        Patches = { new WebGlScenePatch { SceneId = "scene.initial" } },
                        Motions = { Motion("motion.actor", "actor") }
                    }
                }
            }
        };

        WebGlRunBrowserApplyResult result = await adapter.ApplyAsync(frame);

        Assert.True(result.Success);
        Assert.True(result.AppliedInitialScene);
        Assert.Single(runtime.ImportedScenes);
        Assert.Single(runtime.AppliedBatches);
        Assert.Equal(1, result.AppliedStageCount);
        Assert.Equal(2, result.AppliedPatchCount);
        Assert.Equal(2, result.AppliedMotionCount);
        Assert.Equal("run-frame:7", result.RuntimeSnapshot.CurrentCommandBatchId);
        Assert.Equal("stage.symbol", result.RuntimeSnapshot.CurrentStageId);
        Assert.Equal(["stage.symbol"], result.RuntimeSnapshot.CurrentStageIds.ToArray());
        Assert.Equal(["motion.actor"], result.RuntimeSnapshot.ActiveMotionIds.ToArray());
        Assert.Equal(["motion.actor.next"], result.RuntimeSnapshot.QueuedMotionIds.ToArray());
        Assert.Equal(WebGlSceneStageBarrierPolicies.WaitForObjectMotions, result.RuntimeSnapshot.StageBarrier.Policy);
        Assert.Contains("active:actor", result.RuntimeSnapshot.StageBarrier.Blockers);
        Assert.Single(result.RuntimeSnapshot.CommandJournalTail);
    }

    [Fact]
    public async Task Adapter_reports_large_frame_batch_diagnostics_without_per_object_apply_loop()
    {
        var runtime = new RecordingBrowserRuntime
        {
            Diagnostics = new()
            {
                CurrentCommandBatchId = "run-frame:9",
                BatchCommandCount = 32,
                BatchStageCount = 8,
                CommandCountBeforeNormalization = 40,
                CommandCountAfterNormalization = 32,
                InteropCallsAvoided = 31
            }
        };
        var adapter = new WebGlRunBrowserApplyAdapter(runtime);
        var frame = new WebGlRunFrameApplyResult
        {
            FrameIndex = 9,
            CommandBatch = new()
            {
                BatchId = "run-frame:9",
                Stages =
                [
                    .. Enumerable.Range(0, 8).Select(index => new WebGlSceneCommandBatchStage
                    {
                        StageId = $"stage.parallel.{index}",
                        BatchingPolicy = WebGlSceneBatchingPolicies.Parallel,
                        Motions = { Motion($"motion.actor.{index}", $"actor.{index}") }
                    })
                ]
            }
        };

        WebGlRunBrowserApplyResult result = await adapter.ApplyAsync(frame);

        Assert.True(result.Success);
        Assert.Single(runtime.AppliedBatches);
        Assert.Equal(8, runtime.AppliedBatches[0].Stages.Count);
        Assert.Equal("32", result.RuntimeSnapshot.Diagnostics["batchCommandCount"]);
        Assert.Equal("8", result.RuntimeSnapshot.Diagnostics["batchStageCount"]);
        Assert.Equal("40", result.RuntimeSnapshot.Diagnostics["commandCountBeforeNormalization"]);
        Assert.Equal("32", result.RuntimeSnapshot.Diagnostics["commandCountAfterNormalization"]);
        Assert.Equal("31", result.RuntimeSnapshot.Diagnostics["interopCallsAvoided"]);
    }

    [Fact]
    public async Task Adapter_reset_imports_initial_scene_runtime_options()
    {
        var runtime = new RecordingBrowserRuntime();
        var initialScene = new WebGlSceneDocument
        {
            Scene = new WebGlSceneModel { SceneId = "scene.runtime-options" },
            RuntimeOptions = new()
            {
                RenderMode = WebGlRenderModes.Continuous,
                RuntimeKey = "external-runtime"
            }
        };
        var adapter = new WebGlRunBrowserApplyAdapter(runtime, initialScene);

        WebGlRunBrowserApplyResult result = await adapter.ApplyAsync(new WebGlRunFrameApplyResult
        {
            FrameIndex = 1,
            RequiresSceneReset = true,
            CommandBatch = new() { BatchId = "run-frame:1" }
        });

        WebGlSceneDocument importedScene = Assert.Single(runtime.ImportedScenes);
        Assert.True(result.Success);
        Assert.Equal(WebGlRenderModes.Continuous, importedScene.RuntimeOptions.RenderMode);
        Assert.Equal("external-runtime", importedScene.RuntimeOptions.RuntimeKey);
        Assert.Equal(WebGlRenderModes.Continuous, initialScene.RuntimeOptions.RenderMode);
        Assert.DoesNotContain(
            result.Warnings,
            warning => warning.Contains("runtime options are external", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Adapter_reports_runtime_failure_in_typed_result_and_snapshot()
    {
        var runtime = new RecordingBrowserRuntime
        {
            BatchResult = new()
            {
                Success = false,
                CommandId = "batch.failure",
                Errors = { "runtime rejected stage" }
            },
            Diagnostics = new()
            {
                LastError = "runtime rejected stage",
                LastStageError = "stage failed"
            }
        };
        var adapter = new WebGlRunBrowserApplyAdapter(runtime);

        WebGlRunBrowserApplyResult result = await adapter.ApplyAsync(new WebGlRunFrameApplyResult
        {
            FrameIndex = 3,
            CommandBatch = new() { BatchId = "run-frame:3" }
        });

        Assert.False(result.Success);
        Assert.Equal(WebGlRunBrowserApplyFailureReasons.BatchFailed, result.FailureReason);
        Assert.Contains("runtime rejected stage", result.Errors);
        Assert.Contains("stage failed", result.RuntimeSnapshot.RuntimeErrors);
        Assert.Single(runtime.AppliedBatches);
    }

    [Fact]
    public async Task Adapter_fails_reset_without_initial_scene()
    {
        var runtime = new RecordingBrowserRuntime();
        var adapter = new WebGlRunBrowserApplyAdapter(runtime);

        WebGlRunBrowserApplyResult result = await adapter.ApplyAsync(new WebGlRunFrameApplyResult
        {
            FrameIndex = 0,
            RequiresSceneReset = true,
            CommandBatch = new() { BatchId = "run-frame:0" }
        });

        Assert.False(result.Success);
        Assert.Equal(WebGlRunBrowserApplyFailureReasons.ResetFailed, result.FailureReason);
        Assert.Contains(result.Errors, error => error.Contains("no initial scene", StringComparison.OrdinalIgnoreCase));
        Assert.Empty(runtime.ImportedScenes);
        Assert.Empty(runtime.AppliedBatches);
    }

    [Fact]
    public async Task Adapter_fails_reset_import_failure_without_applying_batch()
    {
        var runtime = new RecordingBrowserRuntime
        {
            ImportResult = new()
            {
                Success = false,
                CommandId = "import.scene",
                Errors = { "import failed" }
            }
        };
        var adapter = new WebGlRunBrowserApplyAdapter(runtime, new() { Scene = new() { SceneId = "scene.reset" } });

        WebGlRunBrowserApplyResult result = await adapter.ApplyAsync(new WebGlRunFrameApplyResult
        {
            FrameIndex = 4,
            RequiresSceneReset = true,
            CommandBatch = new() { BatchId = "run-frame:4" }
        });

        Assert.False(result.Success);
        Assert.Equal(WebGlRunBrowserApplyFailureReasons.ResetFailed, result.FailureReason);
        Assert.Contains("import failed", result.Errors);
        Assert.Single(runtime.ImportedScenes);
        Assert.Empty(runtime.AppliedBatches);
    }

    [Fact]
    public async Task Adapter_does_not_apply_frame_result_that_already_contains_errors()
    {
        var runtime = new RecordingBrowserRuntime();
        var adapter = new WebGlRunBrowserApplyAdapter(runtime);

        WebGlRunBrowserApplyResult result = await adapter.ApplyAsync(new WebGlRunFrameApplyResult
        {
            FrameIndex = 5,
            Errors = { "mixed direct and staged commands" },
            CommandBatch = new()
            {
                BatchId = "run-frame:5",
                Stages = { new() { StageId = "stage.should-not-apply" } }
            }
        });

        Assert.False(result.Success);
        Assert.Equal(WebGlRunBrowserApplyFailureReasons.PreApplyValidationFailed, result.FailureReason);
        Assert.Empty(runtime.ImportedScenes);
        Assert.Empty(runtime.AppliedBatches);
    }

    [Fact]
    public async Task Adapter_rejects_legacy_playback_apply_for_multiframe_results()
    {
        var runtime = new RecordingBrowserRuntime();
        var adapter = new WebGlRunBrowserApplyAdapter(runtime, new() { Scene = new() { SceneId = "scene.initial" } });
        var playback = new WebGlRunPlaybackResult
        {
            TargetFrameIndex = 2,
            RequiresSceneReset = true,
            FramesToApply =
            {
                Frame(1, "stage.first"),
                Frame(2, "stage.second")
            }
        };

        WebGlRunBrowserApplyResult result = await adapter.ApplyAsync(playback);

        Assert.False(result.Success);
        Assert.Equal(WebGlRunBrowserApplyFailureReasons.MultiFramePlaybackRequiresExplicitApply, result.FailureReason);
        Assert.Empty(runtime.ImportedScenes);
        Assert.Empty(runtime.AppliedBatches);
    }

    [Fact]
    public async Task Adapter_apply_playback_applies_reset_once_and_frames_in_order()
    {
        var runtime = new RecordingBrowserRuntime();
        var adapter = new WebGlRunBrowserApplyAdapter(runtime, new() { Scene = new() { SceneId = "scene.initial" } });
        var playback = new WebGlRunPlaybackResult
        {
            RequestedCommand = WebGlRunPlaybackCommandKinds.Seek,
            TargetFrameIndex = 2,
            RequiresSceneReset = true,
            FramesToApply =
            {
                Frame(1, "stage.first"),
                Frame(2, "stage.second")
            }
        };

        WebGlRunBrowserPlaybackApplyResult result = await adapter.ApplyPlaybackAsync(playback);

        Assert.True(result.Success);
        Assert.True(result.AppliedInitialScene);
        Assert.Single(runtime.ImportedScenes);
        Assert.Equal(2, runtime.AppliedBatches.Count);
        Assert.Equal([1, 2], result.FrameResults.Select(frame => frame.FrameIndex).ToArray());
        Assert.Equal(["stage.first", "stage.second"], runtime.AppliedBatches.Select(batch => batch.Stages.Single().StageId).ToArray());
    }

    [Fact]
    public async Task Adapter_apply_playback_stops_on_first_failed_frame()
    {
        var runtime = new RecordingBrowserRuntime();
        runtime.BatchResults.Enqueue(new() { Success = true });
        runtime.BatchResults.Enqueue(new()
        {
            Success = false,
            CommandId = "run-frame:2",
            Errors = { "frame two failed" }
        });
        var adapter = new WebGlRunBrowserApplyAdapter(runtime);
        var playback = new WebGlRunPlaybackResult
        {
            RequestedCommand = WebGlRunPlaybackCommandKinds.Seek,
            TargetFrameIndex = 3,
            FramesToApply =
            {
                Frame(1, "stage.first"),
                Frame(2, "stage.second"),
                Frame(3, "stage.third")
            }
        };

        WebGlRunBrowserPlaybackApplyResult result = await adapter.ApplyPlaybackAsync(playback);

        Assert.False(result.Success);
        Assert.Equal(2, result.FailedFrameIndex);
        Assert.Equal(WebGlRunBrowserApplyFailureReasons.BatchFailed, result.FailureReason);
        Assert.Equal(2, runtime.AppliedBatches.Count);
        Assert.Equal([1, 2], result.FrameResults.Select(frame => frame.FrameIndex).ToArray());
        Assert.Contains("frame two failed", result.Errors);
    }

    [Fact]
    public async Task Adapter_bounds_runtime_snapshot_lists()
    {
        var runtime = new RecordingBrowserRuntime
        {
            Diagnostics = new()
            {
                ActiveMotionIds = [.. Enumerable.Range(0, 150).Select(index => $"active.{index}")],
                QueuedMotionIds = [.. Enumerable.Range(0, 150).Select(index => $"queued.{index}")],
                CommandStageRecentJournalEntries =
                [
                    .. Enumerable.Range(0, 20).Select(index => new WebGlRuntimeCommandStageJournalEntry
                    {
                        Sequence = index + 1,
                        StageId = $"stage.{index + 1}",
                        EventKind = "stage-apply",
                        Status = "applied"
                    })
                ]
            }
        };
        var adapter = new WebGlRunBrowserApplyAdapter(runtime);

        WebGlRunBrowserApplyResult result = await adapter.ApplyAsync(new WebGlRunFrameApplyResult
        {
            FrameIndex = 2,
            CommandBatch = new() { BatchId = "run-frame:2" }
        });

        Assert.Equal(100, result.RuntimeSnapshot.ActiveMotionIds.Count);
        Assert.Equal(100, result.RuntimeSnapshot.QueuedMotionIds.Count);
        Assert.Equal(12, result.RuntimeSnapshot.CommandJournalTail.Count);
        Assert.Equal("stage.9", result.RuntimeSnapshot.CommandJournalTail[0].StageId);
        Assert.Equal("stage.20", result.RuntimeSnapshot.CommandJournalTail[^1].StageId);
    }

    private static WebGlObjectMotionCommand Motion(string motionId, string objectId)
        => new()
        {
            MotionId = motionId,
            ObjectId = objectId,
            TargetPosition = new WebGlVector3(1, 0, 0),
            DurationSeconds = 1
        };

    private static WebGlRunFrame Frame(long index, string stageId)
        => new()
        {
            Index = index,
            TimeSeconds = index,
            Stages =
            {
                new()
                {
                    StageId = stageId,
                    Motions = { Motion($"motion.{stageId}", "actor") }
                }
            }
        };

    private sealed class RecordingBrowserRuntime : IWebGlRunBrowserRuntime
    {
        public List<WebGlSceneDocument> ImportedScenes { get; } = [];

        public List<WebGlSceneCommandBatch> AppliedBatches { get; } = [];

        public WebGlSceneCommandResult ImportResult { get; set; } = new() { Success = true };

        public WebGlSceneCommandBatchResult BatchResult { get; set; } = new() { Success = true };

        public Queue<WebGlSceneCommandBatchResult> BatchResults { get; } = [];

        public WebGlRuntimeDiagnostics Diagnostics { get; set; } = new();

        public ValueTask<WebGlSceneCommandResult?> ImportSceneAsync(
            WebGlSceneDocument sceneDocument,
            CancellationToken cancellationToken = default)
        {
            ImportedScenes.Add(sceneDocument);
            return ValueTask.FromResult<WebGlSceneCommandResult?>(ImportResult);
        }

        public ValueTask<WebGlSceneCommandBatchResult?> ApplyCommandBatchAsync(
            WebGlSceneCommandBatch batch,
            CancellationToken cancellationToken = default)
        {
            AppliedBatches.Add(batch);
            return ValueTask.FromResult<WebGlSceneCommandBatchResult?>(BatchResults.Count > 0 ? BatchResults.Dequeue() : BatchResult);
        }

        public ValueTask<WebGlRuntimeDiagnostics?> GetDiagnosticsAsync(CancellationToken cancellationToken = default)
            => ValueTask.FromResult<WebGlRuntimeDiagnostics?>(Diagnostics);
    }
}
