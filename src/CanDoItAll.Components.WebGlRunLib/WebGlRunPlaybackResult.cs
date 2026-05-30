namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunPlaybackResult
{
    public bool Success => Errors.Count == 0;

    public List<string> Errors { get; } = [];

    public WebGlRunFrame? CurrentFrame { get; set; }

    public List<WebGlRunFrame> FramesToApply { get; } = [];

    public bool RequiresSceneReset { get; set; }

    public WebGlRunPlaybackState State { get; set; } = new();
}
