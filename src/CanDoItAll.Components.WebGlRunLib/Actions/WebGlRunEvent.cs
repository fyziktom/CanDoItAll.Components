namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunEvent
{
    public string EventId { get; set; } = string.Empty;
    public string EventKind { get; set; } = string.Empty;
    public string SubjectObjectId { get; set; } = string.Empty;
    public string TargetObjectId { get; set; } = string.Empty;
    public double StartsAtSeconds { get; set; }
    public double DurationSeconds { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = [];
}
