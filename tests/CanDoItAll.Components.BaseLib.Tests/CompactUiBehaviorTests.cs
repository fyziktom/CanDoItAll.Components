using Bunit;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class CompactUiBehaviorTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ExpandTransitionPreservesContentAndExplicitExpansionState(bool isExpanded) {
        using var context = new BunitContext();
        var cut = context.Render<ExpandTransition>(parameters => parameters
            .Add(component => component.IsExpanded, isExpanded)
            .Add(component => component.ChildContent, Markup("<p>Project details</p>")));

        Assert.Equal("Project details", cut.Find(".cda-expand-transition > p").TextContent);
        Assert.Equal(isExpanded, cut.Find(".cda-expand-transition").ClassList.Contains("cda-expand-transition--expanded"));
    }

    [Theory]
    [InlineData(TextAreaSize.Compact, "3")]
    [InlineData(TextAreaSize.Standard, "5")]
    [InlineData(TextAreaSize.Extended, "10")]
    public void TextAreaSizeSelectsRowsFromExpectedContentLength(TextAreaSize size, string expectedRows)
    {
        using var context = new BunitContext();
        var cut = context.Render<TextArea>(parameters => parameters
            .Add(component => component.Size, size));

        Assert.Equal(expectedRows, cut.Find("textarea").GetAttribute("rows"));
    }

    [Fact]
    public void ExplicitTextAreaRowsOverrideSemanticSize()
    {
        using var context = new BunitContext();
        var cut = context.Render<TextArea>(parameters => parameters
            .Add(component => component.Size, TextAreaSize.Extended)
            .Add(component => component.Rows, 4));

        Assert.Equal("4", cut.Find("textarea").GetAttribute("rows"));
    }

    [Theory]
    [InlineData(ModalSize.Compact, "cda-dialog--compact")]
    [InlineData(ModalSize.Medium, "cda-dialog--medium")]
    [InlineData(ModalSize.Wide, "cda-dialog--wide")]
    [InlineData(ModalSize.Full, "cda-dialog--full")]
    public void DialogSizeUsesBoundedSemanticClass(ModalSize size, string expectedClass)
    {
        using var context = CreateDialogContext();
        var cut = context.Render<Dialog>(parameters => parameters
            .Add(component => component.IsOpen, true)
            .Add(component => component.Title, "Edit record")
            .Add(component => component.Size, size)
            .Add(component => component.ChildContent, Markup("<p>Editor</p>")));

        Assert.Contains(expectedClass, cut.Find(".cda-dialog").ClassList);
    }

    [Fact]
    public void DenseDialogChromeIncludesDenseFooter()
    {
        using var context = CreateDialogContext();
        var cut = context.Render<Dialog>(parameters => parameters
            .Add(component => component.IsOpen, true)
            .Add(component => component.Title, "Edit record")
            .Add(component => component.DenseChrome, true)
            .Add(component => component.ChildContent, Markup("<p>Editor</p>"))
            .Add(component => component.Footer, Markup("<button>Save</button>")));

        Assert.Contains("cda-dialog__footer--dense", cut.Find(".cda-dialog__footer").ClassList);
    }

    [Fact]
    public void DialogHeaderKeepsActionsBesideFlexibleCopyWithWrappedFallback()
    {
        using var context = CreateDialogContext();
        var cut = context.Render<Dialog>(parameters => parameters
            .Add(component => component.IsOpen, true)
            .Add(component => component.Title, "Add record")
            .Add(component => component.Subtitle, "Create the record only when it is requested.")
            .Add(component => component.Size, ModalSize.Compact)
            .Add(component => component.HeaderActions, Markup("<button>Preview</button><button>Reset</button>"))
            .Add(component => component.ChildContent, Markup("<p>Editor</p>")));

        var headerLayout = cut.Find(".cda-dialog__header > div");

        Assert.Contains("w-full", headerLayout.ClassList);
        Assert.Contains("flex-wrap", headerLayout.ClassList);
        Assert.Contains("cda-dialog__copy", cut.Find(".cda-dialog__header .rz-stack").ClassList);
        Assert.Contains("flex-wrap", cut.Find(".cda-dialog__header-actions").ClassList);
    }

    [Fact]
    public void DialogPreservesClassAndStyleOnRootAndSurface()
    {
        using var context = CreateDialogContext();
        var cut = context.Render<Dialog>(parameters => parameters
            .Add(component => component.IsOpen, true)
            .Add(component => component.Title, "Edit record")
            .Add(component => component.Class, "consumer-dialog")
            .Add(component => component.Style, "--consumer-dialog:1")
            .Add(component => component.ChildContent, Markup("<p>Editor</p>")));

        var root = cut.Find("dialog");
        var surface = cut.Find(".cda-dialog");

        Assert.Contains("consumer-dialog", root.ClassList);
        Assert.Contains("consumer-dialog", surface.ClassList);
        Assert.Contains("--consumer-dialog:1", root.GetAttribute("style"));
        Assert.Contains("--consumer-dialog:1", surface.GetAttribute("style"));
    }

    [Fact]
    public void StructuredCardRendersMediaTagsContentAndBottomActions()
    {
        using var context = new BunitContext();
        var cut = context.Render<Card>(parameters => parameters
            .Add(component => component.Media, Markup("<img src=\"preview.png\" alt=\"\" />"))
            .Add(component => component.Header, Markup("<h3>Preview</h3>"))
            .Add(component => component.Tags, Markup("<span>Ready</span>"))
            .Add(component => component.ChildContent, Markup("<p>Details</p>"))
            .Add(component => component.Actions, Markup("<button>Edit</button>")));

        Assert.Contains("cda-card--structured", cut.Find(".cda-card").ClassList);
        Assert.NotNull(cut.Find(".cda-card__media img"));
        Assert.Equal("Ready", cut.Find(".cda-card__tags").TextContent);
        Assert.Equal("Details", cut.Find(".cda-card__content").TextContent);
        Assert.Equal("Edit", cut.Find(".cda-card__actions").TextContent);
    }

    [Fact]
    public void ChildOnlyCardPreservesLegacyMarkup()
    {
        using var context = new BunitContext();
        var cut = context.Render<Card>(parameters => parameters
            .Add(component => component.ChildContent, Markup("<p class=\"legacy-content\">Details</p>")));

        Assert.DoesNotContain("cda-card--structured", cut.Find(".cda-card").ClassList);
        Assert.Equal("Details", cut.Find(".cda-card > .legacy-content").TextContent);
        Assert.Empty(cut.FindAll(".cda-card__body"));
    }

    [Fact]
    public void CompactPageHeaderPreservesDescription()
    {
        using var context = new BunitContext();
        var cut = context.Render<PageHeader>(parameters => parameters
            .Add(component => component.Title, "Records")
            .Add(component => component.Description, "Current records and actions.")
            .Add(component => component.Compact, true));

        var description = cut.Find(".cda-page-header__description--compact");

        Assert.Equal("Current records and actions.", description.TextContent);
    }

    [Fact]
    public void SplitFillsItsParentByDefault()
    {
        using var context = new BunitContext();
        var cut = context.Render<Split>();

        Assert.Contains("w-full", cut.Find("div").ClassList);
    }

    [Fact]
    public void SectionCardStretchesArbitraryContent()
    {
        using var context = new BunitContext();
        var cut = context.Render<SectionCard>(parameters => parameters
            .Add(component => component.ChildContent, Markup("<div class=\"form-content\">Form</div>")));

        Assert.Contains("align-items:stretch", cut.Find(".cda-section-card").GetAttribute("style"));
    }

    [Fact]
    public void CardGridUsesContainerAwareTracksWithoutEmptyFixedColumns()
    {
        using var context = new BunitContext();
        var cut = context.Render<CardGrid>(parameters => parameters
            .Add(component => component.ChildContent, Markup("<article>One</article><article>Two</article>")));

        Assert.Contains(
            "--cda-grid-columns-sm:repeat(auto-fit,minmax(min(100%,18rem),1fr))",
            cut.Find("div").GetAttribute("style"));
    }

    [Fact]
    public void SecretFieldUsesContainerAwareWrappingInsteadOfViewportBreakpoints()
    {
        using var context = new BunitContext();
        var cut = context.Render<SecretField>(parameters => parameters
            .Add(component => component.Value, "secret")
            .Add(component => component.SecretName, "api-key")
            .Add(component => component.ShowNameCopy, true));

        Assert.Contains("cda-secret-field", cut.Find("div").ClassList);
        Assert.DoesNotContain("sm:flex-row", cut.Find("div").ClassList);
        Assert.NotNull(cut.Find(".cda-secret-field__control"));
        Assert.Contains("flex-wrap", cut.Find(".cda-secret-field__actions").ClassList);
        Assert.Equal("Show 30s", cut.Find(".cda-secret-field__reveal .rz-button-text").TextContent.Trim());
    }

    [Fact]
    public void SecretFieldCanHideBeforeTimedRevealExpires()
    {
        using var context = new BunitContext();
        var cut = context.Render<SecretField>(parameters => parameters
            .Add(component => component.Value, "secret")
            .Add(component => component.RevealDurationSeconds, 30));

        var reveal = cut.Find(".cda-secret-field__reveal");

        Assert.Equal("password", cut.Find("input").GetAttribute("type"));
        Assert.Equal("Show secret for 30 seconds", reveal.GetAttribute("aria-label"));

        reveal.Click();
        var hide = cut.Find(".cda-secret-field__reveal");

        Assert.Equal("text", cut.Find("input").GetAttribute("type"));
        Assert.False(hide.HasAttribute("disabled"));
        Assert.Equal("Hide secret", hide.GetAttribute("aria-label"));

        hide.Click();

        Assert.Equal("password", cut.Find("input").GetAttribute("type"));
        Assert.Equal("Show secret for 30 seconds", cut.Find(".cda-secret-field__reveal").GetAttribute("aria-label"));
    }

    [Fact]
    public void SelectionListItemWithoutSelectionCallbackRendersPassiveContent()
    {
        using var context = new BunitContext();
        var cut = context.Render<SelectionListItem>(parameters => parameters
            .Add(component => component.Title, "Record"));

        Assert.Empty(cut.FindAll("button.cda-selection-list-item__button"));
        Assert.Equal("Record", cut.Find("div.cda-selection-list-item__button").TextContent.Trim());
    }

    [Fact]
    public void DisabledSelectionListItemDoesNotInvokeSelection()
    {
        using var context = new BunitContext();
        var selectionCount = 0;
        var cut = context.Render<SelectionListItem>(parameters => parameters
            .Add(component => component.Title, "Record")
            .Add(component => component.Disabled, true)
            .Add(component => component.OnSelect, () => selectionCount++));

        cut.Find("button.cda-selection-list-item__button").Click();

        Assert.Equal(0, selectionCount);
    }

    private static RenderFragment Markup(string value)
    {
        return builder => builder.AddMarkupContent(0, value);
    }

    private static BunitContext CreateDialogContext()
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
