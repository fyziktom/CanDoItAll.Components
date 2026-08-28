using CanDoItAll.Components.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace CanDoItAll.Components.BaseLib;

/// <summary>
/// Base class for BaseLib form controls that must be true drop-in replacements for Blazor's
/// native <c>Input*</c> components (real <see cref="EditContext"/>/<c>ValueExpression</c> support).
/// See <c>StyledComponentBase</c> for components that only need plain <c>Value</c>/<c>ValueChanged</c>
/// binding without <c>EditForm</c> involvement.
/// </summary>
public abstract class StyledInputBase<TValue> : InputBase<TValue>
{
    /// <summary>Extra CSS classes appended to the component's own classes rather than replacing them.</summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>Inline styles merged onto the root element.</summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>The visual style applied to the rendered element.</summary>
    [Parameter]
    public InputLook Look { get; set; } = InputLook.Default;

    [CascadingParameter(Name = "FormFieldLabelId")]
    public string? FormFieldLabelId { get; set; }

    /// <summary>
    /// Merges <paramref name="baseClass"/>/<paramref name="baseStyle"/> with <see cref="Class"/>/<see cref="Style"/>,
    /// the component's unmatched attributes, and (when an <see cref="EditContext"/> is present) the field's
    /// validation-state class. Deliberately bypasses <c>InputBase&lt;TValue&gt;.CssClass</c>, which
    /// independently folds <c>AdditionalAttributes["class"]</c> + <c>EditContext.FieldCssClass</c> — using
    /// both would double-emit the consumer's <see cref="Class"/>.
    /// </summary>
    protected IReadOnlyDictionary<string, object>? BuildAttributes(string? baseClass = null, string? baseStyle = null)
    {
        var validationClass = EditContext?.FieldCssClass(FieldIdentifier);
        return ComponentAttributes.WithClassAndStyle(
            AdditionalAttributes,
            CssClassBuilder.Join(baseClass, Class, validationClass),
            CssClassBuilder.JoinStyles(baseStyle, Style));
    }

    protected string? ResolveLabelledBy()
        => HasExplicitAccessibleName() ? null : FormFieldLabelId;

    protected bool HasExplicitAccessibleName()
        => AdditionalAttributes is not null
            && AdditionalAttributes.Keys.Any(key =>
                string.Equals(key, "aria-label", StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, "aria-labelledby", StringComparison.OrdinalIgnoreCase));
}
