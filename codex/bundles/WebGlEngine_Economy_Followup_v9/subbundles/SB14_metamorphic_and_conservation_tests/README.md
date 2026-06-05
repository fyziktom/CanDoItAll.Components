# SB14 — Metamorphic and conservation tests

## Goal

Add property/metamorphic tests to catch simulator noise independent of specific scenario expectations.

## Scope

Repository scope: **Economy**  
Priority: **P1**

Status: **Completed**

## Required implementation work

- Conservation with closed resources.
- Monotonicity under increased transfer magnitude where capacity allows.
- Permutation invariance for independent events.
- No hidden drift under replay.


## Required proof

- property test reports
- negative mutation proof

## Executed proof

- `proof/SB14/property-test-report.json` records passing conservation, monotonicity, independent-event permutation, and replay drift properties.
- `proof/SB14/negative-mutation-proof.json` records the deliberate `store.buyer.grain += 1` final-frame mutation and confirms the conservation detector fails it.
- `proof/SB14/transcripts/metamorphic-property-tests.txt` passes 5/5 focused metamorphic/property tests.
- `proof/SB14/transcripts/source-assertions.txt` verifies the test suite and proof reports cover all required properties.
- `proof/SB14/transcripts/anti-stub-audit.txt` finds no stub/placeholder/not-implemented markers in the new test file.


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
