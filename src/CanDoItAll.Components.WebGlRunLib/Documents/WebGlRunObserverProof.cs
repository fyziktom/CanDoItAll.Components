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
    public string Route { get; set; } = string.Empty;
    public string Viewport { get; set; } = string.Empty;
    public string ScreenshotPath { get; set; } = string.Empty;
    public WebGlRuntimeDiagnostics? RuntimeDiagnostics { get; set; }
    public WebGlRuntimeIdleResult? RuntimeIdleResult { get; set; }
    public List<string> CompletedStageIds { get; set; } = [];
    public Dictionary<string, WebGlVector3> FinalObjectPositions { get; set; } = [];
    public List<string> RuntimeErrors { get; set; } = [];
    public List<string> RuntimeWarnings { get; set; } = [];
    public List<string> ConsoleErrors { get; set; } = [];
    public List<string> UiErrors { get; set; } = [];
    public List<string> UiWarnings { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunObserverProofReport
{
    public string SchemaVersion { get; set; } = WebGlRunObserverProof.CurrentSchemaVersion;
    public string ExpectedDocumentHash { get; set; } = string.Empty;
    public string BrowserLoadedDocumentHash { get; set; } = string.Empty;
    public string ExpectedSceneContentHash { get; set; } = string.Empty;
    public string BrowserLoadedSceneContentHash { get; set; } = string.Empty;
    public string ExpectedDriverHash { get; set; } = string.Empty;
    public string BrowserLoadedDriverHash { get; set; } = string.Empty;
    public bool DocumentHashesMatch { get; set; }
    public bool SceneContentHashesMatch { get; set; }
    public bool DriverHashesMatch { get; set; }
    public bool BrowserRuntimeValid { get; set; }
    public bool UiValid { get; set; }
    public bool ObserverProofValid { get; set; }
    public string ClaimStatus { get; set; } = "observer-not-run";
    public string Route { get; set; } = string.Empty;
    public string Viewport { get; set; } = string.Empty;
    public string ScreenshotPath { get; set; } = string.Empty;
    public bool RuntimeIdle { get; set; }
    public List<string> RuntimeIdleBlockers { get; set; } = [];
    public List<string> CompletedStageIds { get; set; } = [];
    public Dictionary<string, WebGlVector3> ExpectedFinalObjectPositions { get; set; } = [];
    public Dictionary<string, WebGlVector3> BrowserFinalObjectPositions { get; set; } = [];
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
        string expectedSceneHash = WebGlSceneDocumentSerializer.ComputeSceneContentHash(expectedDocument.InitialScene);
        string browserSceneHash = WebGlSceneDocumentSerializer.ComputeSceneContentHash(browserLoadedDocument.InitialScene);
        string expectedDriverHash = expectedDocument.Metadata.GetValueOrDefault(WebGlRunDriverMetadataKeys.DriverHash, string.Empty);
        string browserDriverHash = browserLoadedDocument.Metadata.GetValueOrDefault(WebGlRunDriverMetadataKeys.DriverHash, string.Empty);
        var report = new WebGlRunObserverProofReport
        {
            ExpectedDocumentHash = expectedHash,
            BrowserLoadedDocumentHash = browserHash,
            ExpectedSceneContentHash = expectedSceneHash,
            BrowserLoadedSceneContentHash = browserSceneHash,
            ExpectedDriverHash = expectedDriverHash,
            BrowserLoadedDriverHash = browserDriverHash,
            DocumentHashesMatch = string.Equals(expectedHash, browserHash, StringComparison.Ordinal),
            SceneContentHashesMatch = string.Equals(expectedSceneHash, browserSceneHash, StringComparison.Ordinal),
            DriverHashesMatch = !string.IsNullOrWhiteSpace(expectedDriverHash) &&
                                string.Equals(expectedDriverHash, browserDriverHash, StringComparison.Ordinal),
            BrowserRuntimeValid = observer.BrowserRuntimeExercised && observer.RuntimeErrors.Count == 0,
            UiValid = observer.UiExercised && observer.UiErrors.Count == 0,
            Route = observer.Route,
            Viewport = observer.Viewport,
            ScreenshotPath = observer.ScreenshotPath,
            RuntimeIdle = observer.RuntimeIdleResult?.Idle == true,
            RuntimeIdleBlockers = [.. observer.RuntimeIdleResult?.Blockers ?? []],
            CompletedStageIds = [.. observer.CompletedStageIds.Distinct(StringComparer.Ordinal).OrderBy(static id => id, StringComparer.Ordinal)],
            ExpectedFinalObjectPositions = BuildExpectedFinalObjectPositions(expectedDocument),
            BrowserFinalObjectPositions = new(observer.FinalObjectPositions, StringComparer.Ordinal),
            Metadata = new(observer.Metadata, StringComparer.Ordinal)
        };

        if (!report.DocumentHashesMatch)
        {
            report.Errors.Add("browser-document-hash-mismatch");
        }

        if (!report.SceneContentHashesMatch)
        {
            report.Errors.Add("browser-scene-content-hash-mismatch");
        }

        if (string.IsNullOrWhiteSpace(expectedDriverHash))
        {
            report.Errors.Add("expected-driver-hash-missing");
        }
        else if (string.IsNullOrWhiteSpace(browserDriverHash))
        {
            report.Errors.Add("browser-driver-hash-missing");
        }
        else if (!report.DriverHashesMatch)
        {
            report.Errors.Add("browser-driver-hash-mismatch");
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
        AddDiagnostics(report.Errors, "browser-console-error", observer.ConsoleErrors);
        AddDiagnostics(report.Errors, "ui-error", observer.UiErrors);
        AddDiagnostics(report.Warnings, "browser-runtime-warning", observer.RuntimeWarnings);
        AddDiagnostics(report.Warnings, "ui-warning", observer.UiWarnings);

        if (observer.RuntimeIdleResult is null)
        {
            report.Errors.Add("runtime-idle-proof-missing");
        }
        else if (!observer.RuntimeIdleResult.Idle)
        {
            report.Errors.Add($"runtime-idle-failed:{string.Join(",", observer.RuntimeIdleResult.Blockers)}");
        }

        foreach (string expectedStageId in ExpectedCompletedStageIds(expectedDocument))
        {
            if (!report.CompletedStageIds.Contains(expectedStageId, StringComparer.Ordinal))
            {
                report.Errors.Add($"completed-stage-missing:{expectedStageId}");
            }
        }

        foreach (KeyValuePair<string, WebGlVector3> expectedPosition in report.ExpectedFinalObjectPositions)
        {
            if (!report.BrowserFinalObjectPositions.TryGetValue(expectedPosition.Key, out WebGlVector3 browserPosition) ||
                !SameVector(expectedPosition.Value, browserPosition))
            {
                report.Errors.Add($"final-object-position-mismatch:{expectedPosition.Key}");
            }
        }

        report.ObserverProofValid = report.Errors.Count == 0;
        report.ClaimStatus = !observer.BrowserRuntimeExercised || !observer.UiExercised
            ? "observer-not-run"
            : report.ObserverProofValid
                ? "observer-valid"
                : "observer-failed";
        report.Metadata["browserRuntimeExercised"] = observer.BrowserRuntimeExercised.ToString(System.Globalization.CultureInfo.InvariantCulture);
        report.Metadata["uiExercised"] = observer.UiExercised.ToString(System.Globalization.CultureInfo.InvariantCulture);
        report.Metadata["runtimeIdle"] = report.RuntimeIdle.ToString(System.Globalization.CultureInfo.InvariantCulture);
        report.Metadata["completedStageCount"] = report.CompletedStageIds.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        report.Metadata["finalObjectPositionCount"] = report.BrowserFinalObjectPositions.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        report.Metadata["warningCount"] = report.Warnings.Count.ToString(System.Globalization.CultureInfo.InvariantCulture);
        report.Metadata["documentHashesMatch"] = report.DocumentHashesMatch.ToString(System.Globalization.CultureInfo.InvariantCulture);
        report.Metadata["sceneContentHashesMatch"] = report.SceneContentHashesMatch.ToString(System.Globalization.CultureInfo.InvariantCulture);
        report.Metadata["driverHashesMatch"] = report.DriverHashesMatch.ToString(System.Globalization.CultureInfo.InvariantCulture);
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

    private static List<string> ExpectedCompletedStageIds(WebGlRunDocument document)
        => [.. document.Timeline.Frames
            .SelectMany(static frame => frame.Stages)
            .Select(static stage => stage.StageId)
            .Where(static stageId => !string.IsNullOrWhiteSpace(stageId))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static stageId => stageId, StringComparer.Ordinal)];

    private static Dictionary<string, WebGlVector3> BuildExpectedFinalObjectPositions(WebGlRunDocument document)
    {
        var positions = new Dictionary<string, WebGlVector3>(StringComparer.Ordinal);
        foreach (WebGlSceneObject sceneObject in document.InitialScene.Scene.Objects)
        {
            if (!string.IsNullOrWhiteSpace(sceneObject.Id))
            {
                positions[sceneObject.Id] = sceneObject.Position;
            }
        }

        foreach (WebGlObjectMotionCommand motion in document.Timeline.Frames
                     .OrderBy(static frame => frame.Index)
                     .SelectMany(static frame => frame.Stages.OrderBy(static stage => stage.StageIndex))
                     .SelectMany(static stage => stage.Motions))
        {
            if (!string.IsNullOrWhiteSpace(motion.ObjectId))
            {
                positions[motion.ObjectId] = motion.TargetPosition;
            }
        }

        return positions;
    }

    private static bool SameVector(WebGlVector3 expected, WebGlVector3 actual)
        => IsNear(expected.X, actual.X) &&
           IsNear(expected.Y, actual.Y) &&
           IsNear(expected.Z, actual.Z);

    private static bool IsNear(double expected, double actual)
        => Math.Abs(expected - actual) <= 0.001;

    private static string Sha256(object value)
    {
        string json = JsonSerializer.Serialize(value, JsonOptions);
        return $"sha256:{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant()}";
    }
}
