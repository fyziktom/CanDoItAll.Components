namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneProofSnapshot
{
    public string SceneId { get; set; } = string.Empty;

    public int ObjectCount { get; set; }

    public int LinkCount { get; set; }

    public int SymbolCount { get; set; }

    public int LoadedAssetCount { get; set; }

    public int MissingAssetCount { get; set; }

    public int FallbackObjectCount { get; set; }

    public List<string> SelectedObjectIds { get; set; } = [];

    public string HoveredObjectId { get; set; } = string.Empty;

    public int ViewportWidth { get; set; }

    public int ViewportHeight { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

