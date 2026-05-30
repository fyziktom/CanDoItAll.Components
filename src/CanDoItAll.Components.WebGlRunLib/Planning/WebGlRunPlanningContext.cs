using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunPlanningContext
{
    private Dictionary<string, WebGlSceneObject>? objectIndex;

    public WebGlSceneModel Scene { get; set; } = new();

    public WebGlVisualStateCatalog VisualStates { get; set; } = new();

    public Dictionary<string, WebGlVector3> ObjectPositions { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];

    public IReadOnlyDictionary<string, WebGlSceneObject> ObjectIndex
        => objectIndex ??= Scene.Objects
            .Where(static item => !string.IsNullOrWhiteSpace(item.Id))
            .ToDictionary(static item => item.Id, StringComparer.Ordinal);
}
