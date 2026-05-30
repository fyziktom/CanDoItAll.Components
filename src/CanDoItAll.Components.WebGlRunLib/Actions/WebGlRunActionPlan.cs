using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunActionStep
{
    public string StepId { get; set; } = string.Empty;

    public WebGlRunAction Action { get; set; } = new();

    public double DelaySeconds { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunActionPlan
{
    public int FrameRate { get; set; } = 1;

    public string ActionId { get; set; } = string.Empty;

    public bool IsValid => Errors.Count == 0;

    public List<WebGlRunAction> Actions { get; set; } = [];

    public List<WebGlRunObjectBinding> ObjectBindings { get; set; } = [];

    public List<WebGlScenePatch> Patches { get; set; } = [];

    public List<WebGlObjectMotionCommand> Motions { get; set; } = [];

    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public List<string> DroppedStepIds { get; set; } = [];

    public Dictionary<string, string> TargetResolutionDiagnostics { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];

    public int GeneratedPatchCount => Patches.Count;

    public int GeneratedMotionCount => Motions.Count;
}

public sealed class WebGlRunObjectBinding
{
    public string ObjectId { get; set; } = string.Empty;
    public WebGlVector3? Position { get; set; }
    public WebGlVector3? AnchorPosition { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunActionMapping
{
    public string EventKind { get; set; } = string.Empty;
    public string ActionKind { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = [];
}
