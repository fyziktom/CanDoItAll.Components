using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunBrowserPlaybackApplyOptions
{
    public string RuntimeIdleWaitPolicy { get; set; } = WebGlRunRuntimeIdleWaitPolicies.None;

    public WebGlRunRuntimeIdleWaitOptions RuntimeIdle { get; set; } = new();
}

public sealed class WebGlRunRuntimeIdleWaitOptions
{
    public int TimeoutMs { get; set; } = 2_000;

    public int PollIntervalMs { get; set; } = 16;

    public string Reason { get; set; } = "playback-apply";

    public string PolicyMode { get; set; } = WebGlRuntimeIdlePolicyModes.VisualStrict;
}

public static class WebGlRunRuntimeIdleWaitPolicies
{
    public const string None = nameof(None);

    public const string AfterEachFrame = nameof(AfterEachFrame);

    public const string AfterPlayback = nameof(AfterPlayback);
}

public static class WebGlRunBrowserReplayModes
{
    public const string Incremental = nameof(Incremental);

    public const string AbsoluteReplay = nameof(AbsoluteReplay);

    public const string SnapshotAnchorReplay = nameof(SnapshotAnchorReplay);
}
