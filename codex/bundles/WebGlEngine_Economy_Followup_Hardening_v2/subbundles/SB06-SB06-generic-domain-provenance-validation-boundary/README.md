# SB06 — Generic/domain provenance validation boundary

## Objective

Clarify how generic WebGlRun validators treat domain provenance without allowing domain semantics into generic contracts.

## Status

Prepared. Not implemented.

## Covered Inputs

- `bundle://inputs/raw-user-request.md`
- `bundle://analysis/02-critical-findings.md`
- `bundle://requirements/01-normalized-requirements.md`

## Prerequisites

Previous dependency subbundles from `bundle://plan/01-phase-plan.md` must be completed or explicitly reopened. Review their proof manifests before editing.

## Exact Source References

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Actions/WebGlRunActionPlanValidator.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunProjector.cs`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunValidator.cs`

## Deliverables

- Documented distinction between domain provenance and domain semantic leakage.
- Generic validator test allowing/denying the intended cases.
- Economy bridge test showing source provenance is preserved and validated.

## Dependency Impact

Critical foundation. Downstream subbundles rely on this phase to avoid validating against stale or ambiguous behavior.

## Validation Depth

- Negative test with domain term in generic action kind/stage id rejected.
- Positive test with `source.*` provenance accepted if that policy is chosen.
- Boundary audit still rejects Components source files that reference Economy namespaces or domain projects.

## Implementation Steps

- Decide and implement policy for `source.*` metadata and domain bridge provenance.
- Update `WebGlRunDocumentValidator` and `WebGlRunActionPlanValidator` if needed.
- Ensure WebGlRunLib still rejects domain terms in generic contract fields, action kinds, stage ids, non-source metadata, and public API names.
- Ensure Economy bridge can validate its output through the appropriate validator stack without hiding provenance.

## Scope Exceptions

No scope exceptions are allowed unless this README is updated with an explicit exception, proof path, and follow-up owner.

## Do Not Do

- Do not introduce Economy, production-line, ledger, market, account, buyer, seller, price, station, machine, or work-order semantics into Components packages.
- Do not close this subbundle with source-grep proof only.
- Do not depend on global NuGet cache when package-mode behavior is under test.
- Do not treat browser screenshots as proof unless route, action, viewport, console status, and assertions are recorded.
- Do not silently widen or shrink this subbundle scope.

## Acceptance Checklist

- [ ] Objective for SB06 is implemented or explicitly blocked with a concrete reason.
- [ ] Changed files are listed with hashes in the proof manifest.
- [ ] Failing-first proof exists for critical behavior changes.
- [ ] Passing proof exercises production code paths, not only fixtures/stubs.
- [ ] Boundary and anti-stub scans are recorded.
- [ ] Downstream reopen triggers are updated if reality differs from this plan.

## Proof Required

- Negative test with domain term in generic action kind/stage id rejected.
- Positive test with `source.*` provenance accepted if that policy is chosen.
- Boundary audit still rejects Components source files that reference Economy namespaces or domain projects.

Critical subbundles must also create/update `proof/SB06/semantic-invariants.md` with shallow-pass trap, negative proof, positive proof, production assertions, and raw requirement closure.

## Browser Validation Logging

N/A unless this subbundle changes browser-visible runtime or UI. If browser-visible behavior is touched, record route, viewport, actions, assertions, screenshot paths, console log, and result.

## Progression Gate

SB06 may close only after proof manifest, semantic invariants when critical, command/browser transcripts, and source assertions agree. If any downstream prerequisite is affected, update `bundle://plan/01-phase-plan.md` and reopen impacted phases.

## Suggested Agent Prompt

You are a senior C# / Blazor / vanilla JavaScript implementation agent. Execute only this subbundle. Read the exact source references, run the entry gate, implement the smallest correct changes, add failing-first and passing semantic proof, update `proof/SB06/manifest.md`, and stop after the progression gate passes. If current repo observations contradict this README, repair the bundle before editing production code.
