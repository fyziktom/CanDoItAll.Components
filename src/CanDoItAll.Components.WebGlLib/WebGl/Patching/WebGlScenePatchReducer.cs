namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlScenePatchReducer
{
    public WebGlScenePatchResult Apply(WebGlSceneModel scene, WebGlScenePatch patch)
    {
        ArgumentNullException.ThrowIfNull(scene);
        ArgumentNullException.ThrowIfNull(patch);

        var result = Validate(scene, patch);
        if (!result.IsValid)
        {
            return result;
        }

        var objectsById = scene.Objects.ToDictionary(item => item.Id, StringComparer.Ordinal);
        foreach (var objectId in patch.RemoveObjectIds.Where(static id => !string.IsNullOrWhiteSpace(id)))
        {
            scene.Objects.RemoveAll(item => string.Equals(item.Id, objectId, StringComparison.Ordinal));
            result.RemovedObjectIds.Add(objectId);
        }

        foreach (var sceneObject in patch.AddObjects.Where(static item => !string.IsNullOrWhiteSpace(item.Id)))
        {
            scene.Objects.RemoveAll(item => string.Equals(item.Id, sceneObject.Id, StringComparison.Ordinal));
            scene.Objects.Add(sceneObject);
            objectsById[sceneObject.Id] = sceneObject;
            result.AddedObjectIds.Add(sceneObject.Id);
        }

        foreach (var objectPatch in patch.ObjectPatches.Where(static item => !string.IsNullOrWhiteSpace(item.ObjectId)))
        {
            var target = scene.Objects.FirstOrDefault(
                item => string.Equals(item.Id, objectPatch.ObjectId, StringComparison.Ordinal));
            if (target is null)
            {
                result.Warnings.Add($"Object patch target '{objectPatch.ObjectId}' was not found.");
                continue;
            }

            ApplyObjectPatch(target, objectPatch);
            result.PatchedObjectIds.Add(target.Id);
        }

        foreach (var linkId in patch.RemoveLinkIds.Where(static id => !string.IsNullOrWhiteSpace(id)))
        {
            scene.Links.RemoveAll(item => string.Equals(item.Id, linkId, StringComparison.Ordinal));
            result.RemovedLinkIds.Add(linkId);
        }

        foreach (var link in patch.AddLinks.Where(static item => !string.IsNullOrWhiteSpace(item.Id)))
        {
            scene.Links.RemoveAll(item => string.Equals(item.Id, link.Id, StringComparison.Ordinal));
            scene.Links.Add(link);
            result.AddedLinkIds.Add(link.Id);
        }

        scene.UiState.Revision = patch.NextRevision > 0
            ? patch.NextRevision
            : scene.UiState.Revision + 1;
        result.NextRevision = scene.UiState.Revision;
        return result;
    }

    public WebGlScenePatchResult Validate(WebGlSceneModel scene, WebGlScenePatch patch)
    {
        var result = new WebGlScenePatchResult();
        if (!string.IsNullOrWhiteSpace(patch.SceneId) &&
            !string.Equals(scene.SceneId, patch.SceneId, StringComparison.Ordinal))
        {
            result.Errors.Add($"Patch scene id '{patch.SceneId}' does not match scene '{scene.SceneId}'.");
        }

        if (patch.BaseRevision > 0 && patch.BaseRevision != scene.UiState.Revision)
        {
            result.Warnings.Add($"Patch base revision {patch.BaseRevision} does not match scene revision {scene.UiState.Revision}.");
        }

        foreach (var objectPatch in patch.ObjectPatches)
        {
            if (string.IsNullOrWhiteSpace(objectPatch.ObjectId))
            {
                result.Errors.Add("Object patch id is required.");
            }
        }

        return result;
    }

    private static void ApplyObjectPatch(WebGlSceneObject target, WebGlSceneObjectPatch patch)
    {
        if (patch.Position.HasValue)
        {
            target.Position = patch.Position.Value;
        }

        if (patch.Rotation.HasValue)
        {
            target.Rotation = patch.Rotation.Value;
        }

        if (patch.Scale.HasValue)
        {
            target.Scale = patch.Scale.Value;
        }

        if (patch.Size.HasValue)
        {
            target.Size = patch.Size.Value;
        }

        if (patch.AssetId is not null)
        {
            target.AssetId = patch.AssetId;
        }

        if (patch.Color is not null)
        {
            target.Color = patch.Color;
        }

        if (patch.Symbols is not null)
        {
            target.Symbols = patch.Symbols;
        }

        if (patch.Metadata is not null)
        {
            target.Metadata = new Dictionary<string, string>(patch.Metadata, StringComparer.Ordinal);
        }
    }
}

public sealed class WebGlScenePatchResult
{
    public List<string> Errors { get; } = [];

    public List<string> Warnings { get; } = [];

    public List<string> PatchedObjectIds { get; } = [];

    public List<string> AddedObjectIds { get; } = [];

    public List<string> RemovedObjectIds { get; } = [];

    public List<string> AddedLinkIds { get; } = [];

    public List<string> RemovedLinkIds { get; } = [];

    public int NextRevision { get; set; }

    public bool IsValid => Errors.Count == 0;
}
