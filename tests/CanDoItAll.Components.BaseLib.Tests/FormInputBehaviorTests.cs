using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class FormInputBehaviorTests
{
    [Theory]
    [MemberData(nameof(LabelledControls))]
    public void ControlsUseFormFieldLabelWhenNoExplicitAccessibleName(object component)
    {
        SetProperty(component, "FormFieldLabelId", "field-label");

        var labelledBy = InvokeString(component, "ResolveLabelledBy");

        Assert.Equal("field-label", labelledBy);
    }

    [Theory]
    [MemberData(nameof(ExplicitLabelledControls))]
    public void ControlsPreferExplicitAccessibleNameOverFormFieldLabel(object component)
    {
        SetProperty(component, "FormFieldLabelId", "field-label");
        SetProperty(component, "AdditionalAttributes", new Dictionary<string, object>
        {
            ["aria-label"] = "Explicit label"
        });

        var labelledBy = InvokeString(component, "ResolveLabelledBy");

        Assert.Null(labelledBy);
    }

    [Fact]
    public void TagEditorRootAttributesMergeTestHookWithCallerAttributes()
    {
        var editor = new TagEditor();
        SetProperty(editor, nameof(TagEditor.TestId), "tag-editor");
        SetProperty(editor, nameof(TagEditor.Class), "caller-class");

        var attributes = InvokeAttributes(editor, "ResolveRootAttributes");

        Assert.Equal("tag-editor", attributes["data-testid"]);
        Assert.Contains("cad-tag-textedit", attributes["class"].ToString(), StringComparison.Ordinal);
        Assert.Contains("caller-class", attributes["class"].ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task DisabledTextBoxDoesNotInvokeChangeCallbacks()
    {
        var callbackCount = 0;
        var textBox = new TextBox();
        SetProperty(textBox, nameof(TextBox.Disabled), true);
        SetProperty(textBox, nameof(TextBox.Value), "before");
        SetProperty(
            textBox,
            nameof(TextBox.ValueChanged),
            EventCallback.Factory.Create<string?>(this, _ => callbackCount++));

        await InvokeChangeAsync(textBox, "OnChangedAsync", "after");

        Assert.Equal("before", textBox.Value);
        Assert.Equal(0, callbackCount);
    }

    [Fact]
    public async Task DisabledSliderDoesNotInvokeChangeCallbacks()
    {
        var callbackCount = 0;
        var slider = new Slider<int>();
        SetProperty(slider, nameof(Slider<int>.Disabled), true);
        SetProperty(slider, nameof(Slider<int>.Value), 10);
        SetProperty(
            slider,
            nameof(Slider<int>.ValueChanged),
            EventCallback.Factory.Create<int>(this, _ => callbackCount++));

        await InvokeChangeAsync(slider, "OnChangedAsync", "90");

        Assert.Equal(10, slider.Value);
        Assert.Equal(0, callbackCount);
    }

    [Fact]
    public async Task DisabledEntityPickerDoesNotSelectItem()
    {
        var callbackCount = 0;
        var picker = new EntityPicker();
        SetProperty(picker, nameof(EntityPicker.Disabled), true);
        SetProperty(picker, nameof(EntityPicker.SelectedId), "before");
        SetProperty(
            picker,
            nameof(EntityPicker.SelectedIdChanged),
            EventCallback.Factory.Create<string>(this, _ => callbackCount++));

        var item = new EntityPickerItem
        {
            Id = "after",
            Label = "After"
        };

        await InvokePrivateAsync(picker, "SelectAsync", item);

        Assert.Equal("before", picker.SelectedId);
        Assert.Equal(0, callbackCount);
    }

    public static IEnumerable<object[]> LabelledControls()
    {
        yield return [new Switch()];
        yield return [new Slider<int>()];
        yield return [new Password()];
        yield return [new SecretField()];
        yield return [new TagEditor()];
    }

    public static IEnumerable<object[]> ExplicitLabelledControls()
    {
        yield return [new Slider<int>()];
        yield return [new Password()];
        yield return [new SecretField()];
    }

    private static void SetProperty(object target, string propertyName, object? value)
    {
        target.GetType()
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(target, value);
    }

    private static string? InvokeString(object target, string methodName)
    {
        return (string?)target.GetType()
            .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(target, null);
    }

    private static IReadOnlyDictionary<string, object> InvokeAttributes(object target, string methodName)
    {
        return (IReadOnlyDictionary<string, object>)target.GetType()
            .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(target, null)!;
    }

    private static Task InvokeChangeAsync(object target, string methodName, string value)
    {
        return InvokePrivateAsync(target, methodName, new ChangeEventArgs { Value = value });
    }

    private static Task InvokePrivateAsync(object target, string methodName, params object[] parameters)
    {
        return (Task)target.GetType()
            .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(target, parameters)!;
    }
}
