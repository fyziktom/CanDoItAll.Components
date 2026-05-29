namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlRuntimeDiagnostics
{
    public int CreateCount { get; set; }

    public int UpdateCount { get; set; }

    public int RenderCount { get; set; }

    public int LoadedAssetCount { get; set; }

    public int MissingAssetCount { get; set; }

    public int FallbackObjectCount { get; set; }

    public int ObjectCount { get; set; }

    public int SymbolCount { get; set; }

    public bool DeterministicMode { get; set; }

    public string LastError { get; set; } = string.Empty;

    public List<string> MissingAssetIds { get; set; } = [];
}

