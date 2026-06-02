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

    public int QueuedMotionCount { get; set; }

    public List<string> ActiveMotionIds { get; set; } = [];

    public List<string> QueuedMotionIds { get; set; } = [];

    public string CurrentCommandBatchId { get; set; } = string.Empty;

    public string CurrentCommandStageId { get; set; } = string.Empty;

    public int CompletedCommandStageCount { get; set; }

    public int FailedCommandStageCount { get; set; }

    public int SkippedCommandStageCount { get; set; }

    public int QueuedCommandStageCount { get; set; }

    public string CommandStageBarrierPolicy { get; set; } = string.Empty;

    public double CommandStageWaitSeconds { get; set; }

    public string CommandStageBarrierTarget { get; set; } = string.Empty;

    public List<string> CommandStageBarrierBlockers { get; set; } = [];

    public string CommandStageBarrierEventId { get; set; } = string.Empty;

    public int CommandStageJournalCount { get; set; }

    public int CommandStageJournalDroppedCount { get; set; }

    public WebGlSceneCommandStageJournalCounters CommandStageJournalCounters { get; set; } = new();

    public List<string> CommandStageRecentResultIds { get; set; } = [];

    public List<WebGlSceneCommandStageJournalEntry> CommandStageRecentJournalEntries { get; set; } = [];

    public int RenderCount { get; set; }

    public bool IsRenderLoopActive { get; set; }

    public double AverageFrameTimeMs { get; set; }

    public double PeakFrameTimeMs { get; set; }

    public int FullSceneRebuildCount { get; set; }

    public int TransformOnlyPatchCount { get; set; }

    public int SymbolOnlyPatchCount { get; set; }

    public int LinkOnlyPatchCount { get; set; }

    public int VisualReplacePatchCount { get; set; }

    public int MixedIncrementalPatchCount { get; set; }

    public int GraphStructurePatchCount { get; set; }

    public int SceneRebuildPatchCount { get; set; }

    public string LastPatchClassification { get; set; } = string.Empty;

    public int LinkUpdateCount { get; set; }

    public int LinkGeometryUpdateCount { get; set; }

    public int LinkGeometryRebuildCount { get; set; }

    public int LinksUpdatedLastFrame { get; set; }

    public int LinkSyncScanCount { get; set; }

    public int LinkSyncIndexedHitCount { get; set; }

    public string ActiveAssetProfile { get; set; } = WebGlAssetQualityProfiles.Primitive;

    public string LargestLoadedAssetId { get; set; } = string.Empty;

    public string AssetCacheMode { get; set; } = "state-local";

    public int RetainedSharedTextureCount { get; set; }

    public List<string> SelectedObjectIds { get; set; } = [];

    public string HoveredObjectId { get; set; } = string.Empty;

    public int ViewportWidth { get; set; }

    public int ViewportHeight { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlSceneCommandStageJournalCounters
{
    public int Started { get; set; }

    public int Applied { get; set; }

    public int Completed { get; set; }

    public int Warnings { get; set; }

    public int Failures { get; set; }
}

public sealed class WebGlSceneCommandStageJournalEntry
{
    public int Sequence { get; set; }

    public int TimestampMs { get; set; }

    public string EventKind { get; set; } = string.Empty;

    public string BatchId { get; set; } = string.Empty;

    public string StageId { get; set; } = string.Empty;

    public string ResultId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string BarrierPolicy { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
