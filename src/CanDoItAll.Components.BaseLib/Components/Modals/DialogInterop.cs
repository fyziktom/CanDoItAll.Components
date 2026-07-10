using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib;

internal sealed class DialogInterop(IJSRuntime js) : IAsyncDisposable
{
    internal const string ModulePath = "./_content/CanDoItAll.Components.BaseLib/Components/Modals/Dialog.razor.js";
    internal const string OpenMethod = "openDialog";
    internal const string CloseMethod = "closeDialog";

    private IJSObjectReference? module;

    private async ValueTask<IJSObjectReference> GetModuleAsync()
        => module ??= await js.InvokeAsync<IJSObjectReference>("import", ModulePath);

    public async ValueTask OpenAsync(
        ElementReference dialog,
        string instanceId,
        DotNetObjectReference<Dialog> dotNetReference)
    {
        var currentModule = await GetModuleAsync();
        await currentModule.InvokeVoidAsync(OpenMethod, dialog, instanceId, dotNetReference);
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
