# CanDoItAll.Components

Shared CanDoItAll component libraries isolated from the main app solution.

## Packages

All packages are currently versioned together at `0.1.0`.

| Package | Role |
| --- | --- |
| `CanDoItAll.Components.Common` | Dependency-light primitives and non-rendering contracts. |
| `CanDoItAll.Components.BaseLib` | Primary reusable Razor component library, BaseLib services, theme tokens, and shared CSS output. |
| `CanDoItAll.Components.CanvasLib` | Canvas, graph, and workbench components. |
| `CanDoItAll.Components.Charts` | Typed CanDoItAll chart wrapper over Blazor ApexCharts. |
| `CanDoItAll.Components.Mermaid` | Typed Mermaid diagram component and vendored Mermaid assets. |
| `CanDoItAll.Components.OverlayLib` | Floating overlay and window components. |
| `CanDoItAll.Components.WebGlLib` | WebGL workbench runtime and typed scene contracts. |
| `CanDoItAll.Components.Sandbox` | Component preview and regression host. |

## Build

```powershell
npm install
npm install --prefix Tailwind
npm run tailwind:build
dotnet build CanDoItAll.Components.slnx --configuration Release
```

## Pack

```powershell
dotnet pack CanDoItAll.Components.slnx --configuration Release --output artifacts/packages
```

Copy packages into the main repo private feed:

```powershell
Copy-Item artifacts\packages\*.0.1.0.nupkg C:\repositories\CanDoItAll\ExternalPackages -Force
```

The main repo must consume these packages through `PackageReference`; do not add project references from `C:\repositories\CanDoItAll` back to this repo.

## Styling

Component styles are built from `Tailwind\input.css` and emitted to `src\CanDoItAll.Components.BaseLib\wwwroot\css\output.css`. Main app-specific styles stay in `C:\repositories\CanDoItAll\Tailwind` and emit to `src\CanDoItAll.Web\wwwroot\css\output.css`.
