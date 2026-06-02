# SB10 — Economy simulation docs and public surface audit

## Objective

Refresh Economy documentation and package boundary docs to reflect the new simulation subsystem without making it domain-specific or misleading.

## Status

Completed 2026-06-02.

## Covered Inputs

- `bundle://inputs/raw-user-request.md`
- `bundle://analysis/02-critical-findings.md`
- `bundle://requirements/01-normalized-requirements.md`

## Prerequisites

Previous dependency subbundles from `bundle://plan/01-phase-plan.md` must be completed or explicitly reopened. Review their proof manifests before editing.

## Exact Source References

- `repo://CanDoItAll.Economy/README.md`
- `repo://CanDoItAll.Economy/docs/simulation/architecture-boundaries.md`
- `repo://CanDoItAll.Economy/CanDoItAll.Economy.slnx`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.*`

## Deliverables

- README no longer presents the repo as only a ledger module.
- Simulation docs include dependency diagram and package map.
- Public surface and extension points are documented.

## Dependency Impact

Important. Downstream subbundles rely on this phase to avoid validating against stale or ambiguous behavior.

## Validation Depth

- Documentation review checklist.
- Source package boundary scan.
- No browser proof required unless docs links are hosted in a UI.

## Implementation Steps

- Update root README and docs/simulation to include simulation architecture map.
- Document package/project boundaries for Abstractions, Visualization, SimpleAccounts, Ledger, WebGlBridge, SimulationSandbox, Components, and Node.
- Add public API surface inventory or package-readiness notes.
- Document how future production-line simulators can reuse WebGlRunLib without Economy-specific leakage.

## Scope Exceptions

No scope exceptions are allowed unless this README is updated with an explicit exception, proof path, and follow-up owner.

## Do Not Do

- Do not introduce Economy, production-line, ledger, market, account, buyer, seller, price, station, machine, or work-order semantics into Components packages.
- Do not close this subbundle with source-grep proof only.
- Do not depend on global NuGet cache when package-mode behavior is under test.
- Do not treat browser screenshots as proof unless route, action, viewport, console status, and assertions are recorded.
- Do not silently widen or shrink this subbundle scope.

## Acceptance Checklist

- [x] Objective for SB10 is implemented or explicitly blocked with a concrete reason.
- [x] Changed files are listed with hashes in the proof manifest.
- [x] Failing-first proof exists for critical behavior changes.
- [x] Passing proof exercises production code paths, not only fixtures/stubs.
- [x] Boundary and anti-stub scans are recorded.
- [x] Downstream reopen triggers are updated if reality differs from this plan.

## Proof Required

- Documentation review checklist.
- Source package boundary scan.
- No browser proof required unless docs links are hosted in a UI.

Critical subbundles must also create/update `proof/SB10/semantic-invariants.md` with shallow-pass trap, negative proof, positive proof, production assertions, and raw requirement closure.

## Browser Validation Logging

N/A unless this subbundle changes browser-visible runtime or UI. If browser-visible behavior is touched, record route, viewport, actions, assertions, screenshot paths, console log, and result.

## Progression Gate

SB10 may close only after proof manifest, semantic invariants when critical, command/browser transcripts, and source assertions agree. If any downstream prerequisite is affected, update `bundle://plan/01-phase-plan.md` and reopen impacted phases.

## Suggested Agent Prompt

You are a senior C# / Blazor / vanilla JavaScript implementation agent. Execute only this subbundle. Read the exact source references, run the entry gate, implement the smallest correct changes, add failing-first and passing semantic proof, update `proof/SB10/manifest.md`, and stop after the progression gate passes. If current repo observations contradict this README, repair the bundle before editing production code.
