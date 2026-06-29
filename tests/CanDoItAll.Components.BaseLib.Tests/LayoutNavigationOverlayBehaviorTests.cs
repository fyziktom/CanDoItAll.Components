using CanDoItAll.Components.OverlayLib;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class LayoutNavigationOverlayBehaviorTests
{
    [Theory]
    [InlineData(typeof(ContextMenu))]
    [InlineData(typeof(StickyActionFooter))]
    public void WrapperComponentsUseStyledAttributeContract(Type componentType)
    {
        Assert.True(typeof(StyledComponentBase).IsAssignableFrom(componentType));
    }

    [Fact]
    public void OverlayWindowStateNormalizesGeometryForInterop()
    {
        var normalized = OverlayWindowState.Normalize(new OverlayWindowState
        {
            IsVisible = true,
            IsMinimized = true,
            Left = 10.123,
            Top = -1,
            Width = 320.125,
            Height = 0
        });

        Assert.True(normalized.IsVisible);
        Assert.True(normalized.IsMinimized);
        Assert.Equal(10.12, normalized.Left);
        Assert.Null(normalized.Top);
        Assert.Equal(320.13, normalized.Width);
        Assert.Null(normalized.Height);
    }

    [Fact]
    public void OverlayWindowStateNormalizeNullUsesDefaultVisibleWindow()
    {
        var normalized = OverlayWindowState.Normalize(null);

        Assert.True(normalized.IsVisible);
        Assert.False(normalized.IsMinimized);
        Assert.False(normalized.HasCustomGeometry);
        Assert.Null(normalized.Left);
        Assert.Null(normalized.Top);
        Assert.Null(normalized.Width);
        Assert.Null(normalized.Height);
    }

    [Fact]
    public void OverlayWindowStateNormalizeClearsAllNonPositiveGeometry()
    {
        var normalized = OverlayWindowState.Normalize(new OverlayWindowState
        {
            Left = 0,
            Top = -0.01,
            Width = -320,
            Height = 0
        });

        Assert.False(normalized.HasCustomGeometry);
        Assert.Null(normalized.Left);
        Assert.Null(normalized.Top);
        Assert.Null(normalized.Width);
        Assert.Null(normalized.Height);
    }

    [Fact]
    public void OverlayWindowStateClonePreservesVisibilityAndGeometryWithoutSharing()
    {
        var original = new OverlayWindowState
        {
            IsVisible = false,
            IsMinimized = true,
            Left = 12,
            Top = 24,
            Width = 360,
            Height = 280
        };

        var clone = original.Clone();
        clone.Left = 48;

        Assert.False(clone.IsVisible);
        Assert.True(clone.IsMinimized);
        Assert.Equal(12, original.Left);
        Assert.Equal(48, clone.Left);
        Assert.Equal(original.Top, clone.Top);
        Assert.Equal(original.Width, clone.Width);
        Assert.Equal(original.Height, clone.Height);
    }

    [Fact]
    public void OverlayWindowStateEquivalenceUsesNormalizedValues()
    {
        var left = new OverlayWindowState
        {
            Left = 42.124,
            Top = 24.126,
            Width = 360,
            Height = 280
        };
        var right = new OverlayWindowState
        {
            Left = 42.12,
            Top = 24.13,
            Width = 360,
            Height = 280
        };

        Assert.True(OverlayWindowState.AreEquivalent(left, right));
    }

    [Fact]
    public void OverlayWindowStateEquivalenceTreatsNullAndDefaultAsSameWindow()
    {
        Assert.True(OverlayWindowState.AreEquivalent(null, new OverlayWindowState()));
    }

    [Fact]
    public void OverlayWindowStateEquivalenceDistinguishesVisibilityAndMinimizedSemantics()
    {
        Assert.False(
            OverlayWindowState.AreEquivalent(
                new OverlayWindowState { IsVisible = true, IsMinimized = false },
                new OverlayWindowState { IsVisible = false, IsMinimized = false }));

        Assert.False(
            OverlayWindowState.AreEquivalent(
                new OverlayWindowState { IsVisible = true, IsMinimized = false },
                new OverlayWindowState { IsVisible = true, IsMinimized = true }));
    }
}
