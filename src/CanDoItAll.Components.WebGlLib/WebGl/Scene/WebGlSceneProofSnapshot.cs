namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneProofSnapshot
{
    public string SceneId { get; set; } = string.Empty;

    public int ObjectCount { get; set; }

    public int LinkCount { get; set; }

    public int VisibleObjectCount { get; set; }

    public int HiddenObjectCount { get; set; }

    public int VisibleLinkCount { get; set; }

    public int HiddenLinkCount { get; set; }

    public int SymbolCount { get; set; }

    public int LoadedAssetCount { get; set; }

    public int MissingAssetCount { get; set; }

    public int FallbackObjectCount { get; set; }

    public int ModelInstanceCount { get; set; }

    public int PrimitiveInstanceCount { get; set; }

    public int EstimatedTriangleCount { get; set; }

    public int EstimatedVertexCount { get; set; }

    public int ActiveMotionCount { get; set; }

    public int RenderCount { get; set; }

    public bool IsRenderLoopActive { get; set; }

    public double AverageFrameTimeMs { get; set; }

    public double PeakFrameTimeMs { get; set; }

    public int LinkUpdateCount { get; set; }

    public int LinkGeometryRebuildCount { get; set; }

    public string ActiveAssetProfile { get; set; } = WebGlAssetQualityProfiles.Primitive;

    public string LargestLoadedAssetId { get; set; } = string.Empty;

    public List<string> SelectedObjectIds { get; set; } = [];

    public string HoveredObjectId { get; set; } = string.Empty;

    public int ViewportWidth { get; set; }

    public int ViewportHeight { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}
