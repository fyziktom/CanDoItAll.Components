using Bunit;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class NativeFormInspectorTests {
    private const string ModulePath = "./_content/CanDoItAll.Components.BaseLib/Components/Forms/NativeFormInspector.razor.js";

    [Fact]
    public async Task Inspection_and_focus_use_the_current_form_and_owner_section_contract() {
        await using var context = new BunitContext();
        var module = context.JSInterop.SetupModule(ModulePath);
        module.Setup<NativeFormValidity>("inspect", _ => true).SetResult(new(false, 2));
        module.SetupVoid("focusInvalid", _ => true).SetVoidResult();
        var cut = context.Render<NativeFormInspector>(parameters => parameters
            .Add(component => component.FormId, "first-form")
            .Add(component => component.SectionIndexAttribute, "data-section")
            .Add(component => component.PanelsSelector, ".editor-panels"));

        Assert.Equal(new(false, 2), await cut.InvokeAsync(cut.Instance.InspectAsync));
        cut.Render(parameters => parameters.Add(component => component.FormId, "second-form"));
        await cut.InvokeAsync(cut.Instance.FocusInvalidAsync);

        Assert.Equal(new object?[] { "first-form", "data-section", ".editor-panels" }, module.Invocations["inspect"].Single().Arguments);
        Assert.Equal("second-form", module.Invocations["focusInvalid"].Single().Arguments[0]);
        Assert.Single(context.JSInterop.Invocations["import"]);
    }

    [Fact]
    public async Task A_disposed_form_cannot_read_or_focus_another_opening() {
        await using var context = new BunitContext();
        var cut = context.Render<NativeFormInspector>(parameters => parameters.Add(component => component.FormId, "retired-form"));
        var inspector = cut.Instance;
        await context.DisposeComponentsAsync();

        await Assert.ThrowsAsync<ObjectDisposedException>(inspector.InspectAsync);
        await Assert.ThrowsAsync<ObjectDisposedException>(inspector.FocusInvalidAsync);
        Assert.Empty(context.JSInterop.Invocations);
    }
}
