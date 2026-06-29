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
}
