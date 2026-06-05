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

    [Fact]
    public void Public_api_matches_freeze_snapshot()
    {
        string approved = ReadApproval("webglrunlib-public-api.approved.txt");
        string actual = BuildPublicApiSnapshot("src/CanDoItAll.Components.WebGlRunLib");

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
