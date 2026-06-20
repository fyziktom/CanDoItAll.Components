namespace CanDoItAll.Components.WebGlLib;

public static class WebGlRuntimeBudgetProfiles
{
    public static WebGlRuntimeBudgetOptions Small()
    {
        WebGlRuntimeBudgetOptions profile = Scene100();
        profile.Profile = "small";
        return profile;
    }

    public static WebGlRuntimeBudgetOptions Medium()
    {
        WebGlRuntimeBudgetOptions profile = Scene500();
        profile.Profile = "medium";
        return profile;
    }

    public static WebGlRuntimeBudgetOptions Large()
    {
        WebGlRuntimeBudgetOptions profile = Scene1000Plus();
        profile.Profile = "large";
        return profile;
    }

    public static WebGlRuntimeBudgetOptions Stress()
        => new()
        {
            Profile = "stress",
            MaxSceneObjects = 2_500,
            MaxLoadedAssets = 256,
            MaxAssetCacheEntries = 256,
            MaxActiveMotions = 2_000,
            MaxQueuedMotions = 4_000,
            MaxQueuedCommandStages = 512,
            MaxEstimatedTriangles = 1_000_000
        };

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
