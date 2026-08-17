using System.Collections.Concurrent;

namespace CanDoItAll.Components.Sandbox.Documentation;

/// <summary>
/// Resolves the repo-relative source path of a library component from the sandbox group it's
/// catalogued under and its CLR <see cref="Type"/>, so <c>ExampleBlock</c> call sites don't have
/// to hand-type a path per example.
/// </summary>
/// <remarks>
/// BaseLib lays components out as <c>Components/{Group}/{Component}.razor</c>, and a handful of
/// other libraries (Charts, Gantt, Mermaid, QRCode) lay them out flat as
/// <c>Components/{Component}.razor</c>. Everything else (CanvasLib, OverlayLib, BaseLib's nested
/// <c>Compatibility/</c> folder, and so on) doesn't follow either convention and needs an entry
/// in <see cref="Overrides"/>.
/// </remarks>
public static class ComponentSourceCatalog
{
    private const string BaseLibAssemblyName = "CanDoItAll.Components.BaseLib";

    private static readonly HashSet<string> FlatAssemblies = new(StringComparer.Ordinal)
    {
        "CanDoItAll.Components.Charts",
        "CanDoItAll.Components.Gantt",
        "CanDoItAll.Components.Mermaid",
        "CanDoItAll.Components.QRCode"
    };

    /// <summary>
    /// Explicit path overrides for components that don't fit the BaseLib or flat-assembly
    /// convention. Add an entry the first time an <c>ExampleBlock</c> demoes one of these.
    /// </summary>
    private static readonly IReadOnlyDictionary<Type, string> Overrides = new Dictionary<Type, string>();

    private static readonly ConcurrentDictionary<(SandboxGroupKey? Group, Type Type), string?> Cache = new();

    public static string? Resolve(SandboxGroupKey? group, Type componentType)
        => Cache.GetOrAdd((group, componentType), static key => ResolveCore(key.Group, key.Type));

    private static string? ResolveCore(SandboxGroupKey? group, Type componentType)
    {
        if (Overrides.TryGetValue(componentType, out var overridden))
        {
            return overridden;
        }

        var assemblyName = componentType.Assembly.GetName().Name;
        if (assemblyName is null)
        {
            return null;
        }

        var typeName = componentType.Name;
        var backtick = typeName.IndexOf('`');
        if (backtick >= 0)
        {
            typeName = typeName[..backtick];
        }

        string candidate;
        if (assemblyName == BaseLibAssemblyName && group is { } key)
        {
            candidate = $"src/{assemblyName}/Components/{key}/{typeName}.razor";
        }
        else if (FlatAssemblies.Contains(assemblyName))
        {
            candidate = $"src/{assemblyName}/Components/{typeName}.razor";
        }
        else
        {
            return null;
        }

        return candidate;
    }
}
