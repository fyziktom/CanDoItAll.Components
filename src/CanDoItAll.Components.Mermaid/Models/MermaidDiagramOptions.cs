namespace CanDoItAll.Components.Mermaid;

public sealed class MermaidDiagramOptions
{
    public string Theme { get; set; } = "default";

    public string SecurityLevel { get; set; } = "strict";

    public bool FlowchartUseMaxWidth { get; set; } = true;

    public bool HtmlLabels { get; set; }

    public bool ArchitectureRandomize { get; set; }

    public Dictionary<string, object?> AdditionalConfig { get; set; } = [];
}
