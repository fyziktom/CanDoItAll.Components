using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public interface IWebGlRunDocumentRunner
{
    WebGlRunExecutionState State { get; }

    ValueTask<WebGlRunExecutionResult> LoadAsync(WebGlRunDocument document, CancellationToken cancellationToken = default);

    ValueTask<WebGlRunExecutionResult> SeekAsync(long frameIndex, CancellationToken cancellationToken = default);

    ValueTask<WebGlRunExecutionResult> ApplyCurrentFrameAsync(CancellationToken cancellationToken = default);

    ValueTask<WebGlRunExecutionResult> StepForwardAsync(CancellationToken cancellationToken = default);

    ValueTask<WebGlRunExecutionResult> StepBackwardAsync(CancellationToken cancellationToken = default);

    ValueTask<WebGlRunExecutionResult> PauseAsync(string reason = "paused", CancellationToken cancellationToken = default);

    ValueTask<WebGlRunExecutionResult> CancelAsync(string reason = "canceled", CancellationToken cancellationToken = default);

    ValueTask<WebGlRunExecutionResult> StopAsync(string reason = "stopped", CancellationToken cancellationToken = default);
}

public interface IWebGlRunInitialSceneApplier
{
    ValueTask ApplyInitialSceneAsync(WebGlSceneDocument sceneDocument, CancellationToken cancellationToken = default);
}

public sealed class WebGlRunExecutionState
{
    public WebGlRunDocument Document { get; set; } = new();

    public string RunId { get; set; } = string.Empty;

    public long CurrentFrameIndex { get; set; }

    public bool InitialSceneLoaded { get; set; }

    public string CurrentCommandBatchId { get; set; } = string.Empty;

    public List<string> ActiveStageIds { get; set; } = [];

    public List<string> PendingStageIds { get; set; } = [];

    public List<string> CompletedStageIds { get; set; } = [];

    public List<string> FailedStageIds { get; set; } = [];

    public List<string> SkippedStageIds { get; set; } = [];

    public List<string> CanceledStageIds { get; set; } = [];

    public string PlaybackLifecycleState { get; set; } = WebGlRunPlaybackLifecycleStates.Idle;

    public string LastPlaybackCommandKind { get; set; } = string.Empty;

    public string LastPlaybackStopReason { get; set; } = string.Empty;

    public int PlaybackPauseCount { get; set; }

    public int PlaybackCancelCount { get; set; }

    public int PlaybackStopCount { get; set; }

    public WebGlRunExecutionDiagnostics ExecutionDiagnostics { get; set; } = new();

    public Dictionary<string, string> Diagnostics { get; set; } = [];
}

public sealed class WebGlRunExecutionResult
{
    public bool Succeeded => Errors.Count == 0;

    public string Operation { get; set; } = string.Empty;

    public long CurrentFrameIndex { get; set; }

    public bool AppliedInitialScene { get; set; }

    public bool Paused { get; set; }

    public bool Canceled { get; set; }

    public bool Stopped { get; set; }

    public string PlaybackLifecycleState { get; set; } = WebGlRunPlaybackLifecycleStates.Idle;

    public string PlaybackLifecycleReason { get; set; } = string.Empty;

    public List<string> AppliedStageIds { get; set; } = [];

    public List<string> CanceledStageIds { get; set; } = [];

    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public WebGlRunExecutionDiagnostics ExecutionDiagnostics { get; set; } = new();

    public Dictionary<string, string> Diagnostics { get; set; } = [];
}

public sealed class WebGlRunExecutionDiagnostics
{
    public List<string> UnresolvedObjectIds { get; set; } = [];

    public List<string> FailedMotionIds { get; set; } = [];

    public List<string> FailedPatchIds { get; set; } = [];

    public List<string> FailedLinkIds { get; set; } = [];

    public List<string> SourceFrameIds { get; set; } = [];

    public List<string> SourceStageIds { get; set; } = [];

    public int FailedMotionCount { get; set; }

    public int FailedPatchCount { get; set; }

    public int FailedLinkCount { get; set; }
}
