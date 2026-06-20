using System.Xml.Linq;

namespace CanDoItAll.Components.WebGlRunLib.Tests;

public sealed class ComponentsPackageScopeTests
{
    [Fact]
    public void Package_intent_is_explicit_for_solution_projects()
    {
        string repoRoot = FindRepoRoot();
        var expected = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["src/CanDoItAll.Components.BaseLib/CanDoItAll.Components.BaseLib.csproj"] = "true",
            ["src/CanDoItAll.Components.CanvasLib/CanDoItAll.Components.CanvasLib.csproj"] = "true",
            ["src/CanDoItAll.Components.Charts/CanDoItAll.Components.Charts.csproj"] = "true",
            ["src/CanDoItAll.Components.Common/CanDoItAll.Components.Common.csproj"] = "true",
            ["src/CanDoItAll.Components.Mermaid/CanDoItAll.Components.Mermaid.csproj"] = "true",
            ["src/CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj"] = "true",
            ["src/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj"] = "false",
            ["src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj"] = "true",
            ["src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj"] = "true",
            ["src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj"] = "false",
            ["samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj"] = "false",
            ["samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj"] = "false",
            ["tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj"] = "false",
            ["tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj"] = "false"
        };

        foreach (KeyValuePair<string, string> project in expected)
        {
            string path = Path.Combine(repoRoot, project.Key.Replace('/', Path.DirectorySeparatorChar));
            string? actual = XDocument.Load(path)
                .Descendants("IsPackable")
                .Select(static element => element.Value.Trim())
                .SingleOrDefault();

            Assert.Equal(project.Value, actual);
        }
    }

    [Fact]
    public void WebGlRunLib_generic_sample_supports_project_and_package_modes_without_domain_references()
    {
        string repoRoot = FindRepoRoot();
        string sampleProject = Path.Combine(
            repoRoot,
            "samples",
            "CanDoItAll.Components.WebGlRunLibGenericSample",
            "CanDoItAll.Components.WebGlRunLibGenericSample.csproj");
        string projectXml = File.ReadAllText(sampleProject);

        Assert.Contains("UseComponentsWebGlRunLibPackage", projectXml, StringComparison.Ordinal);
        Assert.Contains("ComponentsWebGlRunLibPackageVersion", projectXml, StringComparison.Ordinal);
        Assert.Contains("CanDoItAll.Components.WebGlRunLib", projectXml, StringComparison.Ordinal);
        Assert.DoesNotContain("CanDoItAll.Economy", projectXml, StringComparison.OrdinalIgnoreCase);
    }

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
}
