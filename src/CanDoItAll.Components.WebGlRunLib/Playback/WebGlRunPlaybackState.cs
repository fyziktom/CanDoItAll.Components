namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunPlaybackState
{
    public WebGlRunId RunId { get; set; } = new(string.Empty);

    public long CurrentFrameIndex { get; set; }

    public string CurrentCommandBatchId { get; set; } = string.Empty;

    public string CurrentStageId { get; set; } = string.Empty;

    public List<string> CurrentStageIds { get; set; } = [];

    public List<string> CurrentActionIds { get; set; } = [];

    public int QueuedStageCount { get; set; }

    public bool InitialSceneLoaded { get; set; }

    public string InitialSceneId { get; set; } = string.Empty;

    public bool IsPlaying { get; set; }

    public double PlaybackSpeed { get; set; } = 1.0;

    public string PlaybackLifecycleState { get; set; } = WebGlRunPlaybackLifecycleStates.Idle;

    public string LastPlaybackCommandKind { get; set; } = string.Empty;

    public string LastPlaybackStopReason { get; set; } = string.Empty;

    public int PlaybackPauseCount { get; set; }

    public int PlaybackCancelCount { get; set; }

    public int PlaybackStopCount { get; set; }
}
