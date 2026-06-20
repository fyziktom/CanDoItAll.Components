using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunRuntimeSnapshot
{
    public string RunId { get; set; } = string.Empty;

    public string InitialSceneId { get; set; } = string.Empty;

    public int InitialObjectCount { get; set; }

    public int InitialLinkCount { get; set; }

    public long CurrentFrameIndex { get; set; }

    public string CurrentCommandBatchId { get; set; } = string.Empty;

    public string CurrentStageId { get; set; } = string.Empty;

    public List<string> CurrentStageIds { get; set; } = [];

    public List<string> CurrentActionIds { get; set; } = [];

    public List<string> ActiveStageIds { get; set; } = [];

    public List<string> QueuedStageIds { get; set; } = [];

    public int QueuedStageCount { get; set; }

    public int ActiveMotionCount { get; set; }

    public List<string> ActiveMotionIds { get; set; } = [];

    public int QueuedMotionCount { get; set; }

    public List<string> QueuedMotionIds { get; set; } = [];

    public List<WebGlRuntimeMotionQueueSnapshot> MotionQueueSnapshot { get; set; } = [];

    public List<WebGlRuntimeCommandStageJournalEntry> CommandJournalTail { get; set; } = [];

    public int CommandJournalDroppedCount { get; set; }

    public WebGlRunStageBarrierSnapshot StageBarrier { get; set; } = new();

    public List<string> RuntimeErrors { get; set; } = [];

    public List<string> RuntimeWarnings { get; set; } = [];

    public bool IsPlaying { get; set; }

    public double PlaybackSpeed { get; set; } = 1.0;

    public string PlaybackLifecycleState { get; set; } = WebGlRunPlaybackLifecycleStates.Idle;

    public string LastPlaybackCommandKind { get; set; } = string.Empty;

    public string LastPlaybackStopReason { get; set; } = string.Empty;

    public int PlaybackPauseCount { get; set; }

    public int PlaybackCancelCount { get; set; }

    public int PlaybackStopCount { get; set; }

    public bool InitialSceneLoaded { get; set; }

    public Dictionary<string, string> RunSourceProvenance { get; set; } = [];

    public Dictionary<string, string> Diagnostics { get; set; } = [];
}

public sealed class WebGlRunStageBarrierSnapshot
{
    public string Policy { get; set; } = string.Empty;

    public string Target { get; set; } = string.Empty;

    public List<string> Blockers { get; set; } = [];

    public string EventId { get; set; } = string.Empty;

    public List<string> ObjectIds { get; set; } = [];

    public double WaitSeconds { get; set; }

    public double ElapsedSeconds { get; set; }

    public double TimeoutSeconds { get; set; }

    public bool TimedOut { get; set; }
}
