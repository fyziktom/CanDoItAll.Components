using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunDocumentValidationResult
{
    public List<string> Errors { get; } = [];

    public List<string> Warnings { get; } = [];

    public bool IsValid => Errors.Count == 0;
}

public sealed class WebGlRunGenericBoundaryOptions
{
    public static WebGlRunGenericBoundaryOptions None { get; } = new();

    public IEnumerable<string> ForbiddenDomainTerms { get; init; } = [];
}

public sealed class WebGlRunDocumentValidator
{
    public const string CurrentSchemaVersion = "webgl-run-document/v1";

    private readonly WebGlRunTimelineValidator timelineValidator = new();
    private readonly WebGlRunGenericBoundaryOptions boundaryOptions;

    public WebGlRunDocumentValidator()
        : this(WebGlRunGenericBoundaryOptions.None)
    {
    }

    public WebGlRunDocumentValidator(WebGlRunGenericBoundaryOptions? boundaryOptions)
    {
        this.boundaryOptions = boundaryOptions ?? WebGlRunGenericBoundaryOptions.None;
    }

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

        ValidateDomainValue("document.runId", document.RunId.Value, result.Errors, boundaryOptions);
        ValidateDomainTerms("document.metadata", document.Metadata, result.Errors, boundaryOptions: boundaryOptions);
        ValidateDriverMetadata("document.metadata", document.Metadata, result.Errors);
        WebGlSceneDocumentValidationResult sceneValidation = WebGlSceneDocumentSerializer.Validate(document.InitialScene);
        foreach (string error in sceneValidation.Errors)
        {
            result.Errors.Add($"Initial scene: {error}");
        }

        foreach (string warning in sceneValidation.Warnings)
        {
            result.Warnings.Add($"Initial scene: {warning}");
        }

        ValidateInitialSceneDomainTerms(document.InitialScene, result.Errors, boundaryOptions);
        WebGlRunTimelineValidationResult timelineValidation = timelineValidator.Validate(document.Timeline);
        foreach (string error in timelineValidation.Errors)
        {
            result.Errors.Add(error);
        }

        ValidateFrames(document.Timeline.Frames, result, boundaryOptions);
        return result;
    }

    private static void ValidateInitialSceneDomainTerms(
        WebGlSceneDocument document,
        List<string> errors,
        WebGlRunGenericBoundaryOptions boundaryOptions)
    {
        ValidateDomainValue("initialScene.documentId", document.DocumentId, errors, boundaryOptions);
        ValidateDomainValue("initialScene.source", document.Source, errors, boundaryOptions);
        ValidateDomainTerms("initialScene.metadata", document.Metadata, errors, boundaryOptions: boundaryOptions);
        ValidateDomainValue("initialScene.scene.id", document.Scene.SceneId, errors, boundaryOptions);
        ValidateDomainTerms("initialScene.scene.metadata", document.Scene.Metadata, errors, boundaryOptions: boundaryOptions);
        ValidateDomainTerms("initialScene.scene.uiState.metadata", document.Scene.UiState.Metadata, errors, boundaryOptions: boundaryOptions);

        foreach (WebGlSceneObject sceneObject in document.Scene.Objects)
        {
            string objectScope = $"initialScene.scene.objects.{sceneObject.Id}";
            ValidateDomainValue($"{objectScope}.id", sceneObject.Id, errors, boundaryOptions);
            ValidateDomainValue($"{objectScope}.kind", sceneObject.Kind, errors, boundaryOptions);
            ValidateDomainValue($"{objectScope}.family", sceneObject.Family, errors, boundaryOptions);
            ValidateDomainTerms($"{objectScope}.metadata", sceneObject.Metadata, errors, boundaryOptions: boundaryOptions);
            foreach (string tag in sceneObject.Tags)
            {
                ValidateDomainValue($"{objectScope}.tags", tag, errors, boundaryOptions);
            }

            foreach (WebGlSceneObjectAnchor anchor in sceneObject.Anchors)
            {
                ValidateDomainValue($"{objectScope}.anchors.{anchor.Key}.key", anchor.Key, errors, boundaryOptions);
                ValidateDomainTerms($"{objectScope}.anchors.{anchor.Key}.metadata", anchor.Metadata, errors, boundaryOptions: boundaryOptions);
            }

            foreach (WebGlStatusSymbol symbol in sceneObject.Symbols)
            {
                ValidateDomainValue($"{objectScope}.symbols.{symbol.Id}.id", symbol.Id, errors, boundaryOptions);
                ValidateDomainValue($"{objectScope}.symbols.{symbol.Id}.semanticKind", symbol.SemanticKind, errors, boundaryOptions);
                ValidateDomainValue($"{objectScope}.symbols.{symbol.Id}.symbolAssetId", symbol.SymbolAssetId, errors, boundaryOptions);
                ValidateDomainTerms($"{objectScope}.symbols.{symbol.Id}.metadata", symbol.Metadata, errors, boundaryOptions: boundaryOptions);
            }
        }

        foreach (WebGlSceneLink link in document.Scene.Links)
        {
            string linkScope = $"initialScene.scene.links.{link.Id}";
            ValidateDomainValue($"{linkScope}.id", link.Id, errors, boundaryOptions);
            ValidateDomainValue($"{linkScope}.kind", link.Kind, errors, boundaryOptions);
            ValidateDomainTerms($"{linkScope}.metadata", link.Metadata, errors, boundaryOptions: boundaryOptions);
        }

        foreach (WebGlSceneLayer layer in document.Scene.Layers)
        {
            string layerScope = $"initialScene.scene.layers.{layer.Id}";
            ValidateDomainValue($"{layerScope}.id", layer.Id, errors, boundaryOptions);
            ValidateDomainValue($"{layerScope}.kind", layer.Kind, errors, boundaryOptions);
            ValidateDomainTerms($"{layerScope}.metadata", layer.Metadata, errors, boundaryOptions: boundaryOptions);
        }
    }

    private static void ValidateFrames(
        IEnumerable<WebGlRunFrame> frames,
        WebGlRunDocumentValidationResult result,
        WebGlRunGenericBoundaryOptions boundaryOptions)
    {
        foreach (WebGlRunFrame frame in frames)
        {
            if (!double.IsFinite(frame.TimeSeconds) || frame.TimeSeconds < 0)
            {
                result.Errors.Add($"Frame '{frame.Index}' time must be finite and non-negative.");
            }

            ValidateDomainTerms($"frame:{frame.Index}.metadata", frame.Metadata, result.Errors, boundaryOptions: boundaryOptions);
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
                    ValidateDomainValue($"stage:{stage.StageId}.id", stage.StageId, result.Errors, boundaryOptions);
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

                ValidateDomainTerms($"stage:{stage.StageId}.metadata", stage.Metadata, result.Errors, boundaryOptions: boundaryOptions);
                ValidateBarrier(stage, result.Errors);
            }
        }
    }

    internal static void ValidateDomainTerms(
        string scope,
        IReadOnlyDictionary<string, string> metadata,
        List<string> errors,
        bool allowSourceProvenance = true,
        WebGlRunGenericBoundaryOptions? boundaryOptions = null)
    {
        foreach (KeyValuePair<string, string> item in metadata)
        {
            if (allowSourceProvenance && WebGlRunGenericBoundaryPolicy.IsSourceProvenanceKey(item.Key))
            {
                WebGlRunGenericBoundaryPolicy.ValidateSourceProvenance($"{scope}.{item.Key}", item.Key, item.Value, errors, boundaryOptions);
                continue;
            }

            if (WebGlRunGenericBoundaryPolicy.IsDriverManifestKey(item.Key))
            {
                WebGlRunGenericBoundaryPolicy.ValidateDriverManifestMetadata($"{scope}.{item.Key}", item.Key, item.Value, errors);
                continue;
            }

            ValidateDomainValue($"{scope}.{item.Key}", item.Key, errors, boundaryOptions);
            ValidateDomainValue($"{scope}.{item.Key}.value", item.Value, errors, boundaryOptions);
        }
    }

    internal static void ValidateDomainValue(
        string scope,
        string value,
        List<string> errors,
        WebGlRunGenericBoundaryOptions? boundaryOptions = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        foreach (string term in WebGlRunGenericBoundaryPolicy.GetForbiddenDomainTerms(boundaryOptions))
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

    private static void ValidateDriverMetadata(
        string scope,
        IReadOnlyDictionary<string, string> metadata,
        List<string> errors)
    {
        bool hasDriverMetadata = metadata.Keys.Any(WebGlRunGenericBoundaryPolicy.IsDriverManifestKey);
        if (!hasDriverMetadata)
        {
            return;
        }

        RequireDriverMetadata(scope, metadata, WebGlRunDriverMetadataKeys.DriverId, errors);
        RequireDriverMetadata(scope, metadata, WebGlRunDriverMetadataKeys.DriverVersion, errors);
        RequireDriverMetadata(scope, metadata, WebGlRunDriverMetadataKeys.DriverHash, errors);
        RequireDriverMetadata(scope, metadata, WebGlRunDriverMetadataKeys.DriverManifestHash, errors);

        if (metadata.TryGetValue(WebGlRunDriverMetadataKeys.DriverHash, out string? driverHash) &&
            metadata.TryGetValue(WebGlRunDriverMetadataKeys.DriverManifestHash, out string? manifestHash) &&
            !string.Equals(driverHash, manifestHash, StringComparison.Ordinal))
        {
            errors.Add($"{scope}.{WebGlRunDriverMetadataKeys.DriverManifestHash} must match {WebGlRunDriverMetadataKeys.DriverHash}.");
        }
    }

    private static void RequireDriverMetadata(
        string scope,
        IReadOnlyDictionary<string, string> metadata,
        string key,
        List<string> errors)
    {
        if (!metadata.TryGetValue(key, out string? value) || string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"{scope}.{key} is required when driver metadata is present.");
            return;
        }

        WebGlRunGenericBoundaryPolicy.ValidateDriverManifestMetadata($"{scope}.{key}", key, value, errors);
    }
}

internal static class WebGlRunGenericBoundaryPolicy
{
    private const int MaxSourceProvenanceKeyLength = 96;
    private const int MaxSourceProvenanceValueLength = 512;
    private const int MaxDriverMetadataValueLength = 128;

    public static bool IsSourceProvenanceKey(string key)
        => key.StartsWith("source.", StringComparison.OrdinalIgnoreCase);

    public static bool IsDriverManifestKey(string key)
        => DriverManifestMetadataKeys.Contains(key);

    public static void ValidateSourceProvenance(
        string scope,
        string key,
        string value,
        List<string> errors,
        WebGlRunGenericBoundaryOptions? boundaryOptions = null)
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

        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (HashBackedSourceProvenanceKeys.Contains(key) && !IsStrictSha256(value))
        {
            errors.Add($"{scope}.value must be a strict sha256 hash.");
        }

        if (OpaqueSourceProvenanceKeys.Contains(key) && !IsOpaqueSourceToken(value))
        {
            errors.Add($"{scope}.value must be an opaque stable token or strict sha256 hash.");
        }

        if (!HashBackedSourceProvenanceKeys.Contains(key))
        {
            foreach (string term in GetForbiddenDomainTerms(boundaryOptions))
            {
                if (value.Contains(term, StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add($"{scope}.value contains domain-specific term '{term}'. Source provenance values must be opaque in generic documents.");
                }
            }
        }
    }

    public static void ValidateDriverManifestMetadata(string scope, string key, string value, List<string> errors)
    {
        if (!DriverManifestMetadataKeys.Contains(key))
        {
            errors.Add($"{scope} uses unsupported driver metadata key '{key}'.");
            return;
        }

        if (value.Length > MaxDriverMetadataValueLength)
        {
            errors.Add($"{scope}.value exceeds the driver metadata length limit of {MaxDriverMetadataValueLength} characters.");
        }

        if ((string.Equals(key, WebGlRunDriverMetadataKeys.DriverHash, StringComparison.Ordinal) ||
             string.Equals(key, WebGlRunDriverMetadataKeys.DriverManifestHash, StringComparison.Ordinal)) &&
            !IsStrictSha256(value))
        {
            errors.Add($"{scope}.value must be a strict sha256 hash.");
        }
    }

    public static string[] GetForbiddenDomainTerms(WebGlRunGenericBoundaryOptions? options)
        => [.. (options ?? WebGlRunGenericBoundaryOptions.None)
            .ForbiddenDomainTerms
            .Where(static term => !string.IsNullOrWhiteSpace(term))
            .Distinct(StringComparer.OrdinalIgnoreCase)];

    public static readonly HashSet<string> AllowedSourceProvenanceKeys = new(StringComparer.Ordinal)
    {
        "source.eventId",
        "source.inputPackHash",
        "source.kind",
        "source.anchorAlias",
        "source.category",
        "source.layerId",
        "source.layout.zone",
        "source.linkId",
        "source.nodeId",
        "source.nodeKind",
        "source.parentId",
        "source.provenanceMode",
        "source.sequence",
        "source.severity",
        "source.simulationFrameId",
        "source.sourceNodeId",
        "source.symbolId",
        "source.targetNodeId",
        "source.traceMapRef",
        WebGlRunDriverMetadataKeys.SourceTraceMapHash,
        "source.traceId",
        "source.visualActionId"
    };

    public static readonly HashSet<string> HashBackedSourceProvenanceKeys = new(StringComparer.Ordinal)
    {
        "source.inputPackHash",
        WebGlRunDriverMetadataKeys.SourceTraceMapHash
    };

    public static readonly HashSet<string> OpaqueSourceProvenanceKeys = new(StringComparer.Ordinal)
    {
        "source.eventId",
        "source.layerId",
        "source.linkId",
        "source.nodeId",
        "source.parentId",
        "source.simulationFrameId",
        "source.sourceNodeId",
        "source.symbolId",
        "source.targetNodeId",
        "source.traceMapRef",
        "source.traceId",
        "source.visualActionId"
    };

    public static readonly HashSet<string> DriverManifestMetadataKeys = new(StringComparer.Ordinal)
    {
        WebGlRunDriverMetadataKeys.DriverId,
        WebGlRunDriverMetadataKeys.DriverVersion,
        WebGlRunDriverMetadataKeys.DriverHash,
        WebGlRunDriverMetadataKeys.DriverManifestHash
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

    private static bool IsOpaqueSourceToken(string value)
    {
        if (IsStrictSha256(value))
        {
            return true;
        }

        int dotIndex = value.IndexOf('.', StringComparison.Ordinal);
        if (dotIndex <= 0 || dotIndex == value.Length - 1)
        {
            return false;
        }

        string prefix = value[..dotIndex];
        string suffix = value[(dotIndex + 1)..];
        return prefix.All(static character => char.IsLower(character) || char.IsDigit(character) || character == '-') &&
               suffix.Length is >= 12 and <= 64 &&
               suffix.All(static character => (character >= '0' && character <= '9') || (character >= 'a' && character <= 'f'));
    }

    private static bool IsStrictSha256(string value)
    {
        const string Prefix = "sha256:";
        if (value.Length != Prefix.Length + 64 ||
            !value.StartsWith(Prefix, StringComparison.Ordinal))
        {
            return false;
        }

        for (int index = Prefix.Length; index < value.Length; index++)
        {
            char character = value[index];
            if (!((character >= '0' && character <= '9') || (character >= 'a' && character <= 'f')))
            {
                return false;
            }
        }

        return true;
    }
}
