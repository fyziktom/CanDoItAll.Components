namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneModel
{
    public string SceneId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Revision { get; set; }

    public WebGlAssetCatalog AssetCatalog { get; set; } = new();

    public WebGlSceneEnvironment Environment { get; set; } = new();

    public WebGlSceneCamera Camera { get; set; } = new();

    public WebGlSceneUiState UiState { get; set; } = new();

    public WebGlInteractionOptions Interaction { get; set; } = new();

    public List<WebGlSceneLayer> Layers { get; set; } = [];

    public List<WebGlSceneObject> Objects { get; set; } = [];

    public List<WebGlSceneLink> Links { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlSceneUiState
{
    public WebGlSceneSelectionState Selection { get; set; } = new();

    public string HoveredObjectId { get; set; } = string.Empty;

    public bool ShowGrid { get; set; } = true;

    public bool ShowGround { get; set; } = true;

    public bool ShowLabels { get; set; } = true;

    public bool ShowSymbols { get; set; } = true;

    public bool ShowLinks { get; set; } = true;

    public bool DeterministicMode { get; set; } = true;

    public string ActiveAssetProfile { get; set; } = WebGlAssetQualityProfiles.Primitive;

    public int Revision { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}
