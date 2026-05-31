namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunPlaybackResult
{
    public bool Success => Errors.Count == 0;

    public string RequestedCommand { get; set; } = string.Empty;

    public long TargetFrameIndex { get; set; }

    public int FramesApplied { get; set; }

    public int StagesQueued { get; set; }

    public List<string> Errors { get; } = [];

    public List<string> Warnings { get; } = [];

    public WebGlRunFrame? CurrentFrame { get; set; }

    public List<WebGlRunFrame> FramesToApply { get; } = [];

    public bool RequiresSceneReset { get; set; }

    public WebGlRunPlaybackState State { get; set; } = new();

    public Dictionary<string, string> RunSourceProvenance { get; set; } = [];
}
