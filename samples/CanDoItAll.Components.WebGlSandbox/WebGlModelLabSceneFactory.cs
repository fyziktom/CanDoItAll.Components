using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlSandbox;

public static class WebGlModelLabSceneFactory
{
    public static WebGlSceneModel Create(
        WebGlAssetCatalog catalog,
        string assetId,
        string variantId,
        string assetProfile)
    {
        assetProfile = WebGlAssetVariantResolver.NormalizeProfile(assetProfile);
        var sceneObject = new WebGlSceneObject
        {
            Id = "model-lab.subject",
            Kind = "model",
            Family = "diagnostic",
            Title = ResolveTitle(catalog, assetId),
            Subtitle = assetProfile,
            Description = "Single generic model diagnostic subject.",
            AssetId = assetId,
            Position = WebGlVector3.Zero,
            Size = new WebGlVector3(3, 3, 3),
            Color = "#60a5fa",
            IsSelectable = true,
            IsDraggable = true,
            Metadata =
            {
                ["proofRole"] = "model-lab-subject"
            }
        };
        if (!string.IsNullOrWhiteSpace(variantId))
        {
            sceneObject.Metadata["assetVariantId"] = variantId;
        }

        return new WebGlSceneModel
        {
            SceneId = "webgl-model-lab",
            Title = "WebGL Model Lab",
            Description = "Domain-neutral single-asset diagnostics scene.",
            AssetCatalog = catalog,
            Environment = new WebGlSceneEnvironment
            {
                BackgroundColor = "#101827",
                GroundColor = "#243247",
                GridColor = "#94a3b8",
                GroundSize = 12,
                GridDivisions = 12,
                AmbientLightIntensity = 0.75,
                DirectionalLightIntensity = 1.2,
                FogEnabled = false
            },
            Camera = new WebGlSceneCamera
            {
                ViewMode = WebGlSceneViewModes.Isometric,
                ProjectionMode = WebGlSceneProjectionModes.Perspective,
                Distance = 9,
                Azimuth = -0.72,
                Polar = 0.92,
                Target = new WebGlVector3(0, 1.25, 0)
            },
            UiState = new WebGlSceneUiState
            {
                ShowGrid = true,
                ShowGround = true,
                ShowLabels = false,
                ShowSymbols = false,
                DeterministicMode = true,
                ActiveAssetProfile = assetProfile
            },
            Interaction = new WebGlInteractionOptions
            {
                AllowHover = true,
                AllowClickSelection = true,
                AllowDragOnGroundPlane = true,
                AllowCameraOrbit = true,
                AllowCameraPan = true,
                AllowCameraZoom = true,
                FitViewOnCreate = true,
                FocusOnDoubleClick = true
            },
            Objects = [sceneObject],
            Metadata =
            {
                ["demo"] = "model-lab",
                ["domain"] = "generic",
                ["assetStrategy"] = assetProfile,
                ["variantId"] = variantId
            }
        };
    }

    private static string ResolveTitle(WebGlAssetCatalog catalog, string assetId)
        => catalog.Assets.FirstOrDefault(item => item.Id == assetId)?.DisplayName ?? assetId;
}
