# SB07 Boundary Audit

## Decision

SB07 passes the hard WebGlLib boundary gate. `CanDoItAll.Components.WebGlLib` remains the generic render substrate and does not reference `CanDoItAll.Components.WebGlRunLib` or Economy/domain packages. The existing command-batch stage runner stays fenced as render-command transport for scene patches, motions, waits, and render-idle barriers; run documents, timelines, replay lifecycle, action planning, and domain semantics remain in `WebGlRunLib` or consuming domain packages.

## Ownership Matrix

| Layer | Belongs here | Does not belong here | SB07 evidence |
| --- | --- | --- | --- |
| WebGlLib | Scene model, scene view, assets, symbols, interaction primitives, scene patches, render-command batches, runtime/proof diagnostics, scene documents. | Run documents, replay lifecycle, scenario lifecycle, persistence providers, economy rules, production-line rules, domain actions. | `bundle://proof/SB07/transcripts/passing-webgllib-boundary-audit.txt`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md` |
| WebGlRunLib | Generic run documents, timelines, frames, action stages, planners, playback controllers, frame-to-scene-batch adapters. | Economy account, market, ledger, production-line station, machine, product, domain resource accounting. | `repo://CanDoItAll.Components/docs/webgl/run-layer-boundary.md`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj` |
| Economy WebGlBridge | Economy visual-frame/action mapping, strict provenance, fallback policy, domain diagnostics. | Shared Components package changes that hard-code economy-only behavior. | `repo://CanDoItAll.Components/docs/webgl/economy-simulation-boundary-plan.md` |

## Dependency Graph

```text
CanDoItAll.Components.WebGlRunLib -> CanDoItAll.Components.WebGlLib -> OverlayLib/BaseLib/Common
WebGlLib-only sample -> CanDoItAll.Components.WebGlLib
WebGlLib -X-> WebGlRunLib
WebGlLib -X-> Economy or other domain packages
```

## Boundary-Sensitive Findings

| Finding | Result | Evidence |
| --- | --- | --- |
| WebGlLib project references | Only `CanDoItAll.Components.OverlayLib` is referenced by WebGlLib; no WebGlRunLib or Economy reference exists. | `bundle://proof/SB07/transcripts/sb07-source-assertions.txt`, `bundle://proof/SB07/transcripts/passing-webgllib-boundary-audit.txt` |
| WebGlRunLib dependency direction | WebGlRunLib references WebGlLib, matching the required one-way dependency direction. | `bundle://proof/SB07/transcripts/sb07-source-assertions.txt` |
| Command-batch stages in WebGlLib | Allowed only as render-command transport. Docs explicitly fence them away from run documents, replay lifecycle, scenario lifecycle, domain events, persistence providers, and domain action semantics. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md`, `repo://CanDoItAll.Components/docs/webgl/run-layer-boundary.md` |
| WebGlLib-only consumption | Minimal Razor sample builds with only a WebGlLib project reference and no WebGlRunLib dependency. | `bundle://proof/SB07/transcripts/passing-webgllib-only-sample-build.txt`, `repo://CanDoItAll.Components/samples/CanDoItAll.Components.WebGlLibOnlyViewer/CanDoItAll.Components.WebGlLibOnlyViewer.csproj` |
| Adversarial boundary failure | The audit probe fails when fed a forbidden `CanDoItAll.Components.WebGlRunLib` reference. | `bundle://proof/SB07/transcripts/failing-boundary-audit-probe.txt` |

## Closure

No production code needed to move out of WebGlLib in SB07. The audit found no hard dependency leak and no economy/domain naming in first-party WebGlLib source. The command-batch stage runner remains in WebGlLib because it is the low-level browser transport consumed by `WebGlSceneView.ApplyCommandBatchAsync`; higher-level run/action semantics are owned by WebGlRunLib.
