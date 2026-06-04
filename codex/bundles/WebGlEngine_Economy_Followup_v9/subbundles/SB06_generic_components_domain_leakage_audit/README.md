# SB06 — Generic Components domain leakage audit

## Goal

Ensure generic WebGlLib/WebGlRunLib production code does not embed economy-specific terms except in tests/audit fixtures.

## Scope

Repository scope: **Components**  
Priority: **P1**

## Required implementation work

- Move hardcoded domain terms out of production policy or make them opt-in options.
- Keep source provenance structural and non-executable.
- Add CI scan limited to src projects.


## Required proof

- domain-leakage-report.json
- negative test with economy terms supplied by test config


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
