using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class StyledInputBehaviorTests
{
    private enum ReviewStatus { Draft, Review, Blocked, Approved }

    private sealed class TextModel
    {
        [Required]
        public string? Title { get; set; }
    }

    private sealed class NumberModel
    {
        public int Confidence { get; set; }
    }

    private sealed class DateModel
    {
        public DateOnly? DueDate { get; set; }
    }

    private sealed class SelectModel
    {
        public ReviewStatus? Status { get; set; }
    }

    private static (BunitContext Context, IRenderedComponent<EditForm> Form) RenderForm(
        object model, RenderFragment<EditContext> body)
    {
        var context = new BunitContext();
        var form = context.Render<EditForm>(parameters => parameters
            .Add(f => f.Model, model)
            .Add(f => f.ChildContent, body));
        return (context, form);
    }

    // 1 + 2: Value binding round-trip + ValueExpression/FieldIdentifier wiring.
    [Fact]
    public void TextInputRoundTripsValueAndMarksFieldModified()
    {
        var model = new TextModel();
        var (context, form) = RenderForm(model, _ => builder =>
        {
            builder.OpenComponent<TextInput>(0);
            builder.AddAttribute(1, "Value", model.Title);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, v => model.Title = v));
            builder.AddAttribute(3, "ValueExpression", (Expression<Func<string?>>)(() => model.Title));
            builder.CloseComponent();
        });
        using var _ = context;

        form.Find("input").Change("Updated title");

        Assert.Equal("Updated title", model.Title);
        Assert.True(form.Instance.EditContext!.IsModified(form.Instance.EditContext.Field(nameof(TextModel.Title))));
    }

    [Fact]
    public void NumberInputRoundTripsValue()
    {
        var model = new NumberModel();
        var (context, form) = RenderForm(model, _ => builder =>
        {
            builder.OpenComponent<NumberInput<int>>(0);
            builder.AddAttribute(1, "Value", model.Confidence);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<int>(this, v => model.Confidence = v));
            builder.AddAttribute(3, "ValueExpression", (Expression<Func<int>>)(() => model.Confidence));
            builder.CloseComponent();
        });
        using var _ = context;

        form.Find("input").Change("42");

        Assert.Equal(42, model.Confidence);
    }

    [Fact]
    public void DateInputRoundTripsValue()
    {
        var model = new DateModel();
        var (context, form) = RenderForm(model, _ => builder =>
        {
            builder.OpenComponent<DateInput<DateOnly?>>(0);
            builder.AddAttribute(1, "Value", model.DueDate);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<DateOnly?>(this, v => model.DueDate = v));
            builder.AddAttribute(3, "ValueExpression", (Expression<Func<DateOnly?>>)(() => model.DueDate));
            builder.CloseComponent();
        });
        using var _ = context;

        form.Find("input").Change("2026-09-01");

        Assert.Equal(new DateOnly(2026, 9, 1), model.DueDate);
    }

    // 3: Validation CSS class application, including the class-double-emission guard.
    [Fact]
    public void ClassParameterIsNotDoubleEmitted()
    {
        var model = new TextModel();
        var (context, form) = RenderForm(model, _ => builder =>
        {
            builder.OpenComponent<TextInput>(0);
            builder.AddAttribute(1, "Value", model.Title);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, v => model.Title = v));
            builder.AddAttribute(3, "ValueExpression", (Expression<Func<string?>>)(() => model.Title));
            builder.AddAttribute(4, "Class", "probe");
            builder.CloseComponent();
        });
        using var _ = context;

        var classAttribute = form.Find("input").GetAttribute("class") ?? string.Empty;
        var occurrences = classAttribute.Split(' ', StringSplitOptions.RemoveEmptyEntries).Count(c => c == "probe");

        Assert.Equal(1, occurrences);
    }

    [Fact]
    public void FieldChangeAddsModifiedClass()
    {
        var model = new TextModel { Title = "Initial" };
        var (context, form) = RenderForm(model, _ => builder =>
        {
            builder.OpenComponent<TextInput>(0);
            builder.AddAttribute(1, "Value", model.Title);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, v => model.Title = v));
            builder.AddAttribute(3, "ValueExpression", (Expression<Func<string?>>)(() => model.Title));
            builder.CloseComponent();
        });
        using var _ = context;

        form.Find("input").Change("Changed");

        Assert.Contains("modified", form.Find("input").GetAttribute("class"));
    }

    [Fact]
    public void ValidationErrorAddsInvalidClass()
    {
        var model = new TextModel { Title = "Initial" };
        var (context, form) = RenderForm(model, editContext => builder =>
        {
            builder.OpenComponent<TextInput>(0);
            builder.AddAttribute(1, "Value", model.Title);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, v => model.Title = v));
            builder.AddAttribute(3, "ValueExpression", (Expression<Func<string?>>)(() => model.Title));
            builder.CloseComponent();
        });
        using var _ = context;

        var editContext = form.Instance.EditContext!;
        var messages = new ValidationMessageStore(editContext);
        messages.Add(editContext.Field(nameof(TextModel.Title)), "Title is required.");
        form.InvokeAsync(editContext.NotifyValidationStateChanged);

        Assert.Contains("invalid", form.Find("input").GetAttribute("class"));
    }

    // 4: FormField aria-labelledby integration.
    [Fact]
    public void CascadedFormFieldLabelIsUsedWhenNoExplicitAccessibleName()
    {
        var model = new TextModel();
        var context = new BunitContext();
        var form = context.Render<CascadingValue<string?>>(cascade => cascade
            .Add(c => c.Name, "FormFieldLabelId")
            .Add(c => c.Value, "label-id")
            .Add(c => c.ChildContent, builder =>
            {
                builder.OpenComponent<EditForm>(0);
                builder.AddAttribute(1, "Model", model);
                builder.AddAttribute(2, "ChildContent", (RenderFragment<EditContext>)(_ => innerBuilder =>
                {
                    innerBuilder.OpenComponent<TextInput>(0);
                    innerBuilder.AddAttribute(1, "Value", model.Title);
                    innerBuilder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, v => model.Title = v));
                    innerBuilder.AddAttribute(3, "ValueExpression", (Expression<Func<string?>>)(() => model.Title));
                    innerBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            }));
        using var _ = context;

        Assert.Equal("label-id", form.Find("input").GetAttribute("aria-labelledby"));
    }

    [Fact]
    public void ExplicitAriaLabelOverridesCascadedFormFieldLabel()
    {
        var model = new TextModel();
        var context = new BunitContext();
        var form = context.Render<CascadingValue<string?>>(cascade => cascade
            .Add(c => c.Name, "FormFieldLabelId")
            .Add(c => c.Value, "label-id")
            .Add(c => c.ChildContent, builder =>
            {
                builder.OpenComponent<EditForm>(0);
                builder.AddAttribute(1, "Model", model);
                builder.AddAttribute(2, "ChildContent", (RenderFragment<EditContext>)(_ => innerBuilder =>
                {
                    innerBuilder.OpenComponent<TextInput>(0);
                    innerBuilder.AddAttribute(1, "Value", model.Title);
                    innerBuilder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, v => model.Title = v));
                    innerBuilder.AddAttribute(3, "ValueExpression", (Expression<Func<string?>>)(() => model.Title));
                    innerBuilder.AddAttribute(4, "aria-label", "Title");
                    innerBuilder.CloseComponent();
                }));
                builder.CloseComponent();
            }));
        using var _ = context;

        var input = form.Find("input");
        Assert.Null(input.GetAttribute("aria-labelledby"));
        Assert.Equal("Title", input.GetAttribute("aria-label"));
    }

    // 5: SelectInput arbitrary ChildContent (foreach, disabled option, nullable/enum TValue).
    [Fact]
    public void SelectInputRendersForeachGeneratedAndDisabledOptions()
    {
        var model = new SelectModel();
        var (context, form) = RenderForm(model, _ => builder =>
        {
            builder.OpenComponent<SelectInput<ReviewStatus?>>(0);
            builder.AddAttribute(1, "Value", model.Status);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<ReviewStatus?>(this, v => model.Status = v));
            builder.AddAttribute(3, "ValueExpression", (Expression<Func<ReviewStatus?>>)(() => model.Status));
            builder.AddAttribute(4, "ChildContent", (RenderFragment)(optionsBuilder =>
            {
                optionsBuilder.OpenElement(0, "option");
                optionsBuilder.AddAttribute(1, "value", "");
                optionsBuilder.AddContent(2, "Unset");
                optionsBuilder.CloseElement();

                var sequence = 3;
                foreach (var status in Enum.GetValues<ReviewStatus>())
                {
                    optionsBuilder.OpenElement(sequence++, "option");
                    optionsBuilder.AddAttribute(sequence++, "value", status.ToString());
                    if (status == ReviewStatus.Blocked)
                    {
                        optionsBuilder.AddAttribute(sequence++, "disabled", true);
                    }
                    optionsBuilder.AddContent(sequence++, status.ToString());
                    optionsBuilder.CloseElement();
                }
            }));
            builder.CloseComponent();
        });
        using var _ = context;

        var options = form.FindAll("option");
        Assert.Equal(5, options.Count);
        Assert.True(options.Single(o => o.GetAttribute("value") == "Blocked").HasAttribute("disabled"));
    }

    [Fact]
    public void SelectInputRoundTripsNullableEnumValue()
    {
        var model = new SelectModel { Status = ReviewStatus.Review };
        var (context, form) = RenderForm(model, _ => builder =>
        {
            builder.OpenComponent<SelectInput<ReviewStatus?>>(0);
            builder.AddAttribute(1, "Value", model.Status);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<ReviewStatus?>(this, v => model.Status = v));
            builder.AddAttribute(3, "ValueExpression", (Expression<Func<ReviewStatus?>>)(() => model.Status));
            builder.AddAttribute(4, "ChildContent", (RenderFragment)(optionsBuilder =>
            {
                optionsBuilder.OpenElement(0, "option");
                optionsBuilder.AddAttribute(1, "value", "");
                optionsBuilder.AddContent(2, "Unset");
                optionsBuilder.CloseElement();

                var sequence = 3;
                foreach (var status in Enum.GetValues<ReviewStatus>())
                {
                    optionsBuilder.OpenElement(sequence++, "option");
                    optionsBuilder.AddAttribute(sequence++, "value", status.ToString());
                    optionsBuilder.AddContent(sequence++, status.ToString());
                    optionsBuilder.CloseElement();
                }
            }));
            builder.CloseComponent();
        });
        using var _ = context;

        form.Find("select").Change("Approved");
        Assert.Equal(ReviewStatus.Approved, model.Status);

        form.Find("select").Change("");
        Assert.Null(model.Status);
    }

    // 6: Disabled state — attribute renders through, no app-level suppression logic.
    [Fact]
    public void DisabledAttributePassesThroughToRenderedElement()
    {
        var model = new TextModel();
        var (context, form) = RenderForm(model, _ => builder =>
        {
            builder.OpenComponent<TextInput>(0);
            builder.AddAttribute(1, "Value", model.Title);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<string?>(this, v => model.Title = v));
            builder.AddAttribute(3, "ValueExpression", (Expression<Func<string?>>)(() => model.Title));
            builder.AddAttribute(4, "disabled", true);
            builder.CloseComponent();
        });
        using var _ = context;

        Assert.True(form.Find("input").HasAttribute("disabled"));
    }

    // 7: NumberInput step precedence.
    [Fact]
    public void NumberInputDefaultsStepToAny()
    {
        var model = new NumberModel();
        var (context, form) = RenderForm(model, _ => builder =>
        {
            builder.OpenComponent<NumberInput<int>>(0);
            builder.AddAttribute(1, "Value", model.Confidence);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<int>(this, v => model.Confidence = v));
            builder.AddAttribute(3, "ValueExpression", (Expression<Func<int>>)(() => model.Confidence));
            builder.CloseComponent();
        });
        using var _ = context;

        Assert.Equal("any", form.Find("input").GetAttribute("step"));
    }

    [Fact]
    public void NumberInputExplicitStepWinsAndIsNotDuplicated()
    {
        var model = new NumberModel();
        var (context, form) = RenderForm(model, _ => builder =>
        {
            builder.OpenComponent<NumberInput<int>>(0);
            builder.AddAttribute(1, "Value", model.Confidence);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<int>(this, v => model.Confidence = v));
            builder.AddAttribute(3, "ValueExpression", (Expression<Func<int>>)(() => model.Confidence));
            builder.AddAttribute(4, "step", "0.5");
            builder.CloseComponent();
        });
        using var _ = context;

        var input = form.Find("input");
        Assert.Equal("0.5", input.GetAttribute("step"));
    }

    // 8: DateInput Type -> HTML type attribute mapping.
    [Theory]
    [InlineData(InputDateType.Date, "date")]
    [InlineData(InputDateType.DateTimeLocal, "datetime-local")]
    [InlineData(InputDateType.Month, "month")]
    public void DateInputTypeMapsToHtmlInputType(InputDateType type, string expectedHtmlType)
    {
        var model = new DateModel();
        var (context, form) = RenderForm(model, _ => builder =>
        {
            builder.OpenComponent<DateInput<DateOnly?>>(0);
            builder.AddAttribute(1, "Value", model.DueDate);
            builder.AddAttribute(2, "ValueChanged", EventCallback.Factory.Create<DateOnly?>(this, v => model.DueDate = v));
            builder.AddAttribute(3, "ValueExpression", (Expression<Func<DateOnly?>>)(() => model.DueDate));
            builder.AddAttribute(4, "Type", type);
            builder.CloseComponent();
        });
        using var _ = context;

        Assert.Equal(expectedHtmlType, form.Find("input").GetAttribute("type"));
    }
}
