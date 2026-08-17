# CanDoItAll.Components.SandboxWasm

Standalone WebAssembly host for the shared Components Sandbox catalog. It has no ASP.NET runtime
dependency and can be published to a static host, including GitHub Pages.

```sh
dotnet run --project samples/CanDoItAll.Components.SandboxWasm/CanDoItAll.Components.SandboxWasm.csproj
dotnet publish samples/CanDoItAll.Components.SandboxWasm/CanDoItAll.Components.SandboxWasm.csproj --configuration Release
```

Deploy the published `wwwroot` directory. GitHub Pages deployments must retain `.nojekyll`, rewrite
the `index.html` base href to the repository path (for example, `/CanDoItAll.Components/`), and add
an SPA fallback for deep links.
