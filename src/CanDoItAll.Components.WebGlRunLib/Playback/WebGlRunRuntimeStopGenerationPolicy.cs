using System.Globalization;
using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public static class WebGlRunRuntimeStopGenerationPolicy
{
    public static bool TryRead(WebGlSceneCommandResult? result, out long generation)
    {
        generation = 0;
        return TryRead(result?.Metadata, out generation) ||
               TryRead(result?.Diagnostics, out generation);
    }

    public static bool IsStale(WebGlSceneCommandResult? result, long currentGeneration)
        => TryRead(result, out long callbackGeneration) && callbackGeneration < currentGeneration;

    private static bool TryRead(IReadOnlyDictionary<string, string>? values, out long generation)
    {
        generation = 0;
        if (values is null ||
            !values.TryGetValue("runtimeStopGeneration", out string? value) ||
            string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out generation);
    }
}
