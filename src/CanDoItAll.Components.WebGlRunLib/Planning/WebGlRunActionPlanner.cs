using System.Globalization;
using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunActionPlanner : IWebGlRunActionPlanner
{
    private readonly WebGlRunVisualStateResolver visualStateResolver = new();

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
                plan.Metadata["orderingMode"] = BatchOrderingMode.Sequential.ToString();
                foreach (WebGlRunAction step in action.Steps)
                {
                    AppendAction(step, context, plan);
                }
                break;
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
                plan.DroppedStepIds.Add(action.ActionId);
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
        MergeDiagnostics(action, resolver, plan);
        if (target is { } targetPosition)
        {
            AddMotion(action, targetPosition, plan, WebGlRunActionKinds.MoveToObject);
        }
    }

    private static void AddReturnToAnchor(WebGlRunAction action, WebGlRunPlanningContext context, WebGlRunActionPlan plan)
    {
        string objectId = action.ResolvedObjectId;
        if (!context.ObjectIndex.TryGetValue(objectId, out WebGlSceneObject? sceneObject))
        {
            plan.Errors.Add($"Return object '{objectId}' was not found.");
            return;
        }

        var resolver = new WebGlRunTargetResolver();
        WebGlVector3 target = resolver.ResolveAnchor(sceneObject, action.Target.AnchorKey, action.Target.Offset);
        MergeDiagnostics(action, resolver, plan);
        AddMotion(action, target, plan, WebGlRunActionKinds.ReturnToAnchor);
    }

    private void AddPosePatch(WebGlRunAction action, WebGlRunPlanningContext context, WebGlRunActionPlan plan)
    {
        string poseKey = FirstNonEmpty(action.PoseKey, action.Parameters.GetValueOrDefault("poseKey"));
        WebGlPoseDefinition? pose = visualStateResolver.ResolvePose(action, context);
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

    private void AddSymbolPatch(WebGlRunAction action, WebGlRunPlanningContext context, WebGlRunActionPlan plan, bool replaceSymbols)
    {
        if (replaceSymbols)
        {
            AddPatch(action, plan, new WebGlSceneObjectPatch { ObjectId = action.ResolvedObjectId, Symbols = [] }, WebGlRunActionKinds.HideSymbol);
            return;
        }

        string symbolKey = FirstNonEmpty(action.SymbolKey, action.Parameters.GetValueOrDefault("symbolKey"), action.Parameters.GetValueOrDefault("symbolKind"), "status");
        WebGlSymbolDefinition? symbol = visualStateResolver.ResolveSymbol(action, context);
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

    private static void MergeDiagnostics(WebGlRunAction action, WebGlRunTargetResolver resolver, WebGlRunActionPlan plan)
    {
        plan.Errors.AddRange(resolver.Diagnostics.Errors);
        plan.Warnings.AddRange(resolver.Diagnostics.Warnings);
        foreach (KeyValuePair<string, string> item in resolver.Diagnostics.Metadata)
        {
            plan.TargetResolutionDiagnostics[$"{action.ActionId}.{item.Key}"] = item.Value;
        }
    }

    private static string NormalizeKind(string value)
        => string.IsNullOrWhiteSpace(value) ? WebGlRunActionKinds.Wait : value;

    private static string FirstNonEmpty(params string?[] values)
        => values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;

    private static bool TryDouble(IReadOnlyDictionary<string, string> values, string key, out double result)
        => double.TryParse(values.GetValueOrDefault(key), NumberStyles.Float, CultureInfo.InvariantCulture, out result);
}
