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

    public string RenderSchedulerMode { get; set; } = WebGlRenderModes.Auto;

    public string LastScheduledReason { get; set; } = string.Empty;

    public double LastDeltaSeconds { get; set; }

    public int EstimatedTriangleCount { get; set; }

    public int EstimatedVertexCount { get; set; }

    public int ObjectCount { get; set; }

    public int VisibleObjectCount { get; set; }

    public int HiddenObjectCount { get; set; }

    public int VisibleLinkCount { get; set; }

    public int HiddenLinkCount { get; set; }

    public int SceneIndexSyncCount { get; set; }

    public string LastSceneIndexSyncReason { get; set; } = string.Empty;

    public int SymbolCount { get; set; }

    public bool DeterministicMode { get; set; }

    public string ActiveAssetProfile { get; set; } = WebGlAssetQualityProfiles.Primitive;

    public string RenderMode { get; set; } = WebGlRenderModes.Auto;

    public string LastFrameReason { get; set; } = string.Empty;

    public double FrameTimeMs { get; set; }

    public double AverageFrameTimeMs { get; set; }

    public double PeakFrameTimeMs { get; set; }

    public double IdleSinceMs { get; set; }

    public string LargestLoadedAssetId { get; set; } = string.Empty;

    public int AssetCacheEntryCount { get; set; }

    public int AssetCacheHitCount { get; set; }

    public int AssetCacheMissCount { get; set; }

    public int DisposedTemplateCount { get; set; }

    public int MaterialCloneCount { get; set; }

    public int DisposedGeometryCount { get; set; }

    public int DisposedMaterialCount { get; set; }

    public int DisposedTextureCount { get; set; }

    public int BatchCommandCount { get; set; }

    public int BatchStageCount { get; set; }

    public int BatchDurationMs { get; set; }

    public int CoalescedPatchCount { get; set; }

    public int DroppedDuplicateMotionCount { get; set; }

    public int PatchedObjectCount { get; set; }

    public int ReplacedObjectGroupCount { get; set; }

    public int SymbolOnlyUpdateCount { get; set; }

    public int LinkUpdateCount { get; set; }

    public int LinkGeometryRebuildCount { get; set; }

    public string LastError { get; set; } = string.Empty;

    public List<string> MissingAssetIds { get; set; } = [];

    public List<string> FailedAssetUris { get; set; } = [];

    public List<string> MissingFallbackAssetIds { get; set; } = [];

    public List<string> FailedPatchCommands { get; set; } = [];

    public List<WebGlSceneCommandResult> FailedCommandDetails { get; set; } = [];

    public List<WebGlModelDiagnostics> ModelDiagnostics { get; set; } = [];
}
