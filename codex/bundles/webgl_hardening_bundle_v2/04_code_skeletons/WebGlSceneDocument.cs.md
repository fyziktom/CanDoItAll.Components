# WebGlSceneDocument.cs skeleton

```csharp
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneDocument
{
    public string SchemaVersion { get; set; } = "webgl-scene-document.v1";

    public string DocumentId { get; set; } = string.Empty;

    public WebGlSceneModel Scene { get; set; } = new();

    public WebGlRuntimeOptions RuntimeOptions { get; set; } = new();

    public DateTimeOffset SavedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public string Source { get; set; } = string.Empty;

    public string ContentHash { get; set; } = string.Empty;

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public static class WebGlSceneDocumentSerializer
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    public static string Serialize(WebGlSceneDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        document.ContentHash = string.Empty;
        var jsonWithoutHash = JsonSerializer.Serialize(document, Options);
        document.ContentHash = ComputeSha256(jsonWithoutHash);
        return JsonSerializer.Serialize(document, Options);
    }

    public static WebGlSceneDocument? Deserialize(string json)
        => JsonSerializer.Deserialize<WebGlSceneDocument>(json, Options);

    private static string ComputeSha256(string text)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
```

This is generic layout serialization only. Do not add save slots, persistence providers, replay logs, or run semantics here.
