namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlRuntimeDiagnostics
{
    public int CreateCount { get; set; }

    public int DisposeCount { get; set; }

    public int UpdateCount { get; set; }

    public int RenderCount { get; set; }

    public int LoadedAssetCount { get; set; }

    public int MissingAssetCount { get; set; }

    public int FallbackObjectCount { get; set; }

    public int ModelInstanceCount { get; set; }

    public int PrimitiveInstanceCount { get; set; }

    public int ActiveMotionCount { get; set; }

    public int MotionAcceptedCount { get; set; }

    public int MotionCompletedCount { get; set; }

    public int MotionFailedCount { get; set; }

    public int AnimatedSymbolCount { get; set; }

    public bool IsRenderLoopActive { get; set; }

    public int EstimatedTriangleCount { get; set; }

    public int EstimatedVertexCount { get; set; }

    public int ObjectCount { get; set; }

    public int SymbolCount { get; set; }

    public bool DeterministicMode { get; set; }

    public string ActiveAssetProfile { get; set; } = WebGlAssetQualityProfiles.Primitive;

    public string RenderMode { get; set; } = WebGlRenderModes.Auto;

    public string LastFrameReason { get; set; } = string.Empty;

    public double FrameTimeMs { get; set; }

    public double IdleSinceMs { get; set; }

    public string LargestLoadedAssetId { get; set; } = string.Empty;

    public string LastError { get; set; } = string.Empty;

    public List<string> MissingAssetIds { get; set; } = [];

    public List<string> FailedAssetUris { get; set; } = [];

    public List<string> MissingFallbackAssetIds { get; set; } = [];

    public List<string> FailedPatchCommands { get; set; } = [];

    public List<WebGlSceneCommandResult> FailedCommandDetails { get; set; } = [];

    public List<WebGlModelDiagnostics> ModelDiagnostics { get; set; } = [];
}
