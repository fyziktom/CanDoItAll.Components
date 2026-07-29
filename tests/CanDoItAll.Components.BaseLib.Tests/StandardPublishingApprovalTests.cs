using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using CanDoItAll.Components.Charts;
using CanDoItAll.Components.Common;
using CanDoItAll.Components.Gantt;
using CanDoItAll.Components.Mermaid;
using CanDoItAll.Components.OverlayLib;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed class StandardPublishingApprovalTests
{
    private const string UpdateApprovalsVariable = "CDA_UPDATE_STANDARD_APPROVALS";

    private static readonly JsonSerializerOptions SnapshotJsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private static readonly StandardProject[] StandardProjects =
    [
        new(
            "CanDoItAll.Components.Common",
            "src/CanDoItAll.Components.Common/CanDoItAll.Components.Common.csproj",
            "src/CanDoItAll.Components.Common",
            typeof(CommonNamespaceMarker).Assembly),
        new(
            "CanDoItAll.Components.BaseLib",
            "src/CanDoItAll.Components.BaseLib/CanDoItAll.Components.BaseLib.csproj",
            "src/CanDoItAll.Components.BaseLib",
            typeof(Button).Assembly),
        new(
            "CanDoItAll.Components.Charts",
            "src/CanDoItAll.Components.Charts/CanDoItAll.Components.Charts.csproj",
            "src/CanDoItAll.Components.Charts",
            typeof(CdaChart).Assembly),
        new(
            "CanDoItAll.Components.OverlayLib",
            "src/CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj",
            "src/CanDoItAll.Components.OverlayLib",
            typeof(OverlayWindow).Assembly),
        new(
            "CanDoItAll.Components.Mermaid",
            "src/CanDoItAll.Components.Mermaid/CanDoItAll.Components.Mermaid.csproj",
            "src/CanDoItAll.Components.Mermaid",
            typeof(MermaidDiagram).Assembly),
        new(
            "CanDoItAll.Components.Gantt",
            "src/CanDoItAll.Components.Gantt/CanDoItAll.Components.Gantt.csproj",
            "src/CanDoItAll.Components.Gantt",
            typeof(GanttChart).Assembly)
    ];

    [Fact]
    public void StandardPublicApiMetadataMatchesFreezeSnapshot()
    {
        string actual = BuildPublicApiMetadataSnapshot();

        AssertApproved("standard-public-api.metadata.approved.json", actual);
    }

    [Fact]
    public void StandardProjectPackabilityMetadataMatchesFreezeSnapshot()
    {
        string actual = BuildProjectPackabilitySnapshot();

        AssertApproved("standard-project-packability.approved.json", actual);
    }

    [Fact]
    public void StandardSourcePackageInputsMatchFreezeSnapshot()
    {
        string actual = BuildSourcePackageInputSnapshot();

        AssertApproved("standard-source-package-inputs.approved.txt", actual);
    }

    [Fact]
    public void CompatibilityShimManifestMatchesFreezeSnapshot()
    {
        string actual = BuildCompatibilityShimSnapshot();

        AssertApproved("standard-compatibility-shims.approved.txt", actual);
    }

    [Fact]
    public void CompatibilityPolicyDocumentsEveryShimAndRemovalGate()
    {
        string repoRoot = FindRepoRoot();
        string policyPath = Path.Combine(repoRoot, "docs", "standard-components-compatibility-policy.md");
        string policy = File.ReadAllText(policyPath);

        Assert.Contains("Do not remove", policy, StringComparison.Ordinal);
        Assert.Contains("SB12", policy, StringComparison.Ordinal);

        foreach (CompatibilityShim shim in BuildCompatibilityShims())
        {
            Assert.Contains($"`{shim.Name}`", policy, StringComparison.Ordinal);
            Assert.Contains($"repo://{shim.RelativePath}", policy, StringComparison.Ordinal);
        }
    }

    private static string BuildPublicApiMetadataSnapshot()
    {
        var snapshot = StandardProjects
            .Select(project => new PackagePublicApiSnapshot
            {
                PackageId = project.PackageId,
                AssemblyName = project.Assembly.GetName().Name ?? project.PackageId,
                Types = project.Assembly
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
                    .ToArray()
            })
            .ToArray();

        return JsonSerializer.Serialize(snapshot, SnapshotJsonOptions) + Environment.NewLine;
    }

    private static string BuildProjectPackabilitySnapshot()
    {
        string repoRoot = FindRepoRoot();
        var directoryBuild = XDocument.Load(Path.Combine(repoRoot, "Directory.Build.props"));
        var directoryBuildTargets = XDocument.Load(Path.Combine(repoRoot, "Directory.Build.targets"));
        string inheritedReadme = ReadFirstProperty(directoryBuild, "PackageReadmeFile");
        string inheritedLicenseExpression = ReadFirstProperty(directoryBuildTargets, "PackageLicenseExpression");
        string inheritedTags = ReadFirstProperty(directoryBuild, "PackageTags");
        string inheritedDescription = ReadFirstProperty(directoryBuild, "Description");

        var snapshot = new
        {
            directoryBuild = new
            {
                isPackableDefault = ReadFirstProperty(directoryBuild, "IsPackable"),
                packageReadmeFile = inheritedReadme,
                packageLicenseExpression = inheritedLicenseExpression,
                packageTags = inheritedTags,
                defaultDescription = inheritedDescription
            },
            projects = StandardProjects.Select(project =>
            {
                string projectPath = Path.Combine(repoRoot, project.ProjectPath);
                var document = XDocument.Load(projectPath);
                string projectDescription = ReadFirstProperty(document, "Description");
                return new
                {
                    packageId = project.PackageId,
                    sdk = document.Root?.Attribute("Sdk")?.Value ?? string.Empty,
                    targetFramework = ReadFirstProperty(document, "TargetFramework"),
                    assemblyName = ReadFirstProperty(document, "AssemblyName"),
                    rootNamespace = ReadFirstProperty(document, "RootNamespace"),
                    isPackable = ReadFirstProperty(document, "IsPackable"),
                    description = projectDescription,
                    effectiveDescription = projectDescription.Length > 0 ? projectDescription : inheritedDescription,
                    hasReadme = File.Exists(Path.Combine(Path.GetDirectoryName(projectPath) ?? repoRoot, inheritedReadme)),
                    packageReferences = ReadItemIncludes(document, "PackageReference"),
                    projectReferences = ReadItemIncludes(document, "ProjectReference")
                };
            }).ToArray()
        };

        return JsonSerializer.Serialize(snapshot, SnapshotJsonOptions) + Environment.NewLine;
    }

    private static string BuildSourcePackageInputSnapshot()
    {
        string repoRoot = FindRepoRoot();
        string[] lines = StandardProjects
            .SelectMany(project =>
            {
                string sourceRoot = Path.Combine(repoRoot, project.SourceRoot);
                return Directory
                    .GetFiles(sourceRoot, "*", SearchOption.AllDirectories)
                    .Where(static path => !IsExcludedSnapshotPath(path))
                    .OrderBy(path => Path.GetRelativePath(repoRoot, path), StringComparer.Ordinal)
                    .Select(path =>
                    {
                        byte[] bytes = ReadSnapshotBytes(path);
                        string hash = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
                        return $"{Path.GetRelativePath(repoRoot, path).Replace('\\', '/')} | bytes={bytes.Length} | sha256={hash}";
                    });
            })
            .ToArray();

        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }

    private static string BuildCompatibilityShimSnapshot()
    {
        return string.Join(
            Environment.NewLine,
            BuildCompatibilityShims()
                .Select(static shim => $"{shim.Name} | {shim.Group} | repo://{shim.RelativePath}")) + Environment.NewLine;
    }

    private static CompatibilityShim[] BuildCompatibilityShims()
    {
        string repoRoot = FindRepoRoot();
        string componentRoot = Path.Combine(repoRoot, "src", "CanDoItAll.Components.BaseLib", "Components");
        return Directory
            .GetFiles(componentRoot, "*.razor", SearchOption.AllDirectories)
            .Where(path => path.Contains($"{Path.DirectorySeparatorChar}Compatibility{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .OrderBy(path => Path.GetRelativePath(repoRoot, path), StringComparer.Ordinal)
            .Select(path =>
            {
                string relativePath = Path.GetRelativePath(repoRoot, path).Replace('\\', '/');
                string[] parts = relativePath.Split('/');
                string group = parts.Length > 3 ? parts[3] : string.Empty;
                return new CompatibilityShim(Path.GetFileNameWithoutExtension(path), group, relativePath);
            })
            .ToArray();
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
                string accessors = $"{(property.GetMethod is null ? string.Empty : "get;")}{(property.SetMethod is null ? string.Empty : "set;")}";
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
            Type parameterType = parameter.ParameterType.IsByRef
                ? parameter.ParameterType.GetElementType() ?? parameter.ParameterType
                : parameter.ParameterType;
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

    private static string ReadFirstProperty(XDocument document, string name)
        => document
            .Descendants(name)
            .Select(static element => element.Value.Trim())
            .FirstOrDefault(static value => value.Length > 0) ?? string.Empty;

    private static string[] ReadItemIncludes(XDocument document, string itemName)
        => document
            .Descendants(itemName)
            .Select(static element => element.Attribute("Include")?.Value.Trim() ?? string.Empty)
            .Where(static value => value.Length > 0)
            .Order(StringComparer.Ordinal)
            .ToArray();

    private static void AssertApproved(string fileName, string actual)
    {
        string approvalPath = ApprovalPath(fileName);
        if (string.Equals(Environment.GetEnvironmentVariable(UpdateApprovalsVariable), "1", StringComparison.Ordinal))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(approvalPath)!);
            File.WriteAllText(approvalPath, actual);
        }

        string approved = File.ReadAllText(approvalPath);
        Assert.Equal(NormalizeNewLines(approved), NormalizeNewLines(actual));
    }

    private static string ApprovalPath(string fileName)
        => Path.Combine(
            FindRepoRoot(),
            "tests",
            "CanDoItAll.Components.BaseLib.Tests",
            "fixtures",
            "approvals",
            fileName);

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
            ".cs" or ".razor" or ".csproj" or ".props" or ".targets" or ".json" or ".js" or ".css" or ".md" or ".txt" or ".xml" or ".mjs" or ".mermaid" => true,
            _ => false
        };

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

    private sealed record StandardProject(string PackageId, string ProjectPath, string SourceRoot, Assembly Assembly);

    private sealed record CompatibilityShim(string Name, string Group, string RelativePath);

    private sealed class PackagePublicApiSnapshot
    {
        public string PackageId { get; set; } = string.Empty;

        public string AssemblyName { get; set; } = string.Empty;

        public PublicApiTypeSnapshot[] Types { get; set; } = [];
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
