# CanDoItAll.Components

Shared CanDoItAll component libraries isolated from the main app solution.

## Packages

All packages are currently versioned together from `CanDoItAllPackageBaseVersion`, which defaults to `0.1.0`.
For package-consumer proof runs, append a unique prerelease suffix with `CanDoItAllPackageProofSuffix`, for example `-sb11.20260602.1`.

| Package | Role |
| --- | --- |
| `CanDoItAll.Components.Common` | Dependency-light primitives and non-rendering contracts. |
| `CanDoItAll.Components.BaseLib` | Primary reusable Razor component library, BaseLib services, theme tokens, and shared CSS output. |
| `CanDoItAll.Components.CanvasLib` | Canvas, graph, and workbench components. |
| `CanDoItAll.Components.Charts` | Typed CanDoItAll chart wrapper over Blazor ApexCharts. |
| `CanDoItAll.Components.Mermaid` | Typed Mermaid diagram component and vendored Mermaid assets. |
| `CanDoItAll.Components.OverlayLib` | Floating overlay and window components. |
| `CanDoItAll.Components.WebGlLib` | WebGL workbench runtime plus generic scene, asset, symbol, interaction, and proof contracts. |
| `CanDoItAll.Components.WebGlRunLib` | Generic run/playback/action/stage contracts layered over WebGlLib scene patches. |
| `CanDoItAll.Components.Sandbox` | Component preview and regression host. |
| `CanDoItAll.Components.WebGlSandbox` | Standalone WebGL proof host for generic scene demos such as the tycoon village. |

## Build

```powershell
npm install
npm install --prefix Tailwind
npm run tailwind:build
dotnet build CanDoItAll.Components.slnx --configuration Release
```

## WebGL Sandbox

Run the standalone WebGL proof host with:

```powershell
dotnet run --project src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
```

Useful routes:

- `/tycoon-village` renders the generic village scene with primitive, mixed GLB, and high-detail GLB profiles, drag/move, motion, export/import, missing-asset fallback, status symbols, selection, and proof snapshots.
- `/asset-catalog` lists logical asset ids, quality tiers, variants, and the GLB or primitive fallback that backs each one.

Domain-specific repositories should map their own data into `WebGlSceneModel` outside this repository. Keep `CanDoItAll.Components.WebGlLib` domain-neutral.

WebGlLib can also be consumed without WebGlRunLib through the minimal Razor sample at `samples/CanDoItAll.Components.WebGlLibOnlyViewer`. The sample supports project-reference mode by default and package mode with `UseComponentsWebGlLibPackage=true`.

Current WebGL hardening proof is large-screen only by design; small-screen layout tuning is out of scope for this bundle.

## Pack

```powershell
$proofSuffix = "-sb11.20260602.1"
dotnet pack CanDoItAll.Components.slnx --configuration Release --output artifacts/packages /p:CanDoItAllPackageProofSuffix=$proofSuffix
```

This emits all shared component packages at the version in `Directory.Build.props`, plus any proof suffix supplied at pack time. The WebGL integration packages are:

- `artifacts\packages\CanDoItAll.Components.WebGlLib.0.1.0.nupkg`
- `artifacts\packages\CanDoItAll.Components.WebGlRunLib.0.1.0.nupkg`
- proof example: `artifacts\packages\CanDoItAll.Components.WebGlLib.0.1.0-sb11.20260602.1.nupkg`
- proof example: `artifacts\packages\CanDoItAll.Components.WebGlRunLib.0.1.0-sb11.20260602.1.nupkg`

When validating package consumers against freshly packed packages, use a proof/local NuGet.config that points at the fresh package output before any older private feed, set an isolated `NUGET_PACKAGES` cache, and pass the full proof version to consumer restore/build properties. A proof run must fail if the stale feed is used or if the consumer restores a project reference instead of the freshly packed package.

Copy packages into the main repo private feed:

```powershell
Copy-Item artifacts\packages\*.0.1.0.nupkg C:\repositories\CanDoItAll\ExternalPackages -Force
```

The main repo must consume these packages through `PackageReference`; do not add project references from `C:\repositories\CanDoItAll` back to this repo.

## Styling

Component styles are built from `Tailwind\input.css` and emitted to `src\CanDoItAll.Components.BaseLib\wwwroot\css\output.css`. Main app-specific styles stay in `C:\repositories\CanDoItAll\Tailwind` and emit to `src\CanDoItAll.Web\wwwroot\css\output.css`.
