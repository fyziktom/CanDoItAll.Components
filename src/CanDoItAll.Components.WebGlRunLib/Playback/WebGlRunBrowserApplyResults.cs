using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunBrowserApplyResult
{
    public bool Success => Errors.Count == 0;

    public string FailureReason { get; set; } = string.Empty;

    public long FrameIndex { get; set; }

    public bool AppliedInitialScene { get; set; }

    public int AppliedStageCount { get; set; }

    public int AppliedPatchCount { get; set; }

    public int AppliedMotionCount { get; set; }

    public WebGlRunRuntimeSnapshot RuntimeSnapshot { get; set; } = new();

    public WebGlRuntimeDiagnostics? RuntimeDiagnostics { get; set; }

    public WebGlSceneCommandBatchResult? CommandBatchResult { get; set; }

    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];
}

public sealed class WebGlRunBrowserPlaybackApplyResult
{
    public bool Success => Errors.Count == 0 && FrameResults.All(static frame => frame.Success);

    public string RequestedCommand { get; set; } = string.Empty;

    public long TargetFrameIndex { get; set; }

    public bool RequiresSceneReset { get; set; }

    public bool AppliedInitialScene { get; set; }

    public long? FailedFrameIndex { get; set; }

    public string FailureReason { get; set; } = string.Empty;

    public List<WebGlRunBrowserApplyResult> FrameResults { get; set; } = [];

    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];
}

public static class WebGlRunBrowserApplyFailureReasons
{
    public const string PreApplyValidationFailed = nameof(PreApplyValidationFailed);

    public const string ResetFailed = nameof(ResetFailed);

    public const string BatchFailed = nameof(BatchFailed);

    public const string MultiFramePlaybackRequiresExplicitApply = nameof(MultiFramePlaybackRequiresExplicitApply);
}
