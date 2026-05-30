namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlVisualStateCatalog
{
    public List<WebGlPoseDefinition> Poses { get; set; } = [];

    public List<WebGlSymbolDefinition> Symbols { get; set; } = [];

    public List<WebGlActionBinding> ActionBindings { get; set; } = [];
}
