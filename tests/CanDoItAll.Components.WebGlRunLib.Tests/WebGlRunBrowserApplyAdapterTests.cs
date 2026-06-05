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
        Assert.Single(runtime.AppliedAndWaitBatches);
        Assert.Empty(runtime.AppliedBatches);
        Assert.True(result.CommandBatchResult?.Settled);
        Assert.Equal(WebGlSceneCommandLifecycleStates.Settled, result.CommandBatchResult?.LifecycleState);
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
    public async Task Adapter_default_apply_uses_command_batch_and_wait_for_settled_result()
    {
        var runtime = new RecordingBrowserRuntime
        {
            BatchResult = new()
            {
                Success = true,
                CommandId = "run-frame:6",
                LifecycleState = WebGlSceneCommandLifecycleStates.Settled,
                Settled = true,
                Diagnostics =
                {
                    ["runtimeIdle"] = "true",
                    ["runtimeIdleTimedOut"] = "false",
                    ["runtimeIdleBlockers"] = ""
                }
            }
        };
        var adapter = new WebGlRunBrowserApplyAdapter(runtime);

        WebGlRunBrowserApplyResult result = await adapter.ApplyAsync(new WebGlRunFrameApplyResult
        {
            FrameIndex = 6,
            CommandBatch = new() { BatchId = "run-frame:6" }
        });

        Assert.True(result.Success);
        Assert.Single(runtime.AppliedAndWaitBatches);
        Assert.Empty(runtime.AppliedBatches);
        Assert.Single(runtime.IdleWaits);
        Assert.Equal("playback-apply:command-batch:6", runtime.IdleWaits[0].Reason);
        Assert.Equal(WebGlSceneCommandLifecycleStates.Settled, result.CommandBatchResult?.LifecycleState);
        Assert.True(result.CommandBatchResult?.Settled);
        Assert.Equal(WebGlSceneCommandLifecycleStates.Settled, result.RuntimeSnapshot.Diagnostics["commandLifecycleState"]);
        Assert.Equal(bool.TrueString, result.RuntimeSnapshot.Diagnostics["commandSettled"]);
        Assert.Equal("true", result.RuntimeSnapshot.Diagnostics["runtimeIdle"]);
    }

    [Fact]
    public async Task Adapter_configured_idle_policy_preserves_scheduled_result_and_idle_blockers()
    {
        var runtime = new RecordingBrowserRuntime
        {
            BatchResult = new()
            {
                Success = true,
                CommandId = "run-frame:8",
                LifecycleState = WebGlSceneCommandLifecycleStates.Scheduled,
                Settled = false,
                Diagnostics =
                {
                    ["runtimeIdle"] = "false",
                    ["runtimeIdleTimedOut"] = "false",
                    ["runtimeIdleBlockers"] = "motion:active:1"
                }
            },
            IdleResult = new()
            {
                Success = true,
                Idle = true,
                Reason = "test",
                Diagnostics = new()
            }
        };
        var adapter = new WebGlRunBrowserApplyAdapter(
            runtime,
            applyOptions: new()
            {
                RuntimeIdleWaitPolicy = WebGlRunRuntimeIdleWaitPolicies.AfterPlayback,
                RuntimeIdle = new() { TimeoutMs = 321, PollIntervalMs = 9, Reason = "SB04-configured-idle" }
            });

        WebGlRunBrowserApplyResult result = await adapter.ApplyAsync(new WebGlRunFrameApplyResult
        {
            FrameIndex = 8,
            CommandBatch = new() { BatchId = "run-frame:8" }
        });

        Assert.True(result.Success);
        Assert.Single(runtime.AppliedBatches);
        Assert.Empty(runtime.AppliedAndWaitBatches);
        Assert.Single(runtime.IdleWaits);
        Assert.Equal(321, runtime.IdleWaits[0].TimeoutMs);
        Assert.Equal(WebGlSceneCommandLifecycleStates.Scheduled, result.CommandBatchResult?.LifecycleState);
        Assert.False(result.CommandBatchResult?.Settled);
        Assert.NotNull(result.RuntimeIdleResult);
        Assert.Equal(WebGlSceneCommandLifecycleStates.Scheduled, result.RuntimeSnapshot.Diagnostics["commandLifecycleState"]);
        Assert.Equal(bool.FalseString, result.RuntimeSnapshot.Diagnostics["commandSettled"]);
        Assert.Equal("motion:active:1", result.RuntimeSnapshot.Diagnostics["runtimeIdleBlockers"]);
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
        Assert.Single(runtime.AppliedAndWaitBatches);
        Assert.Empty(runtime.AppliedBatches);
        Assert.Equal(8, runtime.AppliedAndWaitBatches[0].Stages.Count);
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
        Assert.Single(runtime.AppliedAndWaitBatches);
        Assert.Empty(runtime.AppliedBatches);
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
        Assert.Empty(runtime.AppliedAndWaitBatches);
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
        Assert.Empty(runtime.AppliedAndWaitBatches);
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
        Assert.Empty(runtime.AppliedAndWaitBatches);
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
        Assert.Empty(runtime.AppliedAndWaitBatches);
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
        Assert.Equal(2, runtime.AppliedAndWaitBatches.Count);
        Assert.Empty(runtime.AppliedBatches);
        Assert.Equal([1, 2], result.FrameResults.Select(frame => frame.FrameIndex).ToArray());
        Assert.Equal(["stage.first", "stage.second"], runtime.AppliedAndWaitBatches.Select(batch => batch.Stages.Single().StageId).ToArray());
    }

    [Fact]
    public async Task Adapter_apply_playback_waits_for_idle_after_each_frame_when_policy_requires_it()
    {
        var runtime = new RecordingBrowserRuntime
        {
            IdleResult = new()
            {
                Success = true,
                Idle = true,
                Reason = "test",
                Diagnostics = new()
            }
        };
        var adapter = new WebGlRunBrowserApplyAdapter(
            runtime,
            new() { Scene = new() { SceneId = "scene.initial" } },
            new()
            {
                RuntimeIdleWaitPolicy = WebGlRunRuntimeIdleWaitPolicies.AfterEachFrame,
                RuntimeIdle = new() { TimeoutMs = 123, PollIntervalMs = 7, Reason = "SB02-runtime-idle" }
            });
        var playback = new WebGlRunPlaybackResult
        {
            RequestedCommand = WebGlRunPlaybackCommandKinds.Seek,
            TargetFrameIndex = 2,
            RequiresSceneReset = true,
            ReplayMode = WebGlRunBrowserReplayModes.AbsoluteReplay,
            FramesToApply =
            {
                Frame(1, "stage.first"),
                Frame(2, "stage.second")
            }
        };

        WebGlRunBrowserPlaybackApplyResult result = await adapter.ApplyPlaybackAsync(playback);

        Assert.True(result.Success);
        Assert.Equal(WebGlRunRuntimeIdleWaitPolicies.AfterEachFrame, result.RuntimeIdleWaitPolicy);
        Assert.Equal(WebGlRunBrowserReplayModes.AbsoluteReplay, result.ReplayMode);
        Assert.Equal(2, runtime.IdleWaits.Count);
        Assert.All(runtime.IdleWaits, wait => Assert.Equal(123, wait.TimeoutMs));
        Assert.All(result.FrameResults, frame => Assert.NotNull(frame.RuntimeIdleResult));
    }

    [Fact]
    public async Task Adapter_apply_playback_fails_when_runtime_idle_times_out()
    {
        var runtime = new RecordingBrowserRuntime
        {
            IdleResult = new()
            {
                Success = false,
                Idle = false,
                TimedOut = true,
                TimeoutMs = 10,
                Blockers = { "command-stage:barrier" },
                Diagnostics = new()
                {
                    CommandStageBarrierPolicy = WebGlSceneStageBarrierPolicies.WaitForEvent,
                    CommandStageBarrierBlockers = { "event:manual" }
                }
            }
        };
        var adapter = new WebGlRunBrowserApplyAdapter(
            runtime,
            applyOptions: new()
            {
                RuntimeIdleWaitPolicy = WebGlRunRuntimeIdleWaitPolicies.AfterPlayback,
                RuntimeIdle = new() { TimeoutMs = 10, PollIntervalMs = 1, Reason = "SB02-timeout" }
            });
        var playback = new WebGlRunPlaybackResult
        {
            TargetFrameIndex = 1,
            FramesToApply = { Frame(1, "stage.blocked") }
        };

        WebGlRunBrowserPlaybackApplyResult result = await adapter.ApplyPlaybackAsync(playback);

        Assert.False(result.Success);
        Assert.Equal(WebGlRunBrowserApplyFailureReasons.RuntimeIdleTimeout, result.FailureReason);
        Assert.Single(runtime.IdleWaits);
        Assert.Contains("command-stage:barrier", result.Errors.Single());
        Assert.NotNull(result.FailureSnapshot);
        Assert.Equal(WebGlSceneStageBarrierPolicies.WaitForEvent, result.FailureSnapshot.StageBarrier.Policy);
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
        Assert.Equal(1, result.LastAppliedFrameIndex);
        Assert.Equal(3, result.TargetFrameIndex);
        Assert.Equal(WebGlRunBrowserApplyFailureReasons.BatchFailed, result.FailureReason);
        Assert.Equal(WebGlRunBrowserPlaybackTransactionPolicies.StopOnFirstFailure, result.TransactionPolicy);
        Assert.NotNull(result.FailureSnapshot);
        Assert.Equal("3", result.FailureSnapshot.Diagnostics["targetFrameIndex"]);
        Assert.Equal("1", result.FailureSnapshot.Diagnostics["lastAppliedFrameIndex"]);
        Assert.Equal("2", result.FailureSnapshot.Diagnostics["failedFrameIndex"]);
        Assert.Equal(2, runtime.AppliedAndWaitBatches.Count);
        Assert.Empty(runtime.AppliedBatches);
        Assert.Equal([1, 2], result.FrameResults.Select(frame => frame.FrameIndex).ToArray());
        Assert.Contains("frame two failed", result.Errors);
    }

    [Fact]
    public async Task Adapter_apply_playback_reports_cancellation_and_does_not_apply_later_frames()
    {
        using var cancellation = new CancellationTokenSource();
        var runtime = new RecordingBrowserRuntime
        {
            OnApplyBatch = batch =>
            {
                if (string.Equals(batch.BatchId, "run-frame:2", StringComparison.Ordinal))
                {
                    cancellation.Cancel();
                }
            },
            Diagnostics = new()
            {
                CurrentCommandBatchId = "run-frame:2",
                CurrentCommandStageId = "stage.second",
                QueuedCommandStageCount = 0
            }
        };
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

        WebGlRunBrowserPlaybackApplyResult result = await adapter.ApplyPlaybackAsync(playback, cancellation.Token);

        Assert.False(result.Success);
        Assert.True(result.Canceled);
        Assert.Equal(WebGlRunBrowserApplyFailureReasons.CancellationRequested, result.FailureReason);
        Assert.Equal("frame apply canceled", result.CancellationReason);
        Assert.Equal(2, result.LastAppliedFrameIndex);
        Assert.Equal(2, result.FailedFrameIndex);
        Assert.Equal(3, result.TargetFrameIndex);
        Assert.NotNull(result.FailureSnapshot);
        Assert.Equal("3", result.FailureSnapshot.Diagnostics["targetFrameIndex"]);
        Assert.Equal("2", result.FailureSnapshot.Diagnostics["lastAppliedFrameIndex"]);
        Assert.Equal("frame apply canceled", result.FailureSnapshot.Diagnostics["cancellationReason"]);
        Assert.Equal(2, runtime.AppliedAndWaitBatches.Count);
        Assert.Empty(runtime.AppliedBatches);
        Assert.Equal([1, 2], result.FrameResults.Select(frame => frame.FrameIndex).ToArray());
        Assert.Equal(["stage.first", "stage.second"], runtime.AppliedAndWaitBatches.Select(batch => batch.Stages.Single().StageId).ToArray());
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

        public List<WebGlSceneCommandBatch> AppliedAndWaitBatches { get; } = [];

        public WebGlSceneCommandResult ImportResult { get; set; } = new() { Success = true };

        public WebGlSceneCommandBatchResult BatchResult { get; set; } = new()
        {
            Success = true,
            LifecycleState = WebGlSceneCommandLifecycleStates.Settled,
            Settled = true
        };

        public Queue<WebGlSceneCommandBatchResult> BatchResults { get; } = [];

        public WebGlRuntimeDiagnostics Diagnostics { get; set; } = new();

        public WebGlRuntimeIdleResult IdleResult { get; set; } = new() { Success = true, Idle = true };

        public List<WebGlRunRuntimeIdleWaitOptions> IdleWaits { get; } = [];

        public Action<WebGlSceneCommandBatch>? OnApplyBatch { get; set; }

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
            OnApplyBatch?.Invoke(batch);
            return ValueTask.FromResult<WebGlSceneCommandBatchResult?>(BatchResults.Count > 0 ? BatchResults.Dequeue() : BatchResult);
        }

        public ValueTask<WebGlSceneCommandBatchResult?> ApplyCommandBatchAndWaitAsync(
            WebGlSceneCommandBatch batch,
            WebGlRunRuntimeIdleWaitOptions options,
            bool requireRuntimeIdle = true,
            CancellationToken cancellationToken = default)
        {
            AppliedAndWaitBatches.Add(batch);
            IdleWaits.Add(options);
            OnApplyBatch?.Invoke(batch);
            return ValueTask.FromResult<WebGlSceneCommandBatchResult?>(BatchResults.Count > 0 ? BatchResults.Dequeue() : BatchResult);
        }

        public ValueTask<WebGlRuntimeDiagnostics?> GetDiagnosticsAsync(CancellationToken cancellationToken = default)
            => ValueTask.FromResult<WebGlRuntimeDiagnostics?>(Diagnostics);

        public ValueTask<WebGlRuntimeIdleResult?> WaitForRuntimeIdleAsync(
            WebGlRunRuntimeIdleWaitOptions options,
            CancellationToken cancellationToken = default)
        {
            IdleWaits.Add(options);
            return ValueTask.FromResult<WebGlRuntimeIdleResult?>(IdleResult);
        }
    }
}
