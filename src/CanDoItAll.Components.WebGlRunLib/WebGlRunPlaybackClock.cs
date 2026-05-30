using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunPlaybackClock
{
    public TimeSpan ResolveFrameDelay(WebGlRunTimeline timeline, double playbackSpeed)
    {
        ArgumentNullException.ThrowIfNull(timeline);
        var speed = playbackSpeed > 0 ? playbackSpeed : 1.0;
        var frameRate = timeline.FrameRate > 0 ? timeline.FrameRate : 30;
        return TimeSpan.FromSeconds(1.0 / frameRate / speed);
    }

    public string BuildDeterministicTimelineIdentity(WebGlRunDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var payload = new
        {
            runId = document.RunId.Value,
            frameRate = document.Timeline.FrameRate,
            frames = WebGlRunFrameResolver.OrderFrames(document.Timeline.Frames)
                .Select(static frame => new
                {
                    frame.Index,
                    frame.TimeSeconds,
                    patches = frame.ScenePatches.Select(static patch => patch.Id).Order(StringComparer.Ordinal),
                    motions = frame.Motions.Select(static motion => motion.MotionId).Order(StringComparer.Ordinal)
                })
        };
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant();
    }
}
