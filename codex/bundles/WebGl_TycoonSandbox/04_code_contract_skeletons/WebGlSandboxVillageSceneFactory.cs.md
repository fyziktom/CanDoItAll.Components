# WebGlSandboxVillageSceneFactory skeleton

```csharp
namespace CanDoItAll.Components.WebGlSandbox;

using CanDoItAll.Components.WebGlLib;

public static class WebGlSandboxVillageSceneFactory
{
    public static WebGlSceneModel Create()
    {
        var catalog = WebGlSandboxAssetCatalogFactory.Create();

        return new WebGlSceneModel
        {
            SceneId = "generic-tycoon-village",
            Title = "Generic Tycoon Village",
            Description = "Domain-neutral village proof scene for WebGlLib symbolic 3D rendering.",
            AssetCatalog = catalog,
            Camera = new WebGlSceneCamera
            {
                ViewMode = "isometric",
                ProjectionMode = "perspective",
                Distance = 42,
                Azimuth = -0.78,
                Polar = 1.02
            },
            Objects =
            [
                Building("building.town-hall", "Town Hall", "asset.building.service.default", -6, 0, 0, "#60a5fa"),
                Building("building.house-a", "House A", "asset.building.house.default", -2, 0, 3, "#fbbf24"),
                Building("building.house-b", "House B", "asset.building.house.default", 3, 0, 2, "#34d399"),
                Prop("prop.tree-a", "Tree", "asset.prop.tree.default", -5, 0, 5),
                Agent("agent.worker-a", "Worker A", -1, 0, -2, "available", "#22c55e", 0.35),
                Agent("agent.worker-b", "Worker B", 2, 0, -2.5, "busy", "#f97316", 0.75),
                Agent("agent.worker-c", "Worker C", 5, 0, -1, "warning", "#ef4444", 0.95)
            ],
            Links =
            [
                new WebGlSceneLink
                {
                    Id = "path.town-hall.house-a",
                    SourceObjectId = "building.town-hall",
                    TargetObjectId = "building.house-a",
                    Kind = "path",
                    Color = "#94a3b8",
                    Width = 2
                }
            ]
        };
    }

    private static WebGlSceneObject Building(string id, string title, string assetId, double x, double y, double z, string color)
    {
        return new WebGlSceneObject
        {
            Id = id,
            Kind = "building",
            Family = "village",
            Title = title,
            AssetId = assetId,
            Position = new WebGlVector3(x, y, z),
            Size = new WebGlVector3(2, 2, 2),
            Color = color,
            Tags = ["sandbox", "building"]
        };
    }

    private static WebGlSceneObject Prop(string id, string title, string assetId, double x, double y, double z)
    {
        return new WebGlSceneObject
        {
            Id = id,
            Kind = "prop",
            Family = "village",
            Title = title,
            AssetId = assetId,
            Position = new WebGlVector3(x, y, z),
            Size = new WebGlVector3(1, 1, 1),
            Color = "#22c55e",
            Tags = ["sandbox", "prop"]
        };
    }

    private static WebGlSceneObject Agent(string id, string title, double x, double y, double z, string state, string color, double intensity)
    {
        return new WebGlSceneObject
        {
            Id = id,
            Kind = "agent",
            Family = "village",
            Title = title,
            Subtitle = state,
            AssetId = "asset.agent.person.default",
            Position = new WebGlVector3(x, y, z),
            Size = new WebGlVector3(0.8, 1.8, 0.8),
            Color = "#e2e8f0",
            Tags = ["sandbox", "agent", state],
            Symbols =
            [
                new WebGlStatusSymbol
                {
                    Id = $"{id}.symbol.{state}",
                    SemanticKind = state,
                    SymbolAssetId = "asset.symbol.marker.default",
                    Intensity = intensity,
                    Color = color,
                    Scale = 1.0,
                    HeightOffset = 1.2,
                    EffectKey = intensity > 0.8 ? WebGlSymbolEffects.Pulse : WebGlSymbolEffects.Float,
                    Tooltip = $"{title}: {state}"
                }
            ]
        };
    }
}
```
