namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlModelDiagnostics
{
    public string AssetId { get; set; } = string.Empty;

    public string VariantId { get; set; } = string.Empty;

    public string Uri { get; set; } = string.Empty;

    public bool HasScene { get; set; }

    public int MeshCount { get; set; }

    public int VisibleMeshCount { get; set; }

    public int MaterialCount { get; set; }

    public int TransparentMaterialCount { get; set; }

    public WebGlVector3 Min { get; set; } = WebGlVector3.Zero;

    public WebGlVector3 Max { get; set; } = WebGlVector3.Zero;

    public WebGlVector3 Size { get; set; } = WebGlVector3.Zero;

    public WebGlVector3 Center { get; set; } = WebGlVector3.Zero;

    public List<string> Errors { get; set; } = [];

    public List<string> Warnings { get; set; } = [];

    public Dictionary<string, string> Metadata { get; set; } = [];
}
