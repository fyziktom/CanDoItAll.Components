using System.Reflection;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using CanDoItAll.Components.WebGlRunLib;

namespace CanDoItAll.Components.WebGlRunLib.Tests;

public sealed class WebGlRunFreezeApprovalTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };

    private static readonly JsonSerializerOptions MetadataJsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    [Fact]
    public void Public_api_matches_freeze_snapshot()
    {
        string approved = ReadApproval("webglrunlib-public-api.approved.txt");
        string actual = BuildPublicApiSnapshot("src/CanDoItAll.Components.WebGlRunLib");

        Assert.Equal(NormalizeNewLines(approved), NormalizeNewLines(actual));
    }

    [Fact]
    public void Public_api_metadata_matches_freeze_snapshot()
    {
        string approved = ReadApproval("webglrunlib-public-api.metadata.approved.json");
        string actual = BuildPublicApiMetadataSnapshot(typeof(WebGlRunDocument).Assembly);

        Assert.Equal(NormalizeNewLines(approved), NormalizeNewLines(actual));
    }

    [Fact]
    public void Action_kind_vocabulary_matches_freeze_snapshot()
    {
        string approved = ReadApproval("webglrunlib-action-kinds.approved.txt");
        string actual = string.Join(Environment.NewLine, WebGlRunActionKinds.All.Order(StringComparer.Ordinal)) + Environment.NewLine;

        Assert.Equal(NormalizeNewLines(approved), NormalizeNewLines(actual));
    }

    [Fact]
    public void Action_kind_approval_rejects_deliberate_unapproved_kind_probe()
    {
        string approved = NormalizeNewLines(ReadApproval("webglrunlib-action-kinds.approved.txt"));
        string actualWithDeliberateDrift = NormalizeNewLines(
            string.Join(Environment.NewLine, WebGlRunActionKinds.All.Append("v13-unapproved-action-kind").Order(StringComparer.Ordinal)) + Environment.NewLine);

        Assert.NotEqual(approved, actualWithDeliberateDrift);
    }

    [Fact]
    public void Domain_driver_manifest_schema_matches_freeze_snapshot()
    {
        string approved = ReadApproval("webglrunlib-domain-driver-manifest-schema.approved.txt");
        var shape = new
        {
            WebGlRunDomainMappingDriverManifest.CurrentSchemaVersion,
            Properties = typeof(WebGlRunDomainMappingDriverManifest)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .OrderBy(static property => property.Name, StringComparer.Ordinal)
                .Select(static property => $"{property.Name}:{RenderType(property.PropertyType)}")
                .ToArray(),
            PassThroughManifest = ((IWebGlRunDomainMappingDriver)WebGlRunPassThroughDomainMappingDriver.Instance).Manifest
        };
        string actual = JsonSerializer.Serialize(shape, JsonOptions) + Environment.NewLine;

        Assert.Equal(NormalizeNewLines(approved), NormalizeNewLines(actual));
    }

    [Fact]
    public void Domain_driver_validator_rejects_unsupported_generic_action_mapping_probe()
    {
        WebGlRunDomainMappingDriverValidationResult validation = ((IWebGlRunDomainMappingDriver)new UnsupportedGenericMappingDriver()).Validate();

        Assert.False(validation.IsValid);
        Assert.Contains(validation.Errors, error =>
            error.StartsWith("driver-action-maps-to-unsupported-generic-kind:probe-driver-action:", StringComparison.Ordinal));
    }

    [Fact]
    public void Package_content_matches_freeze_snapshot()
    {
        string approved = ReadApproval("webglrunlib-package-content.approved.txt");
        string actual = BuildPackageContentSnapshot("src/CanDoItAll.Components.WebGlRunLib");

        Assert.Equal(NormalizeNewLines(approved), NormalizeNewLines(actual));
    }

    private static string BuildPublicApiSnapshot(string sourceRoot)
    {
        string repoRoot = FindRepoRoot();
        string absoluteSourceRoot = Path.Combine(repoRoot, sourceRoot);
        string[] lines = Directory.GetFiles(absoluteSourceRoot, "*.cs", SearchOption.AllDirectories)
            .Where(static path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            .Where(static path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
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

    private static string ReadApproval(string fileName)
        => File.ReadAllText(Path.Combine(FindRepoRoot(), "tests", "CanDoItAll.Components.WebGlRunLib.Tests", "fixtures", "approvals", fileName));

    private static string BuildPackageContentSnapshot(string sourceRoot)
    {
        string repoRoot = FindRepoRoot();
        string absoluteSourceRoot = Path.Combine(repoRoot, sourceRoot);
        string[] lines = Directory.GetFiles(absoluteSourceRoot, "*", SearchOption.AllDirectories)
            .Where(static path => !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            .Where(static path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            .OrderBy(path => Path.GetRelativePath(repoRoot, path), StringComparer.Ordinal)
            .Select(path =>
            {
                var info = new FileInfo(path);
                string hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant();
                return $"{Path.GetRelativePath(repoRoot, path).Replace('\\', '/')} | bytes={info.Length} | sha256={hash}";
            })
            .ToArray();

        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }

    private static string NormalizeNewLines(string value)
        => value.Replace("\r\n", "\n", StringComparison.Ordinal);

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

    private static string RenderType(Type type)
    {
        if (!type.IsGenericType)
        {
            return type.Name;
        }

        string genericName = type.Name[..type.Name.IndexOf('`', StringComparison.Ordinal)];
        return $"{genericName}<{string.Join(",", type.GetGenericArguments().Select(RenderType))}>";
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

    private sealed class PublicApiTypeSnapshot
    {
        public string Name { get; set; } = string.Empty;

        public string Kind { get; set; } = string.Empty;

        public bool IsAbstract { get; set; }

        public bool IsSealed { get; set; }

        public string BaseType { get; set; } = string.Empty;

        public string[] Members { get; set; } = [];
    }

    private sealed class UnsupportedGenericMappingDriver : IWebGlRunDomainMappingDriver
    {
        public string DriverId => "unsupported-generic-mapping-probe";
        public string DriverVersion => "1.0.0";
        public string DisplayName => "Unsupported generic mapping probe";
        public WebGlRunGenericBoundaryOptions BoundaryOptions => WebGlRunGenericBoundaryOptions.None;
        public IReadOnlyCollection<string> DriverActionKinds => ["probe-driver-action"];

        public string MapToGenericActionKind(string driverActionKind)
            => "v13-unsupported-generic-action";
    }
}
