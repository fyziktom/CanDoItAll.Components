using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

internal static class WebGlRunStageBarrierPolicy
{
    public static void Apply(WebGlRunAction action, WebGlRunActionStage stage, string fallbackPolicy, params string[] objectIds)
    {
        string policy = NormalizePolicy(FirstNonEmpty(
            action.Metadata.GetValueOrDefault("barrierPolicy"),
            action.Parameters.GetValueOrDefault("barrierPolicy"),
            fallbackPolicy));
        if (string.IsNullOrWhiteSpace(policy))
        {
            return;
        }

        stage.BarrierPolicy = policy;
        stage.Metadata["barrierPolicy"] = policy;
        foreach (string objectId in objectIds.Where(static value => !string.IsNullOrWhiteSpace(value)))
        {
            if (!stage.BarrierObjectIds.Contains(objectId, StringComparer.Ordinal))
            {
                stage.BarrierObjectIds.Add(objectId);
            }
        }

        if (stage.BarrierObjectIds.Count > 0)
        {
            stage.Metadata["barrierObjectIds"] = string.Join(",", stage.BarrierObjectIds);
        }

        string eventId = FirstNonEmpty(
            action.Metadata.GetValueOrDefault("barrierEventId"),
            action.Parameters.GetValueOrDefault("barrierEventId"));
        if (!string.IsNullOrWhiteSpace(eventId))
        {
            stage.BarrierEventId = eventId;
            stage.Metadata["barrierEventId"] = eventId;
        }
    }

    private static string FirstNonEmpty(params string?[] values)
        => values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;

    private static string NormalizePolicy(string policy)
        => policy.Trim().ToLowerInvariant().Replace("_", "-") switch
        {
            "time-delay" or "timedelay" or "waitseconds" => WebGlSceneStageBarrierPolicies.WaitSeconds,
            "manual-step" or "manualstep" => WebGlSceneStageBarrierPolicies.WaitForEvent,
            _ => policy
        };
}
