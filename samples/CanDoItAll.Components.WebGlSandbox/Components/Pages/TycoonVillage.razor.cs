using CanDoItAll.Components.BaseLib;
using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlSandbox.Components.Pages;

public partial class TycoonVillage
{
    private WebGlSceneModel scene = WebGlSandboxVillageSceneFactory.Create();
    private readonly WebGlRuntimeOptions runtimeOptions = new()
    {
        DeterministicMode = true,
        ShowDiagnosticsPanel = true,
        ShowLabels = false,
        ShowSymbols = true,
        AutoFitOnCreate = true,
        RenderMode = WebGlRenderModes.Auto,
        AssetQualityProfile = WebGlAssetQualityProfiles.Primitive
    };

    private WebGlSceneView? sceneView;
    private WebGlSceneModel? exportedScene;
    private WebGlSceneObject? selectedObject;
    private WebGlSceneProofSnapshot? latestSnapshot;
    private WebGlSceneCommandResult? latestPatchResult;
    private string selectedAssetProfile = WebGlAssetQualityProfiles.Primitive;
    private string hoveredObjectId = string.Empty;
    private string runtimeStatusText = "initializing";
    private string runtimeStatusTone = "info";
    private string runtimeError = string.Empty;
    private string lastMoveStatus = "Drag any building, marker, prop, or agent.";
    private string exportStatus = "No exported scene yet.";

    private string SymbolsButtonText => scene.UiState.ShowSymbols ? "Symbols on" : "Symbols off";

    private string SymbolsButtonIcon => scene.UiState.ShowSymbols ? "visibility" : "visibility_off";

    private string LayerButtonText(string layerId, string label)
        => IsLayerVisible(layerId) ? $"{label} on" : $"{label} off";

    private ButtonTone LayerButtonStyle(string layerId)
        => IsLayerVisible(layerId) ? ButtonTone.Default : ButtonTone.Warning;

    private ButtonTone ProfileStyle(string profile)
        => string.Equals(selectedAssetProfile, profile, StringComparison.Ordinal)
            ? ButtonTone.Primary
            : ButtonTone.Default;

    private Task SetAssetProfileAsync(string profile)
    {
        selectedAssetProfile = WebGlAssetVariantResolver.NormalizeProfile(profile);
        runtimeOptions.AssetQualityProfile = selectedAssetProfile;
        scene = WebGlSandboxVillageSceneFactory.Create(selectedAssetProfile);
        selectedObject = null;
        latestSnapshot = null;
        runtimeStatusText = "loading";
        runtimeStatusTone = "info";
        exportStatus = "Profile changed; export proof is pending.";
        return Task.CompletedTask;
    }

    private async Task FitViewAsync()
    {
        if (sceneView is not null)
        {
            await sceneView.FitViewAsync();
        }
    }

    private async Task FocusSelectedAsync()
    {
        if (sceneView is not null && selectedObject is not null)
        {
            await sceneView.FocusObjectAsync(selectedObject.Id);
        }
    }

    private async Task SelectFromListAsync(string objectId)
    {
        selectedObject = scene.Objects.FirstOrDefault(item => item.Id == objectId);
        scene.UiState.Selection.SelectedObjectIds = selectedObject is null ? [] : [selectedObject.Id];
        scene.UiState.Selection.PrimaryObjectId = selectedObject?.Id ?? string.Empty;
        scene.UiState.Revision += 1;
        if (sceneView is not null && selectedObject is not null)
        {
            await sceneView.FocusObjectAsync(selectedObject.Id);
        }
    }

    private void ToggleSymbols()
    {
        scene.UiState.ShowSymbols = !scene.UiState.ShowSymbols;
        runtimeOptions.ShowSymbols = scene.UiState.ShowSymbols;
        scene.UiState.Revision += 1;
    }

    private async Task ToggleLayerAsync(string layerId)
    {
        var layer = scene.Layers.FirstOrDefault(item => item.Id == layerId);
        if (layer is null)
        {
            return;
        }

        layer.IsVisible = !layer.IsVisible;
        scene.UiState.Revision += 1;
        latestSnapshot = sceneView is null ? null : await sceneView.GetProofSnapshotAsync();
    }

    private bool IsLayerVisible(string layerId)
        => scene.Layers.FirstOrDefault(item => item.Id == layerId)?.IsVisible != false;

    private Task ToggleBuildingsLayerAsync() => ToggleLayerAsync("layer.buildings");

    private Task ToggleAgentsLayerAsync() => ToggleLayerAsync("layer.agents");

    private Task TogglePropsLayerAsync() => ToggleLayerAsync("layer.props");

    private async Task MoveRunnerToPlazaAsync()
    {
        if (sceneView is null)
        {
            return;
        }

        await sceneView.EnqueueMotionAsync(new WebGlObjectMotionCommand
        {
            MotionId = "proof.runner.to-plaza",
            ObjectId = "agent.runner",
            TargetPosition = new WebGlVector3(0.4, 0, -0.4),
            DurationSeconds = 1.35,
            Easing = WebGlMotionEasings.EaseInOut
        });
        lastMoveStatus = "Motion enqueued for agent.runner.";
    }

    private async Task ExportSceneStateAsync()
    {
        exportedScene = sceneView is null ? null : await sceneView.ExportSceneAsync();
        exportStatus = exportedScene is null ? "Export failed." : $"Exported revision {exportedScene.UiState.Revision}.";
    }

    private async Task ImportSceneStateAsync()
    {
        if (sceneView is null || exportedScene is null)
        {
            exportStatus = "No exported scene to import.";
            return;
        }

        scene = exportedScene;
        selectedAssetProfile = scene.UiState.ActiveAssetProfile;
        runtimeOptions.AssetQualityProfile = selectedAssetProfile;
        var imported = await sceneView.ImportSceneAsync(scene);
        exportStatus = imported ? "Imported exported scene state." : "Import failed.";
    }

    private async Task TriggerMissingAssetFallbackAsync()
    {
        if (sceneView is null)
        {
            return;
        }

        await sceneView.ApplyPatchAsync(new WebGlScenePatch
        {
            SceneId = scene.SceneId,
            NextRevision = scene.UiState.Revision + 1,
            ObjectPatches =
            [
                new WebGlSceneObjectPatch
                {
                    ObjectId = "agent.helper",
                    AssetId = "asset.missing.intentional",
                    Metadata = new Dictionary<string, string> { ["proofRole"] = "intentional-missing-asset" }
                }
            ]
        });
        latestSnapshot = await sceneView.GetProofSnapshotAsync();
    }

    private Task ApplyBadLinkStrictPatchAsync()
        => ApplyBadLinkPatchAsync(permissiveInvalidLinks: false);

    private Task ApplyBadLinkWarningPatchAsync()
        => ApplyBadLinkPatchAsync(permissiveInvalidLinks: true);

    private async Task ApplyBadLinkPatchAsync(bool permissiveInvalidLinks)
    {
        if (sceneView is null)
        {
            return;
        }

        latestPatchResult = await sceneView.ApplyPatchDetailedAsync(CreateBadLinkPatch(permissiveInvalidLinks));
        latestSnapshot = await sceneView.GetProofSnapshotAsync();
        string mode = latestPatchResult?.Metadata.GetValueOrDefault("patchTransactionMode", "unknown") ?? "unknown";
        lastMoveStatus = latestPatchResult?.Success == true
            ? $"{mode} patch applied for agent.helper."
            : $"{mode} patch failed for agent.helper.";
    }

    private WebGlScenePatch CreateBadLinkPatch(bool permissiveInvalidLinks)
    {
        string mode = permissiveInvalidLinks
            ? WebGlScenePatchTransactionModes.PermissiveInvalidLinks
            : WebGlScenePatchTransactionModes.Strict;
        var patch = new WebGlScenePatch
        {
            SceneId = scene.SceneId,
            Metadata =
            {
                ["commandId"] = permissiveInvalidLinks ? "proof.bad-link.warn" : "proof.bad-link.strict",
                ["patchTransactionMode"] = mode
            },
            ObjectPatches =
            [
                new()
                {
                    ObjectId = "agent.helper",
                    Position = permissiveInvalidLinks
                        ? new WebGlVector3(-7.1, 0, 1.2)
                        : new WebGlVector3(-2.2, 0, 2.2)
                }
            ],
            AddLinks =
            [
                new()
                {
                    Id = permissiveInvalidLinks ? "proof.bad-link.warn" : "proof.bad-link.strict",
                    SourceObjectId = "agent.helper",
                    TargetObjectId = "object.missing"
                }
            ]
        };
        if (permissiveInvalidLinks)
        {
            patch.AddLinks.Insert(0, new WebGlSceneLink
            {
                Id = "proof.good-link.warn",
                SourceObjectId = "agent.helper",
                TargetObjectId = "marker.plaza"
            });
        }

        return patch;
    }

    private async Task CaptureProofSnapshotAsync()
    {
        if (sceneView is not null)
        {
            latestSnapshot = await sceneView.GetProofSnapshotAsync();
        }
    }

    private Task HandleSelectionChanged(WebGlSceneSelectionChangedEventArgs args)
    {
        selectedObject = scene.Objects.FirstOrDefault(item => item.Id == args.PrimaryObjectId);
        scene.UiState.Selection.SelectedObjectIds = [.. args.SelectedObjectIds];
        scene.UiState.Selection.PrimaryObjectId = args.PrimaryObjectId ?? string.Empty;
        return InvokeAsync(StateHasChanged);
    }

    private Task HandleHoverChanged(WebGlSceneHoverChangedEventArgs args)
    {
        hoveredObjectId = args.ObjectId ?? string.Empty;
        return InvokeAsync(StateHasChanged);
    }

    private Task HandleObjectsMoved(WebGlObjectMovedEventArgs args)
    {
        foreach (var position in args.Positions)
        {
            var sceneObject = scene.Objects.FirstOrDefault(item => item.Id == position.ObjectId);
            if (sceneObject is not null)
            {
                sceneObject.Position = position.Position;
            }
        }

        lastMoveStatus = args.Positions.Count == 0
            ? "No object movement committed."
            : $"Moved {string.Join(", ", args.Positions.Select(item => item.ObjectId))}.";
        return InvokeAsync(StateHasChanged);
    }

    private Task HandleRuntimeReady(WebGlRuntimeReadyEventArgs args)
    {
        runtimeStatusText = "ready";
        runtimeStatusTone = "success";
        runtimeError = string.Empty;
        return InvokeAsync(StateHasChanged);
    }

    private Task HandleRuntimeError(WebGlRuntimeErrorEventArgs args)
    {
        runtimeStatusText = "runtime error";
        runtimeStatusTone = "danger";
        runtimeError = args.Detail;
        return InvokeAsync(StateHasChanged);
    }
}
