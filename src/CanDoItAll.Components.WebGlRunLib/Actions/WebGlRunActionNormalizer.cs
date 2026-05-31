namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunActionNormalizationResult
{
    public WebGlRunAction Action { get; set; } = new();

    public List<string> Warnings { get; set; } = [];

    public List<string> Errors { get; set; } = [];

    public bool IsValid => Errors.Count == 0;
}

public sealed class WebGlRunActionNormalizer
{
    public WebGlRunActionNormalizationResult Normalize(WebGlRunAction action)
    {
        ArgumentNullException.ThrowIfNull(action);

        var result = new WebGlRunActionNormalizationResult();
        result.Action = NormalizeAction(action, result.Warnings, result.Errors);
        return result;
    }

    private static WebGlRunAction NormalizeAction(WebGlRunAction action, List<string> warnings, List<string> errors)
    {
        string actionKind = ChooseAlias(action.ActionId, "Kind", action.Kind, "ActionKind", action.ActionKind, warnings);
        string subjectObjectId = ChooseAlias(action.ActionId, "ObjectId", action.ObjectId, "SubjectObjectId", action.SubjectObjectId, warnings);
        string targetObjectId = ChooseAlias(action.ActionId, "TargetObjectId", action.TargetObjectId, "Target.ObjectId", action.Target.ObjectId, warnings);

        string normalizedKind = string.IsNullOrWhiteSpace(actionKind) ? WebGlRunActionKinds.Wait : actionKind;
        var normalized = new WebGlRunAction
        {
            ActionId = action.ActionId,
            SequenceId = action.SequenceId,
            ParentActionId = action.ParentActionId,
            StageIndex = action.StageIndex,
            StageGroupId = FirstNonEmpty(action.StageGroupId, action.Metadata.GetValueOrDefault("stageGroupId")),
            CoalescingScope = NormalizeCoalescingScope(FirstNonEmpty(action.CoalescingScope, action.Metadata.GetValueOrDefault("coalescingScope"))),
            OrderIndex = action.OrderIndex,
            ExecutionPolicy = string.IsNullOrWhiteSpace(action.ExecutionPolicy)
                ? WebGlRunStageExecutionPolicies.PreserveOrder
                : action.ExecutionPolicy,
            Kind = string.Empty,
            ActionKind = normalizedKind,
            ObjectId = string.Empty,
            SubjectObjectId = subjectObjectId,
            TargetObjectId = string.Empty,
            Target = new WebGlRunActionTarget
            {
                ObjectId = targetObjectId,
                AnchorKey = action.Target.AnchorKey,
                FallbackAnchorKey = action.Target.FallbackAnchorKey,
                Offset = action.Target.Offset,
                Position = action.Target.Position,
                Metadata = new Dictionary<string, string>(action.Target.Metadata, StringComparer.Ordinal)
            },
            PoseKey = action.PoseKey,
            SymbolKey = action.SymbolKey,
            StartsAtSeconds = action.StartsAtSeconds,
            DurationSeconds = action.DurationSeconds,
            Easing = action.Easing,
            Steps = [.. action.Steps.Select(step => NormalizeAction(step, warnings, errors))],
            Parameters = new Dictionary<string, string>(action.Parameters, StringComparer.Ordinal),
            Metadata = new Dictionary<string, string>(action.Metadata, StringComparer.Ordinal)
        };

        Validate(normalized, errors);
        return normalized;
    }

    private static void Validate(WebGlRunAction action, List<string> errors)
    {
        if (!IsSupportedActionKind(action.ActionKind))
        {
            errors.Add($"Unsupported WebGL run action kind '{action.ActionKind}' for action '{action.ActionId}'.");
            return;
        }

        if (RequiresSubject(action.ActionKind) && string.IsNullOrWhiteSpace(action.SubjectObjectId))
        {
            errors.Add($"Action '{action.ActionId}' requires a subject object id.");
        }

        if (action.ActionKind is WebGlRunActionKinds.MoveToObject &&
            string.IsNullOrWhiteSpace(action.Target.ObjectId))
        {
            errors.Add($"Action '{action.ActionId}' requires Target.ObjectId for a target-based move.");
        }

        if (action.ActionKind is WebGlRunActionKinds.MoveToPosition &&
            action.Target.Position is null &&
            !(action.Parameters.ContainsKey("x") && action.Parameters.ContainsKey("y") && action.Parameters.ContainsKey("z")))
        {
            errors.Add($"Action '{action.ActionId}' requires an explicit target position.");
        }
    }

    private static string ChooseAlias(
        string actionId,
        string firstName,
        string first,
        string secondName,
        string second,
        List<string> warnings)
    {
        bool hasFirst = !string.IsNullOrWhiteSpace(first);
        bool hasSecond = !string.IsNullOrWhiteSpace(second);
        if (hasFirst && hasSecond && !string.Equals(first, second, StringComparison.Ordinal))
        {
            warnings.Add($"Action '{actionId}' has conflicting aliases {firstName}='{first}' and {secondName}='{second}'; {secondName} was used.");
        }

        return hasSecond ? second : first;
    }

    private static string NormalizeCoalescingScope(string value)
        => value.Trim().ToLowerInvariant() switch
        {
            WebGlRunCoalescingScopes.None => WebGlRunCoalescingScopes.None,
            WebGlRunCoalescingScopes.Frame => WebGlRunCoalescingScopes.Frame,
            WebGlRunCoalescingScopes.StageOnly => WebGlRunCoalescingScopes.StageOnly,
            "stageonly" => WebGlRunCoalescingScopes.StageOnly,
            _ => WebGlRunCoalescingScopes.StageOnly
        };

    private static bool RequiresSubject(string actionKind)
        => actionKind is WebGlRunActionKinds.MoveToObject
            or WebGlRunActionKinds.MoveToPosition
            or WebGlRunActionKinds.ReturnToAnchor
            or WebGlRunActionKinds.SetAsset
            or WebGlRunActionKinds.SetPose
            or WebGlRunActionKinds.ChangePose
            or WebGlRunActionKinds.ShowSymbol
            or WebGlRunActionKinds.UpdateSymbol
            or WebGlRunActionKinds.HideSymbol
            or WebGlRunActionKinds.ResourceTransferVisual
            or WebGlRunActionKinds.PulseLink;

    private static bool IsSupportedActionKind(string actionKind)
        => actionKind is WebGlRunActionKinds.MoveToObject
            or WebGlRunActionKinds.MoveToPosition
            or WebGlRunActionKinds.ReturnToAnchor
            or WebGlRunActionKinds.SetAsset
            or WebGlRunActionKinds.SetPose
            or WebGlRunActionKinds.ChangePose
            or WebGlRunActionKinds.ShowSymbol
            or WebGlRunActionKinds.UpdateSymbol
            or WebGlRunActionKinds.HideSymbol
            or WebGlRunActionKinds.Sequence
            or WebGlRunActionKinds.Parallel
            or WebGlRunActionKinds.ApplyScenePatch
            or WebGlRunActionKinds.ApplyPatch
            or WebGlRunActionKinds.PulseLink
            or WebGlRunActionKinds.ResourceTransferVisual
            or WebGlRunActionKinds.SetLayerVisibility
            or WebGlRunActionKinds.Wait;

    private static string FirstNonEmpty(params string?[] values)
        => values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
}
