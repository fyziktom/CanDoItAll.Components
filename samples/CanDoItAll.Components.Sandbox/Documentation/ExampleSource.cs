using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace CanDoItAll.Components.Sandbox.Documentation;

/// <summary>
/// Serves the original <c>.razor</c> source of example components as embedded resources.
/// </summary>
/// <remarks>
/// Razor compiles to <c>BuildRenderTree</c> calls, so the authored markup cannot be recovered by
/// reflection. The example components are therefore embedded as resources by the project file, which
/// makes the source shown in the catalog the exact bytes that rendered the demo above it, including
/// Razor directives and <c>@code</c> blocks. There is no second copy to keep in sync.
/// </remarks>
public static class ExampleSource
{
    private const string ResourcePrefix = "examples/";

    private static readonly ConcurrentDictionary<string, string> Cache = new(StringComparer.Ordinal);

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
    /// Returns the full source for an example, or <c>null</c> when the resource is missing.
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

        Cache[sourceId] = raw;
        return raw;
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
}
