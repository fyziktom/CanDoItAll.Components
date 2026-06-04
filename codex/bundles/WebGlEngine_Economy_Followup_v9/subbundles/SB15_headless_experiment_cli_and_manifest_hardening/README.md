# SB15 — Headless experiment CLI and manifest hardening

## Goal

Make CLI/catalog runs the recommended source of economic truth and strengthen manifest diff workflow.

## Scope

Repository scope: **Economy**  
Priority: **P1**

## Required implementation work

- CLI run all catalog scenarios including third scenario.
- Manifest diff categorizes scenario/model/policy/oracle/runtime changes.
- Approved volatile artifacts explicitly listed.


## Required proof

- CLI transcripts
- manifest-diff negative/positive proof


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
