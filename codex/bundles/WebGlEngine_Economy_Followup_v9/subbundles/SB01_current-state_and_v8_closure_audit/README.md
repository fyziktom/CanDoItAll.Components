# SB01 — Current-state and v8 closure audit

## Goal

Audit all v8 requirements against current Components/Economy code and evidence.

## Scope

Repository scope: **Both repos**  
Priority: **P0**

## Required implementation work

- Map every v8 requirement to code/proof.
- Mark Done/Partial/Missing with evidence path.
- Fail if any proof transcript is empty but referenced as pass.


## Required proof

- closure-matrix.md
- proof-hygiene-report.json


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
