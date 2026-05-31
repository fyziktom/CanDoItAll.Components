using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

internal static class WebGlRunStageBarrierPolicy
{
    public static void Apply(WebGlRunAction action, WebGlRunActionStage stage, string fallbackPolicy, params string[] objectIds)
    {
        string policy = FirstNonEmpty(
            action.Metadata.GetValueOrDefault("barrierPolicy"),
            action.Parameters.GetValueOrDefault("barrierPolicy"),
            fallbackPolicy);
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
    }

    private static string FirstNonEmpty(params string?[] values)
        => values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
}
