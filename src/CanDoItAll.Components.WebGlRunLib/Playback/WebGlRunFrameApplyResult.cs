using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunFrameApplyResult
{
    public long FrameIndex { get; set; }

    public double TimeSeconds { get; set; }

    public WebGlSceneCommandBatch CommandBatch { get; set; } = new();

    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public static WebGlRunFrameApplyResult FromFrame(WebGlRunFrame frame)
    {
        ArgumentNullException.ThrowIfNull(frame);
        WebGlSceneCommandBatchNormalizationResult normalized = WebGlSceneCommandBatchNormalizer.Normalize(new WebGlSceneCommandBatch
        {
            BatchId = $"run-frame:{frame.Index}",
            OrderingMode = ResolveOrderingMode(frame),
            Patches = [.. frame.ScenePatches.Select(item => item.Patch)],
            Motions = [.. frame.Motions],
            Metadata =
            {
                ["frameIndex"] = frame.Index.ToString(System.Globalization.CultureInfo.InvariantCulture),
                ["timeSeconds"] = frame.TimeSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture)
            }
        });

        return new()
        {
            FrameIndex = frame.Index,
            TimeSeconds = frame.TimeSeconds,
            CommandBatch = normalized.Batch,
            Warnings = [.. normalized.Warnings]
        };
    }

    private static BatchOrderingMode ResolveOrderingMode(WebGlRunFrame frame)
    {
        if (frame.Metadata.TryGetValue("orderingMode", out string? value) &&
            Enum.TryParse(value, ignoreCase: true, out BatchOrderingMode parsed))
        {
            return parsed;
        }

        return frame.Motions.GroupBy(static motion => motion.ObjectId, StringComparer.Ordinal).Any(static group => group.Count() > 1)
            ? BatchOrderingMode.Sequential
            : BatchOrderingMode.CoalesceIndependent;
    }
}
