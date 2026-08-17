using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.Sandbox;

public abstract class CatalogPageBase : ComponentBase, IDisposable
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [CascadingParameter]
    protected SandboxToolbarState? ToolbarState { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "frame")]
    public string? FrameQuery { get; set; }

    /// <summary>Changes whenever the client-side hash state changes, so a retained WASM route refreshes its frame.</summary>
    [Parameter]
    public string? HashRouteState { get; set; }

    protected SandboxFramePreset CurrentFrame => SandboxQueryLinks.UseHashRouting
        ? SandboxQueryLinks.GetHashFrame(NavigationManager)
        : SandboxFramePresetExtensions.Parse(FrameQuery);

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);

        ToolbarState?.Register(
            this,
            CurrentFrame,
            EventCallback.Factory.Create<SandboxFramePreset>(this, NavigateFrame));
    }

    protected void NavigateFrame(SandboxFramePreset frame)
    {
        var destination = SandboxQueryLinks.WithFrame(NavigationManager, frame);
        if (SandboxQueryLinks.UseHashRouting)
        {
            // NavigateTo updates history with pushState, which does not raise the browser's
            // hashchange event. Apply the state now so this retained page and the fixed
            // toolbar/sidebar rerender at once.
            SandboxQueryLinks.UpdateHashState(destination);
            ToolbarState?.Register(
                this,
                frame,
                EventCallback.Factory.Create<SandboxFramePreset>(this, NavigateFrame));
            _ = InvokeAsync(StateHasChanged);
        }

        NavigationManager.NavigateTo(destination);
    }

    public virtual void Dispose()
    {
        ToolbarState?.Unregister(this);
    }
}
