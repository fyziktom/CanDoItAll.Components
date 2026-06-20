namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlAssetVariant
{
    public string Id { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Uri { get; set; } = string.Empty;

    public string QualityTier { get; set; } = WebGlAssetQualityTiers.Unknown;

    public string Format { get; set; } = WebGlAssetFormats.Glb;

    public string PrimitiveKind { get; set; } = WebGlPrimitiveKinds.Box;

    public string FallbackAssetId { get; set; } = string.Empty;

    public WebGlAssetPerformanceHint PerformanceHint { get; set; } = new();

    public string Color { get; set; } = string.Empty;

    public WebGlVector3 Scale { get; set; } = WebGlVector3.One;

    public string ImportRecipeId { get; set; } = string.Empty;

    public WebGlModelImportOptions ImportOptions { get; set; } = new();

    public Dictionary<string, string> Metadata { get; set; } = [];
}
