using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace CanDoItAll.Components.BaseLib;

internal sealed class TabsPanelContent : ComponentBase {
    [Parameter, EditorRequired]
    public TabsItem Item { get; set; } = default!;

    // Render after the parent has delivered the current parameters to its TabsItem children.
    protected override void BuildRenderTree(RenderTreeBuilder builder) => builder.AddContent(0, Item.ChildContent);
}
