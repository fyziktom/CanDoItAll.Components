using CanDoItAll.Components.Common;

namespace CanDoItAll.Components.BaseLib;

public static class ComponentAttributeExtensions
{
    public static IReadOnlyDictionary<string, object>? WithClass(this IReadOnlyDictionary<string, object>? attributes, string? baseClass)
    {
        return ComponentAttributes.WithClass(attributes, baseClass);
    }

    public static IReadOnlyDictionary<string, object>? WithClassAndStyle(this IReadOnlyDictionary<string, object>? attributes, string? baseClass, string? baseStyle)
    {
        return ComponentAttributes.WithClassAndStyle(attributes, baseClass, baseStyle);
    }
}
