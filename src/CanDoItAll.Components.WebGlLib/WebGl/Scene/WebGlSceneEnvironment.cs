namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlSceneEnvironment
{
    public string BackgroundColor { get; set; } = "#0f172a";

    public string GroundColor { get; set; } = "#1f2937";

    public string GridColor { get; set; } = "#64748b";

    public double GroundSize { get; set; } = 36;

    public double GridDivisions { get; set; } = 24;

    public double AmbientLightIntensity { get; set; } = 0.6;

    public double DirectionalLightIntensity { get; set; } = 1.2;

    public bool FogEnabled { get; set; } = true;

    public string FogColor { get; set; } = "#0f172a";

    public double FogNear { get; set; } = 80;

    public double FogFar { get; set; } = 180;
}

