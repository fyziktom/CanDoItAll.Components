# SB06 Semantic Invariants

| ID | Behavior | Shallow-pass trap | failing-first proof | passing proof |
|---|---|---|---|---|
| SB06-I1 | Scene runtime is separate from workbench runtime and exposes `window.CanDoItAll.webglScene`. | Reuse or overwrite `webglWorkbench`, breaking compatibility. | SB01 inventory lists only `webglWorkbench` before execution. | `bundle://proof/SB08/browser/webgl-tycoon-village-final-proof.json` reports both `runtimeExists` and `workbenchNamespaceStillExists` as true. |
| SB06-I2 | Runtime renders actual WebGL canvas with GLB or fallback objects and returns non-empty image data. | Create a canvas element but never draw scene content. | No scene runtime existed in SB01 inventory. | Browser proof image length is 323460 and screenshots show models/symbols. |
| SB06-I3 | Selection and hover are produced by runtime hit-testing and callback flow. | Update inspector from static list click only. | No scene-specific callbacks existed before SB05/SB06. | Browser proof dispatches pointer events to canvas and inspector changes to `building.house-b`. |

## Semantic Adequacy

- Adversarial negative case: a runtime that only creates a blank canvas would fail object/symbol counts, image length, and selected-object proof.
- Semantic positive case: Playwright proof shows 20 objects, 9 symbols, 4 loaded assets, 12 fallbacks, 0 missing assets, and selected object update.
- anti-stub audit: `bundle://proof/SB09/transcripts/anti-stub-audit.txt`

## Production Behavior Artifact Matrix

See `bundle://proof/SB06/manifest.md#production-behavior-artifact-matrix`.

