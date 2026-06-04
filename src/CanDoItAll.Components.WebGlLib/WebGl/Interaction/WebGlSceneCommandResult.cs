namespace CanDoItAll.Components.WebGlLib;

public static class WebGlSceneCommandLifecycleStates
{
    public const string Accepted = "accepted";
    public const string Scheduled = "scheduled";
    public const string Active = "active";
    public const string Settled = "settled";
    public const string Cancelled = "cancelled";
    public const string Failed = "failed";
}

public class WebGlSceneCommandResult
{
    public string CommandId { get; set; } = string.Empty;

    public bool Success { get; set; }

    public bool Succeeded
    {
        get => Success;
        set => Success = value;
    }

    public string SceneId { get; set; } = string.Empty;

    public string CommandKind { get; set; } = string.Empty;

    public string LifecycleState { get; set; } = WebGlSceneCommandLifecycleStates.Accepted;

    public bool Settled { get; set; }

    public int Revision { get; set; }

    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public List<string> AffectedObjectIds { get; set; } = [];

    public List<string> AffectedLinkIds { get; set; } = [];

    public Dictionary<string, string> Diagnostics { get; set; } = [];

    public string Message { get; set; } = string.Empty;

    public WebGlSceneProofSnapshot Snapshot { get; set; } = new();

    public Dictionary<string, string> Metadata { get; set; } = [];
}

