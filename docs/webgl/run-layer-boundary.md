# WebGL Run Layer Boundary

`CanDoItAll.Components.WebGlLib` owns generic scene rendering contracts only. It can describe a scene layout, assets, variants, import diagnostics, interaction state, command results, proof snapshots, render-command batches, and a serializable `WebGlSceneDocument`.

`CanDoItAll.Components.WebGlRunLib` owns the generic run layer above the scene renderer. It may define run documents, timelines, frames, action stages, action planners, playback controllers, and adapters that compile run frames into WebGlLib scene patches and command batches.

Run documents and action plans should be checked with `WebGlRunDocumentValidator` and `WebGlRunActionPlanValidator` before browser playback or domain bridge integration.

Browser playback should go through `WebGlRunDocumentRunner`, `WebGlRunFrameApplyResult`, `WebGlRunBrowserApplyAdapter`, and `WebGlSceneViewBrowserRuntime`. The adapter may call `WebGlSceneView` public APIs such as scene import, command batch application, diagnostics, and proof snapshot export. It must not reach into WebGlLib internals or require WebGlLib to know about run documents.

The run layer and consuming domain packages own simulation concerns such as clocks, replay lifecycle, persistence providers, domain events, pathfinding, collision, and scenario lifecycle. WebGlRunLib must keep those concepts generic; domain-specific mappers belong in the consuming repository.

Scene documents are intentionally storage-neutral. They may preserve a generic scene, runtime options, source, metadata, and content hash, but they must not include economy, process, game-rule, replay-log, or persistence-provider semantics.

## Domain Provenance Boundary

`source.*` metadata is the only generic provenance escape hatch. Consumers may use those metadata keys to preserve upstream ids, hashes, event ids, scenario ids, or bridge names, even when the values come from a domain package. WebGlRunLib stores and reports that provenance but does not interpret it.

Domain provenance is different from domain semantic leakage. Generic contract fields must stay domain-neutral: run ids, action kinds, action ids, stage ids, barrier policies, action parameters, non-source metadata, and public API names must not encode domain-specific semantics. `WebGlRunDocumentValidator` and `WebGlRunActionPlanValidator` therefore reject obvious domain terms in those generic fields while allowing `source.*` metadata values to pass through unchanged.

Domain bridge packages should move source references into `source.*` metadata before handing a run document to the generic validator stack. Plain metadata such as `bridge`, `eventKind`, or `scenarioId` is generic only when its value is domain-neutral; domain-specific values belong under `source.bridge`, `source.eventKind`, `source.scenarioId`, or another `source.*` provenance key.

## Dynamic Object References

WebGlRun playback may reference objects introduced by earlier scene patches. Validators should evaluate object ids against an evolving scene object set: start from the initial scene, apply object additions/removals from accepted patches in frame and stage order, then validate later motions, object patches, and links against the updated set.

Within a single command stage, motions are validated before patch-created objects become known. This keeps stage batching deterministic: a stage may add an object and link it inside the same patch, but a motion to that new object belongs in a later stage or frame. Domain bridge validators should follow the same policy so dynamic scenarios fail early with structured diagnostics instead of relying on browser runtime failures.

## Scene Revision Policy

`WebGlSceneModel.Revision` is the canonical scene revision. `WebGlSceneModel.UiState.Revision` is retained as a backward-compatible mirror for older serialized documents and UI-state consumers.

Scene mutation and document normalization must write both values when UI state is included. `WebGlSceneRevisionPolicy.Commit` is the shared write path for live scene updates and normalized scene documents. If UI state is excluded during serialization, the document keeps the canonical scene revision and resets UI state to defaults so validators and hashes do not compare two divergent revision sources.

Scene content hashing uses the canonical scene revision. A stale UI revision should be normalized to the canonical value instead of producing separate scene or document identity.

## Browser Reset Runtime Options

Runtime options are external to the WebGlRun browser reset path. `WebGlRunBrowserApplyAdapter` imports only the reset scene into the browser runtime; document-level `RuntimeOptions` are stripped to defaults before reset import. If a reset scene contains non-default runtime options, the adapter reports a warning so callers can see that those options were intentionally not applied.

Runtime options should be applied through the scene view/runtime setup owned by the host application. Reset imports must not silently change render mode, device-pixel-ratio limits, diagnostic panel visibility, or other runtime configuration carried in a stored scene document.

## Patch Transaction Modes

Scene patches use strict all-or-none transaction behavior by default. A strict patch that fails scene id, strict base revision, object target, added-object id, or added-link endpoint validation must return an error result without mutating the scene.

The only permissive patch mode currently supported is `permissive-invalid-links`, selected through patch metadata `patchTransactionMode=permissive-invalid-links` or the backward-compatible `missingLinkEndpointMode=warn`. In that mode, invalid added links are skipped, valid object/link operations may still apply, warnings name the skipped links, and result metadata includes `patchTransactionMode`, `missingLinkEndpointMode`, `patchClassification`, and `skippedLinkIds` when applicable.

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
