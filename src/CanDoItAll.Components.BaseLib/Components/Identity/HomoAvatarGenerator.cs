namespace CanDoItAll.Components.BaseLib;

public static class HomoAvatarGenerator
{
    private const string AvatarKey = "homo";
    private const string DefaultSeedText = "homo-avatar";

    private static readonly string[] BodyShapes =
    [
        "<path d=\"M 3,12.5 A 9,9 0 1 1 21,12.5 A 9,9 0 1 1 3,12.5 Z M 5,6.2 C 6.5,3.2 17.5,3.2 19,6.2 M 1.6,12.5 A 1,1 0 1 1 3.6,12.5 A 1,1 0 1 1 1.6,12.5 Z M 20.4,12.5 A 1,1 0 1 1 22.4,12.5 A 1,1 0 1 1 20.4,12.5 Z\"></path>",
        "<path d=\"M 5,12.5 A 7,9.5 0 1 1 19,12.5 A 7,9.5 0 1 1 5,12.5 Z M 6,5 C 9,3.3 14,2.8 18,7\"></path>",
        "<path d=\"M 9,4 H 15 A 6,6 0 0 1 21,10 V 15 A 6,6 0 0 1 15,21 H 9 A 6,6 0 0 1 3,15 V 10 A 6,6 0 0 1 9,4 Z M 4,8.5 C 5,3 10.5,2.7 12.2,3.6 C 13,2.9 19,3.3 20.5,8.5 M 1.6,12.5 A 1,1 0 1 1 3.6,12.5 A 1,1 0 1 1 1.6,12.5 Z M 20.4,12.5 A 1,1 0 1 1 22.4,12.5 A 1,1 0 1 1 20.4,12.5 Z\"></path>",
        "<path d=\"M 12,3 A 7,7 0 0 1 19,10 V 14.5 A 7,7 0 0 1 12,21.5 A 7,7 0 0 1 5,14.5 V 10 A 7,7 0 0 1 12,3 Z M 5.5,7.5 C 6.5,5 7.5,9 8.5,6 C 9.5,3.5 10.5,8 11.5,5.5 C 12.5,3.3 13.5,8 14.5,5.5 C 15.5,3.3 16.5,8 17.5,5.5 C 18,4.3 18.5,6 18.5,7.5 M 3.6,12.25 A 1,1 0 1 1 5.6,12.25 A 1,1 0 1 1 3.6,12.25 Z M 18.4,12.25 A 1,1 0 1 1 20.4,12.25 A 1,1 0 1 1 18.4,12.25 Z\"></path>",
        "<path d=\"m 12,3.5 c 8.091892,0.1473813 7,4 7,8.5 0,4.5 -4,9.5 -7,9.5 C 9,21.5 5,16.5 5,12 5,7.5 3.9081079,3.3526187 12,3.5 Z M 5,11 C 4,10.5 3.7,12 4.7,13 M 19,11 c 1,-0.5 1.3,1 0.3,2\"></path>",
        "<path d=\"M 21,6.5 C 21,18.130915 16.5,21.946374 12,21.946374 7.5,21.946375 3,18.130917 3,6.5 2.264296,1.2857734 21.735704,1.2857725 21,6.5 Z M 5,6 6.5,2.5 l 2,3 1.5,-3.5 2,3 2,-3 1.5,3.5 2,-3 L 19,6\"></path>"
    ];

    private static readonly string[] MouthShapes =
    [
        "<path d=\"m 8,16.5 c 2.666667,2 5.333333,2 8,0 z\"></path>",
        "<path d=\"M 7.5,16 Q 12,20.5 16.5,16 Q 12,18.3 7.5,16 Z M 8.3,16.6 Q 12,17.6 15.7,16.6\"></path>",
        "<path d=\"M 10.3,17 A 1.7,1.9 0 1 1 13.7,17 A 1.7,1.9 0 1 1 10.3,17 Z\"></path>",
        "<path d=\"M 8,17 H 16\"></path>",
        "<path d=\"m 8,16.5 c 2.666667,-2 5.333333,-2 8,0 z\"></path>",
        "<path d=\"m 8.5,17 c 2.550168,1.611791 4.890857,1.248375 7,-1.2 z\"></path>",
        "<path d=\"M 7,15.8 Q 12,21 17,15.8 Q 12,17.8 7,15.8 Z M 9.5,18.3 Q 12,20 14.5,18.3\"></path>",
        "<path d=\"m 8.5,16.8 c 4.026587,1.955771 5.932617,1.2 8.1,-1 z\"></path>"
    ];

    private static readonly string[] EyeShapes =
    [
        "<path d=\"M 6.7,10.5 A 1.8,1.8 0 1 1 10.3,10.5 A 1.8,1.8 0 1 1 6.7,10.5 Z M 8,10.5 A 0.5,0.5 0 1 1 9,10.5 A 0.5,0.5 0 1 1 8,10.5 Z M 13.7,10.5 A 1.8,1.8 0 1 1 17.3,10.5 A 1.8,1.8 0 1 1 13.7,10.5 Z M 15,10.5 A 0.5,0.5 0 1 1 16,10.5 A 0.5,0.5 0 1 1 15,10.5 Z\"></path>",
        "<path d=\"M 6.5,10.5 Q 8.5,8.7 10.5,10.5 Q 8.5,12.3 6.5,10.5 Z M 8.1,10.5 A 0.4,0.4 0 1 1 8.9,10.5 A 0.4,0.4 0 1 1 8.1,10.5 Z M 13.5,10.5 Q 15.5,8.7 17.5,10.5 Q 15.5,12.3 13.5,10.5 Z M 15.1,10.5 A 0.4,0.4 0 1 1 15.9,10.5 A 0.4,0.4 0 1 1 15.1,10.5 Z\"></path>",
        "<path d=\"M 6.5,10.7 Q 8.5,11.8 10.5,10.7 M 6.7,10 H 10.3 M 13.5,10.7 Q 15.5,11.8 17.5,10.7 M 13.7,10 H 17.3\"></path>",
        "<path d=\"M 6.4,10.5 A 2.1,2.1 0 1 1 10.6,10.5 A 2.1,2.1 0 1 1 6.4,10.5 Z M 8.1,10.5 A 0.4,0.4 0 1 1 8.9,10.5 A 0.4,0.4 0 1 1 8.1,10.5 Z M 13.4,10.5 A 2.1,2.1 0 1 1 17.6,10.5 A 2.1,2.1 0 1 1 13.4,10.5 Z M 15.1,10.5 A 0.4,0.4 0 1 1 15.9,10.5 A 0.4,0.4 0 1 1 15.1,10.5 Z\"></path>",
        "<path d=\"M 6.5,11 Q 8.5,8.8 10.5,11 M 13.5,11 Q 15.5,8.8 17.5,11\"></path>",
        "<path d=\"M 6.5,10.6 Q 8.5,9.2 10.5,10.6 Q 8.5,12 6.5,10.6 Z M 10.3,9.9 L 11,9.3 M 10.6,10.3 L 11.4,10 M 10.8,10.8 L 11.6,10.9 M 13.5,10.6 Q 15.5,9.2 17.5,10.6 Q 15.5,12 13.5,10.6 Z M 13.7,9.9 L 13,9.3 M 13.4,10.3 L 12.6,10 M 13.2,10.8 L 12.4,10.9\"></path>",
        "<path d=\"M 6.7,10.5 A 1.8,1.8 0 1 1 10.3,10.5 A 1.8,1.8 0 1 1 6.7,10.5 Z M 8,10.5 A 0.5,0.5 0 1 1 9,10.5 A 0.5,0.5 0 1 1 8,10.5 Z M 13.5,11 Q 15.5,8.8 17.5,11\"></path>",
        "<path d=\"M 6,10.5 A 2.5,2.5 0 1 1 11,10.5 A 2.5,2.5 0 1 1 6,10.5 Z M 13,10.5 A 2.5,2.5 0 1 1 18,10.5 A 2.5,2.5 0 1 1 13,10.5 Z M 11,10.5 H 13\"></path>"
    ];

    private static readonly (string Name, string[] Variants)[] PartsInOrder =
    [
        ("body", BodyShapes),
        ("mouth", MouthShapes),
        ("eyes", EyeShapes)
    ];

    private static readonly string[] Palette =
    [
        "oklch(70.5% 0.213 47.604)",
        "oklch(79.5% 0.184 86.047)",
        "oklch(76.8% 0.233 130.85)",
        "oklch(71.5% 0.143 215.221)",
        "oklch(70.4% 0.14 182.503)",
        "oklch(70.4% 0.04 256.788)"
    ];

    private static readonly int[][] SecondaryPalette =
    [
        [0, 2, 3, 5],
        [2, 3, 4, 5],
        [1, 4, 5],
        [1, 2, 4, 5],
        [1, 5],
        [1, 2, 3, 4]
    ];

    public static string TextToAvatarId(string? text, int length = 12)
        => AvatarGeneratorEngine.TextToAvatarId(text, DefaultSeedText, length);

    public static string RenderSvg(HomoAvatarOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        return AvatarGeneratorEngine.RenderSvg(
            AvatarKey,
            PartsInOrder,
            Palette,
            SecondaryPalette,
            DefaultSeedText,
            options.Id,
            options.Text,
            options.Choices,
            options.Variant,
            options.Dark,
            options.LineWidth,
            options.Background,
            options.Title,
            options.GradientId);
    }
}
