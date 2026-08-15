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

    protected SandboxFramePreset CurrentFrame => SandboxFramePresetExtensions.Parse(FrameQuery);

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
        NavigationManager.NavigateTo(SandboxQueryLinks.WithFrame(NavigationManager, frame));
    }

    public virtual void Dispose()
    {
        ToolbarState?.Unregister(this);
    }
}
