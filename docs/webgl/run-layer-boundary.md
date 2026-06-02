# WebGL Run Layer Boundary

`CanDoItAll.Components.WebGlLib` owns generic scene rendering contracts only. It can describe a scene layout, assets, variants, import diagnostics, interaction state, command results, proof snapshots, render-command batches, and a serializable `WebGlSceneDocument`.

`CanDoItAll.Components.WebGlRunLib` owns the generic run layer above the scene renderer. It may define run documents, timelines, frames, action stages, action planners, playback controllers, and adapters that compile run frames into WebGlLib scene patches and command batches.

Run documents and action plans should be checked with `WebGlRunDocumentValidator` and `WebGlRunActionPlanValidator` before browser playback or domain bridge integration.

Browser playback should go through `WebGlRunDocumentRunner`, `WebGlRunFrameApplyResult`, `WebGlRunBrowserApplyAdapter`, and `WebGlSceneViewBrowserRuntime`. The adapter may call `WebGlSceneView` public APIs such as scene import, command batch application, diagnostics, and proof snapshot export. It must not reach into WebGlLib internals or require WebGlLib to know about run documents.

The run layer and consuming domain packages own simulation concerns such as clocks, replay lifecycle, persistence providers, domain events, pathfinding, collision, and scenario lifecycle. WebGlRunLib must keep those concepts generic; domain-specific mappers belong in the consuming repository.

Scene documents are intentionally storage-neutral. They may preserve a generic scene, runtime options, source, metadata, and content hash, but they must not include economy, process, game-rule, replay-log, or persistence-provider semantics.

Dependency direction:

```text
WebGlRunLib -> WebGlLib
Application or domain package -> WebGlRunLib -> WebGlLib
WebGlLib -X-> WebGlRunLib
WebGlLib -X-> Economy or other domain packages
```

## Package And Project Integration

Components owns the package output for both WebGL layers:

```powershell
dotnet pack CanDoItAll.Components.slnx --configuration Release --output artifacts/packages
```

The current shared package version is controlled by `Directory.Build.props`. The pack command emits `CanDoItAll.Components.WebGlLib.0.1.0.nupkg` and `CanDoItAll.Components.WebGlRunLib.0.1.0.nupkg` under `artifacts\packages`.

Local domain development should use project references only from the consuming repo into Components. For the Economy bridge this means setting `ComponentsRepoRoot` to the Components checkout and leaving package mode off:

```powershell
dotnet build src\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj --configuration Release /p:ComponentsRepoRoot=C:\repositories\CanDoItAll.Components
```

Package-consumption validation should enable the bridge package mode and restore from the freshly packed Components feed:

```powershell
dotnet restore src\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj --configfile C:\path\to\package-proof.NuGet.config /p:UseComponentsWebGlRunLibPackage=true /p:ComponentsWebGlRunLibPackageVersion=0.1.0 /p:ComponentsWebGlLibPackageVersion=0.1.0
dotnet build src\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj --configuration Release --no-restore /p:UseComponentsWebGlRunLibPackage=true /p:ComponentsWebGlRunLibPackageVersion=0.1.0 /p:ComponentsWebGlLibPackageVersion=0.1.0
```

Use a proof/local NuGet.config with `<clear />`, the fresh `artifacts\packages` folder, and nuget.org. This avoids stale private-feed `0.1.0` packages while preserving public dependency restore.

The bridge references both packages in package mode because its code directly consumes WebGlLib scene contracts as well as WebGlRunLib run contracts. Components must not reference Economy in either mode.
