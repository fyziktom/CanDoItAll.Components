# SB17 — Final cross-repo red-team closure

## Goal

Senior QA/architecture review of all changes before claiming ready for research usage.

## Scope

Repository scope: **Both repos**  
Priority: **P0**

## Required implementation work

- Run full focused tests and browser proofs.
- Run domain leakage scans.
- Run all three scenarios via headless runner.
- Produce final decision matrix: exploratory/headless-valid/oracle-valid/browser-observer-valid/research-ready.


## Required proof

- final-red-team-report.md
- validator output
- artifact inventory


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
