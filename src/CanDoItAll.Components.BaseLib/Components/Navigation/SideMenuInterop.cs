using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib;

internal sealed class SideMenuInterop(IJSRuntime js) : IAsyncDisposable
{
    internal const string ModulePath = "./_content/CanDoItAll.Components.BaseLib/Components/Navigation/SideMenu.razor.js";
    internal const string InitializeMethod = "initialize";
    internal const string RefreshMethod = "refresh";
    internal const string SaveExpandedMethod = "saveExpanded";
    internal const string PositionPanelMethod = "positionPanel";
    internal const string DisposeMethod = "dispose";

    private IJSObjectReference? module;

    public async ValueTask<SideMenuInteropInitialization> InitializeAsync(
        string instanceId,
        ElementReference root,
        ElementReference itemViewport,
        ElementReference measureItem,
        DotNetObjectReference<SideMenu> dotNetReference,
        string storageKey,
        bool readStoredExpanded)
    {
        var currentModule = await GetModuleAsync();
        return await currentModule.InvokeAsync<SideMenuInteropInitialization>(
            InitializeMethod,
            instanceId,
            root,
            itemViewport,
            measureItem,
            dotNetReference,
            storageKey,
            readStoredExpanded);
    }

    public async ValueTask RefreshAsync(string instanceId)
    {
        if (module is not null)
        {
            await module.InvokeVoidAsync(RefreshMethod, instanceId);
        }
    }

    public async ValueTask SaveExpandedAsync(string storageKey, bool isExpanded)
    {
        var currentModule = await GetModuleAsync();
        await currentModule.InvokeVoidAsync(SaveExpandedMethod, storageKey, isExpanded);
    }

    public async ValueTask PositionPanelAsync(ElementReference panel)
    {
        var currentModule = await GetModuleAsync();
        await currentModule.InvokeVoidAsync(PositionPanelMethod, panel);
    }

    public async ValueTask DisposeInstanceAsync(string instanceId)
    {
        try
        {
            if (module is not null)
            {
                await module.InvokeVoidAsync(DisposeMethod, instanceId);
            }
        }
        catch (JSDisconnectedException)
        {
        }
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync()
        => module ??= await js.InvokeAsync<IJSObjectReference>("import", ModulePath);

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
        }
    }
}
