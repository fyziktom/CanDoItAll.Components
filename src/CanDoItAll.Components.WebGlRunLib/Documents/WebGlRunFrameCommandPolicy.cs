namespace CanDoItAll.Components.WebGlRunLib;

internal static class WebGlRunFrameCommandPolicy
{
    public static bool HasMixedDirectAndStagedCommands(WebGlRunFrame frame)
        => frame.Stages.Count > 0 && (frame.ScenePatches.Count > 0 || frame.Motions.Count > 0);

    public static string CreateMixedDirectAndStagedCommandsError(long frameIndex)
        => $"Frame '{frameIndex}' cannot mix frame-level commands with staged commands. Put commands in stages, or leave Stages empty for direct frame-level playback.";
}
