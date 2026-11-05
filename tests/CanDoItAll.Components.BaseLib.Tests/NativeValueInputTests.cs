using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class NativeValueInputTests {
    [Fact]
    public async Task LocalDateTimeRendersTheTimeAndKeepsAnIncompleteNativeEdit() {
        using var services = new ServiceCollection().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, NullLoggerFactory.Instance);
        DateTime? value = new(2026, 11, 5, 10, 30, 45);
        var html = await renderer.Dispatcher.InvokeAsync(async () => {
            var rendered = await renderer.RenderComponentAsync<NativeValueInput<DateTime?>>(ParameterView.FromDictionary(new Dictionary<string, object?> {
                [nameof(NativeValueInput<DateTime?>.Value)] = value,
                [nameof(NativeValueInput<DateTime?>.DateType)] = InputDateType.DateTimeLocal,
                [nameof(NativeValueInput<DateTime?>.ValueExpression)] = (System.Linq.Expressions.Expression<Func<DateTime?>>)(() => value)
            }));
            return rendered.ToHtmlString();
        });
        Assert.Contains("type=\"datetime-local\"", html, StringComparison.Ordinal);
        Assert.Contains("2026-11-05T10:30:45", html, StringComparison.Ordinal);
        var component = new NativeValueInput<DateTime?>();
        Set(component, nameof(component.Value), value);
        Set(component, nameof(component.DateType), InputDateType.DateTimeLocal);
        await Invoke(component, "HandleInputAsync", new NativeInputEventArgs { BadInput = true });
        await Invoke(component, "HandleChangeAsync", new ChangeEventArgs { Value = "" });
        Assert.Equal(value, component.Value);
        await Invoke(component, "HandleInputAsync", new NativeInputEventArgs { Value = "2026-11-06T11:45:30" });
        Assert.Equal(new DateTime(2026, 11, 6, 11, 45, 30), component.Value);
    }

    [Fact]
    public async Task LocalDateTimeModeRejectsAnIncompatibleValueType() {
        using var services = new ServiceCollection().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, NullLoggerFactory.Instance);
        DateOnly? value = new(2026, 11, 5);
        await Assert.ThrowsAsync<InvalidOperationException>(() => renderer.Dispatcher.InvokeAsync(async () => {
            await renderer.RenderComponentAsync<NativeValueInput<DateOnly?>>(ParameterView.FromDictionary(new Dictionary<string, object?> {
                [nameof(NativeValueInput<DateOnly?>.Value)] = value,
                [nameof(NativeValueInput<DateOnly?>.DateType)] = InputDateType.DateTimeLocal,
                [nameof(NativeValueInput<DateOnly?>.ValueExpression)] = (System.Linq.Expressions.Expression<Func<DateOnly?>>)(() => value)
            }));
        }));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task NativeDisabledAttributeMatchesTheBooleanParameter(bool disabled) {
        using var services = new ServiceCollection().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, NullLoggerFactory.Instance);
        var value = 4;
        var html = await renderer.Dispatcher.InvokeAsync(async () => {
            var rendered = await renderer.RenderComponentAsync<NativeValueInput<int>>(ParameterView.FromDictionary(new Dictionary<string, object?> {
                [nameof(NativeValueInput<int>.Value)] = value,
                [nameof(NativeValueInput<int>.Disabled)] = disabled,
                [nameof(NativeValueInput<int>.ValueExpression)] = (System.Linq.Expressions.Expression<Func<int>>)(() => value)
            }));
            return rendered.ToHtmlString();
        });

        Assert.Equal(disabled, html.Contains("disabled", StringComparison.Ordinal));
        Assert.Contains("type=\"number\"", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task IncompleteNullableNumberAndItsChangeEventKeepThePriorValueUntilValidInputArrives() {
        var component = new NativeValueInput<decimal?>();
        var pending = new List<bool>();
        var changed = new List<decimal?>();
        Set(component, nameof(component.Value), 143.50m);
        Set(component, nameof(component.PendingChanged), EventCallback.Factory.Create<bool>(this, pending.Add));
        Set(component, nameof(component.ValueChanged), EventCallback.Factory.Create<decimal?>(this, changed.Add));

        await Invoke(component, "HandleInputAsync", new NativeInputEventArgs { Value = "", BadInput = true });
        await Invoke(component, "HandleChangeAsync", new ChangeEventArgs { Value = "" });

        Assert.Equal(143.50m, component.Value);
        Assert.Empty(changed);
        Assert.Equal(new[] { true }, pending);

        await Invoke(component, "HandleInputAsync", new NativeInputEventArgs { Value = "" });

        Assert.Null(component.Value);
        Assert.Equal(new decimal?[] { null }, changed);
        Assert.Equal(new[] { true, false }, pending);
    }

    [Fact]
    public async Task IncompleteNullableDateDoesNotBecomeAnEmptyDate() {
        var component = new NativeValueInput<DateOnly?>();
        var date = new DateOnly(2026, 11, 2);
        Set(component, nameof(component.Value), date);

        await Invoke(component, "HandleInputAsync", new NativeInputEventArgs { BadInput = true });
        await Invoke(component, "HandleChangeAsync", new ChangeEventArgs { Value = "" });

        Assert.Equal(date, component.Value);
        await Invoke(component, "HandleInputAsync", new NativeInputEventArgs { Value = "2026-11-04" });
        Assert.Equal(new DateOnly(2026, 11, 4), component.Value);
    }

    [Fact]
    public async Task UnchangedValidInputStillReportsTheInputIntent() {
        var component = new NativeValueInput<int>();
        var pending = new List<bool>();
        var writes = 0;
        Set(component, nameof(component.Value), 4);
        Set(component, nameof(component.PendingChanged), EventCallback.Factory.Create<bool>(this, pending.Add));
        Set(component, nameof(component.ValueChanged), EventCallback.Factory.Create<int>(this, _ => writes++));

        await Invoke(component, "HandleInputAsync", new NativeInputEventArgs { Value = "4" });

        Assert.Equal(new[] { false }, pending);
        Assert.Equal(0, writes);
    }

    [Fact]
    public async Task MalformedInputDoesNotChangeTheModelAndAValidChangeCanCorrectIt() {
        var component = new NativeValueInput<decimal>();
        var pending = new List<bool>();
        Set(component, nameof(component.Value), 25m);
        Set(component, nameof(component.PendingChanged), EventCallback.Factory.Create<bool>(this, pending.Add));

        await Invoke(component, "HandleChangeAsync", new ChangeEventArgs { Value = "not-a-number" });
        Assert.Equal(25m, component.Value);
        await Invoke(component, "HandleChangeAsync", new ChangeEventArgs { Value = "75" });
        Assert.Equal(75m, component.Value);
        Assert.Equal(new[] { true, false }, pending);
    }

    [Fact]
    public async Task ADisabledInputCannotChangeItsModelOrRaiseCallbacks() {
        var component = new NativeValueInput<int>();
        var callbacks = 0;
        Set(component, nameof(component.Value), 4);
        Set(component, nameof(component.Disabled), true);
        Set(component, nameof(component.PendingChanged), EventCallback.Factory.Create<bool>(this, _ => callbacks++));
        Set(component, nameof(component.ValueChanged), EventCallback.Factory.Create<int>(this, _ => callbacks++));

        await Invoke(component, "HandleInputAsync", new NativeInputEventArgs { Value = "7" });

        Assert.Equal(4, component.Value);
        Assert.Equal(0, callbacks);
    }

    private static void Set(object component, string property, object? value) => component.GetType().GetProperty(property)!.SetValue(component, value);
    private static Task Invoke(object component, string method, object args) => (Task)component.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component, [args])!;
}
