namespace CanDoItAll.Components.WebGlLib;

internal static class WebGlSceneDocumentValidator
{
    public static WebGlSceneDocumentValidationResult Validate(WebGlSceneDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var result = new WebGlSceneDocumentValidationResult();
        if (!string.Equals(document.SchemaVersion, WebGlSceneDocumentSerializer.CurrentSchemaVersion, StringComparison.Ordinal))
        {
            result.Errors.Add($"Unsupported WebGL scene document schema '{document.SchemaVersion}'.");
        }

        if (string.IsNullOrWhiteSpace(document.Scene.SceneId))
        {
            result.Errors.Add("Scene id is required.");
        }

        ValidateForbiddenMetadata(document, result);
        ValidateObjectIds(document.Scene, result);
        ValidateLinks(document.Scene, result);
        ValidateAssets(document.Scene, result);
        ValidateVectors(document.Scene, result);
        return result;
    }

    private static void ValidateForbiddenMetadata(WebGlSceneDocument document, WebGlSceneDocumentValidationResult result)
    {
        foreach (var (scope, metadata) in WebGlSceneDocumentMetadataPolicy.EnumerateMetadataScopes(document))
        {
            foreach (var key in metadata.Keys)
            {
                if (key.StartsWith("source.", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (WebGlSceneDocumentMetadataPolicy.IsForbiddenMetadataKey(key))
                {
                    result.Errors.Add($"Run-layer or domain metadata key '{key}' does not belong in generic WebGL scene document scope '{scope}'.");
                }
            }
        }
    }

    private static void ValidateObjectIds(WebGlSceneModel scene, WebGlSceneDocumentValidationResult result)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var sceneObject in scene.Objects)
        {
            if (string.IsNullOrWhiteSpace(sceneObject.Id))
            {
                result.Errors.Add("Scene object id is required.");
                continue;
            }

            if (!ids.Add(sceneObject.Id))
            {
                result.Errors.Add($"Duplicate scene object id '{sceneObject.Id}'.");
            }
        }
    }

    private static void ValidateLinks(WebGlSceneModel scene, WebGlSceneDocumentValidationResult result)
    {
        var ids = scene.Objects
            .Select(static item => item.Id)
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .ToHashSet(StringComparer.Ordinal);
        var linkIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var link in scene.Links)
        {
            if (string.IsNullOrWhiteSpace(link.Id))
            {
                result.Errors.Add("Scene link id is required.");
            }
            else if (!linkIds.Add(link.Id))
            {
                result.Errors.Add($"Duplicate scene link id '{link.Id}'.");
            }

            if (!ids.Contains(link.SourceObjectId) || !ids.Contains(link.TargetObjectId))
            {
                result.Errors.Add($"Scene link '{link.Id}' references missing endpoint(s): '{link.SourceObjectId}' -> '{link.TargetObjectId}'.");
            }
        }
    }

    private static void ValidateAssets(WebGlSceneModel scene, WebGlSceneDocumentValidationResult result)
    {
        var assetIds = scene.AssetCatalog.Assets
            .Select(static item => item.Id)
            .Where(static id => !string.IsNullOrWhiteSpace(id))
            .ToHashSet(StringComparer.Ordinal);
        foreach (var sceneObject in scene.Objects.Where(item => !string.IsNullOrWhiteSpace(item.AssetId) && !assetIds.Contains(item.AssetId)))
        {
            result.Warnings.Add($"Scene object '{sceneObject.Id}' references asset '{sceneObject.AssetId}' that is not present in the catalog.");
        }

        foreach (var asset in scene.AssetCatalog.Assets)
        {
            if (string.IsNullOrWhiteSpace(asset.Id))
            {
                result.Errors.Add("Asset id is required.");
            }

            if (!string.IsNullOrWhiteSpace(asset.FallbackAssetId) && !assetIds.Contains(asset.FallbackAssetId))
            {
                result.Warnings.Add($"Asset '{asset.Id}' references fallback asset '{asset.FallbackAssetId}' that is not present in the catalog.");
            }

            foreach (var variant in asset.Variants.Where(variant => !string.IsNullOrWhiteSpace(variant.FallbackAssetId) && !assetIds.Contains(variant.FallbackAssetId)))
            {
                result.Warnings.Add($"Asset '{asset.Id}' variant '{variant.Id}' references fallback asset '{variant.FallbackAssetId}' that is not present in the catalog.");
            }
        }
    }

    private static void ValidateVectors(WebGlSceneModel scene, WebGlSceneDocumentValidationResult result)
    {
        ValidateVector("camera target", scene.Camera.Target, result);
        foreach (var sceneObject in scene.Objects)
        {
            ValidateVector($"object '{sceneObject.Id}' position", sceneObject.Position, result);
            ValidateVector($"object '{sceneObject.Id}' rotation", sceneObject.Rotation, result);
            ValidateVector($"object '{sceneObject.Id}' scale", sceneObject.Scale, result);
            ValidateVector($"object '{sceneObject.Id}' size", sceneObject.Size, result);
        }

        foreach (var asset in scene.AssetCatalog.Assets)
        {
            ValidateVector($"asset '{asset.Id}' bounds hint", asset.BoundsHint, result);
            ValidateVector($"asset '{asset.Id}' import rotation offset", asset.ImportOptions.RotationOffset, result);
            ValidateVector($"asset '{asset.Id}' import position offset", asset.ImportOptions.PositionOffset, result);
            foreach (var variant in asset.Variants)
            {
                ValidateVector($"asset '{asset.Id}' variant '{variant.Id}' scale", variant.Scale, result);
                ValidateVector($"asset '{asset.Id}' variant '{variant.Id}' import rotation offset", variant.ImportOptions.RotationOffset, result);
                ValidateVector($"asset '{asset.Id}' variant '{variant.Id}' import position offset", variant.ImportOptions.PositionOffset, result);
            }
        }
    }

    private static void ValidateVector(string scope, WebGlVector3 vector, WebGlSceneDocumentValidationResult result)
    {
        if (!double.IsFinite(vector.X) || !double.IsFinite(vector.Y) || !double.IsFinite(vector.Z))
        {
            result.Errors.Add($"Invalid vector value in {scope}.");
        }
    }
}
