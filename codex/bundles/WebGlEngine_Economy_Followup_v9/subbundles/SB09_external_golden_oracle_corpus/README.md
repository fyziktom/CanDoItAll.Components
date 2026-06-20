# SB09 — External golden oracle corpus

## Goal

Move/duplicate golden oracle scenarios into external JSON corpus with stable expected outputs and diffs.

## Scope

Repository scope: **Economy**  
Priority: **P1**

Status: **Completed**

## Required implementation work

- Create golden oracle pack files for current cases.
- Oracle runner must compare final stores, flows, relationships, issues, metrics, invariants and hash chain.
- Broken expected file must fail with path-addressed diff.


## Required proof

- oracle-corpus/*.json
- oracle-report.json
- negative diff proof

## Executed proof

- `proof/SB09/oracle-corpus/economic-oracles.json` preserves the external corpus used by the GoldenOracleSuite.
- `proof/SB09/oracle-report.json` records the passing corpus report for 7 oracle cases.
- `proof/SB09/negative-diff-proof.json` records the broken expected-value diff at `metrics.resourceTotal`.
- `proof/SB09/oracle-corpus-tests.txt` passes 3/3 GoldenOracleSuite tests.
- `proof/SB09/simulationsandbox-build.txt` builds `CanDoItAll.Economy.SimulationSandbox` with 0 warnings and 0 errors.


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
