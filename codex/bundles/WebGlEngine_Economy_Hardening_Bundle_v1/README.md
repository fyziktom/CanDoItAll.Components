# CanDoItAll WebGL Engine + Economy Hardening Workflow Bundle

Prepared date: 2026-06-01  
Stage: prepared  
Profile: initiative / cross-repo architecture hardening  
Primary target repositories:

- `fyziktom/CanDoItAll.Components`, branch `webgl-engine`
- `fyziktom/CanDoItAll.Economy`, current working branch/main line

## Purpose

This bundle instructs Codex to harden the WebGL/3D visualization architecture across `CanDoItAll.Components` and `CanDoItAll.Economy`.

The required end state is a layered, generic, durable engine foundation:

```text
CanDoItAll.Components.WebGlLib
  ultra-light generic scene/model/asset/render/interactions substrate
  usable for simple model viewing and generic 3D visualizations

CanDoItAll.Components.WebGlRunLib
  generic run/playback/action/stage contracts over WebGlLib scene patches
  no Economy-specific or production-line-specific semantics

CanDoItAll.Economy.Simulation.WebGlBridge
  first real consumer that maps generic economy visual frames/actions to WebGlRunLib
  strict provenance and fallback policy
```

## Critical instructions for Codex

- Implement one subbundle at a time.
- Do not start a downstream subbundle until the previous progression gate passes.
- Do not put Economy, ledger, market, production-line, Vernon Smith, or domain-specific simulation concepts into `WebGlLib` or `WebGlRunLib`.
- Preserve the ultra-light `WebGlLib` use case: a simple app must be able to display a 3D model/scene without referencing `WebGlRunLib`.
- Treat all critical subbundles as semantic proof work, not as structure-only refactoring.
- Every critical subbundle must create `proof/SBxx/manifest.md` and `proof/SBxx/semantic-invariants.md`.
- Every phase has a mandatory refactor checkpoint before downstream work continues.
- Browser proof must include console logs, screenshots where visual behavior changed, diagnostics JSON, and explicit review questions.
- Package/reference proof must cover local project-reference mode and package-consumption mode where possible.

## Bundle structure

- `inputs/` preserves raw request and source references.
- `analysis/` records current state, risks, reopen triggers and QA concerns.
- `requirements/` normalizes the hardening requirements.
- `architecture/` defines the target layers and boundary rules.
- `plan/` contains the dependency map and phase gates.
- `subbundles/` contains implementation-ready phases.
- `templates/` contains proof, invariant, and refactor-gate templates.
- `traceability/` maps requirements, source observations and subbundles.
- `shared-prompts/` contains reusable prompts for Codex and QA.
- `reviews/` contains preparation QA review and execution report skeleton.
- `evidence/` contains source summaries prepared during bundle creation.
- `scripts/` contains a local structural validator for this bundle.

## Execution order summary

```mermaid
flowchart TD
    SB01[SB01 Cross-repo audit]
    SB02[SB02 JS runtime correctness]
    SB03[SB03 Patch transactions + revisions]
    SB04[SB04 Incremental updates + perf]
    SB05[SB05 Resource ownership + asset cache]
    SB06[SB06 Scene docs + diagnostics]
    SB07[SB07 Forced WebGlLib boundary refactor]
    SB08[SB08 WebGlRunLib contracts]
    SB09[SB09 WebGlRunLib runtime integration]
    SB10[SB10 Economy WebGlBridge strict mapping]
    SB11[SB11 Economy generic scenarios + scale]
    SB12[SB12 Cross-repo packaging integration]
    SB13[SB13 Browser/perf/memory proof]
    SB14[SB14 Final QA closure]

    SB01 --> SB02 --> SB03 --> SB04 --> SB05
    SB03 --> SB06
    SB04 --> SB07
    SB05 --> SB07
    SB06 --> SB07
    SB07 --> SB08 --> SB09 --> SB10 --> SB11
    SB09 --> SB12
    SB10 --> SB12
    SB12 --> SB13 --> SB14
```

## Prepared-stage validation

From the bundle root, run:

```powershell
python scripts/validate_bundle.py --stage prepared --profile initiative
```

The script checks required root sections, subbundle README sections, dependency-map sections, traceability files, XLSX presence, and critical proof placeholders.

## Companion XLSX

Open `CanDoItAll_WebGlEngine_Economy_Hardening_Checklists.xlsx` for sortable requirements, source references, subbundle checklist, QA gates, risks, and test matrix.

## Preparation QA status

Prepared validation passed locally:

```text
Bundle validation passed for stage=prepared, profile=initiative, subbundles=14
```

Final preparation inspection is stored in `reviews/02-senior-qa-inspector-final-check.md`.
