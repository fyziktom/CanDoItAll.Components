using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib;

internal sealed class DialogInterop(IJSRuntime js) : IAsyncDisposable
{
    internal const string ModulePath = "./_content/CanDoItAll.Components.BaseLib/Components/Modals/Dialog.razor.js";
    internal const string OpenMethod = "openDialog";
    internal const string CloseMethod = "closeDialog";

    private Task<IJSObjectReference>? moduleTask;
    private bool disposed;

    private Task<IJSObjectReference> GetModuleAsync()
        => moduleTask ??= js.InvokeAsync<IJSObjectReference>("import", ModulePath).AsTask();

    // Opens the browser dialog once the module is loaded, unless the dialog was closed, reopened or removed while the
    // import was pending: such a request would bind a removed element and a disposed .NET reference.
    public async ValueTask<bool> OpenAsync(
        ElementReference dialog,
        string instanceId,
        DotNetObjectReference<Dialog> dotNetReference,
        Func<bool> isCurrentRequest)
    {
        var currentModule = await GetModuleAsync();
        if (disposed)
        {
            await DisposeModuleAsync(currentModule);
            return false;
        }

        if (!isCurrentRequest())
        {
            return false;
        }

        await currentModule.InvokeVoidAsync(OpenMethod, dialog, instanceId, dotNetReference);
        return true;
    }

    public async ValueTask CloseAsync(string instanceId)
    {
        try
        {
            // A module that has not loaded yet has opened nothing to close.
            if (moduleTask is { IsCompletedSuccessfully: true } loaded)
            {
                await loaded.Result.InvokeVoidAsync(CloseMethod, instanceId);
            }
        }
        catch (JSDisconnectedException)
        {
        }
    }

    public async ValueTask DisposeAsync()
    {
        disposed = true;
        // A pending import is released by the open request that awaits it.
        if (moduleTask is { IsCompletedSuccessfully: true } loaded)
        {
            await DisposeModuleAsync(loaded.Result);
        }
    }

    private static async ValueTask DisposeModuleAsync(IJSObjectReference module)
    {
        try
        {
            await module.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }
    }
}
