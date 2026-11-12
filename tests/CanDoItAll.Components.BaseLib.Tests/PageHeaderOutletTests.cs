using Bunit;
using Microsoft.AspNetCore.Components;

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

        cut.Render(parameters => parameters.Add(value => value.Value, outlet).Add(value => value.ChildContent, header));
        Assert.Equal(1, outlet.RegistrationCount);
        title = "Agents";
        cut.Render(parameters => parameters.Add(value => value.Value, outlet).Add(value => value.ChildContent, header));
        Assert.Equal("Agents", outlet.Presentation?.Title);
        Assert.Equal(2, outlet.RegistrationCount);

        cut.Render(parameters => parameters.Add(value => value.Value, outlet).Add(value => value.ChildContent, _ => { }));
        Assert.Same(owner, outlet.ClearedOwner);
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
