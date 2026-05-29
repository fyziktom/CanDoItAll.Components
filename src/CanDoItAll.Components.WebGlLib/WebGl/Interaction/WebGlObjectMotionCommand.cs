namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlObjectMotionCommand
{
    public string MotionId { get; set; } = string.Empty;

    public string ObjectId { get; set; } = string.Empty;

    public WebGlVector3 TargetPosition { get; set; } = WebGlVector3.Zero;

    public WebGlVector3? TargetRotation { get; set; }

    public WebGlVector3? TargetScale { get; set; }

    public double SpeedUnitsPerSecond { get; set; }

    public double DurationSeconds { get; set; }

    public string Easing { get; set; } = WebGlMotionEasings.Linear;

    public bool SnapAtEnd { get; set; } = true;

    public bool ReplaceExistingForObject { get; set; } = true;

    public string QueueMode { get; set; } = WebGlMotionQueueModes.Replace;

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public static class WebGlMotionEasings
{
    public const string Linear = "linear";
    public const string EaseIn = "ease-in";
    public const string EaseOut = "ease-out";
    public const string EaseInOut = "ease-in-out";
}

public static class WebGlMotionQueueModes
{
    public const string Replace = "replace";
    public const string Append = "append";
}
