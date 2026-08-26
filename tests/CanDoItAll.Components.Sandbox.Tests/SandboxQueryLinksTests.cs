using CanDoItAll.Components.Sandbox;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.Sandbox.Tests;

[Collection("Sandbox URL state")]
public sealed class SandboxQueryLinksTests
{
    [Fact]
    public void WithDark_SsrPreservesTheAnchorAndExistingQuery()
    {
        SandboxQueryLinks.UseHashRouting = false;
        var navigation = new TestNavigationManager("http://localhost/mermaid?frame=compact#mermaid-diagram");

        var result = SandboxQueryLinks.WithDark(navigation, true);

        Assert.Equal("http://localhost/mermaid?frame=compact&dark=true#mermaid-diagram", result);
    }

    [Fact]
    public void WithDark_WasmKeepsAllClientRouteStateInsideTheHash()
    {
        SandboxQueryLinks.UseHashRouting = true;
        var navigation = new TestNavigationManager("http://localhost/#mermaid?mermaid-diagram&frame=tablet&extra=value");
        SandboxQueryLinks.UpdateHashState("#mermaid?mermaid-diagram&frame=tablet&extra=value");

        var result = SandboxQueryLinks.WithDark(navigation, true);

        Assert.Equal("#mermaid?mermaid-diagram&extra=value&frame=tablet&dark=true", result);
        SandboxQueryLinks.UpdateHashState(result);
        Assert.True(SandboxQueryLinks.IsDark(navigation));
    }

    [Fact]
    public void BuildCrossPageLink_WasmCarriesDarkStateInTheHash()
    {
        SandboxQueryLinks.UseHashRouting = true;
        var navigation = new TestNavigationManager("http://localhost/#mermaid?mermaid-diagram&dark=true");
        SandboxQueryLinks.UpdateHashState("#mermaid?mermaid-diagram&dark=true");

        var result = SandboxQueryLinks.BuildCrossPageLink(
            navigation,
            "/buttons",
            SandboxFramePreset.LiveViewport,
            isDark: true,
            fragment: "button");

        Assert.Equal("#buttons?button&dark=true", result);
    }

    private sealed class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager(string uri)
            => Initialize("http://localhost/", uri);
    }
}

[CollectionDefinition("Sandbox URL state", DisableParallelization = true)]
public sealed class SandboxUrlStateCollection;
