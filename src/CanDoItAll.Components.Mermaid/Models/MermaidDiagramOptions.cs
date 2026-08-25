namespace CanDoItAll.Components.Mermaid;

public sealed class MermaidDiagramOptions
{
    /// <summary>
    /// Mermaid's own theme name ("default", "dark", "forest", "neutral", "base", ...), or the
    /// sentinel "auto" to follow the nearest data-cad-theme ancestor ("dark" when it resolves to
    /// dark, "default" otherwise) and re-render live when it flips. An explicit non-"auto" value
    /// always overrides and never watches for theme changes.
    /// </summary>
    public string Theme { get; set; } = "auto";

    public string SecurityLevel { get; set; } = "strict";

    public bool FlowchartUseMaxWidth { get; set; } = true;

    public bool HtmlLabels { get; set; }

    public bool ArchitectureRandomize { get; set; }

    public Dictionary<string, object?> AdditionalConfig { get; set; } = [];
}
