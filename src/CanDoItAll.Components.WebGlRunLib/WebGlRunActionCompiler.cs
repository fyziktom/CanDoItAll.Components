using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunActionCompiler
{
    private readonly WebGlRunActionNormalizer actionNormalizer = new();

    public WebGlRunTimeline Compile(WebGlRunActionPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        var frameRate = plan.FrameRate > 0 ? plan.FrameRate : 1;
        var bindings = plan.ObjectBindings
            .Where(static binding => !string.IsNullOrWhiteSpace(binding.ObjectId))
            .ToDictionary(static binding => binding.ObjectId, StringComparer.Ordinal);
        var frames = new Dictionary<long, WebGlRunFrame>();

        var normalizedActions = plan.Actions.Select(action => actionNormalizer.Normalize(action).Action).ToList();
        foreach (var action in Flatten(normalizedActions)
                     .Select(static (action, order) => new { action, order })
                     .OrderBy(static item => item.action.StartsAtSeconds)
                     .ThenBy(static item => item.action.StageIndex < 0 ? item.order : item.action.StageIndex)
                     .ThenBy(static item => item.action.OrderIndex < 0 ? item.order : item.action.OrderIndex)
                     .ThenBy(static item => item.order)
                     .Select(static item => item.action))
        {
            var frame = GetOrCreateFrame(frames, action, frameRate);
            CompileAction(action, bindings, frame);
        }

        foreach (WebGlRunFrame frame in frames.Values)
        {
            frame.ScenePatches = [.. frame.Stages.SelectMany(static stage => stage.ScenePatches)];
            frame.Motions = [.. frame.Stages.SelectMany(static stage => stage.Motions)];
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
        WebGlRunActionStage stage = GetOrCreateStage(frame, action);
        switch (action.ActionKind)
        {
            case WebGlRunActionKinds.MoveToObject:
                AddMotion(action, ResolvePosition(bindings, action.Target.ObjectId), stage);
                break;
            case WebGlRunActionKinds.MoveToPosition:
                AddMotion(action, action.Target.Position ?? ResolvePosition(action.Parameters), stage);
                break;
            case WebGlRunActionKinds.ReturnToAnchor:
                AddMotion(action, ResolveAnchor(bindings, action.SubjectObjectId), stage);
                break;
            case WebGlRunActionKinds.SetAsset:
                AddPatch(action, stage, new WebGlSceneObjectPatch
                {
                    ObjectId = action.SubjectObjectId,
                    AssetId = Get(action.Parameters, "assetId")
                });
                break;
            case WebGlRunActionKinds.SetPose:
            case WebGlRunActionKinds.ChangePose:
                AddPatch(action, stage, new WebGlSceneObjectPatch
                {
                    ObjectId = action.SubjectObjectId,
                    Metadata = new Dictionary<string, string>(StringComparer.Ordinal)
                    {
                        ["poseKey"] = FirstNonEmpty(action.PoseKey, Get(action.Parameters, "poseKey"))
                    }
                });
                break;
            case WebGlRunActionKinds.ShowSymbol:
            case WebGlRunActionKinds.UpdateSymbol:
                AddPatch(action, stage, new WebGlSceneObjectPatch
                {
                    ObjectId = action.SubjectObjectId,
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
                AddPatch(action, stage, new WebGlSceneObjectPatch
                {
                    ObjectId = action.SubjectObjectId,
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
                stage.WaitSeconds = Math.Max(0, action.DurationSeconds);
                frame.Metadata[$"action.{action.ActionId}"] = action.ActionKind;
                break;
        }
    }

    private static IEnumerable<WebGlRunAction> Flatten(IEnumerable<WebGlRunAction> actions)
        => Flatten(actions, sequenceId: string.Empty, parentActionId: string.Empty);

    private static IEnumerable<WebGlRunAction> Flatten(IEnumerable<WebGlRunAction> actions, string sequenceId, string parentActionId)
    {
        var childIndex = 0;
        foreach (WebGlRunAction action in actions)
        {
            if (action.ActionKind is WebGlRunActionKinds.Sequence or WebGlRunActionKinds.Parallel)
            {
                string nextSequenceId = string.IsNullOrWhiteSpace(action.SequenceId) ? action.ActionId : action.SequenceId;
                foreach (WebGlRunAction child in Flatten(action.Steps, nextSequenceId, action.ActionId))
                {
                    yield return child;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(action.SequenceId))
                {
                    action.SequenceId = sequenceId;
                }

                if (string.IsNullOrWhiteSpace(action.ParentActionId))
                {
                    action.ParentActionId = parentActionId;
                }

                if (action.StageIndex < 0)
                {
                    action.StageIndex = childIndex;
                }

                if (action.OrderIndex < 0)
                {
                    action.OrderIndex = childIndex;
                }

                yield return action;
            }

            childIndex++;
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

    private static WebGlRunActionStage GetOrCreateStage(WebGlRunFrame frame, WebGlRunAction action)
    {
        string stageId = string.IsNullOrWhiteSpace(action.Metadata.GetValueOrDefault("stageId"))
            ? action.ActionId
            : action.Metadata["stageId"];
        WebGlRunActionStage? existing = frame.Stages.FirstOrDefault(item => string.Equals(item.StageId, stageId, StringComparison.Ordinal));
        if (existing is not null)
        {
            return existing;
        }

        var stage = new WebGlRunActionStage
        {
            StageId = stageId,
            SequenceId = action.SequenceId,
            ParentActionId = action.ParentActionId,
            StageIndex = action.StageIndex,
            OrderIndex = action.OrderIndex,
            ExecutionPolicy = ResolveExecutionPolicy(action),
            StartsAtSeconds = action.StartsAtSeconds,
            Metadata =
            {
                ["actionId"] = action.ActionId,
                ["actionKind"] = action.ActionKind,
                ["sequenceId"] = action.SequenceId,
                ["parentActionId"] = action.ParentActionId,
                ["stageIndex"] = action.StageIndex.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["orderIndex"] = action.OrderIndex.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["executionPolicy"] = ResolveExecutionPolicy(action),
                ["batchingPolicy"] = ResolveExecutionPolicy(action),
                ["orderingMode"] = ResolveOrderingMode(action).ToString()
            }
        };
        frame.Stages.Add(stage);
        frame.Metadata["orderingMode"] = BatchOrderingMode.Sequential.ToString();
        frame.Metadata["batchingPolicy"] = WebGlRunStageExecutionPolicies.PreserveOrder;
        return stage;
    }

    private static string ResolveExecutionPolicy(WebGlRunAction action)
        => !string.IsNullOrWhiteSpace(action.ExecutionPolicy)
            ? action.ExecutionPolicy
            : action.Metadata.GetValueOrDefault("executionPolicy", WebGlRunStageExecutionPolicies.PreserveOrder);

    private static BatchOrderingMode ResolveOrderingMode(WebGlRunAction action)
        => ResolveExecutionPolicy(action) switch
        {
            WebGlRunStageExecutionPolicies.PreserveOrder => BatchOrderingMode.PreserveOrder,
            WebGlRunStageExecutionPolicies.Parallel => BatchOrderingMode.CoalesceIndependent,
            WebGlRunStageExecutionPolicies.CoalesceWithinStage => BatchOrderingMode.CoalesceIndependent,
            _ => BatchOrderingMode.PreserveOrder
        };

    private static void AddMotion(WebGlRunAction action, WebGlVector3? targetPosition, WebGlRunActionStage stage)
    {
        if (targetPosition is null)
        {
            stage.Metadata[$"action.{action.ActionId}.warning"] = "target-position-missing";
            return;
        }

        stage.Motions.Add(new WebGlObjectMotionCommand
        {
            MotionId = action.ActionId,
            ObjectId = action.SubjectObjectId,
            TargetPosition = targetPosition.Value,
            DurationSeconds = action.DurationSeconds,
            Easing = Get(action.Parameters, "easing", WebGlMotionEasings.EaseInOut),
            Metadata = new Dictionary<string, string>(action.Metadata, StringComparer.Ordinal)
            {
                ["actionKind"] = action.ActionKind
            }
        });
    }

    private static void AddPatch(WebGlRunAction action, WebGlRunActionStage stage, WebGlSceneObjectPatch objectPatch)
        => stage.ScenePatches.Add(new WebGlRunFramePatch
        {
            Id = action.ActionId,
            Patch = new WebGlScenePatch
            {
                ObjectPatches = [objectPatch],
                Metadata =
                {
                    ["commandId"] = action.ActionId,
                    ["actionKind"] = action.ActionKind
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
