# SB13 — Design matrix factor materialization v2

## Goal

Extend factor materialization beyond current narrow bindings and prove factors actually change effective scenario hashes.

## Scope

Repository scope: **Economy**  
Priority: **P1**

Status: **Completed**

## Required implementation work

- Add safe JSON-pointer/patch factor binding or typed bindings for fees, capacities, event schedules, investment rates and policy thresholds.
- Reject no-effect factor cells.
- Record effective scenario hash per run.


## Required proof

- factor-effect-report.json
- design matrix summary with distinct hashes

## Executed proof

- `proof/SB13/factor-effect-report.json` records two materialized factor cells, two distinct effective scenario hashes, two distinct configuration hashes, six applied effects per run, and rejected `design-factor-no-effect:unchangedPointer`.
- `proof/SB13/design-matrix-summary.json` contains `effectiveScenarioHash`, `originalScenarioHash`, `appliedEffects`, and `metadata.distinctEffectiveScenarioHashCount = 2`.
- `proof/SB13/transcripts/design-harness-focused-tests.txt` passes 2/2 focused design-harness tests.
- `proof/SB13/transcripts/source-assertions.txt` verifies the safe JSON pointer binding plus typed capacity, schedule, investment-rate, and policy parameter bindings.
- `proof/SB13/transcripts/anti-stub-audit.txt` finds no stub/placeholder/not-implemented markers in the touched source/test files.


## Hard gates

- No placeholder proof files.
- No empty transcript may be referenced as passing proof.
- Every changed production behavior must have failing-first or negative proof where feasible.
- Browser proof must include screenshot, console logs, diagnostics JSON and explicit assertions when the subbundle touches UI/runtime behavior.
- If any gate cannot be completed, stop and write a `REOPEN.md` with exact remaining work.

## QA review prompts

- Does the change reduce simulator noise or merely document it?
- Does the change keep Components generic?
- Does the change separate headless economic truth from browser observer evidence?
- Could a scenario pass because of fallback/default behavior instead of intended economics?
