using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.Sandbox;

/// <summary>
/// Bridges the currently routed catalog page's frame state up to the fixed
/// top toolbar, which lives in MainLayout outside of @Body.
/// </summary>
public sealed class SandboxToolbarState
{
    private object? owner;

    public bool HasContext => owner is not null;

    public bool ShowCoverage { get; private set; }

    public bool ShowUnused { get; private set; }

    public SandboxFramePreset Frame { get; private set; }

    public EventCallback<SandboxFramePreset> FrameChanged { get; private set; }

    public event Action? Changed;

    public void Register(
        object owner,
        SandboxFramePreset frame,
        EventCallback<SandboxFramePreset> frameChanged)
    {
        // Dirty check is mandatory: Register runs on every SetParametersAsync of every
        // catalog page (i.e. on every render). Without this guard, raising Changed
        // unconditionally would trigger MainLayout.StateHasChanged -> cascading value
        // rebuild -> the routed page's SetParametersAsync runs again -> Register ->
        // Changed -> ... an infinite render loop.
        var unchanged = ReferenceEquals(this.owner, owner)
            && Frame == frame;

        this.owner = owner;
        Frame = frame;
        FrameChanged = frameChanged;

        if (!unchanged)
        {
            Changed?.Invoke();
        }
    }

    public void ToggleCoverage()
    {
        ShowCoverage = !ShowCoverage;
        Changed?.Invoke();
    }

    public void ToggleShowUnused()
    {
        ShowUnused = !ShowUnused;
        Changed?.Invoke();
    }

    public void Unregister(object owner)
    {
        // Only clear if this owner is still the active registration, so
        // Unregister/Register ordering across a navigation can't leave stale state:
        // a stale Unregister from an already-superseded owner is a safe no-op.
        if (!ReferenceEquals(this.owner, owner))
        {
            return;
        }

        this.owner = null;
        Changed?.Invoke();
    }
}
