# SB16 — Performance and comparability budgets for third scenario

## Goal

Add headless and browser observer budgets for new scenario and prevent noisy comparisons.

## Scope

Repository scope: **Both repos**  
Priority: **P2**

## Required implementation work

- Measure materialization/projection/metrics/snapshot/browser settle.
- Headless budget failures -> not-comparable.
- Browser budget failures -> observer warning only.


## Required proof

- performance-budget-report.json
- large-run stress proof


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
