# SB03 — WebGlRun frame command preservation policy

## Objective

Prevent silent command loss when a run frame contains both direct frame-level commands and staged commands.

## Status

Completed 2026-06-02. Gate passed.

## Covered Inputs

- `bundle://inputs/raw-user-request.md`
- `bundle://analysis/02-critical-findings.md`
- `bundle://requirements/01-normalized-requirements.md`

## Prerequisites

Previous dependency subbundles from `bundle://plan/01-phase-plan.md` must be completed or explicitly reopened. Review their proof manifests before editing.

## Exact Source References

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunFrameApplyResult.cs`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs`
- `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/`
- `repo://CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs`

## Deliverables

- No valid `WebGlRunFrame` can lose commands during apply-result generation.
- If mixed commands are rejected, validators must reject before playback.
- If mixed commands are preserved, implicit stage ids must be deterministic and documented.

## Dependency Impact

Critical foundation. Downstream subbundles rely on this phase to avoid validating against stale or ambiguous behavior.

## Validation Depth

- Failing-first test demonstrating previous command loss.
- Passing test for chosen behavior.
- Economy bridge focused test proving generated frames comply with the chosen policy.
- Browser route `/run-playback` still applies batch proof frame.

## Implementation Steps

- Decide between rejecting mixed direct+staged frames or generating deterministic implicit stages.
- Implement validator/compiler behavior consistently.
- Update `WebGlRunFrameApplyResult.FromFrame` so it cannot silently drop direct patches/motions.
- Add tests for direct-only, staged-only, mixed-invalid or mixed-preserved behavior.

## Scope Exceptions

No scope exceptions are allowed unless this README is updated with an explicit exception, proof path, and follow-up owner.

## Do Not Do

- Do not introduce Economy, production-line, ledger, market, account, buyer, seller, price, station, machine, or work-order semantics into Components packages.
- Do not close this subbundle with source-grep proof only.
- Do not depend on global NuGet cache when package-mode behavior is under test.
- Do not treat browser screenshots as proof unless route, action, viewport, console status, and assertions are recorded.
- Do not silently widen or shrink this subbundle scope.

## Acceptance Checklist

- [x] Objective for SB03 is implemented or explicitly blocked with a concrete reason.
- [x] Changed files are listed with hashes in the proof manifest.
- [x] Failing-first proof exists for critical behavior changes.
- [x] Passing proof exercises production code paths, not only fixtures/stubs.
- [x] Boundary and anti-stub scans are recorded.
- [x] Downstream reopen triggers are updated if reality differs from this plan.

## Completion Notes

- Chose Option A from `architecture/03-run-frame-command-semantics.md`: mixed direct frame-level commands and staged commands are invalid.
- Added `WebGlRunFrameCommandPolicy` and wired it into `WebGlRunDocumentValidator` and `WebGlRunFrameApplyResult.FromFrame`.
- Removed staged-command mirroring from `WebGlRunActionCompiler` and `EconomyWebGlActionStageProjector`.
- Updated timeline identity hashing to include staged command payloads.
- Added failing-first and passing tests for mixed rejection, direct-only/staged-only validity, staged-only compiler output, staged identity, and Economy bridge compliance.
- Captured `/run-playback` browser proof showing `Batch frame` applies frame 4 with 24 commands and 24 stages and no console errors/warnings.

## Proof Required

- Failing-first test demonstrating previous command loss.
- Passing test for chosen behavior.
- Economy bridge focused test proving generated frames comply with the chosen policy.
- Browser route `/run-playback` still applies batch proof frame.

Critical subbundles must also create/update `proof/SB03/semantic-invariants.md` with shallow-pass trap, negative proof, positive proof, production assertions, and raw requirement closure.

## Browser Validation Logging

N/A unless this subbundle changes browser-visible runtime or UI. If browser-visible behavior is touched, record route, viewport, actions, assertions, screenshot paths, console log, and result.

## Progression Gate

SB03 may close only after proof manifest, semantic invariants when critical, command/browser transcripts, and source assertions agree. If any downstream prerequisite is affected, update `bundle://plan/01-phase-plan.md` and reopen impacted phases.

## Suggested Agent Prompt

You are a senior C# / Blazor / vanilla JavaScript implementation agent. Execute only this subbundle. Read the exact source references, run the entry gate, implement the smallest correct changes, add failing-first and passing semantic proof, update `proof/SB03/manifest.md`, and stop after the progression gate passes. If current repo observations contradict this README, repair the bundle before editing production code.
