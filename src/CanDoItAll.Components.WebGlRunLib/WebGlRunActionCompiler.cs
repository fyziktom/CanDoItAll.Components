using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunActionCompiler
{
    public WebGlRunTimeline Compile(WebGlRunActionPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        var frameRate = plan.FrameRate > 0 ? plan.FrameRate : 1;
        var bindings = plan.ObjectBindings
            .Where(static binding => !string.IsNullOrWhiteSpace(binding.ObjectId))
            .ToDictionary(static binding => binding.ObjectId, StringComparer.Ordinal);
        var frames = new Dictionary<long, WebGlRunFrame>();

        foreach (var action in Flatten(plan.Actions).OrderBy(static action => action.StartsAtSeconds).ThenBy(static action => action.ActionId, StringComparer.Ordinal))
        {
            var frame = GetOrCreateFrame(frames, action, frameRate);
            CompileAction(action, bindings, frame);
        }

        return new WebGlRunTimeline
        {
            FrameRate = frameRate,
            Frames = frames.Values.OrderBy(static frame => frame.Index).ThenBy(static frame => frame.TimeSeconds).ToList()
        };
    }

    private static void CompileAction(
        WebGlRunAction action,
        IReadOnlyDictionary<string, WebGlRunObjectBinding> bindings,
        WebGlRunFrame frame)
    {
        switch (action.ResolvedKind)
        {
            case WebGlRunActionKinds.MoveToObject:
                AddMotion(action, ResolvePosition(bindings, action.ResolvedTargetObjectId), frame);
                break;
            case WebGlRunActionKinds.MoveToPosition:
                AddMotion(action, action.Target.Position ?? ResolvePosition(action.Parameters), frame);
                break;
            case WebGlRunActionKinds.ReturnToAnchor:
                AddMotion(action, ResolveAnchor(bindings, action.ResolvedObjectId), frame);
                break;
            case WebGlRunActionKinds.SetAsset:
                AddPatch(action, frame, new WebGlSceneObjectPatch
                {
                    ObjectId = action.ResolvedObjectId,
                    AssetId = Get(action.Parameters, "assetId")
                });
                break;
            case WebGlRunActionKinds.SetPose:
            case WebGlRunActionKinds.ChangePose:
                AddPatch(action, frame, new WebGlSceneObjectPatch
                {
                    ObjectId = action.ResolvedObjectId,
                    Metadata = new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["poseKey"] = FirstNonEmpty(action.PoseKey, Get(action.Parameters, "poseKey"))
                    }
                });
                break;
            case WebGlRunActionKinds.ShowSymbol:
            case WebGlRunActionKinds.UpdateSymbol:
                AddPatch(action, frame, new WebGlSceneObjectPatch
                {
                    ObjectId = action.ResolvedObjectId,
                    Symbols =
                    [
                        new WebGlStatusSymbol
                        {
                            Id = Get(action.Parameters, "symbolId", action.ActionId),
                            SemanticKind = FirstNonEmpty(action.SymbolKey, Get(action.Parameters, "symbolKind", "status")),
                            SymbolAssetId = Get(action.Parameters, "symbolAssetId"),
                            Color = Get(action.Parameters, "color", "#facc15"),
                            EffectKey = Get(action.Parameters, "effectKey", WebGlSymbolEffects.Pulse),
                            Tooltip = Get(action.Parameters, "tooltip")
                        }
                    ]
                });
                break;
            case WebGlRunActionKinds.HideSymbol:
                AddPatch(action, frame, new WebGlSceneObjectPatch
                {
                    ObjectId = action.ResolvedObjectId,
                    Symbols = []
                });
                break;
            case WebGlRunActionKinds.Sequence:
            case WebGlRunActionKinds.Parallel:
            case WebGlRunActionKinds.ApplyScenePatch:
            case WebGlRunActionKinds.ApplyPatch:
            case WebGlRunActionKinds.PulseLink:
            case WebGlRunActionKinds.ResourceTransferVisual:
            case WebGlRunActionKinds.SetLayerVisibility:
            case WebGlRunActionKinds.Wait:
                frame.Metadata[$"action.{action.ActionId}"] = action.ResolvedKind;
                break;
        }
    }

    private static IEnumerable<WebGlRunAction> Flatten(IEnumerable<WebGlRunAction> actions)
    {
        foreach (WebGlRunAction action in actions)
        {
            if (action.ResolvedKind is WebGlRunActionKinds.Sequence or WebGlRunActionKinds.Parallel)
            {
                foreach (WebGlRunAction child in Flatten(action.Steps))
                {
                    yield return child;
                }
            }
            else
            {
                yield return action;
            }
        }
    }

    private static WebGlRunFrame GetOrCreateFrame(Dictionary<long, WebGlRunFrame> frames, WebGlRunAction action, int frameRate)
    {
        var index = Math.Max(0, (long)Math.Round(action.StartsAtSeconds * frameRate, MidpointRounding.AwayFromZero));
        if (!frames.TryGetValue(index, out var frame))
        {
            frame = new WebGlRunFrame { Index = index, TimeSeconds = action.StartsAtSeconds };
            frames[index] = frame;
        }

        return frame;
    }

    private static void AddMotion(WebGlRunAction action, WebGlVector3? targetPosition, WebGlRunFrame frame)
    {
        if (targetPosition is null)
        {
            frame.Metadata[$"action.{action.ActionId}.warning"] = "target-position-missing";
            return;
        }

        frame.Motions.Add(new WebGlObjectMotionCommand
        {
            MotionId = action.ActionId,
            ObjectId = action.ResolvedObjectId,
            TargetPosition = targetPosition.Value,
            DurationSeconds = action.DurationSeconds,
            Easing = Get(action.Parameters, "easing", WebGlMotionEasings.EaseInOut),
            Metadata = new Dictionary<string, string>(action.Metadata, StringComparer.Ordinal)
            {
                ["actionKind"] = action.ResolvedKind
            }
        });
    }

    private static void AddPatch(WebGlRunAction action, WebGlRunFrame frame, WebGlSceneObjectPatch objectPatch)
        => frame.ScenePatches.Add(new WebGlRunFramePatch
        {
            Id = action.ActionId,
            Patch = new WebGlScenePatch
            {
                ObjectPatches = [objectPatch],
                Metadata =
                {
                    ["commandId"] = action.ActionId,
                    ["actionKind"] = action.ResolvedKind
                }
            }
        });

    private static WebGlVector3? ResolvePosition(IReadOnlyDictionary<string, WebGlRunObjectBinding> bindings, string objectId)
        => bindings.TryGetValue(objectId, out var binding) ? binding.Position : null;

    private static WebGlVector3? ResolveAnchor(IReadOnlyDictionary<string, WebGlRunObjectBinding> bindings, string objectId)
        => bindings.TryGetValue(objectId, out var binding) ? binding.AnchorPosition ?? binding.Position : null;

    private static WebGlVector3? ResolvePosition(IReadOnlyDictionary<string, string> parameters)
        => TryDouble(parameters, "x", out var x) && TryDouble(parameters, "y", out var y) && TryDouble(parameters, "z", out var z)
            ? new WebGlVector3(x, y, z)
            : null;

    private static string Get(IReadOnlyDictionary<string, string> values, string key, string fallback = "")
        => values.TryGetValue(key, out var value) ? value : fallback;

    private static string FirstNonEmpty(params string[] values)
        => values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;

    private static bool TryDouble(IReadOnlyDictionary<string, string> values, string key, out double result)
        => double.TryParse(Get(values, key), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out result);
}
