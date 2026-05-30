using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunFrame
{
    public long Index { get; set; }

    public double TimeSeconds { get; set; }

    public List<WebGlRunActionStage> Stages { get; set; } = [];

    public List<WebGlRunFramePatch> ScenePatches { get; set; } = [];

    public List<WebGlObjectMotionCommand> Motions { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunActionStage
{
    public string StageId { get; set; } = string.Empty;

    public string SequenceId { get; set; } = string.Empty;

    public string ParentActionId { get; set; } = string.Empty;

    public int StageIndex { get; set; } = -1;

    public int OrderIndex { get; set; } = -1;

    public string ExecutionPolicy { get; set; } = WebGlRunStageExecutionPolicies.PreserveOrder;

    public double StartsAtSeconds { get; set; }

    public double WaitSeconds { get; set; }

    public List<WebGlRunFramePatch> ScenePatches { get; set; } = [];

    public List<WebGlObjectMotionCommand> Motions { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunFramePatch
{
    public string Id { get; set; } = string.Empty;

    public WebGlScenePatch Patch { get; set; } = new();
}
