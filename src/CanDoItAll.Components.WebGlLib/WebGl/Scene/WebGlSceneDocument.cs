namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneDocument
{
    public string SchemaVersion { get; set; } = WebGlSceneDocumentSerializer.CurrentSchemaVersion;

    public string DocumentId { get; set; } = string.Empty;

    public WebGlSceneModel Scene { get; set; } = new();

    public WebGlRuntimeOptions RuntimeOptions { get; set; } = new();

    public DateTimeOffset SavedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public string Source { get; set; } = string.Empty;

    public string SceneContentHash { get; set; } = string.Empty;

    public string DocumentHash { get; set; } = string.Empty;

    public string ContentHash { get; set; } = string.Empty;

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public interface IWebGlSceneDocumentMigrator
{
    WebGlSceneDocumentMigrationResult Migrate(WebGlSceneDocument document);
}

public sealed class WebGlSceneDocumentMigrationResult
{
    public WebGlSceneDocument Document { get; set; } = new();

    public bool Changed { get; set; }

    public List<string> AppliedMigrations { get; set; } = [];

    public List<string> Warnings { get; set; } = [];
}
