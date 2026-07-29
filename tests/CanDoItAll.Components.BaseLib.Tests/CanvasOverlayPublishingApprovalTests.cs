using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CanDoItAll.Components.CanvasLib;
using CanDoItAll.Components.OverlayLib;

namespace CanDoItAll.Components.BaseLib.Tests;

public sealed partial class CanvasOverlayPublishingApprovalTests
{
    private const string UpdateApprovalsVariable = "CDA_UPDATE_CANVAS_OVERLAY_APPROVALS";

    private static readonly JsonSerializerOptions SnapshotJsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private static readonly PublishingProject[] Projects =
    [
        new(
            "CanDoItAll.Components.CanvasLib",
            "src/CanDoItAll.Components.CanvasLib/CanDoItAll.Components.CanvasLib.csproj",
            "src/CanDoItAll.Components.CanvasLib",
            typeof(CanvasLibNamespaceMarker).Assembly),
        new(
            "CanDoItAll.Components.OverlayLib",
            "src/CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj",
            "src/CanDoItAll.Components.OverlayLib",
            typeof(OverlayWindow).Assembly)
    ];

    [Fact]
    public void CanvasOverlayPublicApiMetadataMatchesFreezeSnapshot()
    {
        string actual = BuildPublicApiMetadataSnapshot();

        AssertApproved("canvas-overlay-public-api.metadata.approved.json", actual);
    }

    [Fact]
    public void CanvasOverlayProjectPackabilityMetadataMatchesFreezeSnapshot()
    {
        string actual = BuildProjectPackabilitySnapshot();

        AssertApproved("canvas-overlay-project-packability.approved.json", actual);
    }

    [Fact]
    public void CanvasOverlayStaticWebAssetsMatchFreezeSnapshot()
    {
        string actual = BuildStaticWebAssetSnapshot();

        AssertApproved("canvas-overlay-static-web-assets.approved.txt", actual);
    }

    [Fact]
    public void CanvasOverlayRuntimeDependencyPolicyStaysToolingOnly()
    {
        string repoRoot = FindRepoRoot();
        using JsonDocument packageJson = JsonDocument.Parse(File.ReadAllText(Path.Combine(repoRoot, "package.json")));
        Assert.False(packageJson.RootElement.TryGetProperty("dependencies", out _));

        foreach (PublishingProject project in Projects)
        {
            string sourceRoot = Path.Combine(repoRoot, project.SourceRoot);
            string readme = File.ReadAllText(Path.Combine(sourceRoot, "README.md"));
            Assert.Contains("No npm runtime dependency", readme, StringComparison.OrdinalIgnoreCase);

            string[] runtimeFiles = Directory
                .GetFiles(Path.Combine(sourceRoot, "wwwroot"), "*.js", SearchOption.AllDirectories)
                .OrderBy(static path => path, StringComparer.Ordinal)
                .ToArray();

            foreach (string runtimeFile in runtimeFiles)
            {
                string content = File.ReadAllText(runtimeFile);
                Assert.DoesNotContain("import ", content, StringComparison.Ordinal);
                Assert.DoesNotContain("require(", content, StringComparison.Ordinal);
            }
        }
    }

    [Fact]
    public void CanvasRuntimeAssetExposesStableLowLevelContract()
    {
        string repoRoot = FindRepoRoot();
        string runtimePath = Path.Combine(
            repoRoot,
            "src",
            "CanDoItAll.Components.CanvasLib",
            "wwwroot",
            "js",
            "runtime",
            "canvas-runtime.js");
        string runtime = File.ReadAllText(runtimePath);

        string[] requiredContractMembers =
        [
            "root.canvasRuntime = Object.freeze",
            "class CanvasSurface",
            "class HitRegionRegistry",
            "class PointerRouter",
            "createSurface(options)",
            "createHitRegionRegistry()",
            "createPointerRouter(options)",
            "renderToPngDataUrl",
            "downloadDataUrl",
            "setPointerCapture",
            "ResizeObserver",
            "requestAnimationFrame"
        ];

        foreach (string member in requiredContractMembers)
        {
            Assert.Contains(member, runtime, StringComparison.Ordinal);
        }

        Assert.DoesNotContain("window.addEventListener(\"pointermove\"", runtime, StringComparison.Ordinal);
        Assert.DoesNotContain("window.addEventListener(\"pointerup\"", runtime, StringComparison.Ordinal);

        using JsonDocument manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(
            repoRoot,
            "tools",
            "canvaslib",
            "asset-manifest.json")));
        JsonElement[] runtimeScripts = manifest.RootElement
            .GetProperty("runtimeScripts")
            .EnumerateArray()
            .ToArray();
        int genericRuntimeIndex = Array.FindIndex(
            runtimeScripts,
            static entry => entry.GetProperty("source").GetString() == "js/runtime/canvas-runtime.js");
        int workbenchRuntimeIndex = Array.FindIndex(
            runtimeScripts,
            static entry => entry.GetProperty("source").GetString() == "js/runtime/workbench/01-foundation.js");

        Assert.True(genericRuntimeIndex >= 0, "Canvas runtime must be registered as a generated runtime asset.");
        Assert.True(workbenchRuntimeIndex > genericRuntimeIndex, "Generic canvas mechanics must load before the retained Workbench runtime.");
    }

    private static string BuildPublicApiMetadataSnapshot()
    {
        var snapshot = Projects
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
        string inheritedBaseVersion = ReadFirstProperty(directoryBuild, "CanDoItAllPackageBaseVersion");
        string inheritedReadme = ReadFirstProperty(directoryBuild, "PackageReadmeFile");
        string inheritedLicenseExpression = ReadFirstProperty(directoryBuildTargets, "PackageLicenseExpression");
        string inheritedTags = ReadFirstProperty(directoryBuild, "PackageTags");
        string inheritedDescription = ReadFirstProperty(directoryBuild, "Description");

        var snapshot = new
        {
            directoryBuild = new
            {
                packageBaseVersion = inheritedBaseVersion,
                packageReadmeFile = inheritedReadme,
                packageLicenseExpression = inheritedLicenseExpression,
                packageTags = inheritedTags,
                defaultDescription = inheritedDescription
            },
            projects = Projects.Select(project =>
            {
                string projectPath = Path.Combine(repoRoot, project.ProjectPath);
                string sourceRoot = Path.Combine(repoRoot, project.SourceRoot);
                var document = XDocument.Load(projectPath);
                string explicitPackageVersion = ReadFirstProperty(document, "PackageVersion");
                string effectivePackageVersion = explicitPackageVersion.Length > 0 ? explicitPackageVersion : inheritedBaseVersion;
                string projectDescription = ReadFirstProperty(document, "Description");

                return new
                {
                    packageId = project.PackageId,
                    sdk = document.Root?.Attribute("Sdk")?.Value ?? string.Empty,
                    targetFramework = ReadFirstProperty(document, "TargetFramework"),
                    assemblyName = ReadFirstProperty(document, "AssemblyName"),
                    rootNamespace = ReadFirstProperty(document, "RootNamespace"),
                    isPackable = ReadFirstProperty(document, "IsPackable"),
                    explicitPackageVersion,
                    effectivePackageVersion,
                    readmePackageVersion = ReadReadmePackageVersion(Path.Combine(sourceRoot, inheritedReadme)),
                    description = projectDescription,
                    effectiveDescription = projectDescription.Length > 0 ? projectDescription : inheritedDescription,
                    hasReadme = File.Exists(Path.Combine(sourceRoot, inheritedReadme)),
                    packageReferences = ReadItemIncludes(document, "PackageReference"),
                    projectReferences = ReadItemIncludes(document, "ProjectReference"),
                    generatedAssetComponents = Directory
                        .GetFiles(sourceRoot, "*Assets.razor", SearchOption.AllDirectories)
                        .Select(path => Path.GetRelativePath(repoRoot, path).Replace('\\', '/'))
                        .Order(StringComparer.Ordinal)
                        .ToArray(),
                    staticWebAssetCount = Directory.Exists(Path.Combine(sourceRoot, "wwwroot"))
                        ? Directory.GetFiles(Path.Combine(sourceRoot, "wwwroot"), "*", SearchOption.AllDirectories).Length
                        : 0
                };
            }).ToArray()
        };

        return JsonSerializer.Serialize(snapshot, SnapshotJsonOptions) + Environment.NewLine;
    }

    private static string BuildStaticWebAssetSnapshot()
    {
        string repoRoot = FindRepoRoot();
        string[] lines = Projects
            .SelectMany(project =>
            {
                string sourceRoot = Path.Combine(repoRoot, project.SourceRoot);
                string webRoot = Path.Combine(sourceRoot, "wwwroot");
                string[] files = Directory.Exists(webRoot)
                    ? Directory.GetFiles(webRoot, "*", SearchOption.AllDirectories)
                    : [];

                return files
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

    private static string ReadReadmePackageVersion(string readmePath)
    {
        string readme = File.ReadAllText(readmePath);
        Match match = PackageVersionRegex().Match(readme);
        return match.Success ? match.Groups["version"].Value : string.Empty;
    }

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
            ".cs" or ".razor" or ".csproj" or ".props" or ".targets" or ".json" or ".js" or ".css" or ".md" or ".txt" or ".xml" or ".mjs" => true,
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

    [GeneratedRegex(@"Package version:\s*`(?<version>[^`]+)`", RegexOptions.CultureInvariant)]
    private static partial Regex PackageVersionRegex();

    private sealed record PublishingProject(string PackageId, string ProjectPath, string SourceRoot, Assembly Assembly);

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
