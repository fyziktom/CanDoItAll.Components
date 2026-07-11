using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib;

internal sealed class TooltipInterop : IAsyncDisposable
{
    internal const string ModulePath = "./_content/CanDoItAll.Components.BaseLib/Components/Feedback/Tooltip.razor.js";
    internal const string AnchorMethod = "getAnchorPoint";
    internal const string ClearFocusedTargetMethod = "clearFocusedTarget";
    internal const string ClampMethod = "clampToViewport";

    private readonly IJSRuntime js;
    private readonly SemaphoreSlim operationGate = new(1, 1);
    private IJSObjectReference? module;
    private int disposalStarted;

    public TooltipInterop(IJSRuntime js)
    {
        this.js = js ?? throw new ArgumentNullException(nameof(js));
    }

    public ValueTask<TooltipAnchorPoint?> GetAnchorPointAsync(ElementReference element, string tooltipId)
        => InvokeAsync<TooltipAnchorPoint>(AnchorMethod, element, tooltipId);

    public ValueTask ClearFocusedTargetAsync(ElementReference element, string tooltipId)
        => InvokeVoidAsync(ClearFocusedTargetMethod, element, tooltipId);

    public ValueTask ClampToViewportAsync(ElementReference element)
        => InvokeVoidAsync(ClampMethod, element);

    private async ValueTask<TResult?> InvokeAsync<TResult>(string method, params object?[] arguments)
    {
        if (IsDisposalStarted)
        {
            return default;
        }

        await operationGate.WaitAsync();
        try
        {
            var currentModule = await GetModuleWhileLockedAsync();
            if (currentModule is null || IsDisposalStarted)
            {
                return default;
            }

            return await currentModule.InvokeAsync<TResult>(method, arguments);
        }
        catch (Exception exception) when (IsLifecycleException(exception))
        {
            return default;
        }
        finally
        {
            operationGate.Release();
        }
    }

    private async ValueTask InvokeVoidAsync(string method, params object?[] arguments)
    {
        if (IsDisposalStarted)
        {
            return;
        }

        await operationGate.WaitAsync();
        try
        {
            var currentModule = await GetModuleWhileLockedAsync();
            if (currentModule is not null && !IsDisposalStarted)
            {
                await currentModule.InvokeVoidAsync(method, arguments);
            }
        }
        catch (Exception exception) when (IsLifecycleException(exception))
        {
        }
        finally
        {
            operationGate.Release();
        }
    }

    private async ValueTask<IJSObjectReference?> GetModuleWhileLockedAsync()
    {
        if (IsDisposalStarted)
        {
            return null;
        }

        if (module is not null)
        {
            return module;
        }

        var imported = await js.InvokeAsync<IJSObjectReference>("import", ModulePath);
        if (IsDisposalStarted)
        {
            await DisposeModuleAsync(imported);
            return null;
        }

        module = imported;
        return module;
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref disposalStarted, 1) != 0)
        {
            return;
        }

        IJSObjectReference? currentModule;
        await operationGate.WaitAsync();
        try
        {
            currentModule = module;
            module = null;
        }
        finally
        {
            operationGate.Release();
        }

        if (currentModule is not null)
        {
            await DisposeModuleAsync(currentModule);
        }
    }

    private bool IsDisposalStarted => Volatile.Read(ref disposalStarted) != 0;

    private static bool IsLifecycleException(Exception exception)
        => exception is JSDisconnectedException or ObjectDisposedException;

    private static async ValueTask DisposeModuleAsync(IJSObjectReference module)
    {
        try
        {
            await module.DisposeAsync();
        }
        catch (Exception exception) when (IsLifecycleException(exception))
        {
        }
    }
}

internal sealed record TooltipAnchorPoint(double X, double Y);
