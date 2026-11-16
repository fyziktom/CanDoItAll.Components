using System.Globalization;
using System.Text;

namespace CanDoItAll.Components.BaseLib;

internal static class AvatarGeneratorEngine
{
    private const string Digits = "0123456789";

    private static readonly string[] LightOpacityStops = ["0.24", "0.6", "0.12"];
    private static readonly string[] DarkOpacityStops = ["0.3", "0.5", "0.18"];

    // Follows the BaseLib surface token (flips with the nearest [data-ui-theme] ancestor set by
    // ThemeHost) instead of a literal white, so the mouth/eyes plate doesn't stay white on dark
    // surfaces. Falls back to white outside a ThemeHost, matching the JS default.
    private const string DefaultBackground = "var(--tone-surface, #fff)";

    public static string TextToAvatarId(string? text, string defaultSeedText, int length)
    {
        var input = string.IsNullOrEmpty(text) ? defaultSeedText : text;

        var hash = 0x811c9dc5u;
        foreach (var character in input)
        {
            hash ^= character;
            hash = unchecked(hash * 0x01000193u);
        }

        var state = hash != 0 ? hash : 1u;
        var id = new char[length];
        for (var index = 0; index < length; index++)
        {
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            id[index] = Digits[(int)(state % 10)];
        }

        return new string(id);
    }

    public static string RenderSvg(
        string avatarKey,
        (string Name, string[] Variants)[] partsInOrder,
        string[] palette,
        int[][] secondaryPalette,
        string defaultSeedText,
        string? id,
        string? text,
        IReadOnlyDictionary<string, int>? choices,
        RoboAvatarVariant variant,
        bool dark,
        double? lineWidth,
        string? background,
        string? title,
        string gradientId)
    {
        var resolvedId = id ?? TextToAvatarId(text, defaultSeedText, partsInOrder.Length * 2);
        var shapeIndexes = new int[partsInOrder.Length];
        var colorIndexes = new int[partsInOrder.Length];

        for (var i = 0; i < partsInOrder.Length; i++)
        {
            var (name, variants) = partsInOrder[i];
            shapeIndexes[i] = ResolveIndex(GetChoice(choices, name), IdCharAt(resolvedId, i * 2), variants.Length);
            colorIndexes[i] = ResolveIndex(GetChoice(choices, $"{name}-color"), IdCharAt(resolvedId, i * 2 + 1), palette.Length);
        }

        var bodyColorIndex = colorIndexes[0];
        var useGradientDefs = variant != RoboAvatarVariant.Plain;
        var gradientElementId = $"cda-{avatarKey}-{gradientId}";

        var content = new StringBuilder();

        if (useGradientDefs)
        {
            var opacityStops = dark ? DarkOpacityStops : LightOpacityStops;
            var stopStart = variant == RoboAvatarVariant.Fill ? opacityStops[0] : opacityStops[1];
            var stopEnd = variant == RoboAvatarVariant.Fill ? opacityStops[0] : opacityStops[2];
            var bodyColor = palette[bodyColorIndex];

            content.Append("<defs><radialGradient id=\"").Append(Escape(gradientElementId))
                .Append("\" cx=\"0.5\" cy=\"0.5\" r=\"0.5\" fx=\"0.5\" fy=\".8\">")
                .Append("<stop offset=\"0%\" stop-color=\"").Append(Escape(bodyColor)).Append("\" stop-opacity=\"").Append(stopStart).Append("\" />")
                .Append("<stop offset=\"100%\" stop-color=\"").Append(Escape(bodyColor)).Append("\" stop-opacity=\"").Append(stopEnd).Append("\" />")
                .Append("</radialGradient></defs>");
        }

        for (var i = 0; i < partsInOrder.Length; i++)
        {
            var (name, variants) = partsInOrder[i];
            var shapeMarkup = variants[shapeIndexes[i]];
            var color = i == 0
                ? palette[colorIndexes[0]]
                : palette[secondaryPalette[bodyColorIndex][colorIndexes[i] % secondaryPalette[bodyColorIndex].Length]];

            content.Append("<g data-").Append(Escape(avatarKey)).Append("-part=\"").Append(Escape(name)).Append('"');

            if (lineWidth is { } resolvedLineWidth)
            {
                content.Append(" stroke-width=\"").Append(resolvedLineWidth.ToString(CultureInfo.InvariantCulture)).Append('"');
            }

            var isGradientBody = i == 0 && useGradientDefs;
            if (isGradientBody)
            {
                content.Append(" fill=\"url(#").Append(Escape(gradientElementId)).Append(")\" stroke=\"").Append(Escape(color)).Append('"');
            }
            else if (background == "currentColor")
            {
                content.Append(" stroke=\"").Append(Escape(color)).Append("\" style=\"fill: currentColor;\"");
            }
            else
            {
                content.Append(" stroke=\"").Append(Escape(color)).Append("\" fill=\"").Append(Escape(background ?? DefaultBackground)).Append('"');
            }

            content.Append('>').Append(shapeMarkup).Append("</g>");
        }

        var hasTitle = !string.IsNullOrEmpty(title);
        var svg = new StringBuilder();
        svg.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\"");
        if (hasTitle)
        {
            svg.Append(" role=\"img\"");
        }

        svg.Append('>');
        if (hasTitle)
        {
            svg.Append("<title>").Append(EscapeText(title!)).Append("</title>");
        }

        svg.Append(content);
        svg.Append("</svg>");

        return svg.ToString();
    }

    private static int? GetChoice(IReadOnlyDictionary<string, int>? choices, string key)
        => choices is not null && choices.TryGetValue(key, out var value) ? value : null;

    private static char? IdCharAt(string id, int index)
        => index < id.Length ? id[index] : null;

    private static int ResolveIndex(int? choice, char? idChar, int count)
    {
        if (count <= 0)
        {
            return 0;
        }

        if (choice is int selected)
        {
            return WrapIndex(selected - 1, count);
        }

        return WrapIndex(CharValue(idChar), count);
    }

    private static int CharValue(char? character)
    {
        if (character is null)
        {
            return 0;
        }

        var index = Digits.IndexOf(char.ToLowerInvariant(character.Value));
        return index == -1 ? character.Value : index;
    }

    private static int WrapIndex(int index, int count)
        => ((index % count) + count) % count;

    private static string Escape(string value)
        => value
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("\"", "&quot;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);

    private static string EscapeText(string value)
        => value
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);
}
