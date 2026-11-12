using Bunit;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class NativeTextValueInputTests {
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Input_and_change_deliver_current_text_once_and_accept_an_explicit_clear(bool multiline) {
        using var context = new BunitContext();
        var model = new TextModel();
        var changes = new List<string>();
        var cut = context.Render<NativeValueInput<string>>(parameters => parameters
            .Add(component => component.Value, model.Text)
            .Add(component => component.ValueExpression, () => model.Text)
            .Add(component => component.Multiline, multiline)
            .Add(component => component.ValueChanged, value => {
                model.Text = value;
                changes.Add(value);
            }));
        var selector = multiline ? "textarea" : "input";
        var text = multiline ? "First line\nLater complete input" : "Later complete input";

        await cut.Find(selector).InputAsync(new ChangeEventArgs { Value = text });
        await cut.Find(selector).ChangeAsync(new ChangeEventArgs { Value = text });

        Assert.Equal(text, model.Text);
        Assert.Equal(text, cut.Find(selector).GetAttribute("value"));
        Assert.Equal([text], changes);
        cut.Render(parameters => parameters.Add(component => component.Value, string.Empty));
        Assert.Equal(string.Empty, cut.Find(selector).GetAttribute("value"));
    }

    [Theory]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    public async Task Disabled_or_readonly_text_rejects_native_input(bool multiline, bool disabled, bool readOnly) {
        using var context = new BunitContext();
        var model = new TextModel();
        var calls = 0;
        var cut = context.Render<NativeValueInput<string>>(parameters => parameters
            .Add(component => component.Value, model.Text)
            .Add(component => component.ValueExpression, () => model.Text)
            .Add(component => component.Multiline, multiline)
            .Add(component => component.Disabled, disabled)
            .Add(component => component.ReadOnly, readOnly)
            .Add(component => component.ValueChanged, _ => calls++));

        await cut.Find(multiline ? "textarea" : "input").InputAsync(new ChangeEventArgs { Value = "Refused" });

        Assert.Equal(0, calls);
        Assert.Equal(model.Text, cut.Find(multiline ? "textarea" : "input").GetAttribute("value"));
    }

    [Fact]
    public void Multiline_mode_rejects_a_numeric_value_type() {
        using var context = new BunitContext();
        var value = 3;
        Assert.Throws<InvalidOperationException>(() => context.Render<NativeValueInput<int>>(parameters => parameters
            .Add(component => component.Value, value)
            .Add(component => component.ValueExpression, () => value)
            .Add(component => component.Multiline, true)));
    }

    private sealed class TextModel {
        public string Text { get; set; } = "Original text";
    }
}
