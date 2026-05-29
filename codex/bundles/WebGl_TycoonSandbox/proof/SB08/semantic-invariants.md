# SB08 Semantic Invariants

| ID | Behavior | Shallow-pass trap | failing-first proof | passing proof |
|---|---|---|---|---|
| SB08-I1 | `/tycoon-village` renders a recognizable village with buildings, props, agents, paths, and symbols. | Render only a static HTML panel or a blank canvas. | SB01 inventory records the standalone WebGL sandbox did not exist. | `bundle://proof/SB08/browser/webgl-tycoon-village-final-desktop.png`; `bundle://proof/SB08/browser/webgl-tycoon-village-final-proof.json` |
| SB08-I2 | Selection and hover update inspector from canvas interaction. | Select from list or hardcode selected object. | No route or scene runtime existed before execution. | `bundle://proof/SB08/browser/webgl-tycoon-village-final-proof.json` records pointer-driven selected and hovered `building.house-b`. |
| SB08-I3 | Deterministic proof snapshot returns non-zero object and symbol counts. | Display scene factory counts without asking runtime. | No proof snapshot route existed before execution. | Runtime snapshot reports 20 objects, 9 symbols, 4 loaded assets, 12 fallbacks, 0 missing assets. |

## Semantic Adequacy

- Adversarial negative case: a fixture-only page would fail canvas image length, runtime namespace, and selected-object hit-test checks.
- Semantic positive case: browser screenshots show rendered GLB models, fallback buildings/trees, colored symbols above agents, and inspector proof.
- anti-stub audit: `bundle://proof/SB09/transcripts/anti-stub-audit.txt`

## Production Behavior Artifact Matrix

See `bundle://proof/SB08/manifest.md#production-behavior-artifact-matrix`.

