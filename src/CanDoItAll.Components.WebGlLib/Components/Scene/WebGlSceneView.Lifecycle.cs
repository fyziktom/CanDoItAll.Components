using System.Text.Json;

namespace CanDoItAll.Components.WebGlLib;

public partial class WebGlSceneView
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private const int CompactSceneLifecycleObjectThreshold = 100;

    private static string SerializePayload(WebGlSceneModel scene, WebGlRuntimeOptions options)
        => JsonSerializer.Serialize(new { scene, options }, SerializerOptions);

    private static T? Deserialize<T>(string json)
        => JsonSerializer.Deserialize<T>(json, SerializerOptions);

    private bool ShouldKeepExternalImport(string parameterSceneKey, string parameterSceneId)
    {
        if (string.IsNullOrWhiteSpace(externalImportSceneKey))
        {
            return false;
        }

        if (string.Equals(parameterSceneKey, externalImportSceneKey, StringComparison.Ordinal))
        {
            ClearExternalImportLifecycle();
            return false;
        }

        return string.Equals(parameterSceneKey, externalImportReplacedParameterKey, StringComparison.Ordinal) ||
               (!string.IsNullOrWhiteSpace(parameterSceneId) &&
                string.Equals(parameterSceneId, externalImportReplacedSceneId, StringComparison.Ordinal));
    }

    private void MarkExternalImportApplied(WebGlSceneModel sceneModel, WebGlRuntimeOptions options)
    {
        externalImportReplacedParameterKey = pendingSceneKey ?? CreateSceneLifecycleKey(Scene, Options);
        externalImportReplacedSceneId = Scene.SceneId;
        externalImportSceneKey = CreateSceneLifecycleKey(sceneModel, options);
        Scene = sceneModel;
        Options = options;
        pendingSceneKey = externalImportSceneKey;
        appliedSceneKey = externalImportSceneKey;
    }

    private void ClearExternalImportLifecycle()
    {
        externalImportSceneKey = null;
        externalImportReplacedParameterKey = null;
        externalImportReplacedSceneId = null;
    }

    private static string CreateSceneLifecycleKey(WebGlSceneModel scene, WebGlRuntimeOptions options)
    {
        if (ShouldUseCompactSceneLifecycleKey(scene))
        {
            return JsonSerializer.Serialize(new
            {
                keyKind = "compact",
                sceneId = scene.SceneId,
                sceneRevision = scene.Revision,
                uiRevision = scene.UiState?.Revision ?? 0,
                objectCount = scene.Objects.Count,
                linkCount = scene.Links.Count,
                layerCount = scene.Layers.Count,
                assetCatalogId = scene.AssetCatalog?.CatalogId ?? string.Empty,
                assetCatalogVersion = scene.AssetCatalog?.Version ?? string.Empty,
                runtime = CreateRuntimeLifecycleKey(options)
            }, SerializerOptions);
        }

        return SerializePayload(scene, options);
    }

    private static bool ShouldUseCompactSceneLifecycleKey(WebGlSceneModel scene)
        => scene.Objects.Count >= CompactSceneLifecycleObjectThreshold ||
           scene.Links.Count >= CompactSceneLifecycleObjectThreshold;

    private static string CreateRuntimeLifecycleKey(WebGlRuntimeOptions options)
        => !string.IsNullOrWhiteSpace(options.RuntimeKey)
            ? options.RuntimeKey
            : JsonSerializer.Serialize(options, SerializerOptions);

    private WebGlRuntimeOptions CreateInteropOptions(WebGlRuntimeOptions options)
        => new()
        {
            DeterministicMode = options.DeterministicMode,
            PreserveDrawingBuffer = options.PreserveDrawingBuffer,
            EnableAntialiasing = options.EnableAntialiasing,
            MaximumDevicePixelRatio = options.MaximumDevicePixelRatio,
            RenderMode = options.RenderMode,
            AssetQualityProfile = options.AssetQualityProfile,
            ShowDiagnosticsPanel = options.ShowDiagnosticsPanel,
            ShowLabels = options.ShowLabels,
            LabelVisibilityMode = options.LabelVisibilityMode,
            LabelHoverHideDelayMilliseconds = options.LabelHoverHideDelayMilliseconds,
            ShowSymbols = options.ShowSymbols,
            ShowLinks = options.ShowLinks,
            AutoFitOnCreate = options.AutoFitOnCreate,
            MaxCommandResultHistory = options.MaxCommandResultHistory,
            MaxCommandBatchChildResults = options.MaxCommandBatchChildResults,
            MaxCommandBatchMessages = options.MaxCommandBatchMessages,
            MaxCommandBatchProofSnapshotPositions = options.MaxCommandBatchProofSnapshotPositions,
            NotifyStateChanged = options.NotifyStateChanged && StateChanged.HasDelegate,
            NotifyMotionCompleted = options.NotifyMotionCompleted && MotionCompleted.HasDelegate,
            NotifyCommandCompleted = options.NotifyCommandCompleted && CommandCompleted.HasDelegate,
            NotifyCommandFailed = options.NotifyCommandFailed && CommandFailed.HasDelegate,
            RuntimeBudget = CloneRuntimeBudget(options.RuntimeBudget ?? new WebGlRuntimeBudgetOptions()),
            RuntimeKey = options.RuntimeKey
        };

    private static WebGlRuntimeBudgetOptions CloneRuntimeBudget(WebGlRuntimeBudgetOptions options)
        => new()
        {
            Profile = options.Profile,
            MaxSceneObjects = options.MaxSceneObjects,
            MaxLoadedAssets = options.MaxLoadedAssets,
            MaxAssetCacheEntries = options.MaxAssetCacheEntries,
            MaxActiveMotions = options.MaxActiveMotions,
            MaxQueuedMotions = options.MaxQueuedMotions,
            MaxQueuedCommandStages = options.MaxQueuedCommandStages,
            MaxEstimatedTriangles = options.MaxEstimatedTriangles,
            DegradeWhenExceeded = options.DegradeWhenExceeded
        };
}
