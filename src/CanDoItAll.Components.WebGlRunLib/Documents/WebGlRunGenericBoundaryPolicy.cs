using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlRunLib;

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
