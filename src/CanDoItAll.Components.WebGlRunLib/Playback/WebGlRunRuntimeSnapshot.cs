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

    public int QueuedStageCount { get; set; }

    public bool IsPlaying { get; set; }

    public double PlaybackSpeed { get; set; } = 1.0;

    public bool InitialSceneLoaded { get; set; }

    public Dictionary<string, string> RunSourceProvenance { get; set; } = [];

    public Dictionary<string, string> Diagnostics { get; set; } = [];
}
