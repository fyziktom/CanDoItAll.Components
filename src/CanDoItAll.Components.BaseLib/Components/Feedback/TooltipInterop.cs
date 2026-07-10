using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib;

internal sealed class TooltipInterop(IJSRuntime js) : IAsyncDisposable
{
    internal const string ModulePath = "./_content/CanDoItAll.Components.BaseLib/Components/Feedback/Tooltip.razor.js";
    internal const string AnchorMethod = "getAnchorPoint";
    internal const string ClearFocusedTargetMethod = "clearFocusedTarget";
    internal const string ClampMethod = "clampToViewport";

    private IJSObjectReference? module;

    private async ValueTask<IJSObjectReference> GetModuleAsync()
        => module ??= await js.InvokeAsync<IJSObjectReference>("import", ModulePath);

    public async ValueTask<TooltipAnchorPoint> GetAnchorPointAsync(ElementReference element, string tooltipId)
    {
        var currentModule = await GetModuleAsync();
        return await currentModule.InvokeAsync<TooltipAnchorPoint>(AnchorMethod, element, tooltipId);
    }

    public async ValueTask ClearFocusedTargetAsync(ElementReference element, string tooltipId)
    {
        var currentModule = await GetModuleAsync();
        await currentModule.InvokeVoidAsync(ClearFocusedTargetMethod, element, tooltipId);
    }

    public async ValueTask ClampToViewportAsync(ElementReference element)
    {
        var currentModule = await GetModuleAsync();
        await currentModule.InvokeVoidAsync(ClampMethod, element);
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

internal sealed record TooltipAnchorPoint(double X, double Y);
