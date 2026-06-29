using CanDoItAll.Components.Common;

namespace CanDoItAll.Components.Common.Tests;

public sealed class ComponentAttributesTests
{
    [Fact]
    public void WithClassAndStyle_MergesBaseAndCapturedAttributes()
    {
        var attributes = new Dictionary<string, object>
        {
            ["CLASS"] = "consumer-class",
            ["style"] = "color: red;",
            ["data-id"] = "field-42"
        };

        var merged = ComponentAttributes.WithClassAndStyle(
            attributes,
            "base-class",
            "display: flex;");

        Assert.NotNull(merged);
        Assert.Equal("base-class consumer-class", merged["class"]);
        Assert.Equal("display: flex; color: red;", merged["style"]);
        Assert.Equal("field-42", merged["data-id"]);
    }

    [Fact]
    public void WithClassAndStyle_RemovesEmptyClassAndStyleAttributes()
    {
        var attributes = new Dictionary<string, object>
        {
            ["class"] = "   ",
            ["style"] = "",
            ["aria-label"] = "Search"
        };

        var merged = ComponentAttributes.WithClassAndStyle(attributes, null, null);

        Assert.NotNull(merged);
        Assert.False(merged.ContainsKey("class"));
        Assert.False(merged.ContainsKey("style"));
        Assert.Equal("Search", merged["aria-label"]);
    }

    [Fact]
    public void WithClassAndStyle_ReturnsNullWhenNoAttributesRemain()
    {
        var merged = ComponentAttributes.WithClassAndStyle(null, " ", "");

        Assert.Null(merged);
    }
}
