using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace CanDoItAll.Components.Sandbox.Components;

/// <summary>Layout boundary for the standalone test surface. It deliberately adds no markup.</summary>
public sealed class RawTestLayout : LayoutComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
        => builder.AddContent(0, Body);
}
