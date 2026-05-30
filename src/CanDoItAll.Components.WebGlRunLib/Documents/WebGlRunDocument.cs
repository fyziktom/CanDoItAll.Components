using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunDocument
{
    public string SchemaVersion { get; set; } = "webgl-run-document/v1";

    public WebGlRunId RunId { get; set; } = new(string.Empty);

    public WebGlSceneDocument InitialScene { get; set; } = new();

    public WebGlRunTimeline Timeline { get; set; } = new();

    public Dictionary<string, string> Metadata { get; set; } = [];
}
