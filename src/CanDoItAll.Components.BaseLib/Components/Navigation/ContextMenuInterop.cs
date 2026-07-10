using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib;

internal sealed class ContextMenuInterop(IJSRuntime js) : IAsyncDisposable
{
    internal const string ModulePath = "./_content/CanDoItAll.Components.BaseLib/Components/Navigation/ContextMenu.razor.js";
    internal const string OpenMethod = "openMenu";
    internal const string PositionMethod = "positionMenu";
    internal const string CloseMethod = "closeMenu";

    private IJSObjectReference? module;

    private async ValueTask<IJSObjectReference> GetModuleAsync()
        => module ??= await js.InvokeAsync<IJSObjectReference>("import", ModulePath);

    public async ValueTask OpenAsync(ElementReference menu, string instanceId, double x, double y)
    {
        var currentModule = await GetModuleAsync();
        await currentModule.InvokeVoidAsync(OpenMethod, menu, instanceId, x, y);
    }

    public async ValueTask PositionAsync(ElementReference menu, double x, double y)
    {
        var currentModule = await GetModuleAsync();
        await currentModule.InvokeVoidAsync(PositionMethod, menu, x, y);
    }

    public async ValueTask CloseAsync(string instanceId)
    {
        try
        {
            if (module is not null)
            {
                await module.InvokeVoidAsync(CloseMethod, instanceId);
            }
        }
        catch (JSDisconnectedException)
        {
        }
    }

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
