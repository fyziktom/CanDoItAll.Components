# SB05 Sandbox Taxonomy And Standard Coverage Expansion

## Status

- Status: `Completed`

## Objective

Make the sandbox a reliable standard-component proof harness with logical groups and one-by-one component coverage, while separating Canvas/WebGL proof surfaces.

## Covered Inputs

- RAW04: Analyze sandbox missing components and logical grouping.
- RAW10: Real Playwright screenshots one by one, including interactive states.

## Prerequisites

- SB01 inventory accepted.
- SB03 public component taxonomy known.

## Exact Source References

- repo://src/CanDoItAll.Components.Sandbox/SandboxCatalogRegistry.cs
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages
- repo://src/CanDoItAll.Components.Sandbox/Components/Routes.razor

## Deliverables

- Revised group taxonomy.
- Coverage matrix for every standard component.
- Focused routes/test hooks for Playwright proof.

## Dependency Impact

- Unlocks all visual hardening subbundles.
- If sandbox coverage is weak, screenshots cannot prove publishing readiness.

## Validation Depth

- Critical Semantic Adequacy Gate.
- Coverage count assertions.
- Playwright route smoke for every group.
- Critical foundation: before closure, create `proof/SB05/manifest.md` and `proof/SB05/semantic-invariants.md` with Semantic Adequacy Gate evidence, changed-file hashes, transcripts, source assertions, anti-stub audit, and raw-note literal closure.


## Implementation Steps

- Split standard proof groups from Canvas/WebGL entries.
- Add missing component examples and scenario coverage.
- Expose stable routes/test IDs for one-by-one validation.

## Scope Exceptions

- WebGL and Canvas implementation changes are excluded unless this subbundle explicitly updates sandbox separation metadata without touching their component internals.

## Do Not Do

- Do not refactor unrelated WebGL or Canvas source.
- Do not delete compatibility or AppComponents code without migration proof and consumer checks.
- Do not close UI work from source inspection only.

## Acceptance Checklist

- Every standard component has an owning group and proof route or a documented exception.
- Canvas/WebGL entries are excluded or clearly separated.
- Playwright can navigate to each planned route.

## Proof Required

- Sandbox route smoke transcript.
- Coverage matrix updated in xlsx or generated JSON.
- Screenshots of group index and representative focused route.

## Browser Validation Logging

- Routes: every standard group route plus focused routes introduced by this subbundle.
- Viewports: desktop and mobile.
- Result rows must include component coverage count and screenshot path.

## Progression Gate

- The subbundle validator must pass closure review before downstream dependent subbundles start.
- If proof is weak or a screenshot shows wrapping, clipping, layout, available-space, or interaction defects, keep this subbundle `In progress` and reopen prerequisites as needed.
- Closure proof accepted at `bundle://proof/SB05/manifest.md`.
- Semantic invariants accepted at `bundle://proof/SB05/semantic-invariants.md`.
- Progression result: `Passed`; proceed to SB06-SB09 component group hardening.

## Suggested Agent Prompt

Execute SB05 by treating the sandbox as test infrastructure. Split scope cleanly, then add missing standard-component demos with stable proof hooks.
