using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace CanDoItAll.Components.WebGlLib.Tests;

public sealed partial class WebGlLibFreezeApprovalTests
{
    [Fact]
    public void Public_api_matches_freeze_snapshot()
    {
        string approved = ReadApproval("webgllib-public-api.approved.txt");
        string actual = BuildPublicApiSnapshot("src/CanDoItAll.Components.WebGlLib");

        Assert.Equal(NormalizeNewLines(approved), NormalizeNewLines(actual));
    }

    [Fact]
    public void Webgl_scene_js_surface_matches_freeze_snapshot()
    {
        string repoRoot = FindRepoRoot();
        string sceneEntry = Path.Combine(repoRoot, "src", "CanDoItAll.Components.WebGlLib", "wwwroot", "js", "runtime", "scene", "01-webgl-scene.js");
        string[] methods = File.ReadLines(sceneEntry)
            .Select(line => WebGlSceneMethodRegex().Match(line))
            .Where(static match => match.Success)
            .Select(static match => match.Groups["name"].Value)
            .Order(StringComparer.Ordinal)
            .ToArray();
        string actual = string.Join(Environment.NewLine, methods) + Environment.NewLine;
        string approved = ReadApproval("webgllib-webglscene-js-surface.approved.txt");

        Assert.Equal(NormalizeNewLines(approved), NormalizeNewLines(actual));
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
        => File.ReadAllText(Path.Combine(FindRepoRoot(), "tests", "CanDoItAll.Components.WebGlLib.Tests", "fixtures", "approvals", fileName));

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

    [GeneratedRegex("^\\s{4}(?<name>[A-Za-z0-9_]+)\\s*\\(")]
    private static partial Regex WebGlSceneMethodRegex();
}
