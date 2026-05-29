# SB03 Semantic Invariants

| ID | Behavior | Shallow-pass trap | failing-first proof | passing proof |
|---|---|---|---|---|
| SB03-I1 | Logical asset ids resolve to GLB models or primitive fallbacks without crashing the scene. | Hardcode exact demo models and fail when one is missing. | SB01 inventory shows only four GLB files and no catalog abstraction. | `bundle://proof/SB08/browser/webgl-tycoon-village-final-proof.json` reports 4 loaded assets, 12 fallback objects, and 0 missing assets. |
| SB03-I2 | Catalog metadata remains generic and reusable. | Add model paths directly to scene objects. | `repo://artifacts/webgl-symbolic-tycoon-sandbox/01_INVENTORY.md` lists raw assets only. | `repo://src/CanDoItAll.Components.WebGlSandbox/WebGlSandboxAssetCatalogFactory.cs` maps logical ids to GLB or primitive fallbacks. |

## Semantic Adequacy

- Adversarial negative case: a missing exact house/tree GLB does not crash rendering because the production runtime creates primitive houses and trees.
- Semantic positive case: browser proof renders people/gears/question-box GLBs plus fallback houses/trees/markers.
- anti-stub audit: `bundle://proof/SB09/transcripts/anti-stub-audit.txt`

## Production Behavior Artifact Matrix

See `bundle://proof/SB03/manifest.md#production-behavior-artifact-matrix`.

