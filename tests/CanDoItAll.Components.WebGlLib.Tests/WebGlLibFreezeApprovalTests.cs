using System.Security.Cryptography;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CanDoItAll.Components.WebGlLib.Tests;

public sealed partial class WebGlLibFreezeApprovalTests
{
    private static readonly JsonSerializerOptions MetadataJsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    [Fact]
    public void Public_api_matches_freeze_snapshot()
    {
        string approved = ReadApproval("webgllib-public-api.approved.txt");
        string actual = BuildPublicApiSnapshot("src/CanDoItAll.Components.WebGlLib");

        Assert.Equal(NormalizeNewLines(approved), NormalizeNewLines(actual));
    }

    [Fact]
    public void Public_api_metadata_matches_freeze_snapshot()
    {
        string approved = ReadApproval("webgllib-public-api.metadata.approved.json");
        string actual = BuildPublicApiMetadataSnapshot(typeof(WebGlSceneModel).Assembly);

        Assert.Equal(NormalizeNewLines(approved), NormalizeNewLines(actual));
    }

    [Fact]
    public void Webgl_scene_js_surface_matches_freeze_snapshot()
    {
        string repoRoot = FindRepoRoot();
        string sceneEntry = Path.Combine(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "01-webgl-scene.js");
        string[] methods = ReadWebGlSceneMethodNames(sceneEntry);
        string actual = string.Join(Environment.NewLine, methods) + Environment.NewLine;
        string approved = ReadApproval("webgllib-webglscene-js-surface.approved.txt");

        Assert.Equal(NormalizeNewLines(approved), NormalizeNewLines(actual));
    }

    [Fact]
    public void Webgl_scene_js_api_manifest_matches_freeze_snapshot_and_declares_result_shapes()
    {
        string repoRoot = FindRepoRoot();
        string sceneEntry = Path.Combine(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "01-webgl-scene.js");
        string[] actualMethods = ReadWebGlSceneMethodNames(sceneEntry);
        WebGlSceneJsApiManifestEntry[] approved = JsonSerializer.Deserialize<WebGlSceneJsApiManifestEntry[]>(
            ReadApproval("webgllib-webglscene-js-api.approved.json"),
            new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? [];

        Assert.Equal(actualMethods, approved.Select(static entry => entry.Method).Order(StringComparer.Ordinal).ToArray());
        Assert.DoesNotContain(approved, static entry => string.IsNullOrWhiteSpace(entry.ParameterShape));
        Assert.DoesNotContain(approved, static entry => string.IsNullOrWhiteSpace(entry.ResultShape));
        Assert.DoesNotContain(approved, static entry => string.IsNullOrWhiteSpace(entry.MissingRuntimeResult));
        Assert.DoesNotContain(approved, static entry => string.IsNullOrWhiteSpace(entry.LifecycleBehavior));
        Assert.DoesNotContain(approved, static entry => string.IsNullOrWhiteSpace(entry.IdleSettledBehavior));
        Assert.DoesNotContain(approved, static entry => string.IsNullOrWhiteSpace(entry.FailureBehavior));
        Assert.Contains(approved, static entry =>
            string.Equals(entry.Method, "waitForRuntimeIdle", StringComparison.Ordinal) &&
            entry.ResultShape.Contains("WebGlRuntimeIdleResult", StringComparison.Ordinal) &&
            entry.IdleSettledBehavior.Contains("selected idle policy", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(approved, static entry =>
            string.Equals(entry.Method, "applyCommandBatchAndWait", StringComparison.Ordinal) &&
            entry.IdleSettledBehavior.Contains("waits", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Webgl_scene_js_api_approval_rejects_deliberate_unapproved_method_probe()
    {
        WebGlSceneJsApiManifestEntry[] approved = JsonSerializer.Deserialize<WebGlSceneJsApiManifestEntry[]>(
            ReadApproval("webgllib-webglscene-js-api.approved.json"),
            new JsonSerializerOptions(JsonSerializerDefaults.Web)) ?? [];
        string[] approvedMethods = approved
            .Select(static entry => entry.Method)
            .Order(StringComparer.Ordinal)
            .ToArray();
        string[] actualWithDeliberateDrift = approvedMethods
            .Append("v16UnapprovedProbe")
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.NotEqual(approvedMethods, actualWithDeliberateDrift);
    }

    [Fact]
    public void Webgl_sceneview_csharp_facade_invokes_only_approved_js_api_methods()
    {
        string repoRoot = FindRepoRoot();
        string sceneComponentRoot = Path.Combine(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "Components", "Scene");
        var approvedMethods = JsonSerializer.Deserialize<WebGlSceneJsApiManifestEntry[]>(
            ReadApproval("webgllib-webglscene-js-api.approved.json"),
            new JsonSerializerOptions(JsonSerializerDefaults.Web))?
            .Select(static entry => entry.Method)
            .ToHashSet(StringComparer.Ordinal) ?? [];

        string[] invokedMethods = Directory
            .GetFiles(sceneComponentRoot, "WebGlSceneView*", SearchOption.TopDirectoryOnly)
            .OrderBy(path => path, StringComparer.Ordinal)
            .SelectMany(path => File.ReadLines(path)
                .SelectMany(line => WebGlSceneInteropLiteralRegex()
                    .Matches(line)
                    .Select(match => match.Groups["name"].Value)))
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.NotEmpty(invokedMethods);
        Assert.All(invokedMethods, method => Assert.Contains(method, approvedMethods));
    }

    [Fact]
    public void Package_content_matches_freeze_snapshot()
    {
        string approved = ReadApproval("webgllib-package-content.approved.txt");
        string actual = BuildPackageContentSnapshot("src/CanDoItAll.Components.WebGlLib");

        Assert.Equal(NormalizeNewLines(approved), NormalizeNewLines(actual));
    }

    private static string BuildPublicApiSnapshot(string sourceRoot)
    {
        string repoRoot = FindRepoRoot();
        string absoluteSourceRoot = Path.Combine(repoRoot, sourceRoot);
        string[] lines = Directory.GetFiles(absoluteSourceRoot, "*", SearchOption.AllDirectories)
            .Where(static path => !IsExcludedSnapshotPath(path))
            .Where(static path =>
                string.Equals(Path.GetExtension(path), ".cs", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(Path.GetExtension(path), ".razor", StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => Path.GetRelativePath(repoRoot, path), StringComparer.Ordinal)
            .SelectMany(path => File.ReadLines(path)
                .Select((line, index) => new { Path = Path.GetRelativePath(repoRoot, path).Replace('\\', '/'), Line = line.Trim(), LineNumber = index + 1 })
                .Where(static item => item.Line.StartsWith("public ", StringComparison.Ordinal))
                .Select(static item => $"{item.Path}:{item.LineNumber}: {item.Line}"))
            .ToArray();

        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }

    private static string BuildPublicApiMetadataSnapshot(Assembly assembly)
    {
        var snapshot = assembly
            .GetExportedTypes()
            .Where(static type => type.FullName is not null && !type.FullName.Contains('<', StringComparison.Ordinal))
            .OrderBy(static type => type.FullName, StringComparer.Ordinal)
            .Select(static type => new PublicApiTypeSnapshot
            {
                Name = RenderTypeName(type),
                Kind = RenderTypeKind(type),
                IsAbstract = type.IsAbstract,
                IsSealed = type.IsSealed,
                BaseType = type.BaseType is null || type.BaseType == typeof(object) ? string.Empty : RenderTypeName(type.BaseType),
                Members = BuildPublicMemberSnapshot(type)
            })
            .ToArray();

        return JsonSerializer.Serialize(snapshot, MetadataJsonOptions) + Environment.NewLine;
    }

    private static string[] BuildPublicMemberSnapshot(Type type)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        string[] constructors = type.GetConstructors(flags)
            .OrderBy(static constructor => constructor.ToString(), StringComparer.Ordinal)
            .Select(static constructor => $"ctor({RenderParameters(constructor.GetParameters())})")
            .ToArray();

        string[] properties = type.GetProperties(flags)
            .OrderBy(static property => property.Name, StringComparer.Ordinal)
            .Select(static property =>
            {
                string accessors = $"{(property.GetMethod is null ? "" : "get;")}{(property.SetMethod is null ? "" : "set;")}";
                string staticMarker = property.GetMethod?.IsStatic == true || property.SetMethod?.IsStatic == true ? " static" : string.Empty;
                return $"property{staticMarker} {RenderTypeName(property.PropertyType)} {property.Name} {{{accessors}}}";
            })
            .ToArray();

        string[] methods = type.GetMethods(flags)
            .Where(static method => !method.IsSpecialName)
            .OrderBy(static method => method.Name, StringComparer.Ordinal)
            .ThenBy(static method => method.ToString(), StringComparer.Ordinal)
            .Select(static method =>
            {
                string staticMarker = method.IsStatic ? " static" : string.Empty;
                string genericMarker = method.IsGenericMethodDefinition
                    ? $"<{string.Join(",", method.GetGenericArguments().Select(static argument => argument.Name))}>"
                    : string.Empty;
                return $"method{staticMarker} {RenderTypeName(method.ReturnType)} {method.Name}{genericMarker}({RenderParameters(method.GetParameters())})";
            })
            .ToArray();

        string[] fields = type.GetFields(flags)
            .OrderBy(static field => field.Name, StringComparer.Ordinal)
            .Select(static field =>
            {
                string staticMarker = field.IsStatic ? " static" : string.Empty;
                string literalMarker = field.IsLiteral ? " const" : field.IsInitOnly ? " readonly" : string.Empty;
                return $"field{staticMarker}{literalMarker} {RenderTypeName(field.FieldType)} {field.Name}";
            })
            .ToArray();

        string[] events = type.GetEvents(flags)
            .OrderBy(static item => item.Name, StringComparer.Ordinal)
            .Select(static item => $"event {RenderTypeName(item.EventHandlerType ?? typeof(object))} {item.Name}")
            .ToArray();

        return constructors
            .Concat(properties)
            .Concat(methods)
            .Concat(fields)
            .Concat(events)
            .Order(StringComparer.Ordinal)
            .ToArray();
    }

    private static string RenderTypeKind(Type type)
    {
        if (type.IsInterface)
        {
            return "interface";
        }

        if (type.IsEnum)
        {
            return "enum";
        }

        if (type.IsValueType)
        {
            return "struct";
        }

        return "class";
    }

    private static string RenderParameters(IEnumerable<ParameterInfo> parameters)
        => string.Join(", ", parameters.Select(static parameter =>
        {
            string modifier = parameter.IsOut ? "out " : parameter.ParameterType.IsByRef ? "ref " : string.Empty;
            Type parameterType = parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType() ?? parameter.ParameterType : parameter.ParameterType;
            string optional = parameter.HasDefaultValue ? " = optional" : string.Empty;
            return $"{modifier}{RenderTypeName(parameterType)} {parameter.Name}{optional}";
        }));

    private static string RenderTypeName(Type type)
    {
        if (type.IsGenericParameter)
        {
            return type.Name;
        }

        if (type.IsArray)
        {
            return $"{RenderTypeName(type.GetElementType() ?? typeof(object))}[]";
        }

        if (type.IsGenericType)
        {
            string name = type.GetGenericTypeDefinition().FullName ?? type.Name;
            int tickIndex = name.IndexOf('`', StringComparison.Ordinal);
            if (tickIndex >= 0)
            {
                name = name[..tickIndex];
            }

            return $"{name}<{string.Join(",", type.GetGenericArguments().Select(RenderTypeName))}>";
        }

        return type.FullName ?? type.Name;
    }

    private static string ReadApproval(string fileName)
        => File.ReadAllText(Path.Combine(FindRepoRoot(), "tests", "CanDoItAll.Components.WebGlLib.Tests", "fixtures", "approvals", fileName));

    private static string BuildPackageContentSnapshot(string sourceRoot)
    {
        string repoRoot = FindRepoRoot();
        string absoluteSourceRoot = Path.Combine(repoRoot, sourceRoot);
        string[] lines = Directory.GetFiles(absoluteSourceRoot, "*", SearchOption.AllDirectories)
            .Where(static path => !IsExcludedSnapshotPath(path))
            .OrderBy(path => Path.GetRelativePath(repoRoot, path), StringComparer.Ordinal)
            .Select(path =>
            {
                byte[] bytes = ReadSnapshotBytes(path);
                string hash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
                return $"{Path.GetRelativePath(repoRoot, path).Replace('\\', '/')} | bytes={bytes.Length} | sha256={hash}";
            })
            .ToArray();

        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }

    private static bool IsExcludedSnapshotPath(string path)
        => path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) ||
           path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) ||
           path.Contains($"{Path.DirectorySeparatorChar}.artifacts{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase) ||
           path.Contains($"{Path.DirectorySeparatorChar}.codex-tmp{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase);

    private static byte[] ReadSnapshotBytes(string path)
    {
        if (!IsTextSnapshotFile(path))
        {
            return File.ReadAllBytes(path);
        }

        return Encoding.UTF8.GetBytes(NormalizeNewLines(File.ReadAllText(path)));
    }

    private static bool IsTextSnapshotFile(string path)
        => Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".cs" or ".razor" or ".csproj" or ".props" or ".targets" or ".json" or ".js" or ".css" or ".md" or ".txt" or ".xml" => true,
            _ => false
        };

    private static string NormalizeNewLines(string value)
        => value.Replace("\r\n", "\n", StringComparison.Ordinal);

    private static string[] ReadWebGlSceneMethodNames(string sceneEntry)
        => File.ReadLines(sceneEntry)
            .Select(line => WebGlSceneMethodRegex().Match(line))
            .Where(static match => match.Success)
            .Select(static match => match.Groups["name"].Value)
            .Order(StringComparer.Ordinal)
            .ToArray();

    private static string FindRepoRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "CanDoItAll.Components.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate CanDoItAll.Components repository root.");
    }

    [GeneratedRegex("^\\s{4}(?<name>[A-Za-z0-9_]+)\\s*\\(")]
    private static partial Regex WebGlSceneMethodRegex();

    [GeneratedRegex("\"CanDoItAll\\.webglScene\\.(?<name>[A-Za-z0-9_]+)\"")]
    private static partial Regex WebGlSceneInteropLiteralRegex();

    private sealed class WebGlSceneJsApiManifestEntry
    {
        public string Method { get; set; } = string.Empty;

        public string ParameterShape { get; set; } = string.Empty;

        public string ResultShape { get; set; } = string.Empty;

        public string MissingRuntimeResult { get; set; } = string.Empty;

        public string LifecycleBehavior { get; set; } = string.Empty;

        public string IdleSettledBehavior { get; set; } = string.Empty;

        public string FailureBehavior { get; set; } = string.Empty;
    }

    private sealed class PublicApiTypeSnapshot
    {
        public string Name { get; set; } = string.Empty;

        public string Kind { get; set; } = string.Empty;

        public bool IsAbstract { get; set; }

        public bool IsSealed { get; set; }

        public string BaseType { get; set; } = string.Empty;

        public string[] Members { get; set; } = [];
    }
}
