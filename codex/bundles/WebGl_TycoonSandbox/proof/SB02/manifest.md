# SB02 Proof Manifest

## Scope

Generic scene contracts for `WebGlSceneModel`, objects, links, camera, environment, layers, selection state, and proof snapshots.

## Changed Source

- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneModel.cs`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneObject.cs`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneLink.cs`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneCamera.cs`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneEnvironment.cs`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneLayer.cs`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneProofSnapshot.cs`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneSelectionState.cs`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlVector3.cs`
- Hashes: `bundle://proof/SB09/transcripts/changed-file-hashes.json`

## Command Proof

- failing-first baseline: `repo://artifacts/webgl-symbolic-tycoon-sandbox/01_INVENTORY.md` records that only workbench/node contracts existed before this subbundle.
- passing compile proof: `bundle://proof/SB02/transcripts/webgllib-contract-build.txt`
- downstream passing proof: `bundle://proof/SB09/transcripts/dotnet-build-solution.txt`
- anti-stub audit: `bundle://proof/SB09/transcripts/anti-stub-audit.txt`

## Source Assertions

- Scene contracts use `WebGlScene*` names and do not alter `WebGlWorkbench*` records.
- DTOs use safe defaults and collection initializers for nullable-safe serialization.
- `WebGlSceneModel` can describe buildings, props, agents, links, symbols, environment, camera, and UI state.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| `WebGlSceneModel` state | `repo://src/CanDoItAll.Components.WebGlSandbox/WebGlSandboxVillageSceneFactory.cs` | `repo://src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor` and runtime JS | Constructed in Blazor, serialized by JS interop, rendered by `window.CanDoItAll.webglScene` | `repo://artifacts/webgl-symbolic-tycoon-sandbox/01_INVENTORY.md` shows the previous workbench-only model could not express this object/symbol scene directly. |
| `WebGlSceneProofSnapshot` record | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js` | `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/TycoonVillage.razor` | Runtime snapshot returned to Blazor and displayed in proof panel | `bundle://proof/SB08/browser/webgl-tycoon-village-final-proof.json` proves non-zero counts from production runtime, not a seeded C# fixture. |

