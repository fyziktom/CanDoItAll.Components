using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.Sandbox;

public static class SandboxQueryLinks
{
    public static string WithFrame(NavigationManager navigationManager, SandboxFramePreset frame)
        => navigationManager.GetUriWithQueryParameter(
            "frame",
            frame == SandboxFramePreset.LiveViewport ? null : frame.ToSlug());

    public static string WithDark(NavigationManager navigationManager, bool isDark)
        => navigationManager.GetUriWithQueryParameter("dark", isDark ? "true" : null);

    /// <summary>Builds a link to a different route while carrying forward the current frame/dark state.</summary>
    public static string BuildCrossPageLink(
        NavigationManager navigationManager,
        string path,
        SandboxFramePreset? frame,
        bool isDark)
    {
        var target = path == "/" ? "./" : path.TrimStart('/');
        var parameters = new Dictionary<string, object?>();

        if (frame is { } frameValue && frameValue != SandboxFramePreset.LiveViewport)
        {
            parameters["frame"] = frameValue.ToSlug();
        }

        if (isDark)
        {
            parameters["dark"] = "true";
        }

        if (parameters.Count == 0)
        {
            return target;
        }

        var query = string.Join(
            "&",
            parameters.Select(static parameter =>
                $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value?.ToString() ?? string.Empty)}"));

        return $"{target}?{query}";
    }

    /// <summary>Preserves the current query string verbatim and only changes the #fragment.</summary>
    public static string WithFragment(NavigationManager navigationManager, string fragment)
    {
        var uri = navigationManager.Uri;
        var hashIndex = uri.IndexOf('#');
        var withoutFragment = hashIndex >= 0 ? uri[..hashIndex] : uri;
        return $"{withoutFragment}#{fragment}";
    }
}
