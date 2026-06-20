namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunStageIdPolicy
{
    public string Resolve(WebGlRunAction action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (action.Metadata.TryGetValue("stageId", out string? stageId) && !string.IsNullOrWhiteSpace(stageId))
        {
            return stageId;
        }

        if (!string.IsNullOrWhiteSpace(action.ActionId))
        {
            return action.ActionId;
        }

        string sequenceId = string.IsNullOrWhiteSpace(action.SequenceId) ? "sequence" : action.SequenceId;
        int stageIndex = action.StageIndex < 0 ? 0 : action.StageIndex;
        return $"{sequenceId}.stage.{stageIndex}";
    }
}

