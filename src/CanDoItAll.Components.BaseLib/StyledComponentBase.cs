using CanDoItAll.Components.Common;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.BaseLib;

public abstract class StyledComponentBase : ComponentBase
{
    /// <summary>Extra CSS classes appended to the component's own classes rather than replacing them.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>Inline styles merged onto the root element.</summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>Any other attribute written at the call site, splatted onto the root element.</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected IReadOnlyDictionary<string, object>? BuildAttributes(string? baseClass = null, string? baseStyle = null)
    {
        return ComponentAttributes.WithClassAndStyle(
            AdditionalAttributes,
            CssClassBuilder.Join(baseClass, Class),
            CssClassBuilder.JoinStyles(baseStyle, Style));
    }

    protected string BuildClass(params string?[] values)
    {
        return CssClassBuilder.Join(values.Concat([Class]).ToArray());
    }

    protected string? BuildStyle(params string?[] values)
    {
        return CssClassBuilder.JoinStyles(values.Concat([Style]).ToArray());
    }
}





