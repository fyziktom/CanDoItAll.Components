using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib;

internal sealed class DialogInterop(IJSRuntime js) : IAsyncDisposable
{
    internal const string ModulePath = "./_content/CanDoItAll.Components.BaseLib/Components/Modals/Dialog.razor.js";
    internal const string OpenMethod = "openDialog";
    internal const string CloseMethod = "closeDialog";

    private Task<IJSObjectReference>? moduleTask;
    private Task? moduleDisposal;
    private Task? closeTask;
    private bool disposed;

    public void Retire() => disposed = true;

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
        IJSObjectReference currentModule;
        try {
            currentModule = await GetModuleAsync();
        } catch (Exception exception) when (disposed && exception is JSDisconnectedException or OperationCanceledException) {
            return false;
        }
        if (disposed)
        {
            await ReleaseModuleAsync(currentModule);
            return false;
        }

        if (!isCurrentRequest())
        {
            return false;
        }

        try {
            await currentModule.InvokeVoidAsync(OpenMethod, dialog, instanceId, dotNetReference);
            return !disposed && isCurrentRequest();
        } catch (Exception exception) when (disposed && exception is JSDisconnectedException or OperationCanceledException) {
            return false;
        }
    }

    public ValueTask CloseAsync(string instanceId) {
        if (closeTask is not { IsCompleted: false }) {
            closeTask = CloseCoreAsync(instanceId);
        }
        return new(closeTask);
    }

    private async Task CloseCoreAsync(string instanceId) {
        if (moduleTask is { IsCompletedSuccessfully: true } loaded) {
            await loaded.Result.InvokeVoidAsync(CloseMethod, instanceId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        disposed = true;
        // A pending import is released by the open request that awaits it.
        if (moduleTask is { IsCompletedSuccessfully: true } loaded)
        {
            await ReleaseModuleAsync(loaded.Result);
        }
    }

    private Task ReleaseModuleAsync(IJSObjectReference module) => moduleDisposal ??= DisposeModuleAsync(module);

    private static async Task DisposeModuleAsync(IJSObjectReference module)
    {
        try
        {
            await module.DisposeAsync();
        }
        catch (Exception exception) when (exception is JSDisconnectedException or OperationCanceledException) {
        }
    }
}
