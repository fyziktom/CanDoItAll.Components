namespace CanDoItAll.Components.WebGlLib;

public static class WebGlSceneRevisionPolicy
{
    public static int Resolve(WebGlSceneModel? scene)
    {
        if (scene is null)
        {
            return 0;
        }

        return scene.Revision > 0
            ? scene.Revision
            : Math.Max(0, scene.UiState?.Revision ?? 0);
    }

    public static int ResolveNext(WebGlSceneModel scene, int requestedRevision)
        => requestedRevision > 0 ? requestedRevision : Resolve(scene) + 1;

    public static void Commit(WebGlSceneModel scene, int revision)
    {
        ArgumentNullException.ThrowIfNull(scene);
        revision = Math.Max(0, revision);
        scene.Revision = revision;
        scene.UiState ??= new WebGlSceneUiState();
        scene.UiState.Revision = revision;
    }

    public static void Normalize(WebGlSceneModel scene)
    {
        ArgumentNullException.ThrowIfNull(scene);
        scene.Revision = Resolve(scene);
    }
}
