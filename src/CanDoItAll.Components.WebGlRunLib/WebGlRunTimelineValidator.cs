namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunTimelineValidator
{
    public WebGlRunTimelineValidationResult Validate(WebGlRunTimeline timeline)
    {
        ArgumentNullException.ThrowIfNull(timeline);
        var result = new WebGlRunTimelineValidationResult();
        if (timeline.FrameRate <= 0)
        {
            result.Errors.Add("Timeline frame rate must be positive.");
        }

        var indexes = new HashSet<long>();
        var previousTime = double.NegativeInfinity;
        foreach (var frame in WebGlRunFrameResolver.OrderFrames(timeline.Frames))
        {
            if (frame.Index < 0)
            {
                result.Errors.Add($"Frame index {frame.Index} must be non-negative.");
            }

            if (!indexes.Add(frame.Index))
            {
                result.Errors.Add($"Duplicate frame index '{frame.Index}'.");
            }

            if (frame.TimeSeconds < previousTime)
            {
                result.Errors.Add("Frame time values must be non-decreasing when ordered by frame index.");
            }

            previousTime = frame.TimeSeconds;
        }

        return result;
    }
}

public sealed class WebGlRunTimelineValidationResult
{
    public List<string> Errors { get; } = [];

    public bool Success => Errors.Count == 0;
}
