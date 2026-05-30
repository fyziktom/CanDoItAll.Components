using System.Globalization;
using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public interface IWebGlRunActionPlanner
{
    WebGlRunActionPlan Plan(WebGlRunAction action, WebGlRunPlanningContext context);
}

public sealed class WebGlRunPlanningContext
{
    public WebGlSceneModel Scene { get; set; } = new();

    public WebGlVisualStateCatalog VisualStates { get; set; } = new();

    public Dictionary<string, WebGlVector3> ObjectPositions { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunActionPlanningDiagnostics
{
    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunTargetResolver
{
    public WebGlRunActionPlanningDiagnostics Diagnostics { get; } = new();

    public WebGlVector3? Resolve(WebGlRunActionTarget target, WebGlRunPlanningContext context)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(context);

        if (target.Position is { } position)
        {
            return Add(position, target.Offset);
        }

        if (string.IsNullOrWhiteSpace(target.ObjectId))
        {
            Diagnostics.Errors.Add("Target object id or explicit position is required.");
            return null;
        }

        WebGlSceneObject? sceneObject = context.Scene.Objects.FirstOrDefault(
            item => string.Equals(item.Id, target.ObjectId, StringComparison.Ordinal));
        if (sceneObject is null)
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
            return Add(explicitAnchor.Position ?? Add(sceneObject.Position, explicitAnchor.Offset), offset);
        }

        if (TryResolveMetadataAnchor(sceneObject, anchorKey, out WebGlVector3 metadataAnchor))
        {
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

public sealed class WebGlRunActionPlanner : IWebGlRunActionPlanner
{
    public WebGlRunActionPlan Plan(WebGlRunAction action, WebGlRunPlanningContext context)
    {
        ArgumentNullException.ThrowIfNull(action);
        ArgumentNullException.ThrowIfNull(context);

        var plan = new WebGlRunActionPlan
        {
            ActionId = action.ActionId,
            FrameRate = 1
        };
        AppendAction(action, context, plan);
        return plan;
    }

    private void AppendAction(WebGlRunAction action, WebGlRunPlanningContext context, WebGlRunActionPlan plan)
    {
        string kind = NormalizeKind(action.ResolvedKind);
        switch (kind)
        {
            case WebGlRunActionKinds.Sequence:
            case WebGlRunActionKinds.Parallel:
                foreach (WebGlRunAction step in action.Steps)
                {
                    AppendAction(step, context, plan);
                }
                break;
            case WebGlRunActionKinds.MoveToObject:
                AddMoveToObject(action, context, plan);
                break;
            case WebGlRunActionKinds.MoveToPosition:
                AddMoveToPosition(action, plan);
                break;
            case WebGlRunActionKinds.ReturnToAnchor:
                AddReturnToAnchor(action, context, plan);
                break;
            case WebGlRunActionKinds.ChangePose:
            case WebGlRunActionKinds.SetPose:
                AddPosePatch(action, context, plan);
                break;
            case WebGlRunActionKinds.ShowSymbol:
            case WebGlRunActionKinds.UpdateSymbol:
                AddSymbolPatch(action, context, plan, replaceSymbols: false);
                break;
            case WebGlRunActionKinds.HideSymbol:
                AddSymbolPatch(action, context, plan, replaceSymbols: true);
                break;
            case WebGlRunActionKinds.SetLayerVisibility:
            case WebGlRunActionKinds.Wait:
                plan.Warnings.Add($"Action '{action.ActionId}' of kind '{kind}' does not emit a WebGL command.");
                break;
            case WebGlRunActionKinds.ApplyPatch:
            case WebGlRunActionKinds.ApplyScenePatch:
                plan.Patches.Add(new WebGlScenePatch
                {
                    Metadata =
                    {
                        ["actionId"] = action.ActionId,
                        ["actionKind"] = kind
                    }
                });
                break;
            default:
                plan.Errors.Add($"Unsupported WebGL run action kind '{kind}' for action '{action.ActionId}'.");
                break;
        }
    }

    private static void AddMoveToPosition(WebGlRunAction action, WebGlRunActionPlan plan)
    {
        WebGlVector3? target = action.Target.Position ?? ResolvePosition(action.Parameters);
        if (target is null)
        {
            plan.Errors.Add($"Action '{action.ActionId}' did not include a target position.");
            return;
        }

        AddMotion(action, target.Value, plan, WebGlRunActionKinds.MoveToPosition);
    }

    private static void AddMoveToObject(WebGlRunAction action, WebGlRunPlanningContext context, WebGlRunActionPlan plan)
    {
        var resolver = new WebGlRunTargetResolver();
        WebGlVector3? target = resolver.Resolve(new WebGlRunActionTarget
        {
            ObjectId = action.ResolvedTargetObjectId,
            AnchorKey = action.Target.AnchorKey,
            Offset = action.Target.Offset,
            Position = action.Target.Position
        }, context);
        plan.Errors.AddRange(resolver.Diagnostics.Errors);
        plan.Warnings.AddRange(resolver.Diagnostics.Warnings);
        if (target is { } targetPosition)
        {
            AddMotion(action, targetPosition, plan, WebGlRunActionKinds.MoveToObject);
        }
    }

    private static void AddReturnToAnchor(WebGlRunAction action, WebGlRunPlanningContext context, WebGlRunActionPlan plan)
    {
        string objectId = action.ResolvedObjectId;
        WebGlSceneObject? sceneObject = context.Scene.Objects.FirstOrDefault(item => string.Equals(item.Id, objectId, StringComparison.Ordinal));
        if (sceneObject is null)
        {
            plan.Errors.Add($"Return object '{objectId}' was not found.");
            return;
        }

        var resolver = new WebGlRunTargetResolver();
        WebGlVector3 target = resolver.ResolveAnchor(sceneObject, action.Target.AnchorKey, action.Target.Offset);
        plan.Warnings.AddRange(resolver.Diagnostics.Warnings);
        AddMotion(action, target, plan, WebGlRunActionKinds.ReturnToAnchor);
    }

    private static void AddPosePatch(WebGlRunAction action, WebGlRunPlanningContext context, WebGlRunActionPlan plan)
    {
        string poseKey = FirstNonEmpty(action.PoseKey, action.Parameters.GetValueOrDefault("poseKey"));
        WebGlPoseDefinition? pose = context.VisualStates.Poses.FirstOrDefault(
            item => string.Equals(item.PoseKey, poseKey, StringComparison.Ordinal));
        if (pose is null)
        {
            plan.Warnings.Add($"Pose '{poseKey}' was not found; metadata fallback marker was applied.");
        }

        WebGlSceneObjectPatch objectPatch = new()
        {
            ObjectId = action.ResolvedObjectId,
            AssetId = FirstNonEmpty(pose?.AssetId, action.Parameters.GetValueOrDefault("assetId"), null),
            Rotation = pose?.Rotation,
            Scale = pose?.Scale,
            Metadata = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["poseKey"] = poseKey,
                ["poseResolved"] = pose is null ? "false" : "true"
            }
        };
        if (!string.IsNullOrWhiteSpace(pose?.AssetVariantId))
        {
            objectPatch.Metadata["assetVariantId"] = pose.AssetVariantId;
        }

        AddPatch(action, plan, objectPatch, WebGlRunActionKinds.ChangePose);
    }

    private static void AddSymbolPatch(WebGlRunAction action, WebGlRunPlanningContext context, WebGlRunActionPlan plan, bool replaceSymbols)
    {
        if (replaceSymbols)
        {
            AddPatch(action, plan, new WebGlSceneObjectPatch { ObjectId = action.ResolvedObjectId, Symbols = [] }, WebGlRunActionKinds.HideSymbol);
            return;
        }

        string symbolKey = FirstNonEmpty(action.SymbolKey, action.Parameters.GetValueOrDefault("symbolKey"), action.Parameters.GetValueOrDefault("symbolKind"), "status");
        WebGlSymbolDefinition? symbol = context.VisualStates.Symbols.FirstOrDefault(
            item => string.Equals(item.SymbolKey, symbolKey, StringComparison.Ordinal));
        if (symbol is null)
        {
            plan.Warnings.Add($"Symbol '{symbolKey}' was not found; fallback marker was applied.");
        }

        WebGlStatusSymbol statusSymbol = new()
        {
            Id = FirstNonEmpty(action.Parameters.GetValueOrDefault("symbolId"), $"{action.ActionId}.symbol"),
            SemanticKind = FirstNonEmpty(symbol?.SemanticKind, action.Parameters.GetValueOrDefault("symbolKind"), symbolKey),
            SymbolAssetId = FirstNonEmpty(symbol?.SymbolAssetId, action.Parameters.GetValueOrDefault("symbolAssetId")),
            Color = FirstNonEmpty(symbol?.Color, action.Parameters.GetValueOrDefault("color"), "#facc15"),
            EffectKey = FirstNonEmpty(symbol?.EffectKey, action.Parameters.GetValueOrDefault("effectKey"), WebGlSymbolEffects.Pulse),
            Tooltip = FirstNonEmpty(symbol?.Tooltip, action.Parameters.GetValueOrDefault("tooltip"))
        };
        AddPatch(action, plan, new WebGlSceneObjectPatch { ObjectId = action.ResolvedObjectId, Symbols = [statusSymbol] }, WebGlRunActionKinds.ShowSymbol);
    }

    private static void AddMotion(WebGlRunAction action, WebGlVector3 target, WebGlRunActionPlan plan, string actionKind)
        => plan.Motions.Add(new WebGlObjectMotionCommand
        {
            MotionId = action.ActionId,
            ObjectId = action.ResolvedObjectId,
            TargetPosition = target,
            DurationSeconds = action.DurationSeconds,
            Easing = FirstNonEmpty(action.Easing, action.Parameters.GetValueOrDefault("easing"), WebGlMotionEasings.Linear),
            Metadata =
            {
                ["actionId"] = action.ActionId,
                ["actionKind"] = actionKind
            }
        });

    private static void AddPatch(WebGlRunAction action, WebGlRunActionPlan plan, WebGlSceneObjectPatch objectPatch, string actionKind)
        => plan.Patches.Add(new WebGlScenePatch
        {
            ObjectPatches = [objectPatch],
            Metadata =
            {
                ["actionId"] = action.ActionId,
                ["actionKind"] = actionKind
            }
        });

    private static WebGlVector3? ResolvePosition(IReadOnlyDictionary<string, string> parameters)
        => TryDouble(parameters, "x", out double x) && TryDouble(parameters, "y", out double y) && TryDouble(parameters, "z", out double z)
            ? new WebGlVector3(x, y, z)
            : null;

    private static string NormalizeKind(string value)
        => string.IsNullOrWhiteSpace(value) ? WebGlRunActionKinds.Wait : value;

    private static string FirstNonEmpty(params string?[] values)
        => values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;

    private static bool TryDouble(IReadOnlyDictionary<string, string> values, string key, out double result)
        => double.TryParse(values.GetValueOrDefault(key), NumberStyles.Float, CultureInfo.InvariantCulture, out result);
}
