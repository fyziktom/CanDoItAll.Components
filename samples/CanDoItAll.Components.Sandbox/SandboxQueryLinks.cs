using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.Sandbox;

public static class SandboxQueryLinks
{
    public static bool UseHashRouting { get; set; }

    private static HashRouteState? CurrentHashState { get; set; }

    public static string WithFrame(NavigationManager navigationManager, SandboxFramePreset frame)
    {
        if (!UseHashRouting)
        {
            var currentUri = navigationManager.Uri;
            var fragmentIndex = currentUri.IndexOf('#');
            var fragment = fragmentIndex >= 0 ? currentUri[fragmentIndex..] : string.Empty;
            var uriWithoutFragment = fragmentIndex >= 0 ? currentUri[..fragmentIndex] : currentUri;
            var builder = new UriBuilder(uriWithoutFragment);
            var parameters = builder.Query.TrimStart('?')
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Where(static parameter => !string.Equals(
                    Uri.UnescapeDataString(parameter.Split('=', 2)[0]),
                    "frame",
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (frame != SandboxFramePreset.LiveViewport)
            {
                parameters.Add($"frame={Uri.EscapeDataString(frame.ToSlug())}");
            }

            builder.Query = string.Join("&", parameters);
            return $"{builder.Uri.AbsoluteUri}{fragment}";
        }

        var state = GetHashState(navigationManager);
        return BuildHashLink(state.Route, state.Anchor, frame, state.IsDark, state.ExtraTokens);
    }

    public static string WithDark(NavigationManager navigationManager, bool isDark)
    {
        if (!UseHashRouting)
        {
            // The browser owns an SSR fragment, so the framework helper deliberately preserves it.
            return navigationManager.GetUriWithQueryParameter("dark", isDark ? "true" : null);
        }

        var state = GetHashState(navigationManager);
        return BuildHashLink(state.Route, state.Anchor, state.Frame, isDark, state.ExtraTokens);
    }

    public static bool IsDark(NavigationManager navigationManager)
    {
        if (UseHashRouting)
        {
            return GetHashState(navigationManager).IsDark;
        }

        return navigationManager.ToAbsoluteUri(navigationManager.Uri).Query.TrimStart('?')
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(static pair => pair.Split('=', 2))
            .Any(static pair => pair.Length == 2
                && string.Equals(Uri.UnescapeDataString(pair[0]), "dark", StringComparison.OrdinalIgnoreCase)
                && string.Equals(Uri.UnescapeDataString(pair[1]), "true", StringComparison.OrdinalIgnoreCase));
    }

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
            return BuildHashLink(path == "/" ? string.Empty : hashRoute, fragment, frame, isDark);
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
            return string.IsNullOrWhiteSpace(fragment)
                ? target
                : $"{target}#{fragment}";
        }

        var query = string.Join(
            "&",
            parameters.Select(static parameter =>
                $"{Uri.EscapeDataString(parameter.Key)}={Uri.EscapeDataString(parameter.Value?.ToString() ?? string.Empty)}"));

        var destination = $"{target}?{query}";
        return string.IsNullOrWhiteSpace(fragment) ? destination : $"{destination}#{fragment}";
    }

    /// <summary>Preserves the current query string verbatim and only changes the #fragment.</summary>
    public static string WithFragment(NavigationManager navigationManager, string fragment)
    {
        if (UseHashRouting)
        {
            var state = GetHashState(navigationManager);
            return BuildHashLink(state.Route, fragment, state.Frame, state.IsDark, state.ExtraTokens);
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
            return navigationManager.ToBaseRelativePath(navigationManager.Uri)
                .Split('#', 2)[0];
        }

        if (CurrentHashState is not null)
        {
            return CurrentHashState.Route;
        }

        var route = navigationManager.ToAbsoluteUri(navigationManager.Uri).Fragment
            .TrimStart('#')
            .Split('?', 2)[0];
        return string.IsNullOrWhiteSpace(route) ? string.Empty : route;
    }

    public static SandboxFramePreset GetHashFrame(NavigationManager navigationManager)
    {
        if (!UseHashRouting)
        {
            return SandboxFramePreset.LiveViewport;
        }

        return CurrentHashState is not null
            ? CurrentHashState.Frame
            : ParseHash(navigationManager.ToAbsoluteUri(navigationManager.Uri).Fragment.TrimStart('#')).Frame;
    }

    public static SandboxFramePreset GetCurrentFrame(NavigationManager navigationManager)
    {
        if (UseHashRouting)
        {
            return GetHashFrame(navigationManager);
        }

        var query = navigationManager.ToAbsoluteUri(navigationManager.Uri).Query.TrimStart('?');
        var frame = query.Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(static parameter => parameter.Split('=', 2))
            .FirstOrDefault(static pair => pair.Length == 2
                && string.Equals(Uri.UnescapeDataString(pair[0]), "frame", StringComparison.OrdinalIgnoreCase));
        return frame is { Length: 2 }
            ? SandboxFramePresetExtensions.Parse(Uri.UnescapeDataString(frame[1]))
            : SandboxFramePreset.LiveViewport;
    }

    /// <summary>Stores the current client-side hash state before Blazor's navigation URI is updated.</summary>
    public static string? UpdateHashState(string hash)
    {
        CurrentHashState = ParseHash(hash.TrimStart('#'));
        return CurrentHashState.Anchor;
    }

    public static string BuildRawTestLink(bool isDark)
    {
        if (UseHashRouting)
        {
            return BuildHashLink("test", "raw", null, isDark);
        }

        return isDark ? "test?raw&dark=true" : "test?raw";
    }

    private static string BuildHashLink(
        string route,
        string? anchor,
        SandboxFramePreset? frame,
        bool isDark,
        IReadOnlyList<string>? extraTokens = null)
    {
        var target = string.IsNullOrWhiteSpace(route) ? "#" : $"#{route.TrimStart('/')}";
        var tokens = new List<string>();
        if (!string.IsNullOrWhiteSpace(anchor))
        {
            tokens.Add(Uri.EscapeDataString(anchor));
        }

        if (extraTokens is not null)
        {
            tokens.AddRange(extraTokens);
        }

        if (frame is { } frameValue && frameValue != SandboxFramePreset.LiveViewport)
        {
            tokens.Add($"frame={Uri.EscapeDataString(frameValue.ToSlug())}");
        }

        if (isDark)
        {
            tokens.Add("dark=true");
        }

        return tokens.Count == 0 ? target : $"{target}?{string.Join("&", tokens)}";
    }

    private static HashRouteState GetHashState(NavigationManager navigationManager)
    {
        if (CurrentHashState is not null)
        {
            return CurrentHashState;
        }

        return ParseHash(navigationManager);
    }

    private static HashRouteState ParseHash(NavigationManager navigationManager)
        => ParseHash(navigationManager.ToAbsoluteUri(navigationManager.Uri).Fragment.TrimStart('#'));

    private static HashRouteState ParseHash(string rawHash)
    {
        var parts = rawHash.Split('?', 2);
        var route = Uri.UnescapeDataString(parts[0]).Trim('/');
        string? anchor = null;
        var frame = SandboxFramePreset.LiveViewport;
        var isDark = false;
        var extraTokens = new List<string>();

        if (parts.Length == 2)
        {
            foreach (var token in parts[1].Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var pair = token.Split('=', 2);
                var key = Uri.UnescapeDataString(pair[0]);
                var value = pair.Length == 2 ? Uri.UnescapeDataString(pair[1]) : key;
                if (string.Equals(key, "frame", StringComparison.OrdinalIgnoreCase))
                {
                    frame = SandboxFramePresetExtensions.Parse(value);
                }
                else if (string.Equals(key, "dark", StringComparison.OrdinalIgnoreCase))
                {
                    isDark = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
                }
                else if (anchor is null)
                {
                    anchor = string.Equals(key, "anchor", StringComparison.OrdinalIgnoreCase) && pair.Length == 2
                        ? value
                        : Uri.UnescapeDataString(token);
                }
                else
                {
                    extraTokens.Add(token);
                }
            }
        }

        return new HashRouteState(route, anchor, frame, isDark, extraTokens);
    }

    private sealed record HashRouteState(
        string Route,
        string? Anchor,
        SandboxFramePreset Frame,
        bool IsDark,
        IReadOnlyList<string> ExtraTokens);
}
