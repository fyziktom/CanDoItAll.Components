namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunActionNormalizationResult
{
    public WebGlRunAction Action { get; set; } = new();

    public List<string> Warnings { get; set; } = [];
}

public sealed class WebGlRunActionNormalizer
{
    public WebGlRunActionNormalizationResult Normalize(WebGlRunAction action)
    {
        ArgumentNullException.ThrowIfNull(action);

        var result = new WebGlRunActionNormalizationResult();
        result.Action = NormalizeAction(action, result.Warnings);
        return result;
    }

    private static WebGlRunAction NormalizeAction(WebGlRunAction action, List<string> warnings)
    {
        string actionKind = ChooseAlias(action.ActionId, "Kind", action.Kind, "ActionKind", action.ActionKind, warnings);
        string subjectObjectId = ChooseAlias(action.ActionId, "ObjectId", action.ObjectId, "SubjectObjectId", action.SubjectObjectId, warnings);
        string targetObjectId = ChooseAlias(action.ActionId, "TargetObjectId", action.TargetObjectId, "Target.ObjectId", action.Target.ObjectId, warnings);

        return new WebGlRunAction
        {
            ActionId = action.ActionId,
            Kind = string.Empty,
            ActionKind = actionKind,
            ObjectId = string.Empty,
            SubjectObjectId = subjectObjectId,
            TargetObjectId = string.Empty,
            Target = new WebGlRunActionTarget
            {
                ObjectId = targetObjectId,
                AnchorKey = action.Target.AnchorKey,
                Offset = action.Target.Offset,
                Position = action.Target.Position
            },
            PoseKey = action.PoseKey,
            SymbolKey = action.SymbolKey,
            StartsAtSeconds = action.StartsAtSeconds,
            DurationSeconds = action.DurationSeconds,
            Easing = action.Easing,
            Steps = [.. action.Steps.Select(step => NormalizeAction(step, warnings))],
            Parameters = new Dictionary<string, string>(action.Parameters, StringComparer.Ordinal),
            Metadata = new Dictionary<string, string>(action.Metadata, StringComparer.Ordinal)
        };
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
}

