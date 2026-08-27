using CanDoItAll.Components.BaseLib;
using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlSandbox;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.WebGlSandbox.Components.Pages;

public partial class ModelLab
{
    private readonly WebGlAssetCatalog catalog = WebGlSandboxAssetCatalogFactory.Create();
    private WebGlRuntimeOptions runtimeOptions = new()
    {
        DeterministicMode = true,
        ShowDiagnosticsPanel = true,
        ShowLabels = false,
        ShowSymbols = false,
        AutoFitOnCreate = true,
        RenderMode = WebGlRenderModes.Auto,
        AssetQualityProfile = WebGlAssetQualityProfiles.Primitive
    };
    private WebGlSceneModel scene = default!;
    private WebGlSceneView? sceneView;
    private WebGlRuntimeDiagnostics? latestDiagnostics;
    private WebGlSceneProofSnapshot? latestSnapshot;
    private string selectedAssetId = "asset.building.house.default";
    private string selectedVariantId = string.Empty;
    private string selectedProfile = WebGlAssetQualityProfiles.Primitive;
    private string runtimeStatus = "initializing";
    private string runtimeError = string.Empty;
    private bool debugBounds;
    private bool pendingDiagnosticsCapture;
    private WebGlModelImportOptions localImportOptions = new();
    private string suggestedRecipeJson = string.Empty;

    protected override void OnInitialized()
    {
        ResetLocalImportOptions();
        RebuildScene();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!pendingDiagnosticsCapture || sceneView is null)
        {
            return;
        }

        pendingDiagnosticsCapture = false;
        await Task.Delay(1400);
        await CaptureProofAsync();
        await InvokeAsync(StateHasChanged);
    }

    private WebGlAssetDefinition? SelectedAsset => catalog.Assets.FirstOrDefault(item => item.Id == selectedAssetId);

    private WebGlModelDiagnostics? ActiveDiagnostics => latestDiagnostics?.ModelDiagnostics
        .FirstOrDefault(item => item.AssetId == selectedAssetId && (string.IsNullOrWhiteSpace(selectedVariantId) || item.VariantId == selectedVariantId));

    private string DiagnosticsStatus => ActiveDiagnostics is null ? "pending" : ActiveDiagnostics.Errors.Count > 0 ? "error" : ActiveDiagnostics.Warnings.Count > 0 ? "warning" : "ok";

    private string DiagnosticsTone => ActiveDiagnostics is null ? "info" : ActiveDiagnostics.Errors.Count > 0 ? "danger" : ActiveDiagnostics.Warnings.Count > 0 ? "warning" : "success";

    private string BoundsText => ActiveDiagnostics is null
        ? "Bounds pending."
        : $"Bounds {ActiveDiagnostics.Size.X:0.###} x {ActiveDiagnostics.Size.Y:0.###} x {ActiveDiagnostics.Size.Z:0.###}; center {ActiveDiagnostics.Center.X:0.###}, {ActiveDiagnostics.Center.Y:0.###}, {ActiveDiagnostics.Center.Z:0.###}.";

    private string ActiveImportOptionsText
        => $"scale {localImportOptions.UnitScale:0.###}; {localImportOptions.FitMode}; {localImportOptions.CenterMode}; double-sided {localImportOptions.ForceDoubleSidedMaterial}; visibility {localImportOptions.NormalizeMaterialVisibility}; tint disabled {localImportOptions.DisableTint}.";

    private ButtonTone ProfileStyle(string profile)
        => string.Equals(selectedProfile, profile, StringComparison.Ordinal)
            ? ButtonTone.Primary
            : ButtonTone.Default;

    private void SetProfile(string profile)
    {
        selectedProfile = WebGlAssetVariantResolver.NormalizeProfile(profile);
        runtimeOptions.AssetQualityProfile = selectedProfile;
        RebuildScene();
    }

    private void ToggleDebugBounds()
    {
        debugBounds = !debugBounds;
        RebuildScene();
    }

    private void HandleAssetChanged(ChangeEventArgs args)
    {
        selectedAssetId = Convert.ToString(args.Value) ?? selectedAssetId;
        selectedVariantId = string.Empty;
        ResetLocalImportOptions();
        RebuildScene();
    }

    private void HandleVariantChanged(ChangeEventArgs args)
    {
        selectedVariantId = Convert.ToString(args.Value) ?? string.Empty;
        ResetLocalImportOptions();
        RebuildScene();
    }

    private void HandleUnitScaleChanged(ChangeEventArgs args)
    {
        localImportOptions.UnitScale = Math.Max(0.0001, Convert.ToDouble(args.Value, System.Globalization.CultureInfo.InvariantCulture));
        RebuildScene();
    }

    private void HandleFitModeChanged(ChangeEventArgs args)
    {
        localImportOptions.FitMode = Convert.ToString(args.Value) ?? WebGlModelFitModes.FitBounds;
        RebuildScene();
    }

    private void HandleCenterModeChanged(ChangeEventArgs args)
    {
        localImportOptions.CenterMode = Convert.ToString(args.Value) ?? WebGlModelCenterModes.CenterBottom;
        RebuildScene();
    }

    private void HandleDoubleSidedChanged(ChangeEventArgs args)
    {
        localImportOptions.ForceDoubleSidedMaterial = Convert.ToBoolean(args.Value);
        RebuildScene();
    }

    private void HandleNormalizeVisibilityChanged(ChangeEventArgs args)
    {
        localImportOptions.NormalizeMaterialVisibility = Convert.ToBoolean(args.Value);
        RebuildScene();
    }

    private void HandleDisableTintChanged(ChangeEventArgs args)
    {
        localImportOptions.DisableTint = Convert.ToBoolean(args.Value);
        RebuildScene();
    }

    private void ExportRecipeJson()
    {
        suggestedRecipeJson = System.Text.Json.JsonSerializer.Serialize(new WebGlModelImportRecipe
        {
            Id = $"recipe.{selectedAssetId}.{(string.IsNullOrWhiteSpace(selectedVariantId) ? "base" : selectedVariantId)}",
            DisplayName = $"{SelectedAsset?.DisplayName ?? selectedAssetId} import recipe",
            Options = localImportOptions,
            Metadata =
            {
                ["assetId"] = selectedAssetId,
                ["variantId"] = selectedVariantId,
                ["profile"] = selectedProfile
            }
        }, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web) { WriteIndented = true });
    }

    private async Task CaptureProofAsync()
    {
        latestSnapshot = sceneView is null ? null : await sceneView.GetProofSnapshotAsync();
        latestDiagnostics = sceneView is null ? null : await sceneView.GetDiagnosticsAsync();
        if (latestDiagnostics is not null)
        {
            runtimeStatus = $"ready; renders {latestDiagnostics.RenderCount}; models {latestDiagnostics.ModelInstanceCount}; fallbacks {latestDiagnostics.FallbackObjectCount}";
            runtimeError = string.Empty;
        }
    }

    private async Task HandleRuntimeReady(WebGlRuntimeReadyEventArgs args)
    {
        runtimeStatus = $"ready; renders {args.Diagnostics.RenderCount}; models {args.Diagnostics.ModelInstanceCount}; fallbacks {args.Diagnostics.FallbackObjectCount}";
        runtimeError = string.Empty;
        await CaptureProofAsync();
    }

    private Task HandleRuntimeError(WebGlRuntimeErrorEventArgs args)
    {
        runtimeStatus = "runtime error";
        runtimeError = args.Detail;
        return InvokeAsync(StateHasChanged);
    }

    private void RebuildScene()
    {
        runtimeStatus = "loading";
        latestDiagnostics = null;
        latestSnapshot = null;
        pendingDiagnosticsCapture = true;
        localImportOptions.DebugBounds = debugBounds;
        var labCatalog = CloneCatalogWithImportOptions(catalog, selectedAssetId, selectedVariantId, localImportOptions);
        scene = WebGlModelLabSceneFactory.Create(labCatalog, selectedAssetId, selectedVariantId, selectedProfile);
    }

    private void ResetLocalImportOptions()
    {
        var options = selectedVariantId.Length > 0
            ? SelectedAsset?.Variants.FirstOrDefault(item => item.Id == selectedVariantId)?.ImportOptions
            : SelectedAsset?.ImportOptions;
        localImportOptions = CloneOptions(options ?? new WebGlModelImportOptions());
        localImportOptions.DebugBounds = debugBounds;
        suggestedRecipeJson = string.Empty;
    }

    private static WebGlModelImportOptions CloneOptions(WebGlModelImportOptions options)
        => System.Text.Json.JsonSerializer.Deserialize<WebGlModelImportOptions>(
            System.Text.Json.JsonSerializer.Serialize(options)) ?? new();

    private static WebGlAssetCatalog CloneCatalogWithImportOptions(WebGlAssetCatalog source, string assetId, string variantId, WebGlModelImportOptions options)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(source);
        var clone = System.Text.Json.JsonSerializer.Deserialize<WebGlAssetCatalog>(json) ?? WebGlSandboxAssetCatalogFactory.Create();
        var asset = clone.Assets.FirstOrDefault(item => item.Id == assetId);
        if (asset is null)
        {
            return clone;
        }

        if (string.IsNullOrWhiteSpace(variantId))
        {
            asset.ImportOptions = CloneOptions(options);
            return clone;
        }

        var variant = asset.Variants.FirstOrDefault(item => item.Id == variantId);
        if (variant is not null)
        {
            variant.ImportOptions = CloneOptions(options);
        }

        return clone;
    }
}
