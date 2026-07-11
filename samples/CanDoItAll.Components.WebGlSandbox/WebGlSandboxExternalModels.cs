using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlSandbox;

internal readonly record struct WebGlSandboxExternalModel(
    string Id,
    string DisplayName,
    string RelativeUri,
    string PrimitiveKind,
    string Color,
    long ByteSize,
    string Category);

internal static class WebGlSandboxExternalModels
{
    public static readonly IReadOnlyList<WebGlSandboxExternalModel> All =
    [
        Model("asset.external.building.blacksmith", "Blacksmith", "glb/buildings/Blacksmith.glb", WebGlPrimitiveKinds.House, "#f59e0b", 618_248, "building"),
        Model("asset.external.building.house-1", "House 1", "glb/buildings/House_1.glb", WebGlPrimitiveKinds.House, "#60a5fa", 391_772, "building"),
        Model("asset.external.building.house-2", "House 2", "glb/buildings/House_2.glb", WebGlPrimitiveKinds.House, "#38bdf8", 483_576, "building"),
        Model("asset.external.building.inn", "Inn", "glb/buildings/Inn.glb", WebGlPrimitiveKinds.House, "#a78bfa", 466_280, "building"),
        Model("asset.external.building.mill", "Mill", "glb/buildings/Mill.glb", WebGlPrimitiveKinds.House, "#f97316", 518_868, "building"),
        Model("asset.external.building.stable", "Stable", "glb/buildings/Stable.glb", WebGlPrimitiveKinds.House, "#34d399", 409_288, "building"),
        Model("asset.external.prop.bags", "Bags", "glb/props/Bags.glb", WebGlPrimitiveKinds.Box, "#a16207", 20_556, "prop"),
        Model("asset.external.prop.barrel", "Barrel", "glb/props/Barrel.glb", WebGlPrimitiveKinds.Cylinder, "#92400e", 51_332, "prop"),
        Model("asset.external.prop.bonfire", "Bonfire", "glb/props/Bonfire.glb", WebGlPrimitiveKinds.Cone, "#f97316", 29_308, "prop"),
        Model("asset.external.prop.bonfire-lit", "Bonfire lit", "glb/props/Bonfire_Lit.glb", WebGlPrimitiveKinds.Cone, "#f97316", 48_744, "prop"),
        Model("asset.external.prop.cart", "Cart", "glb/props/Cart.glb", WebGlPrimitiveKinds.Box, "#f59e0b", 106_888, "prop"),
        Model("asset.external.prop.cauldron", "Cauldron", "glb/props/Cauldron.glb", WebGlPrimitiveKinds.Cylinder, "#64748b", 45_424, "prop"),
        Model("asset.external.prop.crate", "Crate", "glb/props/Crate.glb", WebGlPrimitiveKinds.Box, "#a16207", 34_968, "prop"),
        Model("asset.external.prop.hay", "Hay", "glb/props/Hay.glb", WebGlPrimitiveKinds.Box, "#eab308", 26_700, "prop"),
        Model("asset.external.prop.market-stand", "Market stand", "glb/props/MarketStand_2.glb", WebGlPrimitiveKinds.Box, "#fb7185", 52_948, "prop"),
        Model("asset.external.prop.path-square", "Path square", "glb/props/Path_Square.glb", WebGlPrimitiveKinds.Box, "#94a3b8", 19_032, "prop"),
        Model("asset.external.prop.path-straight", "Path straight", "glb/props/Path_Straight.glb", WebGlPrimitiveKinds.Box, "#94a3b8", 37_464, "prop"),
        Model("asset.external.prop.smoke", "Smoke", "glb/props/Smoke.glb", WebGlPrimitiveKinds.Marker, "#cbd5e1", 23_228, "prop"),
        Model("asset.external.prop.well", "Well", "glb/props/Well.glb", WebGlPrimitiveKinds.Cylinder, "#38bdf8", 107_864, "prop"),
        Model("asset.external.person.female-looking-up", "Female looking up", "glb/people/Female_LookingUp.glb", WebGlPrimitiveKinds.Person, "#f8d8bd", 98_040, "person"),
        Model("asset.external.person.female-picking-up", "Female picking up", "glb/people/Female_PickingUp.glb", WebGlPrimitiveKinds.Person, "#f8d8bd", 102_016, "person"),
        Model("asset.external.person.female-running", "Female running", "glb/people/Female_Running.glb", WebGlPrimitiveKinds.Person, "#f8d8bd", 99_932, "person"),
        Model("asset.external.person.female-sitting", "Female sitting", "glb/people/Female_Sitting.glb", WebGlPrimitiveKinds.Person, "#f8d8bd", 101_692, "person"),
        Model("asset.external.person.female-sitting-cheering", "Female sitting cheering", "glb/people/Female_Sitting_Cheering.glb", WebGlPrimitiveKinds.Person, "#f8d8bd", 101_700, "person"),
        Model("asset.external.person.female-standing", "Female standing", "glb/people/Female_Standing.glb", WebGlPrimitiveKinds.Person, "#f8d8bd", 101_508, "person"),
        Model("asset.external.person.female-standing-covering-eyes", "Female standing covering eyes", "glb/people/Female_Standing_CoveringEyes.glb", WebGlPrimitiveKinds.Person, "#f8d8bd", 101_840, "person"),
        Model("asset.external.person.female-standing-hips", "Female standing hips", "glb/people/Female_Standing_Hips.glb", WebGlPrimitiveKinds.Person, "#f8d8bd", 98_192, "person"),
        Model("asset.external.person.female-walking", "Female walking", "glb/people/Female_Walking.glb", WebGlPrimitiveKinds.Person, "#f8d8bd", 101_888, "person"),
        Model("asset.external.person.male-looking-up", "Male looking up", "glb/people/Male_LookingUp.glb", WebGlPrimitiveKinds.Person, "#e2e8f0", 69_440, "person"),
        Model("asset.external.person.male-picking-up", "Male picking up", "glb/people/Male_PickingUp.glb", WebGlPrimitiveKinds.Person, "#e2e8f0", 69_508, "person"),
        Model("asset.external.person.male-running", "Male running", "glb/people/Male_Running.glb", WebGlPrimitiveKinds.Person, "#e2e8f0", 69_440, "person"),
        Model("asset.external.person.male-sitting", "Male sitting", "glb/people/Male_Sitting.glb", WebGlPrimitiveKinds.Person, "#e2e8f0", 69_444, "person"),
        Model("asset.external.person.male-sitting-cheering", "Male sitting cheering", "glb/people/Male_Sitting_Cheering.glb", WebGlPrimitiveKinds.Person, "#e2e8f0", 69_456, "person"),
        Model("asset.external.person.male-standing", "Male standing", "glb/people/Male_Standing.glb", WebGlPrimitiveKinds.Person, "#e2e8f0", 69_452, "person"),
        Model("asset.external.person.male-standing-covering-eyes", "Male standing covering eyes", "glb/people/Male_Standing_CoveringEyes.glb", WebGlPrimitiveKinds.Person, "#e2e8f0", 69_456, "person"),
        Model("asset.external.person.male-standing-hips", "Male standing hips", "glb/people/Male_Standing_Hips.glb", WebGlPrimitiveKinds.Person, "#e2e8f0", 69_420, "person"),
        Model("asset.external.person.male-standing-waving", "Male standing waving", "glb/people/Male_Standing_Waving.glb", WebGlPrimitiveKinds.Person, "#e2e8f0", 69_484, "person"),
        Model("asset.external.person.male-walking", "Male walking", "glb/people/Male_Walking.glb", WebGlPrimitiveKinds.Person, "#e2e8f0", 69_412, "person"),
        Model("asset.external.person.woman-standing-waving", "Woman standing waving", "glb/people/Woman_Standing_Waving.glb", WebGlPrimitiveKinds.Person, "#f8d8bd", 99_880, "person")
    ];

    private static WebGlSandboxExternalModel Model(
        string id,
        string displayName,
        string relativeUri,
        string primitiveKind,
        string color,
        long byteSize,
        string category)
        => new(id, displayName, relativeUri, primitiveKind, color, byteSize, category);
}
