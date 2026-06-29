using CanDoItAll.Components.Mermaid;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class MermaidSourceNormalizerTests
{
    [Fact]
    public void NormalizeDedentsCommonRawStringIndentation()
    {
        var source = """
            flowchart LR
                start[Start]
                start --> done[Done]
            """;

        var normalized = MermaidSourceNormalizer.Normalize(source);

        Assert.Equal(
            """
            flowchart LR
            start[Start]
            start --> done[Done]
            """,
            normalized);
    }

    [Fact]
    public void NormalizePreservesRelativeIndentationAfterDedent()
    {
        var source = """
            mindmap
                root((Mermaid wrapper))
                    Render
                    Interact
            """;

        var normalized = MermaidSourceNormalizer.Normalize(source);

        Assert.Equal(
            """
            mindmap
            root((Mermaid wrapper))
                Render
                Interact
            """,
            normalized);
    }
}
