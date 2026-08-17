# CanDoItAll.Components.SandboxWasm

Standalone WebAssembly host for the shared Components Sandbox catalog. It has no ASP.NET runtime
dependency and can be published to a static host, including GitHub Pages.

```sh
dotnet run --project samples/CanDoItAll.Components.SandboxWasm/CanDoItAll.Components.SandboxWasm.csproj
dotnet publish samples/CanDoItAll.Components.SandboxWasm/CanDoItAll.Components.SandboxWasm.csproj --configuration Release
```

Deploy the published `wwwroot` directory. The relative `index.html` base href works for a GitHub
Pages project site and a custom-domain root; retain `.nojekyll` and add an SPA fallback for deep
links if the host supports one.
