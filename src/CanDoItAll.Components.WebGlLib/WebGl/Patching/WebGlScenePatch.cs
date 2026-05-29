namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlScenePatch
{
    public string SceneId { get; set; } = string.Empty;

    public int BaseRevision { get; set; }

    public int NextRevision { get; set; }

    public List<WebGlSceneObjectPatch> ObjectPatches { get; set; } = [];

    public List<WebGlSceneObject> AddObjects { get; set; } = [];

    public List<string> RemoveObjectIds { get; set; } = [];

    public List<WebGlSceneLink> AddLinks { get; set; } = [];

    public List<string> RemoveLinkIds { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlSceneObjectPatch
{
    public string ObjectId { get; set; } = string.Empty;

    public WebGlVector3? Position { get; set; }

    public WebGlVector3? Rotation { get; set; }

    public WebGlVector3? Scale { get; set; }

    public WebGlVector3? Size { get; set; }

    public string? AssetId { get; set; }

    public string? Color { get; set; }

    public List<WebGlStatusSymbol>? Symbols { get; set; }

    public Dictionary<string, string>? Metadata { get; set; }
}
