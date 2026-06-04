using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunObserverSnapshot
{
    public bool BrowserRuntimeExercised { get; set; }
    public bool UiExercised { get; set; }
    public List<string> RuntimeErrors { get; set; } = [];
    public List<string> RuntimeWarnings { get; set; } = [];
    public List<string> UiErrors { get; set; } = [];
    public List<string> UiWarnings { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunObserverProofReport
{
    public string SchemaVersion { get; set; } = WebGlRunObserverProof.CurrentSchemaVersion;
    public string ExpectedDocumentHash { get; set; } = string.Empty;
    public string BrowserLoadedDocumentHash { get; set; } = string.Empty;
    public bool DocumentHashesMatch { get; set; }
    public bool BrowserRuntimeValid { get; set; }
    public bool UiValid { get; set; }
    public bool ObserverProofValid { get; set; }
    public string ClaimStatus { get; set; } = "observer-not-run";
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public static class WebGlRunObserverProof
{
    public const string CurrentSchemaVersion = "webgl-run-observer-proof/v1";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static WebGlRunObserverProofReport Compare(
        WebGlRunDocument expectedDocument,
        WebGlRunDocument browserLoadedDocument,
        WebGlRunObserverSnapshot observer)
    {
        ArgumentNullException.ThrowIfNull(expectedDocument);
        ArgumentNullException.ThrowIfNull(browserLoadedDocument);
        ArgumentNullException.ThrowIfNull(observer);

        string expectedHash = ComputeDocumentHash(expectedDocument);
        string browserHash = ComputeDocumentHash(browserLoadedDocument);
        var report = new WebGlRunObserverProofReport
        {
            ExpectedDocumentHash = expectedHash,
            BrowserLoadedDocumentHash = browserHash,
            DocumentHashesMatch = string.Equals(expectedHash, browserHash, StringComparison.Ordinal),
            BrowserRuntimeValid = observer.BrowserRuntimeExercised && observer.RuntimeErrors.Count == 0,
            UiValid = observer.UiExercised && observer.UiErrors.Count == 0,
            Metadata = new(observer.Metadata, StringComparer.Ordinal)
        };

        if (!report.DocumentHashesMatch)
        {
            report.Errors.Add("browser-document-hash-mismatch");
        }

        if (!observer.BrowserRuntimeExercised)
        {
            report.Errors.Add("browser-runtime-not-exercised");
        }

        if (!observer.UiExercised)
        {
            report.Errors.Add("ui-not-exercised");
        }

        AddDiagnostics(report.Errors, "browser-runtime-error", observer.RuntimeErrors);
        AddDiagnostics(report.Errors, "ui-error", observer.UiErrors);
        AddDiagnostics(report.Warnings, "browser-runtime-warning", observer.RuntimeWarnings);
        AddDiagnostics(report.Warnings, "ui-warning", observer.UiWarnings);

        report.ObserverProofValid = report.Errors.Count == 0;
        report.ClaimStatus = !observer.BrowserRuntimeExercised || !observer.UiExercised
            ? "observer-not-run"
            : report.ObserverProofValid
                ? "observer-valid"
                : "observer-failed";
        report.Metadata["browserRuntimeExercised"] = observer.BrowserRuntimeExercised.ToString(System.Globalization.CultureInfo.InvariantCulture);
        report.Metadata["uiExercised"] = observer.UiExercised.ToString(System.Globalization.CultureInfo.InvariantCulture);
        report.Metadata["warningCount"] = report.Warnings.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        return report;
    }

    public static string ComputeDocumentHash(WebGlRunDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        string sceneContentHash = WebGlSceneDocumentSerializer.ComputeSceneContentHash(document.InitialScene);
        string timelineIdentity = new WebGlRunPlaybackClock().BuildDeterministicTimelineIdentity(document);
        return Sha256(new
        {
            document.SchemaVersion,
            RunId = document.RunId.Value,
            InitialSceneContentHash = sceneContentHash,
            TimelineIdentity = timelineIdentity,
            Metadata = new SortedDictionary<string, string>(document.Metadata, StringComparer.Ordinal)
        });
    }

    private static void AddDiagnostics(List<string> target, string prefix, IEnumerable<string> values)
    {
        foreach (string value in values.Where(static value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.Ordinal))
        {
            target.Add($"{prefix}:{value}");
        }
    }

    private static string Sha256(object value)
    {
        string json = JsonSerializer.Serialize(value, JsonOptions);
        return $"sha256:{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant()}";
    }
}
