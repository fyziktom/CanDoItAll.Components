namespace CanDoItAll.Components.WebGlLib;

public static class WebGlRuntimeBudgetProfiles
{
    public static WebGlRuntimeBudgetOptions Scene100()
        => new()
        {
            Profile = "scene-100",
            MaxSceneObjects = 100,
            MaxLoadedAssets = 64,
            MaxAssetCacheEntries = 64,
            MaxActiveMotions = 128,
            MaxQueuedMotions = 256,
            MaxQueuedCommandStages = 64,
            MaxEstimatedTriangles = 100_000
        };

    public static WebGlRuntimeBudgetOptions Scene500()
        => new()
        {
            Profile = "scene-500",
            MaxSceneObjects = 500,
            MaxLoadedAssets = 128,
            MaxAssetCacheEntries = 128,
            MaxActiveMotions = 500,
            MaxQueuedMotions = 1_000,
            MaxQueuedCommandStages = 128,
            MaxEstimatedTriangles = 250_000
        };

    public static WebGlRuntimeBudgetOptions Scene1000Plus()
        => new()
        {
            Profile = "scene-1000-plus",
            MaxSceneObjects = 1_200,
            MaxLoadedAssets = 192,
            MaxAssetCacheEntries = 192,
            MaxActiveMotions = 1_000,
            MaxQueuedMotions = 2_000,
            MaxQueuedCommandStages = 256,
            MaxEstimatedTriangles = 500_000
        };
}
