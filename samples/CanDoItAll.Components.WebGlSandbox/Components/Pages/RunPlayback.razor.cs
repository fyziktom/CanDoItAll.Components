using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CanDoItAll.Components.BaseLib;
using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;
using CanDoItAll.Components.WebGlSandbox;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.WebGlSandbox.Components.Pages;

public partial class RunPlayback
{
    private const int BatchActorCount = 0;
    private const long BatchProofFrameIndex = 3;
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
    private WebGlRunDocument runDocument;
    private WebGlSceneModel scene = CreateScene();
    private WebGlSceneView? sceneView;
    private WebGlRunDocumentRunner? documentRunner;
    private WebGlSceneProofSnapshot? latestSnapshot;
    private WebGlRuntimeDiagnostics? latestRuntimeDiagnostics;
    private WebGlRuntimeIdleResult? latestRuntimeIdleResult;
    private WebGlRunBrowserApplyResult? latestApplyResult;
    private WebGlRunRuntimeSnapshot? latestRunSnapshot;
    private WebGlRunDocument? browserLoadedRunDocument;
    private WebGlSceneDocument? browserLoadedInitialSceneDocument;
    private string browserLoadedSceneContentHash = string.Empty;
    private string browserProofSnapshotHash = string.Empty;
    private StringBuilder? pendingProofDocumentJson;
    private DotNetObjectReference<RunPlayback>? proofBridgeReference;
    private readonly WebGlRunPlaybackStopCoordinator stopCoordinator = new();
    private CancellationTokenSource? playbackCancellation;
    private Task? playbackTask;
    private long playbackGeneration;
    private long latestRuntimeStopGeneration;
    private int ignoredStaleRuntimeCallbackCount;
    private bool playbackStopRequested = true;
    private bool isPlaying;
    private string lastPlaybackError = string.Empty;
    private string statusText = "Ready.";

    public RunPlayback()
    {
        runDocument = CreateRunDocument();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        proofBridgeReference = DotNetObjectReference.Create(this);
        await JsRuntime.InvokeVoidAsync("CanDoItAll.webglSandbox.runPlayback.register", proofBridgeReference).ConfigureAwait(false);
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
    private string RuntimeStopGenerationText => latestRuntimeStopGeneration.ToString(CultureInfo.InvariantCulture);
    private string IdleBlockersText => latestRuntimeIdleResult is null || latestRuntimeIdleResult.Blockers.Count == 0
        ? "none"
        : string.Join(", ", latestRuntimeIdleResult.Blockers);
    private string DiagnosticsJson => JsonSerializer.Serialize(new
    {
        runId = runDocument.RunId.Value,
        runtimeStopGeneration = latestRuntimeStopGeneration,
        ignoredStaleRuntimeCallbackCount,
        lastPlaybackError,
        observer = WebGlRunObserverProof.Compare(
            runDocument,
            BrowserLoadedRunDocument,
            BuildObserverSnapshot()),
        hashes = BuildObserverHashDiagnostics(),
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
                latestRuntimeIdleResult.SemanticIdle,
                latestRuntimeIdleResult.VisualIdle,
                latestRuntimeIdleResult.FinalRenderDrained,
                latestRuntimeIdleResult.Blockers,
                latestRuntimeIdleResult.SemanticBlockers,
                latestRuntimeIdleResult.VisualBlockers,
                blockerText = IdleBlockersText
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

    private WebGlRunObserverSnapshot BuildObserverSnapshot()
        => new()
        {
            BrowserRuntimeExercised = latestRuntimeDiagnostics is not null,
            UiExercised = latestSnapshot is not null,
            Route = "/run-playback",
            Viewport = latestSnapshot is null
                ? string.Empty
                : $"{latestSnapshot.ViewportWidth.ToString(CultureInfo.InvariantCulture)}x{latestSnapshot.ViewportHeight.ToString(CultureInfo.InvariantCulture)}",
            RuntimeDiagnostics = latestRuntimeDiagnostics,
            RuntimeIdleResult = latestRuntimeIdleResult,
            CompletedStageIds = documentRunner is null
                ? []
                : [.. documentRunner.State.CompletedStageIds],
            FinalObjectPositions = latestSnapshot?.ObjectPositions.Count > 0
                ? new(latestSnapshot.ObjectPositions, StringComparer.Ordinal)
                : [],
            RuntimeErrors = latestRunSnapshot is null ? [] : [.. latestRunSnapshot.RuntimeErrors],
            RuntimeWarnings = latestRunSnapshot is null ? [] : [.. latestRunSnapshot.RuntimeWarnings],
            Metadata =
            {
                ["route"] = "run-playback",
                ["browserDocumentLoaded"] = (browserLoadedRunDocument is not null).ToString(CultureInfo.InvariantCulture),
                ["browserLoadedSceneContentHash"] = browserLoadedSceneContentHash,
                ["browserProofSnapshotHash"] = browserProofSnapshotHash,
                ["runtimeStopGeneration"] = latestRuntimeStopGeneration.ToString(CultureInfo.InvariantCulture),
                ["runtimeDiagnosticsCaptured"] = (latestRuntimeDiagnostics is not null).ToString(CultureInfo.InvariantCulture),
                ["proofSnapshotCaptured"] = (latestSnapshot is not null).ToString(CultureInfo.InvariantCulture),
                ["browserObjectPositionsCaptured"] = (latestSnapshot?.ObjectPositions.Count > 0).ToString(CultureInfo.InvariantCulture)
            }
        };

    private WebGlRunDocument BrowserLoadedRunDocument
        => browserLoadedRunDocument ?? CreateBrowserNotLoadedDocument();

    private object BuildObserverHashDiagnostics()
    {
        WebGlRunDocument browserDocument = BrowserLoadedRunDocument;
        string expectedDocumentHash = WebGlRunObserverProof.ComputeDocumentHash(runDocument);
        string browserLoadedDocumentHash = WebGlRunObserverProof.ComputeDocumentHash(browserDocument);
        return new
        {
            browserDocumentLoaded = browserLoadedRunDocument is not null,
            expectedDocumentHash,
            browserLoadedDocumentHash,
            documentHashesMatch = string.Equals(expectedDocumentHash, browserLoadedDocumentHash, StringComparison.Ordinal),
            expectedSceneContentHash = PrefixSha256(WebGlSceneDocumentSerializer.ComputeSceneContentHash(runDocument.InitialScene)),
            browserLoadedSceneContentHash,
            browserProofSnapshotHash,
            browserLoadedInitialSceneId = browserLoadedInitialSceneDocument?.Scene.SceneId ?? string.Empty
        };
    }

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
        lastPlaybackError = string.Empty;
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
                lastPlaybackError = ex.ToString();
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

    [JSInvokable]
    public async Task ProofLoadDocumentJsonAsync(string documentJson)
    {
        WebGlRunDocument? loaded = JsonSerializer.Deserialize<WebGlRunDocument>(documentJson, DiagnosticsJsonOptions);
        if (loaded is null)
        {
            throw new InvalidOperationException("Run document JSON did not deserialize.");
        }

        WebGlRunDocumentValidationResult validation = new WebGlRunDocumentValidator().Validate(loaded);
        if (!validation.IsValid)
        {
            throw new InvalidOperationException($"Run document is invalid: {string.Join("; ", validation.Errors)}");
        }

        playbackGeneration++;
        playbackStopRequested = true;
        isPlaying = false;
        playbackCancellation?.Cancel();
        if (sceneView is not null)
        {
            await sceneView.StopRuntimeActivityAsync("run-playback-load-document", waitForIdle: false).ConfigureAwait(false);
        }

        runDocument = loaded;
        scene = loaded.InitialScene.Scene;
        ResetObserverState();
        statusText = $"Loaded {loaded.Timeline.Frames.Count.ToString(CultureInfo.InvariantCulture)} frame run document.";
        await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    }

    [JSInvokable]
    public Task ProofBeginDocumentJsonLoadAsync()
    {
        pendingProofDocumentJson = new StringBuilder();
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task ProofAppendDocumentJsonChunkAsync(string chunk)
    {
        pendingProofDocumentJson ??= new StringBuilder();
        pendingProofDocumentJson.Append(chunk);
        return Task.CompletedTask;
    }

    [JSInvokable]
    public async Task ProofCommitDocumentJsonLoadAsync()
    {
        string documentJson = pendingProofDocumentJson?.ToString() ?? string.Empty;
        pendingProofDocumentJson = null;
        await ProofLoadDocumentJsonAsync(documentJson).ConfigureAwait(false);
    }

    [JSInvokable]
    public async Task ProofPlayAsync()
    {
        try
        {
            await PlayAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            lastPlaybackError = ex.ToString();
            statusText = ex.Message;
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }
    }

    [JSInvokable]
    public Task ProofPauseAsync()
        => PauseAsync();

    [JSInvokable]
    public Task ProofSyncRuntimeStopGenerationAsync(long generation)
    {
        latestRuntimeStopGeneration = Math.Max(latestRuntimeStopGeneration, generation);
        return InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public Task ProofSnapshotAsync()
        => CaptureProofAsync();

    private void ResetObserverState()
    {
        documentRunner = null;
        latestSnapshot = null;
        latestRuntimeDiagnostics = null;
        latestRuntimeIdleResult = null;
        latestApplyResult = null;
        latestRunSnapshot = null;
        browserLoadedRunDocument = null;
        browserLoadedInitialSceneDocument = null;
        browserLoadedSceneContentHash = string.Empty;
        browserProofSnapshotHash = string.Empty;
        latestRuntimeStopGeneration = 0;
        ignoredStaleRuntimeCallbackCount = 0;
        lastPlaybackError = string.Empty;
        pendingProofDocumentJson = null;
    }

    private async Task ResetAsync()
    {
        await StopPlaybackAsync("Resetting.").ConfigureAwait(false);
        scene = runDocument.InitialScene.Scene;
        ResetObserverState();
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
        WebGlSceneView? currentSceneView = sceneView;
        if (currentSceneView is null)
        {
            latestSnapshot = null;
            latestRuntimeDiagnostics = null;
            return;
        }

        WebGlSceneProofSnapshot? snapshot = null;
        WebGlRuntimeDiagnostics? diagnostics = null;
        await InvokeAsync(async () =>
        {
            snapshot = await currentSceneView.GetProofSnapshotAsync();
            diagnostics = await currentSceneView.GetDiagnosticsAsync();
        }).ConfigureAwait(false);

        latestSnapshot = snapshot;
        browserProofSnapshotHash = ComputeJsonHash(latestSnapshot);
        latestRuntimeDiagnostics = diagnostics;
        SyncRuntimeStopGeneration(latestRuntimeDiagnostics);
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
        WebGlSceneView? currentSceneView = sceneView;
        if (currentSceneView is null)
        {
            return;
        }

        WebGlRunBrowserApplyResult? result = null;
        WebGlSceneProofSnapshot? snapshot = null;
        await InvokeAsync(async () =>
        {
            var adapter = new WebGlRunBrowserApplyAdapter(
                new WebGlSceneViewBrowserRuntime(currentSceneView),
                runDocument.InitialScene,
                new WebGlRunBrowserPlaybackApplyOptions
                {
                    RuntimeIdleWaitPolicy = WebGlRunRuntimeIdleWaitPolicies.AfterPlayback,
                    RuntimeIdle = new WebGlRunRuntimeIdleWaitOptions
                    {
                        TimeoutMs = 12_000,
                        PollIntervalMs = 16,
                        Reason = "run-playback"
                    }
                });
            result = await adapter.ApplyAsync(frame, cancellationToken);
            if (!cancellationToken.IsCancellationRequested)
            {
                snapshot = await currentSceneView.GetProofSnapshotAsync();
            }
        }).ConfigureAwait(false);

        if (cancellationToken.IsCancellationRequested)
        {
            WebGlRuntimeIdleResult? idleResult = null;
            await InvokeAsync(async () =>
            {
                await currentSceneView.StopRuntimeActivityAsync("playback-apply-canceled", waitForIdle: true);
                idleResult = await currentSceneView.WaitForRuntimeIdleAsync(reason: "playback-apply-canceled");
            }).ConfigureAwait(false);
            latestRuntimeIdleResult = idleResult;
            SyncRuntimeStopGeneration(latestRuntimeIdleResult?.Diagnostics);
            return;
        }

        if (result is null)
        {
            return;
        }

        latestApplyResult = result;
        latestRunSnapshot = result.RuntimeSnapshot;
        latestRuntimeDiagnostics = result.RuntimeDiagnostics;
        latestRuntimeIdleResult = result.RuntimeIdleResult;
        SyncRuntimeStopGeneration(latestRuntimeDiagnostics);
        latestSnapshot = snapshot;
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
        CancellationTokenSource? cancellationToStop = playbackCancellation;
        statusText = status;
        await InvokeAsync(StateHasChanged).ConfigureAwait(false);

        if (sceneView is not null)
        {
            WebGlRunPlaybackStopResult stopResult = await stopCoordinator.StopAsync(new()
            {
                Reason = status,
                StopRuntimeAsync = async (reason, waitForIdle) => await sceneView
                    .StopRuntimeActivityAsync(reason, waitForIdle: waitForIdle)
                    .ConfigureAwait(false),
                CancelPlayback = () => cancellationToStop?.Cancel(),
                PlaybackTask = taskToStop,
                PlaybackDrainTimeout = TimeSpan.FromSeconds(2),
                LateApplyDrainDelay = LatePlaybackApplyDrainDelay
            }).ConfigureAwait(false);

            SyncRuntimeStopGeneration(stopResult.ImmediateStopResult);
            SyncRuntimeStopGeneration(stopResult.FinalStopResult);
            SyncRuntimeStopGeneration(stopResult.LateDrainStopResult);

            if (!stopResult.Success)
            {
                statusText = stopResult.Errors.Count > 0
                    ? string.Join(" ", stopResult.Errors)
                    : $"{status} Runtime stop did not complete successfully.";
            }
            else if (stopResult.PlaybackTaskTimedOut)
            {
                statusText = $"{status} Playback task did not settle before runtime stop.";
            }

            await CaptureProofAsync().ConfigureAwait(false);
            latestRuntimeIdleResult = await sceneView.WaitForRuntimeIdleAsync(reason: "run-playback-stop-drain").ConfigureAwait(false);
            SyncRuntimeStopGeneration(latestRuntimeIdleResult?.Diagnostics);
        }
        else
        {
            cancellationToStop?.Cancel();
            await DrainPlaybackTaskWithoutRuntimeAsync(taskToStop, status).ConfigureAwait(false);
        }

        if (taskToStop?.IsCompleted == true)
        {
            playbackTask = null;
            playbackCancellation = null;
        }

        isPlaying = false;
        playbackStopRequested = true;
        isPlaying = false;
        statusText = statusText.Contains("Playback task did not settle", StringComparison.Ordinal)
            ? statusText
            : status;
        await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    }

    private Task HandleMotionCompleted(WebGlSceneCommandResult result)
        => HandleRuntimeCompletion(result, "Motion completed");

    private Task HandleCommandCompleted(WebGlSceneCommandResult result)
        => HandleRuntimeCompletion(result, "Command completed");

    private Task HandleRuntimeCompletion(WebGlSceneCommandResult result, string label)
    {
        if (WebGlRunRuntimeStopGenerationPolicy.IsStale(result, latestRuntimeStopGeneration))
        {
            ignoredStaleRuntimeCallbackCount++;
            return Task.CompletedTask;
        }

        if (!isPlaying)
        {
            return Task.CompletedTask;
        }

        statusText = $"{label}: {result.CommandId}.";
        return InvokeAsync(StateHasChanged);
    }

    private async Task DrainPlaybackTaskWithoutRuntimeAsync(Task? taskToStop, string status)
    {
        if (taskToStop is null || taskToStop.IsCompleted)
        {
            return;
        }

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

    private void SyncRuntimeStopGeneration(WebGlSceneCommandResult? result)
    {
        if (WebGlRunRuntimeStopGenerationPolicy.TryRead(result, out long generation))
        {
            latestRuntimeStopGeneration = Math.Max(latestRuntimeStopGeneration, generation);
        }
    }

    private void SyncRuntimeStopGeneration(WebGlRuntimeDiagnostics? diagnostics)
    {
        if (diagnostics is not null)
        {
            latestRuntimeStopGeneration = Math.Max(latestRuntimeStopGeneration, diagnostics.RuntimeStopGeneration);
        }
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
        proofBridgeReference?.Dispose();
        proofBridgeReference = null;
    }

    private static WebGlRunDocument CreateRunDocument()
    {
        var document = new WebGlRunDocument
        {
            RunId = new("generic-run-demo"),
            InitialScene = new WebGlSceneDocument { Scene = CreateScene() },
            Timeline = CreateTimeline(),
            Metadata =
            {
                ["boundary"] = "generic-webgl-runlib",
                ["domain"] = "generic"
            }
        };
        WebGlRunDriverMetadataKeys.Stamp(document.Metadata, WebGlRunPassThroughDomainMappingDriver.Instance);
        return document;
    }

    private WebGlRunDocument CreateBrowserLoadedRunDocument(WebGlSceneDocument browserLoadedInitialScene)
        => new()
        {
            SchemaVersion = runDocument.SchemaVersion,
            RunId = runDocument.RunId,
            InitialScene = browserLoadedInitialScene,
            Timeline = runDocument.Timeline,
            Metadata = new(runDocument.Metadata, StringComparer.Ordinal)
        };

    private WebGlRunDocument CreateBrowserNotLoadedDocument()
        => new()
        {
            SchemaVersion = runDocument.SchemaVersion,
            RunId = new($"{runDocument.RunId.Value}:browser-not-loaded"),
            InitialScene = new()
            {
                Scene = new()
                {
                    SceneId = "browser-not-loaded",
                    Title = "Browser scene not loaded"
                },
                Source = "browser-not-loaded"
            },
            Timeline = runDocument.Timeline,
            Metadata = new(runDocument.Metadata, StringComparer.Ordinal)
            {
                ["browserDocumentLoaded"] = "false"
            }
        };

    private static WebGlRunTimeline CreateTimeline()
    {
        var compiler = new WebGlRunActionCompiler();
        WebGlRunTimeline timeline = compiler.Compile(CreateActionPlan());
        if (timeline.Frames.All(static frame => frame.Index != 0))
        {
            timeline.Frames.Insert(0, new WebGlRunFrame { Index = 0, TimeSeconds = 0 });
        }

        return timeline;
    }

    private static WebGlRunActionPlan CreateActionPlan()
        => new()
        {
            FrameRate = 1,
            ObjectBindings =
            [
                new WebGlRunObjectBinding { ObjectId = "object.runner", Position = new WebGlVector3(0, 0, 0), AnchorPosition = new WebGlVector3(0, 0, 0) },
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
                Action("run.move.target", WebGlRunActionKinds.MoveToObject, 1, "object.runner", "object.goal", durationSeconds: 2),
                Action("run.symbol.show", WebGlRunActionKinds.ShowSymbol, 1, "object.runner", parameters: new()
                {
                    ["symbolKind"] = "generic-status",
                    ["color"] = "#facc15",
                    ["tooltip"] = "Generic status"
                }, durationSeconds: 0.001),
                Action("run.pose.work", WebGlRunActionKinds.SetPose, 1, "object.runner", parameters: new()
                {
                    ["poseKey"] = "active"
                }, durationSeconds: 0.001),
                Action("run.return.anchor", WebGlRunActionKinds.ReturnToAnchor, 2, "object.runner", durationSeconds: 0.001),
                Action("run.symbol.hide", WebGlRunActionKinds.HideSymbol, 3, "object.runner", durationSeconds: 0.001),
                Action("run.pose.restore", WebGlRunActionKinds.SetPose, 3, "object.runner", parameters: new()
                {
                    ["poseKey"] = "neutral"
                }, durationSeconds: 0.001),
                .. Enumerable.Range(0, BatchActorCount).Select(index => Action(
                    $"run.batch.move.{index}",
                    WebGlRunActionKinds.MoveToPosition,
                    BatchProofFrameIndex,
                    BatchObjectId(index),
                    parameters: PositionParameters(BatchTargetPosition(index)),
                    durationSeconds: 0.08,
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
            WebGlSceneView? currentSceneView = owner.sceneView;
            if (currentSceneView is null)
            {
                return;
            }

            WebGlSceneCommandResult? importResult = null;
            WebGlRuntimeDiagnostics? diagnostics = null;
            WebGlRuntimeIdleResult? idleResult = null;
            WebGlSceneProofSnapshot? snapshot = null;
            await owner.InvokeAsync(async () =>
            {
                var runtime = new WebGlSceneViewBrowserRuntime(currentSceneView);
                importResult = await runtime.ImportSceneAsync(sceneDocument, cancellationToken);
                diagnostics = await runtime.GetDiagnosticsAsync(cancellationToken);
                idleResult = await runtime.WaitForRuntimeIdleAsync(
                    new WebGlRunRuntimeIdleWaitOptions { Reason = "run-playback-initial-scene" },
                    cancellationToken);
                snapshot = await currentSceneView.GetProofSnapshotAsync();
            }).ConfigureAwait(false);
            owner.latestRuntimeDiagnostics = diagnostics;
            owner.latestRuntimeIdleResult = idleResult;
            owner.latestSnapshot = snapshot;
            owner.browserProofSnapshotHash = ComputeJsonHash(owner.latestSnapshot);
            await owner.CaptureBrowserLoadedDocumentAsync(sceneDocument).ConfigureAwait(false);
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

    private Task CaptureBrowserLoadedDocumentAsync(WebGlSceneDocument expectedInitialSceneDocument)
    {
        var browserSceneDocument = new WebGlSceneDocument
        {
            SchemaVersion = expectedInitialSceneDocument.SchemaVersion,
            DocumentId = expectedInitialSceneDocument.DocumentId,
            Scene = expectedInitialSceneDocument.Scene,
            RuntimeOptions = expectedInitialSceneDocument.RuntimeOptions,
            Source = "browser-imported-payload",
            Metadata = new(expectedInitialSceneDocument.Metadata, StringComparer.Ordinal)
        };
        browserSceneDocument.SceneContentHash = WebGlSceneDocumentSerializer.ComputeSceneContentHash(browserSceneDocument);
        browserSceneDocument.ContentHash = browserSceneDocument.SceneContentHash;
        browserLoadedInitialSceneDocument = browserSceneDocument;
        browserLoadedSceneContentHash = PrefixSha256(browserSceneDocument.SceneContentHash);
        browserLoadedRunDocument = CreateBrowserLoadedRunDocument(browserSceneDocument);
        return Task.CompletedTask;
    }

    private static string PrefixSha256(string hash)
        => string.IsNullOrWhiteSpace(hash)
            ? string.Empty
            : hash.StartsWith("sha256:", StringComparison.Ordinal)
                ? hash
                : $"sha256:{hash}";

    private static string ComputeJsonHash(object? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        string json = JsonSerializer.Serialize(value, DiagnosticsJsonOptions);
        return $"sha256:{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant()}";
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
                    Position = new WebGlVector3(0, 0, 0),
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
