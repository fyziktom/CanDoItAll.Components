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

    public int QueuedMotionCount { get; set; }

    public List<string> ActiveMotionIds { get; set; } = [];

    public List<string> QueuedMotionIds { get; set; } = [];

    public List<WebGlRuntimeMotionQueueSnapshot> MotionQueueSnapshot { get; set; } = [];

    public int MaxMotionQueueLength { get; set; }

    public int CancelledMotionCount { get; set; }

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

    public string RuntimeBudgetProfile { get; set; } = string.Empty;

    public bool DegradedRenderingActive { get; set; }

    public int RuntimeBudgetWarningCount { get; set; }

    public List<string> RuntimeBudgetWarnings { get; set; } = [];

    public int RuntimeBudgetMaxSceneObjects { get; set; }

    public int RuntimeBudgetMaxLoadedAssets { get; set; }

    public int RuntimeBudgetMaxActiveMotions { get; set; }

    public int RuntimeBudgetMaxQueuedMotions { get; set; }

    public string LastFrameReason { get; set; } = string.Empty;

    public double FrameTimeMs { get; set; }

    public double AverageFrameTimeMs { get; set; }

    public double PeakFrameTimeMs { get; set; }

    public double IdleSinceMs { get; set; }

    public string LargestLoadedAssetId { get; set; } = string.Empty;

    public string AssetCacheMode { get; set; } = "state-local";

    public int AssetCacheEntryCount { get; set; }

    public int AssetCacheHitCount { get; set; }

    public int AssetCacheMissCount { get; set; }

    public int DisposedTemplateCount { get; set; }

    public int AssetCachePendingDisposalCount { get; set; }

    public int AssetCacheDisposedPromiseCount { get; set; }

    public int AssetCacheDisposalErrorCount { get; set; }

    public int MaterialCloneCount { get; set; }

    public int DisposedGeometryCount { get; set; }

    public int DisposedMaterialCount { get; set; }

    public int DisposedTextureCount { get; set; }

    public int RetainedSharedTextureCount { get; set; }

    public int BatchCommandCount { get; set; }

    public int BatchStageCount { get; set; }

    public int BatchDurationMs { get; set; }

    public string CurrentCommandBatchId { get; set; } = string.Empty;

    public string CurrentCommandStageId { get; set; } = string.Empty;

    public int CompletedCommandStageCount { get; set; }

    public int FailedCommandStageCount { get; set; }

    public int QueuedCommandStageCount { get; set; }

    public int SkippedCommandStageCount { get; set; }

    public int CommandStageCancelledCount { get; set; }

    public double CommandStageWaitSeconds { get; set; }

    public string CommandStageBarrierPolicy { get; set; } = string.Empty;

    public string CommandStageBarrierTarget { get; set; } = string.Empty;

    public List<string> CommandStageBarrierBlockers { get; set; } = [];

    public string CommandStageBarrierEventId { get; set; } = string.Empty;

    public List<string> CommandStageBarrierObjectIds { get; set; } = [];

    public double CommandStageBarrierElapsedSeconds { get; set; }

    public double CommandStageBarrierTimeoutSeconds { get; set; }

    public bool CommandStageBarrierTimedOut { get; set; }

    public string LastStageBarrierWarning { get; set; } = string.Empty;

    public List<string> CompletedCommandStageIds { get; set; } = [];

    public List<string> FailedCommandStageIds { get; set; } = [];

    public List<string> SkippedCommandStageIds { get; set; } = [];

    public string LastStageError { get; set; } = string.Empty;

    public List<WebGlRuntimeCommandStageResult> CommandStageResultLog { get; set; } = [];

    public List<WebGlRuntimeCommandStageQueueItem> CommandStageQueueSnapshot { get; set; } = [];

    public int CommandStageJournalCount { get; set; }

    public int CommandStageJournalDroppedCount { get; set; }

    public Dictionary<string, int> CommandStageJournalCounters { get; set; } = [];

    public List<string> CommandStageRecentResultIds { get; set; } = [];

    public List<WebGlRuntimeCommandStageJournalEntry> CommandStageRecentJournalEntries { get; set; } = [];

    public string LastStageCancelReason { get; set; } = string.Empty;

    public int RuntimeStopCount { get; set; }

    public string LastRuntimeStopReason { get; set; } = string.Empty;

    public int ClearedMotionCount { get; set; }

    public int LastRuntimeStopClearedMotionCount { get; set; }

    public int LastRuntimeStopCancelledCommandStageCount { get; set; }

    public int CommandCountBeforeNormalization { get; set; }

    public int CommandCountAfterNormalization { get; set; }

    public int CoalescedPatchCount { get; set; }

    public int DroppedDuplicateMotionCount { get; set; }

    public int PreservedOrderedDuplicateMotionCount { get; set; }

    public int InteropCallsAvoided { get; set; }

    public int PatchedObjectCount { get; set; }

    public int FullSceneRebuildCount { get; set; }

    public int TransformOnlyPatchCount { get; set; }

    public int SymbolOnlyPatchCount { get; set; }

    public int LinkOnlyPatchCount { get; set; }

    public int VisualReplacePatchCount { get; set; }

    public int MixedIncrementalPatchCount { get; set; }

    public int GraphStructurePatchCount { get; set; }

    public int SceneRebuildPatchCount { get; set; }

    public string LastPatchClassification { get; set; } = string.Empty;

    public int ReplacedObjectGroupCount { get; set; }

    public int SymbolOnlyUpdateCount { get; set; }

    public int LinkUpdateCount { get; set; }

    public int LinkGeometryUpdateCount { get; set; }

    public int LinkGeometryRebuildCount { get; set; }

    public int LinksUpdatedLastFrame { get; set; }

    public int LinkSyncScanCount { get; set; }

    public int LinkSyncIndexedHitCount { get; set; }

    public string LastError { get; set; } = string.Empty;

    public List<string> MissingAssetIds { get; set; } = [];

    public List<string> FailedAssetUris { get; set; } = [];

    public List<string> MissingFallbackAssetIds { get; set; } = [];

    public List<string> FailedPatchCommands { get; set; } = [];

    public List<WebGlSceneCommandResult> FailedCommandDetails { get; set; } = [];

    public List<WebGlModelDiagnostics> ModelDiagnostics { get; set; } = [];
}

public sealed class WebGlRuntimeMotionQueueSnapshot
{
    public string ObjectId { get; set; } = string.Empty;

    public List<string> QueuedMotionIds { get; set; } = [];
}

public sealed class WebGlRuntimeCommandStageQueueItem
{
    public string BatchId { get; set; } = string.Empty;

    public string StageId { get; set; } = string.Empty;

    public string BarrierPolicy { get; set; } = string.Empty;

    public double WaitSeconds { get; set; }

    public string BarrierEventId { get; set; } = string.Empty;
}

public sealed class WebGlRuntimeCommandStageResult
{
    public string ResultId { get; set; } = string.Empty;

    public string BatchId { get; set; } = string.Empty;

    public string StageId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string BarrierPolicy { get; set; } = string.Empty;

    public string Error { get; set; } = string.Empty;
}

public sealed class WebGlRuntimeCommandStageJournalEntry
{
    public int Sequence { get; set; }

    public long TimestampMs { get; set; }

    public string EventKind { get; set; } = string.Empty;

    public string BatchId { get; set; } = string.Empty;

    public string StageId { get; set; } = string.Empty;

    public string ResultId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string BarrierPolicy { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
