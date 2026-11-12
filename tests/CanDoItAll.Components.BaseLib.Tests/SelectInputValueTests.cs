using System.Linq.Expressions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class SelectInputValueTests {
    [Theory]
    [InlineData(true, "false", false)]
    [InlineData(false, "true", true)]
    public void BooleanOptionsRoundTripAndNotifyTheirValidationField(bool initial, string selection, bool expected) {
        using var context = new BunitContext();
        var model = new Selection<bool> { Value = initial };
        var form = RenderSelect(context, model);

        Assert.Equal(initial ? "true" : "false", form.Find("select").GetAttribute("value"));

        form.Find("select").Change(selection);

        Assert.Equal(expected, model.Value);
        Assert.Equal(selection, form.Find("select").GetAttribute("value"));
        Assert.True(form.Instance.EditContext!.IsModified(form.Instance.EditContext.Field(nameof(model.Value))));
    }

    [Fact]
    public void NullableBooleanOptionsRoundTripThroughUnset() {
        using var context = new BunitContext();
        var model = new Selection<bool?>();
        var form = RenderSelect(context, model);

        Assert.True(string.IsNullOrEmpty(form.Find("select").GetAttribute("value")));
        form.Find("select").Change("true");
        Assert.True(model.Value);
        form.Find("select").Change("false");
        Assert.False(model.Value);
        form.Find("select").Change(string.Empty);
        Assert.Null(model.Value);
        Assert.Empty(form.Instance.EditContext!.GetValidationMessages());
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-boolean")]
    public void InvalidBooleanOptionsPreserveValueUntilValidSelection(string selection) {
        using var context = new BunitContext();
        var model = new Selection<bool> { Value = true };
        var form = RenderSelect(context, model);

        form.Find("select").Change(selection);

        Assert.True(model.Value);
        Assert.Single(form.Instance.EditContext!.GetValidationMessages());
        Assert.Contains("invalid", form.Find("select").ClassList);

        form.Find("select").Change("false");

        Assert.False(model.Value);
        Assert.Empty(form.Instance.EditContext.GetValidationMessages());
    }

    [Fact]
    public void EmptyStringOptionPreservesTheEmptyStringContract() {
        using var context = new BunitContext();
        var model = new Selection<string> { Value = "voice-id" };
        var form = RenderSelect(context, model);

        form.Find("select").Change(string.Empty);

        Assert.Equal(string.Empty, model.Value);
        Assert.Empty(form.Instance.EditContext!.GetValidationMessages());
    }

    private static IRenderedComponent<EditForm> RenderSelect<TValue>(BunitContext context, Selection<TValue> model)
        => context.Render<EditForm>(parameters => parameters
            .Add(form => form.Model, model)
            .Add(form => form.ChildContent, _ => builder => {
                builder.OpenComponent<SelectInput<TValue>>(0);
                builder.AddAttribute(1, nameof(SelectInput<TValue>.Value), model.Value);
                builder.AddAttribute(2, nameof(SelectInput<TValue>.ValueChanged), EventCallback.Factory.Create<TValue>(model, value => model.Value = value));
                builder.AddAttribute(3, nameof(SelectInput<TValue>.ValueExpression), (Expression<Func<TValue>>)(() => model.Value));
                builder.AddAttribute(4, nameof(SelectInput<TValue>.ChildContent), (RenderFragment)(options =>
                    options.AddMarkupContent(0, "<option value=\"\">Unset</option><option value=\"true\">Yes</option><option value=\"false\">No</option>")));
                builder.CloseComponent();
            }));

    private sealed class Selection<TValue> {
        public TValue Value { get; set; } = default!;
    }
}
