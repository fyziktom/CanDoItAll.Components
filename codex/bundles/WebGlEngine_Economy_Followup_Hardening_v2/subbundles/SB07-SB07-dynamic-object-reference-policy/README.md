# SB07 — Dynamic object reference validation policy

## Objective

Decide whether Economy WebGL bridge v1 supports dynamic object creation or explicitly enforces static initial-scene object ids.

## Status

Completed 2026-06-02.

## Covered Inputs

- `bundle://inputs/raw-user-request.md`
- `bundle://analysis/02-critical-findings.md`
- `bundle://requirements/01-normalized-requirements.md`

## Prerequisites

Previous dependency subbundles from `bundle://plan/01-phase-plan.md` must be completed or explicitly reopened. Review their proof manifests before editing.

## Exact Source References

- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunValidator.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Patching/WebGlScenePatch.cs`

## Deliverables

- Static-only or dynamic-supported policy committed to docs.
- Validator behavior aligns with policy.
- Tests include add-object-then-motion reference cases or explicit rejection.

## Dependency Impact

Conditional critical. Downstream subbundles rely on this phase to avoid validating against stale or ambiguous behavior.

## Validation Depth

- Failing-first test showing current static-only assumption or dynamic false failure.
- Passing test for chosen policy.
- Economy scenario inventory stating whether existing examples need dynamic objects.

## Implementation Steps

- Audit current and planned scenarios for add/remove object patches.
- If static-only: document and test that bridge rejects dynamic graph patches with clear errors.
- If dynamic-supported: update validator to simulate object id set through frame/stage sequence and validate references against evolving scene state.
- Ensure future production-line simulations have a documented extension path.

## Scope Exceptions

No scope exceptions were taken. The selected policy is dynamic-supported: objects introduced by earlier patches/stages/frames are valid targets for later commands, while same-stage motion to a newly created object remains invalid.

## Do Not Do

- Do not introduce Economy, production-line, ledger, market, account, buyer, seller, price, station, machine, or work-order semantics into Components packages.
- Do not close this subbundle with source-grep proof only.
- Do not depend on global NuGet cache when package-mode behavior is under test.
- Do not treat browser screenshots as proof unless route, action, viewport, console status, and assertions are recorded.
- Do not silently widen or shrink this subbundle scope.

## Acceptance Checklist

- [x] Objective for SB07 is implemented or explicitly blocked with a concrete reason.
- [x] Changed files are listed with hashes in the proof manifest.
- [x] Failing-first proof exists for critical behavior changes.
- [x] Passing proof exercises production code paths, not only fixtures/stubs.
- [x] Boundary and anti-stub scans are recorded.
- [x] Downstream reopen triggers are updated if reality differs from this plan.

## Proof Required

- Failing-first test showing current static-only assumption or dynamic false failure.
- Passing test for chosen policy.
- Economy scenario inventory stating whether existing examples need dynamic objects.

Critical subbundles must also create/update `proof/SB07/semantic-invariants.md` with shallow-pass trap, negative proof, positive proof, production assertions, and raw requirement closure.

## Browser Validation Logging

N/A. SB07 changed validator behavior and tests only; no browser-visible runtime or UI behavior changed.

## Progression Gate

SB07 may close only after proof manifest, semantic invariants when critical, command/browser transcripts, and source assertions agree. If any downstream prerequisite is affected, update `bundle://plan/01-phase-plan.md` and reopen impacted phases.

## Suggested Agent Prompt

You are a senior C# / Blazor / vanilla JavaScript implementation agent. Execute only this subbundle. Read the exact source references, run the entry gate, implement the smallest correct changes, add failing-first and passing semantic proof, update `proof/SB07/manifest.md`, and stop after the progression gate passes. If current repo observations contradict this README, repair the bundle before editing production code.
