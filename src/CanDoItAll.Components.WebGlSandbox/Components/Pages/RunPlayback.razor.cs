using System.Globalization;
using System.Text.Json;
using CanDoItAll.Components.BaseLib;
using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;
using CanDoItAll.Components.WebGlSandbox;

namespace CanDoItAll.Components.WebGlSandbox.Components.Pages;

public partial class RunPlayback
{
    private const int BatchActorCount = 24;
    private const long BatchProofFrameIndex = 4;
    private static readonly TimeSpan LatePlaybackApplyDrainDelay = TimeSpan.FromMilliseconds(750);
    private static readonly JsonSerializerOptions DiagnosticsJsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly WebGlRuntimeOptions runtimeOptions = new()
    {
        DeterministicMode = true,
        RenderMode = WebGlRenderModes.Auto,
        AssetQualityProfile = WebGlAssetQualityProfiles.Primitive,
        ShowLabels = false,
        ShowSymbols = true
    };
    private readonly WebGlRunDocument runDocument;
    private WebGlSceneModel scene = CreateScene();
    private WebGlSceneView? sceneView;
    private WebGlRunDocumentRunner? documentRunner;
    private WebGlSceneProofSnapshot? latestSnapshot;
    private WebGlRuntimeDiagnostics? latestRuntimeDiagnostics;
    private WebGlRuntimeIdleResult? latestRuntimeIdleResult;
    private WebGlRunBrowserApplyResult? latestApplyResult;
    private WebGlRunRuntimeSnapshot? latestRunSnapshot;
    private CancellationTokenSource? playbackCancellation;
    private Task? playbackTask;
    private long playbackGeneration;
    private bool playbackStopRequested = true;
    private bool isPlaying;
    private string statusText = "Ready.";

    public RunPlayback()
    {
        runDocument = CreateRunDocument();
    }

    private long CurrentFrameIndex => documentRunner?.State.CurrentFrameIndex ?? 0;
    private bool IsPlaying => isPlaying && !playbackStopRequested;
    private long MaxFrameIndex => runDocument.Timeline.Frames.Count == 0
        ? 0
        : runDocument.Timeline.Frames.Max(static frame => frame.Index);
    private string PlaybackStatus => IsPlaying ? "playing" : "paused";
    private string PlaybackTone => IsPlaying ? "success" : "info";
    private string CurrentBatchIdText => FirstNonEmpty(
        latestRuntimeDiagnostics?.CurrentCommandBatchId,
        latestRunSnapshot?.CurrentCommandBatchId,
        "none");
    private string BatchCommandCountText => CountText(latestRuntimeDiagnostics?.BatchCommandCount);
    private string BatchStageCountText => CountText(latestRuntimeDiagnostics?.BatchStageCount);
    private string CommandCountBeforeText => CountText(latestRuntimeDiagnostics?.CommandCountBeforeNormalization);
    private string CommandCountAfterText => CountText(latestRuntimeDiagnostics?.CommandCountAfterNormalization);
    private string InteropCallsAvoidedText => CountText(latestRuntimeDiagnostics?.InteropCallsAvoided);
    private string QueuedStageCountText => CountText(latestRuntimeDiagnostics?.QueuedCommandStageCount);
    private string DiagnosticsJson => JsonSerializer.Serialize(new
    {
        runId = runDocument.RunId.Value,
        currentFrameIndex = CurrentFrameIndex,
        isPlaying = IsPlaying,
        latestApply = latestApplyResult is null
            ? null
            : new
            {
                latestApplyResult.FrameIndex,
                latestApplyResult.AppliedStageCount,
                latestApplyResult.AppliedPatchCount,
                latestApplyResult.AppliedMotionCount,
                latestApplyResult.Success
            },
        batch = latestRuntimeDiagnostics is null
            ? null
            : new
            {
                latestRuntimeDiagnostics.CurrentCommandBatchId,
                latestRuntimeDiagnostics.BatchCommandCount,
                latestRuntimeDiagnostics.BatchStageCount,
                latestRuntimeDiagnostics.CommandCountBeforeNormalization,
                latestRuntimeDiagnostics.CommandCountAfterNormalization,
                latestRuntimeDiagnostics.InteropCallsAvoided,
                latestRuntimeDiagnostics.QueuedCommandStageCount,
                latestRuntimeDiagnostics.CommandStageJournalCount
            },
        idle = latestRuntimeIdleResult is null
            ? null
            : new
            {
                latestRuntimeIdleResult.Idle,
                latestRuntimeIdleResult.TimedOut,
                latestRuntimeIdleResult.ElapsedMs,
                latestRuntimeIdleResult.Blockers
            },
        runSnapshot = latestRunSnapshot is null
            ? null
            : new
            {
                latestRunSnapshot.CurrentFrameIndex,
                latestRunSnapshot.CurrentCommandBatchId,
                latestRunSnapshot.CurrentStageIds,
                latestRunSnapshot.ActiveMotionCount,
                latestRunSnapshot.QueuedMotionCount,
                latestRunSnapshot.Diagnostics
            },
        proofSnapshot = latestSnapshot is null
            ? null
            : new
            {
                latestSnapshot.RenderCount,
                latestSnapshot.ObjectCount,
                latestSnapshot.VisibleObjectCount,
                latestSnapshot.ActiveMotionCount,
                latestSnapshot.CurrentCommandBatchId,
                latestSnapshot.CommandStageJournalCount
            }
    }, DiagnosticsJsonOptions);

    private async Task PlayAsync()
    {
        if (isPlaying || !await EnsureRunnerAsync().ConfigureAwait(false))
        {
            return;
        }

        playbackCancellation?.Cancel();
        playbackCancellation?.Dispose();
        playbackCancellation = new CancellationTokenSource();
        CancellationTokenSource cancellationSource = playbackCancellation;
        long generation = ++playbackGeneration;
        playbackStopRequested = false;
        isPlaying = true;
        statusText = "Playing generic sequence.";
        await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        playbackTask = InvokeAsync(() => PlayLoopAsync(generation, cancellationSource));
    }

    private async Task PlayLoopAsync(long generation, CancellationTokenSource cancellationSource)
    {
        CancellationToken cancellationToken = cancellationSource.Token;
        try
        {
            while (IsActivePlayback(generation, cancellationToken) && CurrentFrameIndex < MaxFrameIndex)
            {
                WebGlRunExecutionResult result = await documentRunner!.StepForwardAsync(cancellationToken).ConfigureAwait(false);
                if (!IsActivePlayback(generation, cancellationToken))
                {
                    break;
                }

                await CompleteExecutionAsync(result, $"Played generic frame {CurrentFrameIndex}.").ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    break;
                }

                await Task.Delay(TimeSpan.FromMilliseconds(140), cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            if (playbackGeneration == generation)
            {
                statusText = "Playback canceled.";
            }
        }
        catch (Exception ex)
        {
            if (playbackGeneration == generation)
            {
                statusText = ex.Message;
            }
        }
        finally
        {
            if (playbackGeneration == generation)
            {
                isPlaying = false;
                if (ReferenceEquals(playbackCancellation, cancellationSource))
                {
                    playbackCancellation = null;
                }

                await InvokeAsync(StateHasChanged).ConfigureAwait(false);
            }

            cancellationSource.Dispose();
        }
    }

    private Task PauseAsync()
        => StopPlaybackAsync("Paused.");

    private Task CancelAsync()
        => StopPlaybackAsync("Canceled.");

    private async Task ResetAsync()
    {
        await StopPlaybackAsync("Resetting.").ConfigureAwait(false);
        scene = CreateScene();
        documentRunner = null;
        latestApplyResult = null;
        latestRunSnapshot = null;
        latestRuntimeDiagnostics = null;
        latestRuntimeIdleResult = null;
        latestSnapshot = null;
        await InvokeAsync(StateHasChanged).ConfigureAwait(false);

        if (await EnsureRunnerAsync(forceReload: true).ConfigureAwait(false))
        {
            statusText = "Reset generic run.";
            await CaptureProofAsync().ConfigureAwait(false);
        }
    }

    private async Task StepAsync()
    {
        await StopPlaybackAsync("Stepping.").ConfigureAwait(false);
        if (!await EnsureRunnerAsync().ConfigureAwait(false))
        {
            return;
        }

        WebGlRunExecutionResult result = await documentRunner!.StepForwardAsync().ConfigureAwait(false);
        await CompleteExecutionAsync(result, $"Applied generic frame {CurrentFrameIndex}.").ConfigureAwait(false);
    }

    private Task ApplyBatchProofFrameAsync()
        => SeekAsync(BatchProofFrameIndex);

    private async Task SeekAsync(long frameIndex)
    {
        await StopPlaybackAsync($"Seeking frame {frameIndex.ToString(CultureInfo.InvariantCulture)}.").ConfigureAwait(false);
        if (!await EnsureRunnerAsync().ConfigureAwait(false))
        {
            return;
        }

        WebGlRunExecutionResult seek = await documentRunner!.SeekAsync(frameIndex).ConfigureAwait(false);
        if (!seek.Succeeded)
        {
            await CompleteExecutionAsync(seek, string.Empty).ConfigureAwait(false);
            return;
        }

        WebGlRunExecutionResult apply = await documentRunner.ApplyCurrentFrameAsync().ConfigureAwait(false);
        await CompleteExecutionAsync(apply, $"Applied generic frame {CurrentFrameIndex}.").ConfigureAwait(false);
    }

    private async Task CaptureProofAsync()
    {
        if (sceneView is null)
        {
            latestSnapshot = null;
            latestRuntimeDiagnostics = null;
            return;
        }

        latestSnapshot = await sceneView.GetProofSnapshotAsync().ConfigureAwait(false);
        latestRuntimeDiagnostics = await sceneView.GetDiagnosticsAsync().ConfigureAwait(false);
        await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    }

    private async Task<bool> EnsureRunnerAsync(bool forceReload = false)
    {
        if (sceneView is null)
        {
            statusText = "WebGL runtime is still initializing.";
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
            return false;
        }

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator().Validate(runDocument);
        if (!validation.IsValid)
        {
            statusText = string.Join(" ", validation.Errors);
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
            return false;
        }

        if (documentRunner is not null && !forceReload)
        {
            return true;
        }

        documentRunner = new WebGlRunDocumentRunner(new BrowserFrameApplier(this));
        WebGlRunExecutionResult load = await documentRunner.LoadAsync(runDocument).ConfigureAwait(false);
        await CompleteExecutionAsync(load, "Loaded generic run document.").ConfigureAwait(false);
        return load.Succeeded;
    }

    private async Task CompleteExecutionAsync(WebGlRunExecutionResult result, string successMessage)
    {
        statusText = result.Succeeded
            ? successMessage
            : string.Join(" ", result.Errors);
        await CaptureProofAsync().ConfigureAwait(false);
    }

    private async Task ApplyFrameThroughAdapterAsync(WebGlRunFrameApplyResult frame, CancellationToken cancellationToken)
    {
        if (sceneView is null)
        {
            return;
        }

        var adapter = new WebGlRunBrowserApplyAdapter(
            new WebGlSceneViewBrowserRuntime(sceneView),
            runDocument.InitialScene,
            new WebGlRunBrowserPlaybackApplyOptions
            {
                RuntimeIdleWaitPolicy = WebGlRunRuntimeIdleWaitPolicies.AfterPlayback,
                RuntimeIdle = new WebGlRunRuntimeIdleWaitOptions
                {
                    TimeoutMs = 2_000,
                    PollIntervalMs = 16,
                    Reason = "run-playback"
                }
            });
        WebGlRunBrowserApplyResult result = await adapter.ApplyAsync(frame, cancellationToken).ConfigureAwait(false);
        if (cancellationToken.IsCancellationRequested)
        {
            await sceneView.StopRuntimeActivityAsync("playback-apply-canceled", waitForIdle: true).ConfigureAwait(false);
            latestRuntimeIdleResult = await sceneView.WaitForRuntimeIdleAsync(reason: "playback-apply-canceled").ConfigureAwait(false);
            return;
        }

        latestApplyResult = result;
        latestRunSnapshot = result.RuntimeSnapshot;
        latestRuntimeDiagnostics = result.RuntimeDiagnostics;
        latestRuntimeIdleResult = result.RuntimeIdleResult;
        latestSnapshot = await sceneView.GetProofSnapshotAsync().ConfigureAwait(false);
        statusText = result.Success
            ? $"Applied generic frame {frame.FrameIndex.ToString(CultureInfo.InvariantCulture)} as one command batch."
            : string.Join(" ", result.Errors);
    }

    private async Task StopPlaybackAsync(string status)
    {
        playbackGeneration++;
        playbackStopRequested = true;
        isPlaying = false;
        Task? taskToStop = playbackTask;
        playbackCancellation?.Cancel();
        statusText = status;
        if (taskToStop is not null && !taskToStop.IsCompleted)
        {
            try
            {
                await taskToStop.WaitAsync(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (TimeoutException)
            {
                statusText = $"{status} Playback task did not settle before runtime stop.";
            }
        }

        if (taskToStop?.IsCompleted == true)
        {
            playbackTask = null;
            playbackCancellation = null;
        }

        isPlaying = false;
        if (sceneView is not null)
        {
            WebGlSceneCommandResult? stopResult = await sceneView.StopRuntimeActivityAsync(status, waitForIdle: true).ConfigureAwait(false);
            if (stopResult?.Success == false)
            {
                statusText = string.Join(" ", stopResult.Errors);
            }

            await CaptureProofAsync().ConfigureAwait(false);
            latestRuntimeIdleResult = await sceneView.WaitForRuntimeIdleAsync(reason: "run-playback-stop").ConfigureAwait(false);
            await Task.Delay(LatePlaybackApplyDrainDelay).ConfigureAwait(false);
            WebGlSceneCommandResult? drainResult = await sceneView.StopRuntimeActivityAsync($"{status} Late apply drain.", waitForIdle: true).ConfigureAwait(false);
            if (drainResult?.Success == false)
            {
                statusText = string.Join(" ", drainResult.Errors);
            }

            await CaptureProofAsync().ConfigureAwait(false);
            latestRuntimeIdleResult = await sceneView.WaitForRuntimeIdleAsync(reason: "run-playback-stop-drain").ConfigureAwait(false);
        }

        playbackStopRequested = true;
        isPlaying = false;
        statusText = statusText.Contains("Playback task did not settle", StringComparison.Ordinal)
            ? statusText
            : status;
        await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    }

    private Task HandleMotionCompleted(WebGlSceneCommandResult result)
    {
        if (!isPlaying)
        {
            return Task.CompletedTask;
        }

        statusText = $"Motion completed: {result.CommandId}.";
        return InvokeAsync(StateHasChanged);
    }

    private bool IsActivePlayback(long generation, CancellationToken cancellationToken)
        => playbackGeneration == generation &&
           isPlaying &&
           !cancellationToken.IsCancellationRequested;

    public void Dispose()
    {
        playbackGeneration++;
        playbackStopRequested = true;
        isPlaying = false;
        playbackCancellation?.Cancel();
        playbackCancellation?.Dispose();
        playbackCancellation = null;
        playbackTask = null;
    }

    private static WebGlRunDocument CreateRunDocument()
    {
        var compiler = new WebGlRunActionCompiler();
        return new WebGlRunDocument
        {
            RunId = new("generic-run-demo"),
            InitialScene = new WebGlSceneDocument { Scene = CreateScene() },
            Timeline = compiler.Compile(CreateActionPlan()),
            Metadata =
            {
                ["boundary"] = "generic-webgl-runlib",
                ["domain"] = "generic"
            }
        };
    }

    private static WebGlRunActionPlan CreateActionPlan()
        => new()
        {
            FrameRate = 1,
            ObjectBindings =
            [
                new WebGlRunObjectBinding { ObjectId = "object.runner", Position = new WebGlVector3(-3, 0, 0), AnchorPosition = new WebGlVector3(-3, 0, 0) },
                new WebGlRunObjectBinding { ObjectId = "object.goal", Position = new WebGlVector3(3, 0, 0), AnchorPosition = new WebGlVector3(3, 0, 0) },
                .. Enumerable.Range(0, BatchActorCount).Select(index => new WebGlRunObjectBinding
                {
                    ObjectId = BatchObjectId(index),
                    Position = BatchStartPosition(index),
                    AnchorPosition = BatchStartPosition(index)
                })
            ],
            Actions =
            [
                Action("run.move.target", WebGlRunActionKinds.MoveToObject, 0, "object.runner", "object.goal"),
                Action("run.symbol.show", WebGlRunActionKinds.ShowSymbol, 1, "object.runner", parameters: new()
                {
                    ["symbolKind"] = "generic-status",
                    ["color"] = "#facc15",
                    ["tooltip"] = "Generic status"
                }),
                Action("run.pose.work", WebGlRunActionKinds.SetPose, 1, "object.runner", parameters: new()
                {
                    ["poseKey"] = "active"
                }),
                Action("run.return.anchor", WebGlRunActionKinds.ReturnToAnchor, 2, "object.runner"),
                Action("run.symbol.hide", WebGlRunActionKinds.HideSymbol, 3, "object.runner"),
                Action("run.pose.restore", WebGlRunActionKinds.SetPose, 3, "object.runner", parameters: new()
                {
                    ["poseKey"] = "neutral"
                }),
                .. Enumerable.Range(0, BatchActorCount).Select(index => Action(
                    $"run.batch.move.{index}",
                    WebGlRunActionKinds.MoveToPosition,
                    BatchProofFrameIndex,
                    BatchObjectId(index),
                    parameters: PositionParameters(BatchTargetPosition(index)),
                    durationSeconds: 0.5,
                    coalescingScope: WebGlRunCoalescingScopes.Frame))
            ]
        };

    private static WebGlRunAction Action(
        string id,
        string kind,
        double startsAtSeconds,
        string subjectObjectId,
        string targetObjectId = "",
        Dictionary<string, string>? parameters = null,
        double durationSeconds = 0.32,
        string coalescingScope = WebGlRunCoalescingScopes.StageOnly)
        => new()
        {
            ActionId = id,
            ActionKind = kind,
            SubjectObjectId = subjectObjectId,
            TargetObjectId = targetObjectId,
            StartsAtSeconds = startsAtSeconds,
            DurationSeconds = durationSeconds,
            Parameters = parameters ?? [],
            CoalescingScope = coalescingScope
        };

    private sealed class BrowserFrameApplier(RunPlayback owner) : IWebGlRunFrameApplier, IWebGlRunInitialSceneApplier
    {
        public async ValueTask ApplyInitialSceneAsync(WebGlSceneDocument sceneDocument, CancellationToken cancellationToken = default)
        {
            if (owner.sceneView is null)
            {
                return;
            }

            var runtime = new WebGlSceneViewBrowserRuntime(owner.sceneView);
            WebGlSceneCommandResult? importResult = await runtime.ImportSceneAsync(sceneDocument, cancellationToken).ConfigureAwait(false);
            owner.latestRuntimeDiagnostics = await runtime.GetDiagnosticsAsync(cancellationToken).ConfigureAwait(false);
            owner.latestRuntimeIdleResult = await runtime.WaitForRuntimeIdleAsync(
                new WebGlRunRuntimeIdleWaitOptions { Reason = "run-playback-initial-scene" },
                cancellationToken).ConfigureAwait(false);
            owner.latestSnapshot = await owner.sceneView.GetProofSnapshotAsync().ConfigureAwait(false);
            owner.latestRunSnapshot = new WebGlRunRuntimeSnapshot
            {
                RunId = owner.runDocument.RunId.Value,
                InitialSceneId = sceneDocument.Scene.SceneId,
                InitialObjectCount = sceneDocument.Scene.Objects.Count,
                InitialLinkCount = sceneDocument.Scene.Links.Count,
                InitialSceneLoaded = importResult?.Success == true,
                RuntimeErrors = importResult is null ? [] : [.. importResult.Errors],
                RuntimeWarnings = importResult is null ? [] : [.. importResult.Warnings]
            };
        }

        public async ValueTask ApplyAsync(WebGlRunFrameApplyResult frame, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await owner.ApplyFrameThroughAdapterAsync(frame, cancellationToken).ConfigureAwait(false);
        }
    }

    private static WebGlSceneModel CreateScene()
        => new()
        {
            SceneId = "generic-run-playback",
            Title = "Generic Run Playback",
            AssetCatalog = WebGlSandboxAssetCatalogFactory.Create(),
            Environment = new WebGlSceneEnvironment
            {
                BackgroundColor = "#101827",
                GroundColor = "#253349",
                GridColor = "#94a3b8",
                GroundSize = 14,
                GridDivisions = 14,
                FogEnabled = false
            },
            Camera = new WebGlSceneCamera
            {
                ViewMode = WebGlSceneViewModes.Isometric,
                Distance = 13,
                Target = new WebGlVector3(0, 0.6, 0)
            },
            UiState = new WebGlSceneUiState
            {
                ShowLabels = false,
                ShowSymbols = true,
                ActiveAssetProfile = WebGlAssetQualityProfiles.Primitive
            },
            Objects =
            [
                new WebGlSceneObject
                {
                    Id = "object.runner",
                    Kind = "marker",
                    Family = "generic-run",
                    Title = "Runner",
                    AssetId = "asset.symbol.marker.default",
                    Position = new WebGlVector3(-3, 0, 0),
                    Size = new WebGlVector3(0.8, 0.8, 0.8),
                    Color = "#38bdf8",
                    IsSelectable = true
                },
                new WebGlSceneObject
                {
                    Id = "object.goal",
                    Kind = "marker",
                    Family = "generic-run",
                    Title = "Goal",
                    AssetId = "asset.symbol.ready.default",
                    Position = new WebGlVector3(3, 0, 0),
                    Size = new WebGlVector3(0.8, 0.8, 0.8),
                    Color = "#22c55e"
                },
                .. Enumerable.Range(0, BatchActorCount).Select(index => new WebGlSceneObject
                {
                    Id = BatchObjectId(index),
                    Kind = "marker",
                    Family = "generic-batch",
                    Title = $"Actor {index + 1}",
                    AssetId = "asset.symbol.marker.default",
                    Position = BatchStartPosition(index),
                    Size = new WebGlVector3(0.42, 0.42, 0.42),
                    Color = index % 2 == 0 ? "#a78bfa" : "#f59e0b"
                })
            ],
            Links =
            [
                new WebGlSceneLink
                {
                    Id = "link.path",
                    SourceObjectId = "object.runner",
                    TargetObjectId = "object.goal",
                    Kind = "timeline-path"
                }
            ],
            Metadata =
            {
                ["demo"] = "run-playback",
                ["domain"] = "generic"
            }
        };

    private static string BatchObjectId(int index)
        => $"object.batch.{index}";

    private static WebGlVector3 BatchStartPosition(int index)
        => new(-5.2 + (index % 8) * 1.48, 0, -3.2 + (index / 8) * 0.62);

    private static WebGlVector3 BatchTargetPosition(int index)
        => new(-5.2 + (index % 8) * 1.48, 0, 2.2 - (index / 8) * 0.62);

    private static Dictionary<string, string> PositionParameters(WebGlVector3 position)
        => new(StringComparer.Ordinal)
        {
            ["x"] = ToInvariant(position.X),
            ["y"] = ToInvariant(position.Y),
            ["z"] = ToInvariant(position.Z)
        };

    private static string CountText(int? value)
        => value.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);

    private static string ToInvariant(double value)
        => value.ToString(CultureInfo.InvariantCulture);

    private static string FirstNonEmpty(params string?[] values)
        => values.FirstOrDefault(static value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
}
