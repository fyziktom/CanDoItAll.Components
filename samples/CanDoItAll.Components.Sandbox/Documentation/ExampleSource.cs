using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace CanDoItAll.Components.Sandbox.Documentation;

/// <summary>
/// Serves the original <c>.razor</c> markup of the example components as embedded resources.
/// </summary>
/// <remarks>
/// Razor compiles to <c>BuildRenderTree</c> calls, so the authored markup cannot be recovered by
/// reflection. The example components are therefore embedded as resources by the project file, which
/// makes the snippet shown in the catalog the exact bytes that rendered the demo above it. There is
/// no second copy to keep in sync.
/// </remarks>
public static class ExampleSource
{
    private const string ResourcePrefix = "examples/";

    private static readonly ConcurrentDictionary<string, string> Cache = new(StringComparer.Ordinal);

    private static readonly string[] LeadingDirectives =
    [
        "@namespace",
        "@using",
        "@inherits",
        "@implements",
        "@inject",
        "@layout",
        "@page",
        "@attribute",
        "@typeparam",
        "@rendermode"
    ];

    /// <summary>
    /// Maps a normalised resource name onto the manifest name emitted by the build.
    /// </summary>
    /// <remarks>
    /// The project file builds logical names from MSBuild's <c>%(RecursiveDir)</c>, which uses a
    /// backslash on Windows and a forward slash elsewhere. Indexing on a normalised key keeps the
    /// <c>SourceId</c> written in markup identical on every platform.
    /// </remarks>
    private static readonly IReadOnlyDictionary<string, string> ResourceIndex = typeof(ExampleSource).Assembly
        .GetManifestResourceNames()
        .Where(static name => Normalise(name).StartsWith(ResourcePrefix, StringComparison.OrdinalIgnoreCase))
        .ToDictionary(Normalise, static name => name, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Lists every embedded example resource name, for diagnostics and coverage checks.
    /// </summary>
    public static IReadOnlyList<string> AvailableSources { get; } = ResourceIndex.Keys
        .OrderBy(static name => name, StringComparer.Ordinal)
        .ToArray();

    private static string Normalise(string name) => name.Replace('\\', '/');

    /// <summary>
    /// Returns the presentable markup for an example, or <c>null</c> when the resource is missing.
    /// </summary>
    /// <param name="sourceId">
    /// Logical resource name, for example <c>examples/Actions/PrimaryHierarchy.razor</c>.
    /// </param>
    public static string? TryGet(string sourceId)
    {
        if (string.IsNullOrWhiteSpace(sourceId))
        {
            return null;
        }

        if (Cache.TryGetValue(sourceId, out var cached))
        {
            return cached;
        }

        var raw = ReadResource(sourceId);
        if (raw is null)
        {
            return null;
        }

        var presentable = Present(raw);
        Cache[sourceId] = presentable;
        return presentable;
    }

    private static string? ReadResource(string sourceId)
    {
        if (!ResourceIndex.TryGetValue(Normalise(sourceId), out var manifestName))
        {
            return null;
        }

        using var stream = typeof(ExampleSource).Assembly.GetManifestResourceStream(manifestName);
        if (stream is null)
        {
            return null;
        }

        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// Removes the compiler directives and the trailing <c>@code</c> block, then dedents, so the
    /// snippet reads as markup somebody can paste into their own page.
    /// </summary>
    internal static string Present(string source)
    {
        var lines = source.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n').ToList();

        RemoveLeadingDirectives(lines);
        RemoveCodeBlock(lines);
        Trim(lines);

        return Dedent(lines);
    }

    private static void RemoveLeadingDirectives(List<string> lines)
    {
        while (lines.Count > 0)
        {
            var candidate = lines[0].TrimStart();

            var isDirective = LeadingDirectives.Any(directive =>
                candidate.StartsWith(directive, StringComparison.Ordinal));

            if (!isDirective && candidate.Length > 0)
            {
                return;
            }

            lines.RemoveAt(0);
        }
    }

    private static void RemoveCodeBlock(List<string> lines)
    {
        var index = lines.FindLastIndex(static line =>
            line.TrimStart().StartsWith("@code", StringComparison.Ordinal));

        if (index >= 0)
        {
            lines.RemoveRange(index, lines.Count - index);
        }
    }

    private static void Trim(List<string> lines)
    {
        while (lines.Count > 0 && lines[0].Trim().Length == 0)
        {
            lines.RemoveAt(0);
        }

        while (lines.Count > 0 && lines[^1].Trim().Length == 0)
        {
            lines.RemoveAt(lines.Count - 1);
        }
    }

    private static string Dedent(List<string> lines)
    {
        if (lines.Count == 0)
        {
            return string.Empty;
        }

        var indent = lines
            .Where(static line => line.Trim().Length > 0)
            .Select(static line => line.Length - line.TrimStart().Length)
            .DefaultIfEmpty(0)
            .Min();

        if (indent == 0)
        {
            return string.Join('\n', lines);
        }

        var dedented = lines.Select(line => line.Length >= indent ? line[indent..] : line.TrimStart());
        return string.Join('\n', dedented);
    }
}
