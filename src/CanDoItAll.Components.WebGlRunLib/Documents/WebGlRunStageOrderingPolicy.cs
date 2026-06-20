namespace CanDoItAll.Components.WebGlRunLib;

public static class WebGlRunStageOrderingPolicy
{
    public static IReadOnlyList<WebGlRunActionStage> OrderStages(WebGlRunFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);
        return OrderStages(frame.Stages);
    }

    public static IReadOnlyList<WebGlRunActionStage> OrderStages(IEnumerable<WebGlRunActionStage> stages)
    {
        ArgumentNullException.ThrowIfNull(stages);
        return stages
            .OrderBy(static stage => stage.StartsAtSeconds)
            .ThenBy(static stage => EffectiveIndex(stage.StageIndex))
            .ThenBy(static stage => EffectiveIndex(stage.OrderIndex))
            .ThenBy(static stage => stage.StageId, StringComparer.Ordinal)
            .ToArray();
    }

    private static int EffectiveIndex(int index)
        => index < 0 ? int.MaxValue : index;
}
