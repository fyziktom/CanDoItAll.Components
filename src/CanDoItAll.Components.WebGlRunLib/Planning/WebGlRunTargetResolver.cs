using System.Globalization;
using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunTargetResolver
{
    public WebGlRunActionPlanningDiagnostics Diagnostics { get; } = new();

    public WebGlVector3? Resolve(WebGlRunActionTarget target, WebGlRunPlanningContext context)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(context);

        if (target.Position is { } position)
        {
            Diagnostics.Metadata["resolution"] = "explicit-position";
            return Add(position, target.Offset);
        }

        if (string.IsNullOrWhiteSpace(target.ObjectId))
        {
            Diagnostics.Errors.Add("Target object id or explicit position is required.");
            return null;
        }

        if (!context.ObjectIndex.TryGetValue(target.ObjectId, out WebGlSceneObject? sceneObject))
        {
            Diagnostics.Errors.Add($"Target object '{target.ObjectId}' was not found.");
            return null;
        }

        string anchorKey = string.IsNullOrWhiteSpace(target.AnchorKey)
            ? WebGlRunAnchorKeys.Center
            : target.AnchorKey;
        return ResolveAnchor(sceneObject, anchorKey, target.Offset);
    }

    public WebGlVector3 ResolveAnchor(WebGlSceneObject sceneObject, string anchorKey, WebGlVector3 offset)
    {
        WebGlSceneObjectAnchor? explicitAnchor = sceneObject.Anchors.FirstOrDefault(
            item => string.Equals(item.Key, anchorKey, StringComparison.OrdinalIgnoreCase));
        if (explicitAnchor is not null)
        {
            Diagnostics.Metadata["resolution"] = "object-anchor";
            return Add(explicitAnchor.Position ?? Add(sceneObject.Position, explicitAnchor.Offset), offset);
        }

        if (TryResolveMetadataAnchor(sceneObject, anchorKey, out WebGlVector3 metadataAnchor))
        {
            Diagnostics.Metadata["resolution"] = "metadata-anchor";
            return Add(metadataAnchor, offset);
        }

        WebGlVector3 position = sceneObject.Position;
        WebGlVector3 half = new(sceneObject.Size.X / 2, sceneObject.Size.Y / 2, sceneObject.Size.Z / 2);
        WebGlVector3 resolved = anchorKey.ToLowerInvariant() switch
        {
            WebGlRunAnchorKeys.Base => position,
            WebGlRunAnchorKeys.Top => Add(position, new WebGlVector3(0, sceneObject.Size.Y, 0)),
            WebGlRunAnchorKeys.Front => Add(position, new WebGlVector3(0, 0, half.Z)),
            WebGlRunAnchorKeys.Back => Add(position, new WebGlVector3(0, 0, -half.Z)),
            WebGlRunAnchorKeys.Left => Add(position, new WebGlVector3(-half.X, 0, 0)),
            WebGlRunAnchorKeys.Right => Add(position, new WebGlVector3(half.X, 0, 0)),
            WebGlRunAnchorKeys.Home or WebGlRunAnchorKeys.Work or WebGlRunAnchorKeys.Use or WebGlRunAnchorKeys.Admin => position,
            _ => position
        };

        Diagnostics.Metadata["resolution"] = IsBuiltInAnchor(anchorKey) ? "built-in-anchor" : "object-center-fallback";
        if (!string.Equals(anchorKey, WebGlRunAnchorKeys.Center, StringComparison.OrdinalIgnoreCase) &&
            !IsBuiltInAnchor(anchorKey))
        {
            Diagnostics.Warnings.Add($"Anchor '{anchorKey}' was not defined on object '{sceneObject.Id}', so center was used.");
        }

        return Add(resolved, offset);
    }

    private static bool IsBuiltInAnchor(string anchorKey)
        => anchorKey.Equals(WebGlRunAnchorKeys.Center, StringComparison.OrdinalIgnoreCase) ||
           anchorKey.Equals(WebGlRunAnchorKeys.Base, StringComparison.OrdinalIgnoreCase) ||
           anchorKey.Equals(WebGlRunAnchorKeys.Top, StringComparison.OrdinalIgnoreCase) ||
           anchorKey.Equals(WebGlRunAnchorKeys.Front, StringComparison.OrdinalIgnoreCase) ||
           anchorKey.Equals(WebGlRunAnchorKeys.Back, StringComparison.OrdinalIgnoreCase) ||
           anchorKey.Equals(WebGlRunAnchorKeys.Left, StringComparison.OrdinalIgnoreCase) ||
           anchorKey.Equals(WebGlRunAnchorKeys.Right, StringComparison.OrdinalIgnoreCase) ||
           anchorKey.Equals(WebGlRunAnchorKeys.Home, StringComparison.OrdinalIgnoreCase) ||
           anchorKey.Equals(WebGlRunAnchorKeys.Work, StringComparison.OrdinalIgnoreCase) ||
           anchorKey.Equals(WebGlRunAnchorKeys.Use, StringComparison.OrdinalIgnoreCase) ||
           anchorKey.Equals(WebGlRunAnchorKeys.Admin, StringComparison.OrdinalIgnoreCase);

    private static bool TryResolveMetadataAnchor(WebGlSceneObject sceneObject, string anchorKey, out WebGlVector3 position)
    {
        position = WebGlVector3.Zero;
        if (!sceneObject.Metadata.TryGetValue($"anchor.{anchorKey}", out string? encoded))
        {
            return false;
        }

        string[] parts = encoded.Split(',', StringSplitOptions.TrimEntries);
        if (parts.Length != 3 ||
            !decimal.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out decimal x) ||
            !decimal.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out decimal y) ||
            !decimal.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out decimal z))
        {
            return false;
        }

        position = new WebGlVector3((double)x, (double)y, (double)z);
        return true;
    }

    private static WebGlVector3 Add(WebGlVector3 left, WebGlVector3 right)
        => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
}
