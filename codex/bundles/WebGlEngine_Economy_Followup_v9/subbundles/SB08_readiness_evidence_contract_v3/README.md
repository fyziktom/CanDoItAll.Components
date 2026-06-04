# SB08 — Readiness evidence contract v3

## Goal

Make readiness runtime/UI/oracle bands artifact-backed, not boolean-backed.

## Scope

Repository scope: **Economy**  
Priority: **P0**

## Required implementation work

- Introduce EvidenceRef model with kind/path/hash/schemaVersion.
- Bands can be exercised only when required evidence validators pass.
- ResearchReady requires evidence validators, not only flags.


## Required proof

- readiness-report-v3 tests
- broken evidence negative test


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
