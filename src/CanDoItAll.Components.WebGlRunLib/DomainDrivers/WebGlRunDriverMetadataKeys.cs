namespace CanDoItAll.Components.WebGlRunLib;

public static class WebGlRunDriverMetadataKeys
{
    public const string DriverId = "driver.id";
    public const string DriverVersion = "driver.version";
    public const string DriverHash = "driver.hash";
    public const string DriverManifestHash = "driver.manifestHash";
    public const string SourceTraceMapHash = "source.traceMapHash";

    public static void Stamp(
        IDictionary<string, string> metadata,
        IWebGlRunDomainMappingDriver driver,
        string traceMapHash = "")
    {
        ArgumentNullException.ThrowIfNull(metadata);
        ArgumentNullException.ThrowIfNull(driver);

        WebGlRunDomainMappingDriverManifest manifest = driver.Manifest;
        metadata[DriverId] = manifest.DriverId;
        metadata[DriverVersion] = manifest.DriverVersion;
        metadata[DriverHash] = driver.DriverHash;
        metadata[DriverManifestHash] = manifest.DriverHash;
        if (!string.IsNullOrWhiteSpace(traceMapHash))
        {
            metadata[SourceTraceMapHash] = traceMapHash;
        }
    }
}
