# SB02 Semantic Invariants

## Invariants

| ID | Behavior | Shallow-pass trap | failing-first proof | passing proof |
|---|---|---|---|---|
| SB02-I1 | Contracts describe generic buildings, props, agents, status symbols, links, camera, environment, and UI state without domain leakage. | Add empty classes with only file names. | `repo://artifacts/webgl-symbolic-tycoon-sandbox/01_INVENTORY.md` shows workbench/node-only contracts before execution. | `bundle://proof/SB02/transcripts/webgllib-contract-build.txt`; `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneModel.cs` |
| SB02-I2 | Proof snapshot is a real contract consumed by runtime and sandbox. | Define a snapshot DTO but never return it from production runtime. | No `WebGlSceneProofSnapshot` existed in SB01 inventory. | `bundle://proof/SB08/browser/webgl-tycoon-village-final-proof.json` returns object/symbol/asset counts from `window.CanDoItAll.webglScene`. |

## Semantic Adequacy

- Adversarial negative case: missing the generic scene contracts would leave only `WebGlWorkbenchSurface`, which cannot directly represent object symbols, asset catalog, scene environment, and proof snapshot as required.
- Semantic positive case: `/tycoon-village` uses these contracts to render 20 objects, 5 links, and 9 symbols.
- anti-stub audit: `bundle://proof/SB09/transcripts/anti-stub-audit.txt`
- Raw-note closure: Generic scene contracts are Solved by source files and build proof.

## Production Behavior Artifact Matrix

See `bundle://proof/SB02/manifest.md#production-behavior-artifact-matrix`.

