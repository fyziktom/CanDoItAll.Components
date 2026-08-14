namespace CanDoItAll.Components.BaseLib;

public sealed record HomoAvatarOptions
{
    /// <summary>Explicit digit-string seed. Takes priority over <see cref="Text"/> when set.</summary>
    public string? Id { get; init; }

    /// <summary>Free text hashed into an id when <see cref="Id"/> is not set.</summary>
    public string? Text { get; init; }

    /// <summary>Manual per-part overrides. Shape: key is the part name (e.g. "body"). Color: key is "{part}-color". Values are 1-based.</summary>
    public IReadOnlyDictionary<string, int>? Choices { get; init; }

    public RoboAvatarVariant Variant { get; init; } = RoboAvatarVariant.Plain;

    public bool Dark { get; init; }

    public double? LineWidth { get; init; }

    /// <summary>Fill color for non-gradient parts. Defaults to the BaseLib surface token (white outside a ThemeHost). Pass "currentColor" to inherit the surrounding text color.</summary>
    public string? Background { get; init; }

    public string? Title { get; init; }

    /// <summary>Uniqueness token embedded in the gradient element id, so multiple avatars on one page don't collide.</summary>
    public string GradientId { get; init; } = "g";
}
