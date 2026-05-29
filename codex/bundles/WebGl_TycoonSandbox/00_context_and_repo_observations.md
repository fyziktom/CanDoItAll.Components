# 00 - Context and repository observations

## Observed repository shape

Target repository:

```text
fyziktom/CanDoItAll.Components
```

Current root README describes the repository as shared CanDoItAll component libraries isolated from the main app solution. It lists `CanDoItAll.Components.WebGlLib` as the WebGL workbench runtime and typed scene contracts, and `CanDoItAll.Components.Sandbox` as the component preview and regression host.

Current solution file includes:

```text
src/CanDoItAll.Components.BaseLib
src/CanDoItAll.Components.CanvasLib
src/CanDoItAll.Components.Charts
src/CanDoItAll.Components.Common
src/CanDoItAll.Components.Mermaid
src/CanDoItAll.Components.OverlayLib
src/CanDoItAll.Components.Sandbox
src/CanDoItAll.Components.WebGlLib
```

At the time this bundle was prepared, `CanDoItAll.Components.WebGlSandbox` is not present in the solution and should be added as a separate sandbox project.

## Observed WebGlLib shape

`CanDoItAll.Components.WebGlLib` is a Razor class library targeting `net10.0`, browser-supported, with a dependency on:

```text
Microsoft.AspNetCore.Components.Web
CanDoItAll.Components.OverlayLib
```

Its description says it is a universal WebGL workbench concept runtime with typed scene contracts and deterministic browser proof hooks.

The current runtime uses Three.js modules and GLTF loading. The existing JS runtime imports `THREE` from vendored `three.module.min.js`, and scene graph code imports `GLTFLoader` and `SkeletonUtils`.

## Current limitation

The current public model names are still workbench/node/edge oriented:

```text
WebGlWorkbenchSurface
WebGlWorkbenchNode
WebGlWorkbenchEdge
WebGlWorkbenchAnchor
WebGlWorkbenchUiState
WebGlWorkbenchCameraState
```

The current presets and layout names still reflect workflow/process usage:

```text
overview
roles
dependencies
branching
focus

center-lane
alternating-arc
layered-orbit
critical-path-spine
fanout-corridor
radial-burst
```

The current JS scene graph still resolves visual kinds as role/branch/step and uses hardcoded node model configs for `role`, `branch`, and `step`.

This is good existing proof, but it should not become the only abstraction for tycoon-like generic visualization. Add a new generic scene layer beside the workbench layer.

## Desired target

Create a new domain-neutral layer:

```text
WebGlSceneModel
WebGlSceneObject
WebGlSceneLink
WebGlSceneCamera
WebGlSceneLayer
WebGlAssetCatalog
WebGlAssetDefinition
WebGlStatusSymbol
WebGlInteractionState
WebGlRuntimeSnapshot
```

Keep the old `WebGlWorkbench` working.

Add a new sandbox project:

```text
src/CanDoItAll.Components.WebGlSandbox
```

The sandbox should reference:

```text
CanDoItAll.Components.BaseLib
CanDoItAll.Components.OverlayLib
CanDoItAll.Components.WebGlLib
```

No dependency on processes, economy, or main app modules.
