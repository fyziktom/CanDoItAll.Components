using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.Sandbox;

public static class SandboxQueryLinks
{
    public static bool UseHashRouting { get; set; }

    public static string? CurrentHashRoute { get; set; }

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
        bool isDark,
        string? fragment = null)
    {
        var target = path == "/" ? "./" : path.TrimStart('/');
        var parameters = new Dictionary<string, object?>();

        if (UseHashRouting)
        {
            var hashRoute = path.TrimStart('/');
            target = path == "/" ? "#" : $"#{hashRoute}";
            if (!string.IsNullOrWhiteSpace(fragment))
            {
                target += $"?{Uri.EscapeDataString(fragment)}";
            }
        }

        if (!UseHashRouting && frame is { } frameValue && frameValue != SandboxFramePreset.LiveViewport)
        {
            parameters["frame"] = frameValue.ToSlug();
        }

        if (!UseHashRouting && isDark)
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

        var destination = UseHashRouting
            ? $"./?{query}{target}"
            : $"{target}?{query}";
        return string.IsNullOrWhiteSpace(fragment) || UseHashRouting
            ? destination
            : $"{destination}#{fragment}";
    }

    /// <summary>Preserves the current query string verbatim and only changes the #fragment.</summary>
    public static string WithFragment(NavigationManager navigationManager, string fragment)
    {
        if (UseHashRouting)
        {
            var route = GetCurrentRoute(navigationManager);
            return string.IsNullOrWhiteSpace(route)
                ? "#"
                : $"#{route}?{Uri.EscapeDataString(fragment)}";
        }

        var uri = navigationManager.Uri;
        var hashIndex = uri.IndexOf('#');
        var withoutFragment = hashIndex >= 0 ? uri[..hashIndex] : uri;
        return $"{withoutFragment}#{fragment}";
    }

    public static string GetCurrentRoute(NavigationManager navigationManager)
    {
        if (!UseHashRouting)
        {
            return navigationManager.ToBaseRelativePath(navigationManager.Uri);
        }

        if (CurrentHashRoute is not null)
        {
            return CurrentHashRoute;
        }

        var route = navigationManager.ToAbsoluteUri(navigationManager.Uri).Fragment
            .TrimStart('#')
            .Split('?', 2)[0];
        return string.IsNullOrWhiteSpace(route) ? string.Empty : route;
    }
}
