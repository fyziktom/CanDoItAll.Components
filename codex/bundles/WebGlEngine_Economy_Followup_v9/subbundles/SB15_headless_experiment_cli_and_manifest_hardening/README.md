# SB15 — Headless experiment CLI and manifest hardening

## Goal

Make CLI/catalog runs the recommended source of economic truth and strengthen manifest diff workflow.

## Scope

Repository scope: **Economy**  
Priority: **P1**

Status: **Completed**

## Required implementation work

- CLI run all catalog scenarios including third scenario.
- Manifest diff categorizes scenario/model/policy/oracle/runtime changes.
- Approved volatile artifacts explicitly listed.


## Required proof

- CLI transcripts
- manifest-diff negative/positive proof

## Executed proof

- `proof/SB15/transcripts/cli-run-all-catalog.txt` runs `scenario run --all` against the runtime catalog and records all three scenario statuses, including `multi-goods-elite:headless-valid`.
- `proof/SB15/cli-catalog-run-summary.json` records three headless manifests and explicit `approvedVolatileArtifacts` for each.
- `proof/SB15/transcripts/manifest-diff-positive.txt` proves repeated `multi-goods-elite` headless manifests are equivalent.
- `proof/SB15/transcripts/manifest-diff-negative.txt` proves a mutated manifest reports scenario, model, policy, oracle, and runtime categories.
- `proof/SB15/transcripts/headless-cli-focused-tests.txt` passes 6/6 focused CLI/headless tests.


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
