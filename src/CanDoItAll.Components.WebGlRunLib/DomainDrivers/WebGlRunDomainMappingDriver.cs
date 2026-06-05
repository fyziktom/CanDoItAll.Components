using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CanDoItAll.Components.WebGlRunLib;

public interface IWebGlRunDomainMappingDriver
{
    string DriverId { get; }
    string DriverVersion => "1.0.0";
    string DisplayName { get; }
    WebGlRunGenericBoundaryOptions BoundaryOptions { get; }
    IReadOnlyCollection<string> DriverActionKinds { get; }
    string DriverHash => WebGlRunDomainMappingDriverManifest.ComputeDriverHash(this);
    WebGlRunDomainMappingDriverManifest Manifest => WebGlRunDomainMappingDriverManifest.FromDriver(this);
    string MapToGenericActionKind(string driverActionKind);

    IReadOnlyDictionary<string, string> ScrubMetadata(IReadOnlyDictionary<string, string> metadata)
        => WebGlRunDomainMappingDriverMetadataScrubber.Scrub(metadata, BoundaryOptions);

    WebGlRunDomainMappingDriverValidationResult Validate()
        => WebGlRunDomainMappingDriverValidator.Validate(this);
}

public sealed class WebGlRunPassThroughDomainMappingDriver : IWebGlRunDomainMappingDriver
{
    public static WebGlRunPassThroughDomainMappingDriver Instance { get; } = new();

    public string DriverId => "generic-pass-through";
    public string DriverVersion => "1.0.0";
    public string DisplayName => "Generic pass-through";
    public WebGlRunGenericBoundaryOptions BoundaryOptions => WebGlRunGenericBoundaryOptions.None;
    public IReadOnlyCollection<string> DriverActionKinds => [];

    public string MapToGenericActionKind(string driverActionKind)
        => string.IsNullOrWhiteSpace(driverActionKind) ? WebGlRunActionKinds.Wait : driverActionKind;
}

public sealed class WebGlRunDomainMappingDriverManifest
{
    public const string CurrentSchemaVersion = "webgl-run-domain-driver-manifest/v1";

    public string SchemaVersion { get; set; } = CurrentSchemaVersion;
    public string DriverId { get; set; } = string.Empty;
    public string DriverVersion { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string DriverHash { get; set; } = string.Empty;
    public Dictionary<string, string> ActionKindMappings { get; set; } = new(StringComparer.Ordinal);
    public List<string> ForbiddenDomainTerms { get; set; } = [];
    public List<string> AllowedSourceProvenanceKeys { get; set; } = [];

    public static WebGlRunDomainMappingDriverManifest FromDriver(IWebGlRunDomainMappingDriver driver)
    {
        ArgumentNullException.ThrowIfNull(driver);
        var manifest = new WebGlRunDomainMappingDriverManifest
        {
            DriverId = driver.DriverId,
            DriverVersion = driver.DriverVersion,
            DisplayName = driver.DisplayName,
            ActionKindMappings = driver.DriverActionKinds
                .Where(static kind => !string.IsNullOrWhiteSpace(kind))
                .Distinct(StringComparer.Ordinal)
                .OrderBy(static kind => kind, StringComparer.Ordinal)
                .ToDictionary(static kind => kind, kind => driver.MapToGenericActionKind(kind), StringComparer.Ordinal),
            ForbiddenDomainTerms = [.. WebGlRunGenericBoundaryPolicy.GetForbiddenDomainTerms(driver.BoundaryOptions)
                .OrderBy(static term => term, StringComparer.OrdinalIgnoreCase)],
            AllowedSourceProvenanceKeys = [.. WebGlRunGenericBoundaryPolicy.AllowedSourceProvenanceKeys
                .OrderBy(static key => key, StringComparer.Ordinal)]
        };
        manifest.DriverHash = ComputeManifestHash(manifest);
        return manifest;
    }

    public static string ComputeDriverHash(IWebGlRunDomainMappingDriver driver)
        => FromDriver(driver).DriverHash;

    public static string ComputeManifestHash(WebGlRunDomainMappingDriverManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);
        return Sha256(new
        {
            manifest.SchemaVersion,
            manifest.DriverId,
            manifest.DriverVersion,
            manifest.DisplayName,
            ActionKindMappings = manifest.ActionKindMappings
                .OrderBy(static item => item.Key, StringComparer.Ordinal)
                .ToDictionary(static item => item.Key, static item => item.Value, StringComparer.Ordinal),
            ForbiddenDomainTerms = manifest.ForbiddenDomainTerms
                .OrderBy(static term => term, StringComparer.OrdinalIgnoreCase),
            AllowedSourceProvenanceKeys = manifest.AllowedSourceProvenanceKeys
                .OrderBy(static key => key, StringComparer.Ordinal)
        });
    }

    private static string Sha256(object value)
    {
        string json = JsonSerializer.Serialize(value, JsonOptions);
        return $"sha256:{Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant()}";
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}

public sealed class WebGlRunDomainMappingDriverValidationResult
{
    public List<string> Errors { get; } = [];
    public List<string> Warnings { get; } = [];
    public bool IsValid => Errors.Count == 0;
}

public static class WebGlRunDomainMappingDriverMetadataScrubber
{
    public static Dictionary<string, string> Scrub(
        IReadOnlyDictionary<string, string> metadata,
        WebGlRunGenericBoundaryOptions? boundaryOptions)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        var scrubbed = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (KeyValuePair<string, string> item in metadata)
        {
            if (string.IsNullOrWhiteSpace(item.Key))
            {
                continue;
            }

            if (WebGlRunGenericBoundaryPolicy.IsSourceProvenanceKey(item.Key))
            {
                if (WebGlRunGenericBoundaryPolicy.AllowedSourceProvenanceKeys.Contains(item.Key))
                {
                    scrubbed[item.Key] = item.Value;
                }

                continue;
            }

            if (ContainsForbiddenDomainTerm(item.Key, boundaryOptions) ||
                ContainsForbiddenDomainTerm(item.Value, boundaryOptions))
            {
                continue;
            }

            scrubbed[item.Key] = item.Value;
        }

        return scrubbed;
    }

    private static bool ContainsForbiddenDomainTerm(string value, WebGlRunGenericBoundaryOptions? boundaryOptions)
        => !string.IsNullOrWhiteSpace(value) &&
           WebGlRunGenericBoundaryPolicy.GetForbiddenDomainTerms(boundaryOptions)
               .Any(term => value.Contains(term, StringComparison.OrdinalIgnoreCase));
}

public static class WebGlRunDomainMappingDriverValidator
{
    public static WebGlRunDomainMappingDriverValidationResult Validate(IWebGlRunDomainMappingDriver driver)
    {
        ArgumentNullException.ThrowIfNull(driver);
        var result = new WebGlRunDomainMappingDriverValidationResult();
        if (string.IsNullOrWhiteSpace(driver.DriverId))
        {
            result.Errors.Add("driver-id-required");
        }

        if (string.IsNullOrWhiteSpace(driver.DriverVersion))
        {
            result.Errors.Add("driver-version-required");
        }

        if (!IsStrictSha256(driver.DriverHash))
        {
            result.Errors.Add("driver-hash-invalid");
        }

        WebGlRunDomainMappingDriverManifest manifest = driver.Manifest;
        if (!string.Equals(driver.DriverHash, manifest.DriverHash, StringComparison.Ordinal))
        {
            result.Errors.Add("driver-manifest-hash-mismatch");
        }

        foreach (string driverActionKind in driver.DriverActionKinds.Where(static kind => !string.IsNullOrWhiteSpace(kind)))
        {
            string mappedActionKind = driver.MapToGenericActionKind(driverActionKind);
            if (!WebGlRunActionKinds.All.Contains(mappedActionKind))
            {
                result.Errors.Add($"driver-action-maps-to-unsupported-generic-kind:{driverActionKind}:{mappedActionKind}");
            }

            if (ContainsForbiddenDomainTerm(mappedActionKind, driver.BoundaryOptions))
            {
                result.Errors.Add($"driver-action-maps-to-domain-shaped-generic-kind:{driverActionKind}:{mappedActionKind}");
            }
        }

        string[] forbiddenDomainTerms = WebGlRunGenericBoundaryPolicy.GetForbiddenDomainTerms(driver.BoundaryOptions);
        if (forbiddenDomainTerms.Length > 0)
        {
            string forbiddenTerm = forbiddenDomainTerms[0];
            IReadOnlyDictionary<string, string> scrubbed = driver.ScrubMetadata(new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["source.eventId"] = $"{forbiddenTerm}-opaque-source-id",
                ["source.policy"] = "should-be-dropped",
                [$"driver.{forbiddenTerm}Policy"] = $"{forbiddenTerm} behavior",
                ["generic"] = "neutral"
            });
            if (!scrubbed.ContainsKey("source.eventId"))
            {
                result.Errors.Add("driver-scrubber-dropped-allowed-source-provenance");
            }

            if (scrubbed.ContainsKey("source.policy") || scrubbed.ContainsKey($"driver.{forbiddenTerm}Policy"))
            {
                result.Errors.Add("driver-scrubber-left-domain-or-policy-metadata");
            }
        }

        return result;
    }

    private static bool ContainsForbiddenDomainTerm(string value, WebGlRunGenericBoundaryOptions? boundaryOptions)
        => !string.IsNullOrWhiteSpace(value) &&
           WebGlRunGenericBoundaryPolicy.GetForbiddenDomainTerms(boundaryOptions)
               .Any(term => value.Contains(term, StringComparison.OrdinalIgnoreCase));

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
