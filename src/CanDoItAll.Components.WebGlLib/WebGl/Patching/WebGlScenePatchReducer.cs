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

        var objectsById = scene.Objects
            .Where(static item => !string.IsNullOrWhiteSpace(item.Id))
            .ToDictionary(item => item.Id, StringComparer.Ordinal);
        foreach (var objectId in patch.RemoveObjectIds.Where(static id => !string.IsNullOrWhiteSpace(id)))
        {
            if (!objectsById.ContainsKey(objectId))
            {
                result.Warnings.Add($"Object '{objectId}' was not found for removal.");
                continue;
            }

            scene.Objects.RemoveAll(item => string.Equals(item.Id, objectId, StringComparison.Ordinal));
            var removedLinkIds = scene.Links
                .Where(item => string.Equals(item.SourceObjectId, objectId, StringComparison.Ordinal) ||
                               string.Equals(item.TargetObjectId, objectId, StringComparison.Ordinal))
                .Select(static item => item.Id)
                .Where(static id => !string.IsNullOrWhiteSpace(id))
                .ToArray();
            scene.Links.RemoveAll(item => string.Equals(item.SourceObjectId, objectId, StringComparison.Ordinal) ||
                                          string.Equals(item.TargetObjectId, objectId, StringComparison.Ordinal));
            foreach (var layer in scene.Layers)
            {
                layer.ObjectIds.RemoveAll(id => string.Equals(id, objectId, StringComparison.Ordinal));
            }

            objectsById.Remove(objectId);
            result.RemovedObjectIds.Add(objectId);
            result.RemovedLinkIds.AddRange(removedLinkIds);
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
            if (scene.Links.All(item => !string.Equals(item.Id, linkId, StringComparison.Ordinal)))
            {
                result.Warnings.Add($"Link '{linkId}' was not found for removal.");
                continue;
            }

            scene.Links.RemoveAll(item => string.Equals(item.Id, linkId, StringComparison.Ordinal));
            result.RemovedLinkIds.Add(linkId);
        }

        var missingLinkEndpointMode = WebGlScenePatchPolicy.ResolveMissingLinkEndpointMode(result);
        foreach (var link in patch.AddLinks.Where(static item => !string.IsNullOrWhiteSpace(item.Id)))
        {
            if (!objectsById.ContainsKey(link.SourceObjectId) || !objectsById.ContainsKey(link.TargetObjectId))
            {
                var message = $"Link '{link.Id}' references missing endpoint(s): '{link.SourceObjectId}' -> '{link.TargetObjectId}'.";
                if (string.Equals(missingLinkEndpointMode, WebGlScenePatchPolicy.MissingLinkEndpointModeWarn, StringComparison.OrdinalIgnoreCase))
                {
                    WebGlScenePatchPolicy.AddWarning(result, message);
                    WebGlScenePatchPolicy.AddSkippedLinkId(result, link.Id);
                    continue;
                }

                result.Errors.Add(message);
                continue;
            }

            scene.Links.RemoveAll(item => string.Equals(item.Id, link.Id, StringComparison.Ordinal));
            scene.Links.Add(link);
            result.AddedLinkIds.Add(link.Id);
        }

        if (result.AffectedObjectIds.Count == 0 && result.AffectedLinkIds.Count == 0)
        {
            result.NextRevision = WebGlSceneRevisionPolicy.Resolve(scene);
            return result;
        }

        WebGlSceneRevisionPolicy.Commit(scene, WebGlSceneRevisionPolicy.ResolveNext(scene, patch.NextRevision));
        result.NextRevision = scene.Revision;
        result.Revision = scene.Revision;
        result.SceneId = scene.SceneId;
        return result;
    }

    public WebGlScenePatchResult Validate(WebGlSceneModel scene, WebGlScenePatch patch)
    {
        var result = new WebGlScenePatchResult
        {
            SceneId = scene.SceneId,
            Revision = WebGlSceneRevisionPolicy.Resolve(scene)
        };
        WebGlScenePatchPolicy.PopulateResultMetadata(result, patch);
        var missingLinkEndpointMode = WebGlScenePatchPolicy.ResolveMissingLinkEndpointMode(result);

        if (!string.IsNullOrWhiteSpace(patch.SceneId) &&
            !string.Equals(scene.SceneId, patch.SceneId, StringComparison.Ordinal))
        {
            result.Errors.Add($"Patch scene id '{patch.SceneId}' does not match scene '{scene.SceneId}'.");
        }

        var currentRevision = WebGlSceneRevisionPolicy.Resolve(scene);
        if (patch.BaseRevision > 0 && patch.BaseRevision != currentRevision)
        {
            var message = $"Patch base revision {patch.BaseRevision} does not match scene revision {currentRevision}.";
            if (WebGlScenePatchPolicy.IsStrictBaseRevision(patch))
            {
                result.Errors.Add(message);
            }
            else
            {
                result.Warnings.Add(message);
            }
        }

        foreach (var sceneObject in patch.AddObjects)
        {
            if (string.IsNullOrWhiteSpace(sceneObject.Id))
            {
                result.Errors.Add("Added object id is required.");
            }
        }

        var availableObjectIds = scene.Objects
            .Select(static item => item.Id)
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .Except(patch.RemoveObjectIds.Where(static id => !string.IsNullOrWhiteSpace(id)), StringComparer.Ordinal)
            .Concat(patch.AddObjects.Select(static item => item.Id).Where(static id => !string.IsNullOrWhiteSpace(id)))
            .ToHashSet(StringComparer.Ordinal);
        foreach (var objectPatch in patch.ObjectPatches)
        {
            if (string.IsNullOrWhiteSpace(objectPatch.ObjectId))
            {
                result.Errors.Add("Object patch id is required.");
                continue;
            }

            if (!availableObjectIds.Contains(objectPatch.ObjectId))
            {
                result.Errors.Add($"Object patch target '{objectPatch.ObjectId}' was not found.");
            }
        }

        foreach (var link in patch.AddLinks)
        {
            if (string.IsNullOrWhiteSpace(link.Id))
            {
                result.Errors.Add("Added link id is required.");
                continue;
            }

            if (availableObjectIds.Contains(link.SourceObjectId) && availableObjectIds.Contains(link.TargetObjectId))
            {
                continue;
            }

            var message = $"Link '{link.Id}' references missing endpoint(s): '{link.SourceObjectId}' -> '{link.TargetObjectId}'.";
            if (string.Equals(missingLinkEndpointMode, WebGlScenePatchPolicy.MissingLinkEndpointModeWarn, StringComparison.OrdinalIgnoreCase))
            {
                WebGlScenePatchPolicy.AddWarning(result, message);
                WebGlScenePatchPolicy.AddSkippedLinkId(result, link.Id);
            }
            else
            {
                result.Errors.Add(message);
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
