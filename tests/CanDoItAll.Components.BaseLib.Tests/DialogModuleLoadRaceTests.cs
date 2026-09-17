using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.BaseLib.Tests;

// The first open of a dialog waits for its JavaScript module. A host can remove, close or reopen the dialog while that
// import is pending (a deep link whose record turns out to be missing closes its loading dialog at once). A removed or
// closed dialog must never reach the browser: its element is gone and its .NET reference is disposed, and serializing
// that reference fails the whole Blazor Server circuit. The open requests resume on the renderer's dispatcher, so each
// test waits for an observable outcome that only the latest request can produce before it asserts the complete record.
public sealed class DialogModuleLoadRaceTests
{
    private const string ModulePath = "./_content/CanDoItAll.Components.BaseLib/Components/Modals/Dialog.razor.js";

    [Fact]
    public async Task A_dialog_removed_while_its_module_loads_is_never_opened_and_releases_the_module()
    {
        using var context = new BunitContext();
        var runtime = new PendingImportRuntime();
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var module = new RecordingModule();
        var host = context.Render<DialogHost>(parameters => parameters.Add(component => component.Rendered, true));
        Assert.Equal([ModulePath], runtime.Imports);

        host.Render(parameters => parameters.Add(component => component.Rendered, false));
        await host.InvokeAsync(() => runtime.CompleteImport(module));

        // The request either releases the module or opens the removed dialog; only the first is correct.
        host.WaitForAssertion(() => Assert.True(module.Disposed || module.Opens.Count > 0));
        Assert.Empty(module.Opens);
        Assert.True(module.Disposed);
    }

    [Fact]
    public async Task A_dialog_closed_and_reopened_while_its_module_loads_opens_once_for_the_current_element()
    {
        using var context = new BunitContext();
        var runtime = new PendingImportRuntime();
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var module = new RecordingModule();
        var host = context.Render<DialogHost>(parameters => parameters
            .Add(component => component.Rendered, true)
            .Add(component => component.IsOpen, true));
        var firstElement = RenderedDialogElement(host);

        host.Render(parameters => parameters.Add(component => component.IsOpen, false));
        host.Render(parameters => parameters.Add(component => component.IsOpen, true));
        var currentElement = RenderedDialogElement(host);
        Assert.NotEqual(firstElement, currentElement);
        await host.InvokeAsync(() => runtime.CompleteImport(module));

        // The stale request resumes first; once the current request has opened, the record is complete.
        host.WaitForAssertion(() => Assert.Contains(currentElement, module.Opens));
        Assert.Equal([currentElement], module.Opens);
        Assert.False(module.Disposed);

        // A later close reaches the loaded module, and the dialog stays usable for the next open.
        host.Render(parameters => parameters.Add(component => component.IsOpen, false));
        host.Render(parameters => parameters.Add(component => component.IsOpen, true));
        var reopenedElement = RenderedDialogElement(host);
        host.WaitForAssertion(() => Assert.Contains(reopenedElement, module.Opens));
        Assert.Equal([currentElement, reopenedElement], module.Opens);
        Assert.Equal(1, module.Closes);
    }

    [Fact]
    public async Task A_dialog_closed_while_its_module_loads_opens_nothing_until_it_is_opened_again()
    {
        using var context = new BunitContext();
        var runtime = new PendingImportRuntime();
        context.Services.AddSingleton<IJSRuntime>(runtime);
        var module = new RecordingModule();
        var host = context.Render<DialogHost>(parameters => parameters
            .Add(component => component.Rendered, true)
            .Add(component => component.IsOpen, true));

        host.Render(parameters => parameters.Add(component => component.IsOpen, false));
        await host.InvokeAsync(() => runtime.CompleteImport(module));
        host.Render(parameters => parameters.Add(component => component.IsOpen, true));
        var reopenedElement = RenderedDialogElement(host);

        // Only the reopened element is ever opened; the closed request never reaches the module.
        host.WaitForAssertion(() => Assert.Contains(reopenedElement, module.Opens));
        Assert.Equal([reopenedElement], module.Opens);
    }

    private static string RenderedDialogElement(IRenderedComponent<DialogHost> host)
        => host.Find("dialog").GetAttribute("blazor:elementReference")
           ?? throw new InvalidOperationException("The rendered dialog carries no element reference.");

    private sealed class DialogHost : ComponentBase
    {
        [Parameter]
        public bool Rendered { get; set; }

        [Parameter]
        public bool IsOpen { get; set; } = true;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (!Rendered)
            {
                return;
            }

            builder.OpenComponent<Dialog>(0);
            builder.AddAttribute(1, nameof(Dialog.IsOpen), IsOpen);
            builder.AddAttribute(2, nameof(Dialog.Title), "Module load race");
            builder.AddAttribute(3, nameof(Dialog.ChildContent), (RenderFragment)(content => content.AddContent(0, "Body")));
            builder.CloseComponent();
        }
    }

    // Keeps the module import pending until the test completes it; the dialog issues no other runtime call.
    private sealed class PendingImportRuntime : IJSRuntime
    {
        private readonly TaskCompletionSource<IJSObjectReference> import = new();

        public List<string> Imports { get; } = [];

        public void CompleteImport(IJSObjectReference module) => import.SetResult(module);

        public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            if (identifier != "import" || typeof(TValue) != typeof(IJSObjectReference))
            {
                throw new InvalidOperationException($"Unexpected runtime call '{identifier}'.");
            }

            Imports.Add((string)args![0]!);
            return (TValue)await import.Task;
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => InvokeAsync<TValue>(identifier, args);
    }

    // Records the element each open binds and the number of closes.
    private sealed class RecordingModule : IJSObjectReference
    {
        public List<string> Opens { get; } = [];

        public int Closes { get; private set; }

        public bool Disposed { get; private set; }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            switch (identifier)
            {
                case "openDialog":
                    Opens.Add(((ElementReference)args![0]!).Id);
                    break;
                case "closeDialog":
                    Closes++;
                    break;
                default:
                    throw new InvalidOperationException($"Unexpected module call '{identifier}'.");
            }

            return ValueTask.FromResult<TValue>(default!);
        }

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
            => InvokeAsync<TValue>(identifier, args);

        public ValueTask DisposeAsync()
        {
            Disposed = true;
            return ValueTask.CompletedTask;
        }
    }
}
