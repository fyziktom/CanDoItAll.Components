namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlModelImportOptions
{
    public double UnitScale { get; set; } = 1;

    public string FitMode { get; set; } = WebGlModelFitModes.FitBounds;

    public string CenterMode { get; set; } = WebGlModelCenterModes.CenterBottom;

    public double FixedScale { get; set; } = 1;

    public WebGlVector3 RotationOffset { get; set; } = WebGlVector3.Zero;

    public WebGlVector3 PositionOffset { get; set; } = WebGlVector3.Zero;

    public bool ForceDoubleSidedMaterial { get; set; }

    public bool NormalizeMaterialVisibility { get; set; }

    public bool DebugBounds { get; set; }

    public bool DisableTint { get; set; }

    public string CameraPresetHint { get; set; } = string.Empty;

    public List<string> KnownIssueNotes { get; set; } = [];
}

public static class WebGlModelFitModes
{
    public const string FitBounds = "fit-bounds";
    public const string OriginalScale = "original-scale";
    public const string FixedScale = "fixed-scale";
}

public static class WebGlModelCenterModes
{
    public const string CenterBottom = "center-bottom";
    public const string CenterBounds = "center-bounds";
    public const string PreserveOrigin = "preserve-origin";
}

public sealed class WebGlModelImportRecipe
{
    public string Id { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public WebGlModelImportOptions Options { get; set; } = new();

    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlModelImportRecipeResolver
{
    public WebGlModelImportOptions Resolve(WebGlAssetCatalog catalog, WebGlAssetDefinition asset, WebGlAssetVariant? variant = null)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(asset);

        var resolved = Merge(new WebGlModelImportOptions(), ResolveRecipe(catalog, asset.ImportRecipeId)?.Options);
        resolved = Merge(resolved, asset.ImportOptions);
        if (variant is null)
        {
            return resolved;
        }

        resolved = Merge(resolved, ResolveRecipe(catalog, variant.ImportRecipeId)?.Options);
        return Merge(resolved, variant.ImportOptions);
    }

    private static WebGlModelImportRecipe? ResolveRecipe(WebGlAssetCatalog catalog, string recipeId)
        => string.IsNullOrWhiteSpace(recipeId)
            ? null
            : catalog.ModelImportRecipes.FirstOrDefault(item => string.Equals(item.Id, recipeId, StringComparison.Ordinal));

    private static WebGlModelImportOptions Merge(WebGlModelImportOptions current, WebGlModelImportOptions? next)
    {
        if (next is null)
        {
            return current;
        }

        return new WebGlModelImportOptions
        {
            UnitScale = next.UnitScale != 1 ? next.UnitScale : current.UnitScale,
            FitMode = string.IsNullOrWhiteSpace(next.FitMode) ? current.FitMode : next.FitMode,
            CenterMode = string.IsNullOrWhiteSpace(next.CenterMode) ? current.CenterMode : next.CenterMode,
            FixedScale = next.FixedScale != 1 ? next.FixedScale : current.FixedScale,
            RotationOffset = next.RotationOffset == WebGlVector3.Zero ? current.RotationOffset : next.RotationOffset,
            PositionOffset = next.PositionOffset == WebGlVector3.Zero ? current.PositionOffset : next.PositionOffset,
            ForceDoubleSidedMaterial = next.ForceDoubleSidedMaterial || current.ForceDoubleSidedMaterial,
            NormalizeMaterialVisibility = next.NormalizeMaterialVisibility || current.NormalizeMaterialVisibility,
            DebugBounds = next.DebugBounds || current.DebugBounds,
            DisableTint = next.DisableTint || current.DisableTint,
            CameraPresetHint = string.IsNullOrWhiteSpace(next.CameraPresetHint) ? current.CameraPresetHint : next.CameraPresetHint,
            KnownIssueNotes = next.KnownIssueNotes.Count > 0 ? [.. next.KnownIssueNotes] : [.. current.KnownIssueNotes]
        };
    }
}
