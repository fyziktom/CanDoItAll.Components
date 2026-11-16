using System.Reflection;
using Bunit;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class FormInputBehaviorTests
{
    [Fact]
    public async Task A_pending_textarea_callback_keeps_current_input_through_an_unchanged_parent_render() {
        using var context = new BunitContext();
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var cut = context.Render<TextAreaInput>(parameters => parameters
            .Add(component => component.Value, "Original")
            .Add(component => component.Immediate, true)
            .Add(component => component.ValueChanged, _ => release.Task));
        var pending = cut.InvokeAsync(() => cut.Find("textarea").InputAsync(new() { Value = "Later unblurred input" }));
        cut.WaitForAssertion(() => Assert.Equal("Later unblurred input", cut.Find("textarea").GetAttribute("value")));

        cut.Render(parameters => parameters.Add(component => component.Value, "Original"));

        Assert.Equal("Later unblurred input", cut.Find("textarea").GetAttribute("value"));
        release.TrySetResult();
        await pending;
        cut.Render(parameters => parameters.Add(component => component.Value, "Later unblurred input"));
        cut.Render(parameters => parameters.Add(component => component.Value, string.Empty));
        Assert.Equal(string.Empty, cut.Find("textarea").GetAttribute("value"));
    }

    [Fact]
    public async Task ControlledTextAreaEmitsBothCallbacksWithoutMutatingItsValue() {
        var area = new TextAreaInput();
        var callbacks = new List<string>();
        SetProperty(area, nameof(TextAreaInput.Value), "Original textarea value");
        SetProperty(area, nameof(TextAreaInput.ValueChanged), EventCallback.Factory.Create<string?>(this, value => callbacks.Add($"value:{value}")));
        SetProperty(area, nameof(TextAreaInput.Change), EventCallback.Factory.Create<string?>(this, value => callbacks.Add($"change:{value}")));

        await InvokePrivateAsync(area, "OnChangedAsync", "Current multiline\nvalue");

        Assert.Equal("Original textarea value", area.Value);
        Assert.Equal(["value:Current multiline\nvalue", "change:Current multiline\nvalue"], callbacks);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task DisabledOrReadOnlyTextAreaRejectsNativeValueCallbacks(bool disabled, bool readOnly) {
        var area = new TextAreaInput();
        var calls = 0;
        SetProperty(area, nameof(TextAreaInput.Disabled), disabled);
        SetProperty(area, nameof(TextAreaInput.ReadOnly), readOnly);
        SetProperty(area, nameof(TextAreaInput.Value), "Kept textarea value");
        SetProperty(area, nameof(TextAreaInput.ValueChanged), EventCallback.Factory.Create<string?>(this, _ => calls++));
        SetProperty(area, nameof(TextAreaInput.Change), EventCallback.Factory.Create<string?>(this, _ => calls++));

        await InvokePrivateAsync(area, "OnChangedAsync", "Refused value");

        Assert.Equal(0, calls);
        Assert.Equal("Kept textarea value", area.Value);
    }

    [Fact]
    public async Task TagEditorReportsUncommittedInputWithoutChangingTheTagValue() {
        var editor = new TagEditor();
        string? pending = null;
        var tagEvents = 0;
        SetProperty(editor, nameof(TagEditor.Value), new[] { "kept" });
        SetProperty(editor, nameof(TagEditor.PendingInputChanged), EventCallback.Factory.Create<string>(this, value => pending = value));
        SetProperty(editor, nameof(TagEditor.ValueChanged), EventCallback.Factory.Create<IReadOnlyList<string>>(this, _ => tagEvents++));

        await InvokePrivateAsync(editor, "HandleInputAsync", new ChangeEventArgs { Value = "unfinished" });

        Assert.Equal("unfinished", pending);
        Assert.Equal("unfinished", editor.PendingInput);
        Assert.Equal(new[] { "kept" }, editor.Value);
        Assert.Equal(0, tagEvents);
    }

    [Fact]
    public async Task TagEditorReportsTheCommaRemainderBeforeEmittingCommittedTags() {
        var editor = new TagEditor();
        var events = new List<string>();
        IReadOnlyList<string>? tags = null;
        SetProperty(editor, nameof(TagEditor.PendingInputChanged), EventCallback.Factory.Create<string>(this, value => events.Add($"pending:{value}")));
        SetProperty(editor, nameof(TagEditor.ValueChanged), EventCallback.Factory.Create<IReadOnlyList<string>>(this, values => {
            tags = values;
            events.Add("tags");
        }));

        await InvokePrivateAsync(editor, "HandleInputAsync", new ChangeEventArgs { Value = "committed, unfinished" });

        Assert.Equal(new[] { "pending:unfinished", "tags" }, events);
        Assert.Equal(new[] { "committed" }, tags);
        Assert.Empty(editor.Value);
        Assert.Equal("unfinished", editor.PendingInput);
    }

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
    public async Task ControlledCheckBoxEmitsValueWithoutMutatingParameter()
    {
        bool? emittedValue = null;
        var checkBox = new CheckBox<bool>();
        SetProperty(checkBox, nameof(CheckBox<bool>.Value), false);
        SetProperty(checkBox, nameof(CheckBox<bool>.ValueChanged), EventCallback.Factory.Create<bool>(this, value => emittedValue = value));

        await InvokePrivateAsync(checkBox, "OnChangedAsync", new ChangeEventArgs { Value = true });

        Assert.False(checkBox.Value);
        Assert.True(emittedValue);
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
