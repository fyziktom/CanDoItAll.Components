using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.Sandbox;

public abstract class CatalogPageBase : ComponentBase
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery(Name = "scenario")]
    public string? ScenarioQuery { get; set; }

    [Parameter, SupplyParameterFromQuery(Name = "frame")]
    public string? FrameQuery { get; set; }

    protected SandboxScenarioKey CurrentScenario => SandboxScenarioKeyExtensions.Parse(ScenarioQuery);

    protected SandboxFramePreset CurrentFrame => SandboxFramePresetExtensions.Parse(FrameQuery);

    protected void NavigateScenario(SandboxScenarioKey scenario)
    {
        NavigationManager.NavigateTo(SandboxQueryLinks.WithScenario(NavigationManager, scenario));
    }

    protected void NavigateFrame(SandboxFramePreset frame)
    {
        NavigationManager.NavigateTo(SandboxQueryLinks.WithFrame(NavigationManager, frame));
    }
}
