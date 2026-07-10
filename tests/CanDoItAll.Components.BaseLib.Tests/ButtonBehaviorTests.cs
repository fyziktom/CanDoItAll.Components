using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class ButtonBehaviorTests
{
    [Fact]
    public async Task ButtonPreventsConcurrentClickCallbacks()
    {
        var button = new Button();
        var firstClickGate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var clickCount = 0;

        SetClickCallback(
            button,
            EventCallback.Factory.Create(
                new object(),
                async () =>
                {
                    Interlocked.Increment(ref clickCount);
                    await firstClickGate.Task;
                }));

        var firstClick = InvokeClickAsync(button);

        await WaitUntilAsync(() => Volatile.Read(ref clickCount) == 1);

        var secondClick = InvokeClickAsync(button);
        await Task.Delay(25);

        Assert.Equal(1, Volatile.Read(ref clickCount));

        firstClickGate.SetResult();
        await Task.WhenAll(firstClick, secondClick);
    }

    [Fact]
    public async Task DisabledAnchorDoesNotInvokeClickCallback()
    {
        var clickCount = 0;
        var button = new Button();
        SetParameter(button, nameof(Button.Disabled), true);
        SetParameter(button, nameof(Button.Href), "/should-not-open");
        SetClickCallback(button, EventCallback.Factory.Create(this, () => clickCount++));

        await InvokeClickAsync(button);

        Assert.Equal(0, clickCount);
    }

    [Fact]
    public void BlankTargetDefaultsToNoopenerAndNoreferrer()
    {
        var button = new Button();
        SetParameter(button, nameof(Button.Href), "https://example.test");
        SetParameter(button, nameof(Button.Target), "_blank");

        var resolvedRel = (string?)typeof(Button)
            .GetProperty("ResolvedRel", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(button);

        Assert.Equal("noopener noreferrer", resolvedRel);
    }

    private static void SetClickCallback(Button button, EventCallback callback)
    {
        typeof(Button)
            .GetProperty(nameof(Button.Click), BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(button, callback);
    }

    private static void SetParameter(Button button, string propertyName, object? value)
    {
        typeof(Button)
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(button, value);
    }

    private static Task InvokeClickAsync(Button button)
    {
        return (Task)typeof(Button)
            .GetMethod("OnClickAsync", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(button, null)!;
    }

    private static async Task WaitUntilAsync(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        while (!condition())
        {
            await Task.Delay(10, timeout.Token);
        }
    }
}
