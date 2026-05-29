using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlSandbox;

public static class WebGlSandboxVillageSceneFactory
{
    public static WebGlSceneModel Create(string assetProfile = WebGlAssetQualityProfiles.Primitive)
    {
        assetProfile = WebGlAssetVariantResolver.NormalizeProfile(assetProfile);
        var catalog = WebGlSandboxAssetCatalogFactory.Create();
        var highDetail = string.Equals(assetProfile, WebGlAssetQualityProfiles.HighDetailGlb, StringComparison.Ordinal);

        var objects = new List<WebGlSceneObject>
        {
            Building("building.town-hall", "Town Hall", "asset.building.service.default", -6, 0, -2, "#60a5fa", "coordination", highDetail ? "variant.service.inn.high" : string.Empty),
            Building("building.workshop", "Workshop", "asset.building.service.default", 2.5, 0, -5, "#f59e0b", "craft", highDetail ? "variant.service.blacksmith.high" : string.Empty),
            Building("building.house-a", "House A", "asset.building.house.default", -2.5, 0, 3.2, "#38bdf8", "home", string.Empty),
            Building("building.house-b", "House B", "asset.building.house.default", 3.4, 0, 2.6, "#34d399", "home", highDetail ? "variant.house.house-2.high" : string.Empty),
            Building("building.storehouse", "Storehouse", "asset.building.house.default", 6.8, 0, -1.4, "#a78bfa", "storage", highDetail ? "variant.service.stable.high" : string.Empty),
            Building("building.kiosk", "Kiosk", "asset.building.house.default", -7.8, 0, 3.8, "#fb7185", "service", string.Empty),
            Prop("prop.tree-a", "Oak marker", -8.5, 0, -5.2, "#16a34a"),
            Prop("prop.tree-b", "Pine marker", -4.2, 0, 6.7, "#22c55e"),
            Prop("prop.tree-c", "Garden tree", 5.8, 0, 5.4, "#65a30d"),
            Marker("marker.plaza", "Plaza marker", 0, 0, 0, "#facc15"),
            Marker("marker.north-gate", "North gate", 0, 0, -8.5, "#38bdf8")
        };

        objects.AddRange(
        [
            Agent("agent.pathfinder", "Pathfinder", -4.4, 0, -1.2, "available", "#22c55e", 0.28, WebGlSymbolEffects.Float, "asset.symbol.ready.default", highDetail ? "variant.person.male-standing.high" : string.Empty),
            Agent("agent.builder", "Builder", -1.2, 0, -3.4, "busy", "#f97316", 0.72, WebGlSymbolEffects.Pulse, "asset.symbol.warning.default", highDetail ? "variant.person.female-standing.high" : string.Empty),
            Agent("agent.runner", "Runner", 1.2, 0, -1.6, "ready", "#22c55e", 0.46, WebGlSymbolEffects.ScaleByIntensity, "asset.symbol.ready.default", highDetail ? "variant.person.male-running.high" : string.Empty),
            Agent("agent.scout", "Scout", 4.6, 0, -3.1, "info", "#38bdf8", 0.38, WebGlSymbolEffects.Spin, "asset.symbol.info.default", highDetail ? "variant.person.female-running.high" : string.Empty),
            Agent("agent.guide", "Guide", -5.8, 0, 2.2, "needs-input", "#facc15", 0.66, WebGlSymbolEffects.Blink, "asset.symbol.marker.default", string.Empty),
            Agent("agent.caretaker", "Caretaker", -0.2, 0, 4.4, "available", "#22c55e", 0.34, WebGlSymbolEffects.Float, "asset.symbol.ready.default", string.Empty),
            Agent("agent.lookout", "Lookout", 4.5, 0, 5.2, "warning", "#ef4444", 0.92, WebGlSymbolEffects.Glow, "asset.symbol.warning.default", string.Empty),
            Agent("agent.coordinator", "Coordinator", 7.6, 0, 1.4, "busy", "#f97316", 0.78, WebGlSymbolEffects.Pulse, "asset.symbol.marker.default", string.Empty),
            Agent("agent.helper", "Helper", -8.2, 0, 0.6, "ready", "#22c55e", 0.52, WebGlSymbolEffects.ScaleByIntensity, "asset.symbol.info.default", string.Empty)
        ]);

        return new WebGlSceneModel
        {
            SceneId = "generic-tycoon-village",
            Title = "Generic Tycoon Village",
            Description = "Domain-neutral village proof scene for symbolic 3D rendering.",
            AssetCatalog = catalog,
            Environment = new WebGlSceneEnvironment
            {
                BackgroundColor = "#0f172a",
                GroundColor = "#263241",
                GridColor = "#94a3b8",
                GroundSize = 34,
                GridDivisions = 34,
                AmbientLightIntensity = 0.72,
                DirectionalLightIntensity = 1.18,
                FogEnabled = true,
                FogNear = 58,
                FogFar = 138
            },
            Camera = new WebGlSceneCamera
            {
                ViewMode = WebGlSceneViewModes.Isometric,
                ProjectionMode = WebGlSceneProjectionModes.Perspective,
                Distance = 27,
                Azimuth = -0.78,
                Polar = 1.0,
                Target = new WebGlVector3(0, 0.8, 0)
            },
            UiState = new WebGlSceneUiState
            {
                ShowGrid = true,
                ShowGround = true,
                ShowLabels = false,
                ShowSymbols = true,
                DeterministicMode = true,
                ActiveAssetProfile = assetProfile
            },
            Interaction = new WebGlInteractionOptions
            {
                AllowHover = true,
                AllowClickSelection = true,
                AllowMultiSelect = true,
                AllowDragOnGroundPlane = true,
                AllowCameraOrbit = true,
                AllowCameraPan = true,
                AllowCameraZoom = true,
                FitViewOnCreate = true,
                FocusOnDoubleClick = true
            },
            Layers =
            [
                new WebGlSceneLayer { Id = "layer.buildings", Title = "Buildings", Kind = "objects", ObjectIds = [.. objects.Where(o => o.Kind == "building").Select(o => o.Id)] },
                new WebGlSceneLayer { Id = "layer.agents", Title = "Agents", Kind = "objects", ObjectIds = [.. objects.Where(o => o.Kind == "agent").Select(o => o.Id)] },
                new WebGlSceneLayer { Id = "layer.props", Title = "Props", Kind = "objects", ObjectIds = [.. objects.Where(o => o.Kind is "prop" or "marker").Select(o => o.Id)] }
            ],
            Objects = objects,
            Links =
            [
                Path("path.plaza.town-hall", "marker.plaza", "building.town-hall"),
                Path("path.plaza.workshop", "marker.plaza", "building.workshop"),
                Path("path.plaza.house-a", "marker.plaza", "building.house-a"),
                Path("path.plaza.house-b", "marker.plaza", "building.house-b"),
                Path("path.gate.plaza", "marker.north-gate", "marker.plaza")
            ],
            Metadata =
            {
                ["demo"] = "tycoon-village",
                ["domain"] = "generic",
                ["assetStrategy"] = assetProfile,
                ["largeScreenOnly"] = "true"
            }
        };
    }

    private static WebGlSceneObject Building(string id, string title, string assetId, double x, double y, double z, string color, string tag, string variantId)
    {
        var sceneObject = new WebGlSceneObject
        {
            Id = id,
            Kind = "building",
            Family = "village",
            Title = title,
            Subtitle = tag,
            Description = "Generic village building used for scene proof.",
            AssetId = assetId,
            Position = new WebGlVector3(x, y, z),
            Size = new WebGlVector3(2.25, 2.15, 2.2),
            Color = color,
            IsDraggable = true,
            Tags = ["sandbox", "building", tag],
            Metadata =
            {
                ["category"] = tag,
                ["proofRole"] = "building"
            }
        };

        AddVariant(sceneObject, variantId);
        return sceneObject;
    }

    private static WebGlSceneObject Prop(string id, string title, double x, double y, double z, string color)
        => new()
        {
            Id = id,
            Kind = "prop",
            Family = "village",
            Title = title,
            Subtitle = "prop",
            AssetId = "asset.prop.tree.default",
            Position = new WebGlVector3(x, y, z),
            Size = new WebGlVector3(1.25, 2.35, 1.25),
            Color = color,
            Tags = ["sandbox", "prop", "tree"],
            IsSelectable = true,
            IsDraggable = true
        };

    private static WebGlSceneObject Marker(string id, string title, double x, double y, double z, string color)
        => new()
        {
            Id = id,
            Kind = "marker",
            Family = "village",
            Title = title,
            Subtitle = "navigation",
            AssetId = "asset.symbol.marker.default",
            Position = new WebGlVector3(x, y, z),
            Size = new WebGlVector3(0.78, 0.78, 0.78),
            Color = color,
            Tags = ["sandbox", "marker"],
            IsDraggable = true
        };

    private static WebGlSceneObject Agent(
        string id,
        string title,
        double x,
        double y,
        double z,
        string state,
        string color,
        double intensity,
        string effect,
        string symbolAssetId,
        string variantId)
    {
        var sceneObject = new WebGlSceneObject
        {
            Id = id,
            Kind = "agent",
            Family = "village",
            Title = title,
            Subtitle = state,
            Description = "Generic village agent with a status symbol.",
            AssetId = "asset.agent.person.default",
            Position = new WebGlVector3(x, y, z),
            Size = new WebGlVector3(0.82, 1.78, 0.82),
            Color = "#e2e8f0",
            Tags = ["sandbox", "agent", state],
            IsDraggable = true,
            Symbols =
            [
                new WebGlStatusSymbol
                {
                    Id = $"{id}.symbol.{state}",
                    SemanticKind = state,
                    SymbolAssetId = symbolAssetId,
                    Intensity = intensity,
                    Color = color,
                    Scale = 1,
                    HeightOffset = 0.42,
                    EffectKey = effect,
                    Tooltip = $"{title}: {state}",
                    SortOrder = 1
                }
            ],
            Metadata =
            {
                ["status"] = state,
                ["proofRole"] = "agent"
            }
        };

        AddVariant(sceneObject, variantId);
        return sceneObject;
    }

    private static void AddVariant(WebGlSceneObject sceneObject, string variantId)
    {
        if (!string.IsNullOrWhiteSpace(variantId))
        {
            sceneObject.Metadata["assetVariantId"] = variantId;
        }
    }

    private static WebGlSceneLink Path(string id, string sourceObjectId, string targetObjectId)
        => new()
        {
            Id = id,
            SourceObjectId = sourceObjectId,
            TargetObjectId = targetObjectId,
            Kind = "path",
            Label = "path",
            Color = "#cbd5e1",
            Width = 2,
            Opacity = 0.68
        };
}
