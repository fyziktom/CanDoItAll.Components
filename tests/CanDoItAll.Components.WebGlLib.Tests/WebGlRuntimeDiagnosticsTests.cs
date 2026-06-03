using System.Text.Json;
using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlLib.Tests;

public sealed class WebGlRuntimeDiagnosticsTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void Runtime_diagnostics_round_trips_incremental_patch_counters()
    {
        const string json = """
            {
              "fullSceneRebuildCount": 2,
              "transformOnlyPatchCount": 100,
              "symbolOnlyPatchCount": 3,
              "linkOnlyPatchCount": 4,
              "visualReplacePatchCount": 5,
              "mixedIncrementalPatchCount": 6,
              "graphStructurePatchCount": 1,
              "sceneRebuildPatchCount": 1,
              "lastPatchClassification": "transform-only",
              "linkGeometryUpdateCount": 199,
              "linkGeometryRebuildCount": 199,
              "linksUpdatedLastFrame": 3,
              "linkSyncScanCount": 7,
              "linkSyncIndexedHitCount": 6
            }
            """;

        var diagnostics = JsonSerializer.Deserialize<WebGlRuntimeDiagnostics>(json, JsonOptions);

        Assert.NotNull(diagnostics);
        Assert.Equal(2, diagnostics.FullSceneRebuildCount);
        Assert.Equal(100, diagnostics.TransformOnlyPatchCount);
        Assert.Equal(3, diagnostics.SymbolOnlyPatchCount);
        Assert.Equal(4, diagnostics.LinkOnlyPatchCount);
        Assert.Equal(5, diagnostics.VisualReplacePatchCount);
        Assert.Equal(6, diagnostics.MixedIncrementalPatchCount);
        Assert.Equal(1, diagnostics.GraphStructurePatchCount);
        Assert.Equal(1, diagnostics.SceneRebuildPatchCount);
        Assert.Equal("transform-only", diagnostics.LastPatchClassification);
        Assert.Equal(199, diagnostics.LinkGeometryUpdateCount);
        Assert.Equal(199, diagnostics.LinkGeometryRebuildCount);
        Assert.Equal(3, diagnostics.LinksUpdatedLastFrame);
        Assert.Equal(7, diagnostics.LinkSyncScanCount);
        Assert.Equal(6, diagnostics.LinkSyncIndexedHitCount);
    }

    [Fact]
    public void Runtime_diagnostics_round_trips_asset_cache_ownership_counters()
    {
        const string json = """
            {
              "assetCacheMode": "state-local",
              "assetCacheEntryCount": 1,
              "assetCacheHitCount": 3,
              "assetCacheMissCount": 1,
              "disposedTemplateCount": 1,
              "assetCachePendingDisposalCount": 0,
              "assetCacheDisposedPromiseCount": 4,
              "assetCacheDisposalErrorCount": 0,
              "materialCloneCount": 2,
              "disposedGeometryCount": 4,
              "disposedMaterialCount": 5,
              "disposedTextureCount": 3,
              "retainedSharedTextureCount": 2
            }
            """;

        var diagnostics = JsonSerializer.Deserialize<WebGlRuntimeDiagnostics>(json, JsonOptions);

        Assert.NotNull(diagnostics);
        Assert.Equal("state-local", diagnostics.AssetCacheMode);
        Assert.Equal(1, diagnostics.AssetCacheEntryCount);
        Assert.Equal(3, diagnostics.AssetCacheHitCount);
        Assert.Equal(1, diagnostics.AssetCacheMissCount);
        Assert.Equal(1, diagnostics.DisposedTemplateCount);
        Assert.Equal(0, diagnostics.AssetCachePendingDisposalCount);
        Assert.Equal(4, diagnostics.AssetCacheDisposedPromiseCount);
        Assert.Equal(0, diagnostics.AssetCacheDisposalErrorCount);
        Assert.Equal(2, diagnostics.MaterialCloneCount);
        Assert.Equal(4, diagnostics.DisposedGeometryCount);
        Assert.Equal(5, diagnostics.DisposedMaterialCount);
        Assert.Equal(3, diagnostics.DisposedTextureCount);
        Assert.Equal(2, diagnostics.RetainedSharedTextureCount);
    }

    [Fact]
    public void Runtime_diagnostics_round_trips_runtime_budget_fields()
    {
        const string json = """
            {
              "runtimeBudgetProfile": "economy-large",
              "degradedRenderingActive": true,
              "runtimeBudgetWarningCount": 2,
              "runtimeBudgetWarnings": [
                "scene objects 750 exceeds budget 500",
                "queued motions 900 exceeds budget 512"
              ],
              "runtimeBudgetMaxSceneObjects": 500,
              "runtimeBudgetMaxLoadedAssets": 128,
              "runtimeBudgetMaxActiveMotions": 256,
              "runtimeBudgetMaxQueuedMotions": 512
            }
            """;

        var diagnostics = JsonSerializer.Deserialize<WebGlRuntimeDiagnostics>(json, JsonOptions);

        Assert.NotNull(diagnostics);
        Assert.Equal("economy-large", diagnostics.RuntimeBudgetProfile);
        Assert.True(diagnostics.DegradedRenderingActive);
        Assert.Equal(2, diagnostics.RuntimeBudgetWarningCount);
        Assert.Equal(2, diagnostics.RuntimeBudgetWarnings.Count);
        Assert.Equal(500, diagnostics.RuntimeBudgetMaxSceneObjects);
        Assert.Equal(128, diagnostics.RuntimeBudgetMaxLoadedAssets);
        Assert.Equal(256, diagnostics.RuntimeBudgetMaxActiveMotions);
        Assert.Equal(512, diagnostics.RuntimeBudgetMaxQueuedMotions);
    }

    [Fact]
    public void Runtime_budget_profiles_define_100_500_and_1000_plus_scene_budgets()
    {
        WebGlRuntimeBudgetOptions scene100 = WebGlRuntimeBudgetProfiles.Scene100();
        WebGlRuntimeBudgetOptions scene500 = WebGlRuntimeBudgetProfiles.Scene500();
        WebGlRuntimeBudgetOptions scene1000Plus = WebGlRuntimeBudgetProfiles.Scene1000Plus();

        Assert.Equal("scene-100", scene100.Profile);
        Assert.Equal(100, scene100.MaxSceneObjects);
        Assert.Equal(128, scene100.MaxActiveMotions);
        Assert.Equal(256, scene100.MaxQueuedMotions);
        Assert.Equal(64, scene100.MaxQueuedCommandStages);
        Assert.Equal(100_000, scene100.MaxEstimatedTriangles);

        Assert.Equal("scene-500", scene500.Profile);
        Assert.Equal(500, scene500.MaxSceneObjects);
        Assert.Equal(500, scene500.MaxActiveMotions);
        Assert.Equal(1_000, scene500.MaxQueuedMotions);
        Assert.Equal(128, scene500.MaxQueuedCommandStages);
        Assert.Equal(250_000, scene500.MaxEstimatedTriangles);

        Assert.Equal("scene-1000-plus", scene1000Plus.Profile);
        Assert.Equal(1_200, scene1000Plus.MaxSceneObjects);
        Assert.Equal(1_000, scene1000Plus.MaxActiveMotions);
        Assert.Equal(2_000, scene1000Plus.MaxQueuedMotions);
        Assert.Equal(256, scene1000Plus.MaxQueuedCommandStages);
        Assert.Equal(500_000, scene1000Plus.MaxEstimatedTriangles);
    }

    [Fact]
    public void Runtime_diagnostics_round_trips_large_scene_performance_budget_counters()
    {
        const string json = """
            {
              "batchDurationMs": 37,
              "fullSceneRebuildCount": 1,
              "transformOnlyPatchCount": 1000,
              "assetCacheEntryCount": 24,
              "disposedTemplateCount": 6,
              "assetCachePendingDisposalCount": 0,
              "assetCacheDisposedPromiseCount": 6,
              "queuedMotionCount": 750,
              "maxMotionQueueLength": 12,
              "queuedCommandStageCount": 15,
              "runtimeBudgetProfile": "scene-1000-plus",
              "runtimeBudgetWarningCount": 0
            }
            """;

        var diagnostics = JsonSerializer.Deserialize<WebGlRuntimeDiagnostics>(json, JsonOptions);

        Assert.NotNull(diagnostics);
        Assert.Equal(37, diagnostics.BatchDurationMs);
        Assert.Equal(1, diagnostics.FullSceneRebuildCount);
        Assert.Equal(1000, diagnostics.TransformOnlyPatchCount);
        Assert.Equal(24, diagnostics.AssetCacheEntryCount);
        Assert.Equal(6, diagnostics.DisposedTemplateCount);
        Assert.Equal(0, diagnostics.AssetCachePendingDisposalCount);
        Assert.Equal(6, diagnostics.AssetCacheDisposedPromiseCount);
        Assert.Equal(750, diagnostics.QueuedMotionCount);
        Assert.Equal(12, diagnostics.MaxMotionQueueLength);
        Assert.Equal(15, diagnostics.QueuedCommandStageCount);
        Assert.Equal("scene-1000-plus", diagnostics.RuntimeBudgetProfile);
        Assert.Equal(0, diagnostics.RuntimeBudgetWarningCount);
    }

    [Fact]
    public void Runtime_diagnostics_deserializes_browser_resource_cache_capture_shape()
    {
        const string json = """
            {
              "loadedAssetCount": 1,
              "missingAssetCount": 0,
              "fallbackObjectCount": 0,
              "modelInstanceCount": 2,
              "activeAssetProfile": "model-high",
              "largestLoadedAssetId": "asset.symbol.marker.default:asset.symbol.marker.default.model-low",
              "assetCacheMode": "state-local",
              "assetCacheEntryCount": 1,
              "assetCacheHitCount": 4,
              "assetCacheMissCount": 1,
              "disposedTemplateCount": 1,
              "assetCachePendingDisposalCount": 0,
              "assetCacheDisposedPromiseCount": 1,
              "assetCacheDisposalErrorCount": 0,
              "disposedTextureCount": 2,
              "retainedSharedTextureCount": 6,
              "linksUpdatedLastFrame": 0,
              "linkSyncScanCount": 0,
              "linkSyncIndexedHitCount": 0,
              "missingAssetIds": [],
              "missingFallbackAssetIds": [],
              "modelDiagnostics": [
                {
                  "assetId": "asset.symbol.marker.default",
                  "variantId": "asset.symbol.marker.default.model-low",
                  "uri": "http://127.0.0.1:5284/_content/CanDoItAll.Components.WebGlLib/assets/model/question_box.glb",
                  "hasScene": true,
                  "meshCount": 9,
                  "visibleMeshCount": 9,
                  "materialCount": 9,
                  "warnings": [ "Model bounds are extreme; unit scale or axis conversion may be wrong." ],
                  "errors": []
                }
              ]
            }
            """;

        var diagnostics = JsonSerializer.Deserialize<WebGlRuntimeDiagnostics>(json, JsonOptions);

        Assert.NotNull(diagnostics);
        Assert.Equal(1, diagnostics.LoadedAssetCount);
        Assert.Equal(0, diagnostics.MissingAssetCount);
        Assert.Equal(0, diagnostics.FallbackObjectCount);
        Assert.Equal(2, diagnostics.ModelInstanceCount);
        Assert.Equal("model-high", diagnostics.ActiveAssetProfile);
        Assert.Equal("state-local", diagnostics.AssetCacheMode);
        Assert.Equal(4, diagnostics.AssetCacheHitCount);
        Assert.Equal(1, diagnostics.AssetCacheMissCount);
        Assert.Equal(1, diagnostics.DisposedTemplateCount);
        Assert.Equal(0, diagnostics.AssetCachePendingDisposalCount);
        Assert.Equal(1, diagnostics.AssetCacheDisposedPromiseCount);
        Assert.Equal(0, diagnostics.AssetCacheDisposalErrorCount);
        Assert.Equal(2, diagnostics.DisposedTextureCount);
        Assert.Equal(6, diagnostics.RetainedSharedTextureCount);
        Assert.Equal(0, diagnostics.LinksUpdatedLastFrame);
        Assert.Equal(0, diagnostics.LinkSyncScanCount);
        Assert.Equal(0, diagnostics.LinkSyncIndexedHitCount);
        Assert.Empty(diagnostics.MissingAssetIds);
        Assert.Empty(diagnostics.MissingFallbackAssetIds);
        Assert.Single(diagnostics.ModelDiagnostics);
        Assert.Equal(9, diagnostics.ModelDiagnostics[0].MeshCount);
    }

    [Fact]
    public void Proof_snapshot_round_trips_link_sync_and_asset_cache_counters()
    {
        const string json = """
            {
              "assetCacheMode": "state-local",
              "assetCacheEntryCount": 0,
              "assetCacheHitCount": 3,
              "assetCacheMissCount": 1,
              "disposedTemplateCount": 1,
              "assetCachePendingDisposalCount": 0,
              "assetCacheDisposedPromiseCount": 1,
              "assetCacheDisposalErrorCount": 0,
              "disposedTextureCount": 2,
              "retainedSharedTextureCount": 4,
              "linkGeometryUpdateCount": 10,
              "linkGeometryRebuildCount": 10,
              "linksUpdatedLastFrame": 2,
              "linkSyncScanCount": 8,
              "linkSyncIndexedHitCount": 7
            }
            """;

        var snapshot = JsonSerializer.Deserialize<WebGlSceneProofSnapshot>(json, JsonOptions);

        Assert.NotNull(snapshot);
        Assert.Equal("state-local", snapshot.AssetCacheMode);
        Assert.Equal(0, snapshot.AssetCacheEntryCount);
        Assert.Equal(3, snapshot.AssetCacheHitCount);
        Assert.Equal(1, snapshot.AssetCacheMissCount);
        Assert.Equal(1, snapshot.DisposedTemplateCount);
        Assert.Equal(0, snapshot.AssetCachePendingDisposalCount);
        Assert.Equal(1, snapshot.AssetCacheDisposedPromiseCount);
        Assert.Equal(0, snapshot.AssetCacheDisposalErrorCount);
        Assert.Equal(2, snapshot.DisposedTextureCount);
        Assert.Equal(4, snapshot.RetainedSharedTextureCount);
        Assert.Equal(10, snapshot.LinkGeometryUpdateCount);
        Assert.Equal(10, snapshot.LinkGeometryRebuildCount);
        Assert.Equal(2, snapshot.LinksUpdatedLastFrame);
        Assert.Equal(8, snapshot.LinkSyncScanCount);
        Assert.Equal(7, snapshot.LinkSyncIndexedHitCount);
    }
}
