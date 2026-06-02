using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CanDoItAll.Components.WebGlLib;

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
                    directPatches = frame.ScenePatches.OrderBy(static patch => patch.Id, StringComparer.Ordinal).Select(CreatePatchIdentity),
                    directMotions = frame.Motions.OrderBy(static motion => motion.MotionId, StringComparer.Ordinal).Select(CreateMotionIdentity),
                    stages = frame.Stages
                        .OrderBy(static stage => stage.StageIndex < 0 ? int.MaxValue : stage.StageIndex)
                        .ThenBy(static stage => stage.OrderIndex < 0 ? int.MaxValue : stage.OrderIndex)
                        .ThenBy(static stage => stage.StageId, StringComparer.Ordinal)
                        .Select(static stage => new
                        {
                            stage.StageId,
                            stage.StageIndex,
                            stage.OrderIndex,
                            stage.StartsAtSeconds,
                            patches = stage.ScenePatches.OrderBy(static patch => patch.Id, StringComparer.Ordinal).Select(CreatePatchIdentity),
                            motions = stage.Motions.OrderBy(static motion => motion.MotionId, StringComparer.Ordinal).Select(CreateMotionIdentity)
                        })
                })
        };
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant();
    }

    private static object CreateMotionIdentity(WebGlObjectMotionCommand motion)
        => new
        {
            motion.MotionId,
            motion.ObjectId,
            motion.TargetPosition,
            motion.DurationSeconds,
            motion.Easing,
            motion.QueueMode,
            motion.QueuePolicy
        };

    private static object CreatePatchIdentity(WebGlRunFramePatch framePatch)
        => new
        {
            framePatch.Id,
            framePatch.Patch.SceneId,
            objects = framePatch.Patch.ObjectPatches.Select(static patch => new
            {
                patch.ObjectId,
                patch.AssetId,
                patch.Color,
                symbolIds = patch.Symbols is null ? [] : patch.Symbols.Select(static symbol => symbol.Id).Order(StringComparer.Ordinal).ToArray()
            }),
            addObjects = framePatch.Patch.AddObjects.Select(static item => item.Id).Order(StringComparer.Ordinal),
            removeObjects = framePatch.Patch.RemoveObjectIds.Order(StringComparer.Ordinal),
            addLinks = framePatch.Patch.AddLinks.Select(static link => link.Id).Order(StringComparer.Ordinal)
        };
}
