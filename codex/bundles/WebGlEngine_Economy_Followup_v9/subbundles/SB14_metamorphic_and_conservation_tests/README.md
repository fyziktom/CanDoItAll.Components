# SB14 — Metamorphic and conservation tests

## Goal

Add property/metamorphic tests to catch simulator noise independent of specific scenario expectations.

## Scope

Repository scope: **Economy**  
Priority: **P1**

## Required implementation work

- Conservation with closed resources.
- Monotonicity under increased transfer magnitude where capacity allows.
- Permutation invariance for independent events.
- No hidden drift under replay.


## Required proof

- property test reports
- negative mutation proof


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
