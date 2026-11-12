using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class PageHeaderOutletTests {
    [Fact]
    public void HeaderRendersInlineWithoutAShellOutlet() {
        using var context = new BunitContext();
        var cut = context.Render<PageHeader>(parameters => parameters
            .Add(header => header.Title, "Projects")
            .Add(header => header.Description, "Open work"));

        Assert.Equal("Projects", cut.Find("h1").TextContent);
        Assert.Contains("Open work", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void HeaderPublishesItsSlotsAndClearsOnlyItsOwnRegistrationOnDisposal() {
        using var context = new BunitContext();
        var outlet = new RecordingOutlet();
        var title = "Projects";
        RenderFragment actions = builder => builder.AddMarkupContent(0, "<button>Create</button>");
        RenderFragment stats = builder => builder.AddContent(0, "3 projects");
        RenderFragment header = builder => {
            builder.OpenComponent<PageHeader>(0);
            builder.AddAttribute(1, nameof(PageHeader.Title), title);
            builder.AddAttribute(2, nameof(PageHeader.Description), "Open work");
            builder.AddAttribute(3, nameof(PageHeader.Actions), actions);
            builder.AddAttribute(4, nameof(PageHeader.Stats), stats);
            builder.CloseComponent();
        };
        var cut = context.Render<CascadingValue<IPageHeaderOutlet>>(parameters => parameters
            .Add(value => value.Value, outlet)
            .Add(value => value.ChildContent, header));
        var owner = cut.FindComponent<PageHeader>().Instance;

        Assert.Empty(cut.FindAll("h1"));
        Assert.Same(owner, outlet.Owner);
        Assert.Equal("Projects", outlet.Presentation?.Title);
        Assert.Same(actions, outlet.Presentation?.Actions);
        Assert.Same(stats, outlet.Presentation?.Stats);

        var registrationCount = outlet.RegistrationCount;
        cut.Render(parameters => parameters.Add(value => value.Value, outlet).Add(value => value.ChildContent, header));
        Assert.True(outlet.RegistrationCount > registrationCount);
        registrationCount = outlet.RegistrationCount;
        title = "Agents";
        cut.Render(parameters => parameters.Add(value => value.Value, outlet).Add(value => value.ChildContent, header));
        Assert.Equal("Agents", outlet.Presentation?.Title);
        Assert.True(outlet.RegistrationCount > registrationCount);

        cut.Render(parameters => parameters.Add(value => value.Value, outlet).Add(value => value.ChildContent, _ => { }));
        Assert.Same(owner, outlet.ClearedOwner);
    }

    [Fact]
    public void HeaderRefreshesTheOutletWhenTheSameFragmentCapturesUpdatedValues() {
        using var context = new BunitContext();
        var outlet = context.Render<StatsOutlet>();
        var count = 0;
        RenderFragment stats = builder => builder.AddContent(0, $"{count} projects");
        RenderFragment header = builder => {
            builder.OpenComponent<PageHeader>(0);
            builder.AddAttribute(1, nameof(PageHeader.Title), "Projects");
            builder.AddAttribute(2, nameof(PageHeader.Stats), stats);
            builder.CloseComponent();
        };
        var cut = context.Render<CascadingValue<IPageHeaderOutlet>>(parameters => parameters
            .Add(value => value.Value, outlet.Instance)
            .Add(value => value.ChildContent, header));

        Assert.Equal("0 projects", outlet.Markup);

        count = 2;
        cut.Render(parameters => parameters
            .Add(value => value.Value, outlet.Instance)
            .Add(value => value.ChildContent, header));

        Assert.Same(stats, outlet.Instance.Presentation?.Stats);
        Assert.Equal("2 projects", outlet.Markup);
    }

    [Fact]
    public void HeaderReleasesThePreviousOutletWhenItsShellChanges() {
        using var context = new BunitContext();
        var previous = new RecordingOutlet();
        var next = new RecordingOutlet();
        RenderFragment header = builder => {
            builder.OpenComponent<PageHeader>(0);
            builder.AddAttribute(1, nameof(PageHeader.Title), "Projects");
            builder.CloseComponent();
        };
        var cut = context.Render<CascadingValue<IPageHeaderOutlet>>(parameters => parameters
            .Add(value => value.Value, previous)
            .Add(value => value.ChildContent, header));
        var owner = cut.FindComponent<PageHeader>().Instance;

        cut.Render(parameters => parameters.Add(value => value.Value, next).Add(value => value.ChildContent, header));

        Assert.Same(owner, previous.ClearedOwner);
        Assert.Same(owner, next.Owner);
        Assert.Equal("Projects", next.Presentation?.Title);
    }

    private sealed class StatsOutlet : ComponentBase, IPageHeaderOutlet {
        private object? owner;
        public PageHeaderPresentation? Presentation { get; private set; }

        public void Register(object owner, PageHeaderPresentation presentation) {
            this.owner = owner;
            Presentation = presentation;
            StateHasChanged();
        }

        public void Clear(object owner) {
            if (ReferenceEquals(this.owner, owner)) {
                this.owner = null;
                Presentation = null;
                StateHasChanged();
            }
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder) {
            builder.AddContent(0, Presentation?.Stats);
        }
    }

    private sealed class RecordingOutlet : IPageHeaderOutlet {
        public object? Owner { get; private set; }
        public object? ClearedOwner { get; private set; }
        public PageHeaderPresentation? Presentation { get; private set; }
        public int RegistrationCount { get; private set; }

        public void Register(object owner, PageHeaderPresentation presentation) {
            Owner = owner;
            Presentation = presentation;
            RegistrationCount++;
        }

        public void Clear(object owner) {
            ClearedOwner = owner;
            if (ReferenceEquals(Owner, owner)) {
                Owner = null;
                Presentation = null;
            }
        }
    }
}
