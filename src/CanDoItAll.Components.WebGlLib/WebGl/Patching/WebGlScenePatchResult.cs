namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlScenePatchResult
{
    public string SceneId { get; set; } = string.Empty;

    public string CommandKind { get; set; } = "patch";

    public List<string> Errors { get; } = [];

    public List<string> Warnings { get; } = [];

    public List<string> PatchedObjectIds { get; } = [];

    public List<string> AddedObjectIds { get; } = [];

    public List<string> RemovedObjectIds { get; } = [];

    public List<string> AddedLinkIds { get; } = [];

    public List<string> RemovedLinkIds { get; } = [];

    public Dictionary<string, string> Metadata { get; } = [];

    public int NextRevision { get; set; }

    public int Revision { get; set; }

    public List<string> AffectedObjectIds => PatchedObjectIds
        .Concat(AddedObjectIds)
        .Concat(RemovedObjectIds)
        .Where(static id => !string.IsNullOrWhiteSpace(id))
        .Distinct(StringComparer.Ordinal)
        .ToList();

    public List<string> AffectedLinkIds => AddedLinkIds
        .Concat(RemovedLinkIds)
        .Where(static id => !string.IsNullOrWhiteSpace(id))
        .Distinct(StringComparer.Ordinal)
        .ToList();

    public bool Success => IsValid;

    public bool IsValid => Errors.Count == 0;
}
