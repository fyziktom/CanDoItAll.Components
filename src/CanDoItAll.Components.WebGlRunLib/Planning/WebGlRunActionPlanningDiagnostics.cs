namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunActionPlanningDiagnostics
{
    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}
