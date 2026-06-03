using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunDocumentValidationResult
{
    public List<string> Errors { get; } = [];

    public List<string> Warnings { get; } = [];

    public bool IsValid => Errors.Count == 0;
}

public sealed class WebGlRunDocumentValidator
{
    public const string CurrentSchemaVersion = "webgl-run-document/v1";

    private readonly WebGlRunTimelineValidator timelineValidator = new();

    public WebGlRunDocumentValidationResult Validate(WebGlRunDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        var result = new WebGlRunDocumentValidationResult();
        if (!string.Equals(document.SchemaVersion, CurrentSchemaVersion, StringComparison.Ordinal))
        {
            result.Errors.Add($"Unsupported WebGL run document schema '{document.SchemaVersion}'.");
        }

        if (string.IsNullOrWhiteSpace(document.RunId.Value))
        {
            result.Errors.Add("Run id is required.");
        }

        ValidateDomainTerms("document.metadata", document.Metadata, result.Errors);
        WebGlSceneDocumentValidationResult sceneValidation = WebGlSceneDocumentSerializer.Validate(document.InitialScene);
        foreach (string error in sceneValidation.Errors)
        {
            result.Errors.Add($"Initial scene: {error}");
        }

        foreach (string warning in sceneValidation.Warnings)
        {
            result.Warnings.Add($"Initial scene: {warning}");
        }

        WebGlRunTimelineValidationResult timelineValidation = timelineValidator.Validate(document.Timeline);
        foreach (string error in timelineValidation.Errors)
        {
            result.Errors.Add(error);
        }

        ValidateFrames(document.Timeline.Frames, result);
        return result;
    }

    private static void ValidateFrames(IEnumerable<WebGlRunFrame> frames, WebGlRunDocumentValidationResult result)
    {
        foreach (WebGlRunFrame frame in frames)
        {
            if (!double.IsFinite(frame.TimeSeconds) || frame.TimeSeconds < 0)
            {
                result.Errors.Add($"Frame '{frame.Index}' time must be finite and non-negative.");
            }

            ValidateDomainTerms($"frame:{frame.Index}.metadata", frame.Metadata, result.Errors);
            if (WebGlRunFrameCommandPolicy.HasMixedDirectAndStagedCommands(frame))
            {
                result.Errors.Add(WebGlRunFrameCommandPolicy.CreateMixedDirectAndStagedCommandsError(frame.Index));
            }

            var stageIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (WebGlRunActionStage stage in frame.Stages)
            {
                if (string.IsNullOrWhiteSpace(stage.StageId))
                {
                    result.Errors.Add($"Frame '{frame.Index}' contains a stage without a stage id.");
                }
                else
                {
                    ValidateDomainValue($"stage:{stage.StageId}.id", stage.StageId, result.Errors);
                    if (!stageIds.Add(stage.StageId))
                    {
                        result.Errors.Add($"Frame '{frame.Index}' contains duplicate stage id '{stage.StageId}'.");
                    }
                }

                if (!double.IsFinite(stage.StartsAtSeconds) || stage.StartsAtSeconds < 0)
                {
                    result.Errors.Add($"Stage '{stage.StageId}' start time must be finite and non-negative.");
                }

                if (!double.IsFinite(stage.WaitSeconds) || stage.WaitSeconds < 0)
                {
                    result.Errors.Add($"Stage '{stage.StageId}' wait seconds must be finite and non-negative.");
                }

                ValidateDomainTerms($"stage:{stage.StageId}.metadata", stage.Metadata, result.Errors);
                ValidateBarrier(stage, result.Errors);
            }
        }
    }

    internal static void ValidateDomainTerms(
        string scope,
        IReadOnlyDictionary<string, string> metadata,
        List<string> errors,
        bool allowSourceProvenance = true)
    {
        foreach (KeyValuePair<string, string> item in metadata)
        {
            if (allowSourceProvenance && WebGlRunGenericBoundaryPolicy.IsSourceProvenanceKey(item.Key))
            {
                WebGlRunGenericBoundaryPolicy.ValidateSourceProvenance($"{scope}.{item.Key}", item.Key, item.Value, errors);
                continue;
            }

            ValidateDomainValue($"{scope}.{item.Key}", item.Key, errors);
            ValidateDomainValue($"{scope}.{item.Key}.value", item.Value, errors);
        }
    }

    internal static void ValidateDomainValue(string scope, string value, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        foreach (string term in WebGlRunGenericBoundaryPolicy.ForbiddenDomainTerms)
        {
            if (value.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"{scope} contains domain-specific term '{term}'.");
            }
        }
    }

    internal static void ValidateBarrier(WebGlRunActionStage stage, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(stage.BarrierPolicy))
        {
            return;
        }

        string policy = stage.BarrierPolicy.Trim().ToLowerInvariant();
        if (!WebGlRunGenericBoundaryPolicy.AllowedSceneBarrierPolicies.Contains(policy))
        {
            errors.Add($"Stage '{stage.StageId}' uses unsupported barrier policy '{stage.BarrierPolicy}'.");
            return;
        }

        if (string.Equals(policy, WebGlSceneStageBarrierPolicies.WaitForEvent, StringComparison.Ordinal) &&
            string.IsNullOrWhiteSpace(stage.BarrierEventId))
        {
            errors.Add($"Stage '{stage.StageId}' uses event barrier without a barrier event id.");
        }

        if (string.Equals(policy, WebGlSceneStageBarrierPolicies.WaitForObjectMotions, StringComparison.Ordinal) &&
            stage.BarrierObjectIds.Count == 0)
        {
            errors.Add($"Stage '{stage.StageId}' uses object-motion barrier without barrier object ids.");
        }
    }
}

internal static class WebGlRunGenericBoundaryPolicy
{
    private const int MaxSourceProvenanceKeyLength = 96;
    private const int MaxSourceProvenanceValueLength = 512;

    public static bool IsSourceProvenanceKey(string key)
        => key.StartsWith("source.", StringComparison.OrdinalIgnoreCase);

    public static void ValidateSourceProvenance(string scope, string key, string value, List<string> errors)
    {
        if (!AllowedSourceProvenanceKeys.Contains(key))
        {
            errors.Add($"{scope} uses unsupported source provenance key '{key}'. Use the generic source provenance allowlist instead of arbitrary domain metadata.");
        }

        if (key.Length > MaxSourceProvenanceKeyLength)
        {
            errors.Add($"{scope} exceeds the source provenance key length limit of {MaxSourceProvenanceKeyLength} characters.");
        }

        foreach (string term in DisallowedSourcePolicyTerms)
        {
            if (key.Contains(term, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"{scope} uses executable or behavioral source policy term '{term}'. Source metadata is traceability only.");
            }
        }

        if (value.Length > MaxSourceProvenanceValueLength)
        {
            errors.Add($"{scope}.value exceeds the source provenance length limit of {MaxSourceProvenanceValueLength} characters.");
        }
    }

    public static readonly string[] ForbiddenDomainTerms =
    [
        "economy",
        "ledger",
        "market",
        "production-line",
        "productionline",
        "work-order",
        "workorder",
        "machine",
        "account",
        "buyer",
        "seller",
        "price",
        "vernon"
    ];

    public static readonly HashSet<string> AllowedSourceProvenanceKeys = new(StringComparer.Ordinal)
    {
        "source.eventId",
        "source.domain",
        "source.inputPackHash",
        "source.kind",
        "source.parentId",
        "source.sequence",
        "source.simulationFrameId",
        "source.traceId",
        "source.visualActionId"
    };

    public static readonly string[] DisallowedSourcePolicyTerms =
    [
        "assetQualityProfile",
        "barrierPolicy",
        "batchingPolicy",
        "behavior",
        "command",
        "permission",
        "policy",
        "renderMode",
        "rule",
        "script"
    ];

    public static readonly HashSet<string> AllowedSceneBarrierPolicies = new(StringComparer.Ordinal)
    {
        WebGlSceneStageBarrierPolicies.None,
        WebGlSceneStageBarrierPolicies.WaitSeconds,
        WebGlSceneStageBarrierPolicies.WaitForActiveMotions,
        WebGlSceneStageBarrierPolicies.WaitForObjectMotions,
        WebGlSceneStageBarrierPolicies.WaitForRenderIdle,
        WebGlSceneStageBarrierPolicies.WaitForEvent
    };
}
