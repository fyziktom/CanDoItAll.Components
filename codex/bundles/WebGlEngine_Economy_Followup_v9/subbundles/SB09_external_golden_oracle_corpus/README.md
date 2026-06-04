# SB09 — External golden oracle corpus

## Goal

Move/duplicate golden oracle scenarios into external JSON corpus with stable expected outputs and diffs.

## Scope

Repository scope: **Economy**  
Priority: **P1**

## Required implementation work

- Create golden oracle pack files for current cases.
- Oracle runner must compare final stores, flows, relationships, issues, metrics, invariants and hash chain.
- Broken expected file must fail with path-addressed diff.


## Required proof

- oracle-corpus/*.json
- oracle-report.json
- negative diff proof


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
