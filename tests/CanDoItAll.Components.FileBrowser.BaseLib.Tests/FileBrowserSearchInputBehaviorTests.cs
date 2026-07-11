using System.Reflection;
using CanDoItAll.Components.FileBrowser.Core;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.FileBrowser.BaseLib.Tests;

public sealed class FileBrowserSearchInputBehaviorTests
{
    [Fact]
    public async Task SearchInputCallbackReturnsBeforeTheDebounceCompletes()
    {
        var browser = new FileBrowser();
        SetParameter(
            browser,
            nameof(FileBrowser.Session),
            DispatchProxy.Create<IFileBrowserSession, UnusedSessionProxy>());
        SetParameter(browser, nameof(FileBrowser.SearchDebounceMilliseconds), 60_000);
        MethodInfo callback = typeof(FileBrowser).GetMethod(
            "HandleSearchTextChangedAsync",
            BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Search callback was not found.");

        var completion = Assert.IsAssignableFrom<Task>(callback.Invoke(browser, ["handbook"]));

        Assert.True(completion.IsCompletedSuccessfully);
        await browser.DisposeAsync();
    }

    [Fact]
    public async Task ToolbarRetainsTheLatestDraftBeforeForwardedCallbackCompletes()
    {
        var toolbar = new FileBrowserToolbar();
        var callbackRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        SetParameter(toolbar, nameof(FileBrowserToolbar.SearchText), "hand");
        SetParameter(
            toolbar,
            nameof(FileBrowserToolbar.SearchTextChanged),
            EventCallback.Factory.Create<string?>(
                new object(),
                new Func<string?, Task>(_ => callbackRelease.Task)));
        MethodInfo callback = typeof(FileBrowserToolbar).GetMethod(
            "HandleSearchTextInputAsync",
            BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("Toolbar search callback was not found.");

        var completion = Assert.IsAssignableFrom<Task>(callback.Invoke(toolbar, ["handbook"]));

        Assert.Equal("handbook", toolbar.SearchText);
        Assert.False(completion.IsCompleted);
        callbackRelease.SetResult();
        await completion;
    }

    private static void SetParameter(object component, string propertyName, object value)
        => component.GetType().GetProperty(propertyName)?.SetValue(component, value);

    public class UnusedSessionProxy : DispatchProxy
    {
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
            => throw new InvalidOperationException(
                $"The debounce regression test unexpectedly invoked '{targetMethod?.Name}'.");
    }
}
