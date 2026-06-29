namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class ComponentAttributeCompatibilityTests
{
    [Fact]
    public void BaseLibExtensionPreservesClassAndStyleMergeOrder()
    {
        IReadOnlyDictionary<string, object> attributes = new Dictionary<string, object>
        {
            ["class"] = "consumer-class",
            ["style"] = "color: red;",
            ["data-id"] = "button-7"
        };

        var merged = attributes.WithClassAndStyle("base-class", "display: inline-flex;");

        Assert.NotNull(merged);
        Assert.Equal("base-class consumer-class", merged["class"]);
        Assert.Equal("display: inline-flex; color: red;", merged["style"]);
        Assert.Equal("button-7", merged["data-id"]);
    }

    [Fact]
    public void StyledComponentBasePreservesBaseClassComponentClassAndCapturedClassOrder()
    {
        var component = new TestStyledComponent();
        component.Configure(
            "component-class",
            "gap: 0.5rem;",
            new Dictionary<string, object>
            {
                ["class"] = "consumer-class",
                ["style"] = "color: red;",
                ["aria-label"] = "Save"
            });

        var merged = component.Merge("base-class", "display: flex;");

        Assert.NotNull(merged);
        Assert.Equal("base-class component-class consumer-class", merged["class"]);
        Assert.Equal("display: flex; gap: 0.5rem; color: red;", merged["style"]);
        Assert.Equal("Save", merged["aria-label"]);
    }

    private sealed class TestStyledComponent : StyledComponentBase
    {
        public void Configure(
            string? componentClass,
            string? componentStyle,
            IReadOnlyDictionary<string, object>? additionalAttributes)
        {
            Class = componentClass;
            Style = componentStyle;
            AdditionalAttributes = additionalAttributes;
        }

        public IReadOnlyDictionary<string, object>? Merge(string? baseClass, string? baseStyle)
            => BuildAttributes(baseClass, baseStyle);
    }
}
