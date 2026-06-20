# SB16 — Performance and comparability budgets for third scenario

## Goal

Add headless and browser observer budgets for new scenario and prevent noisy comparisons.

## Scope

Repository scope: **Both repos**  
Priority: **P2**

Status: **Completed**

## Required implementation work

- Measure materialization/projection/metrics/snapshot/browser settle.
- Headless budget failures -> not-comparable.
- Browser budget failures -> observer warning only.


## Required proof

- performance-budget-report.json
- large-run stress proof

## Executed proof

- `proof/SB16/performance-budget-report.json` measures `multi-goods-elite` materialization, projection, metrics, snapshot build/serialization, and browser batch settle under the large profile.
- `proof/SB16/performance-budget-report.json` also records negative comparability proof: headless overage drives `not-comparable`, browser-only overage remains a warning.
- `proof/SB16/large-run-stress-proof.json` records the 100 actor / 500 resource / 1000 event stress run and budget report.
- `proof/SB16/webglrun-large-playback-budget-metrics.json` records Components WebGlRun large playback metrics.
- `proof/SB16/transcripts/*budget*.txt` and `proof/SB16/transcripts/large-run-stress-test.txt` pass the focused test runs.


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
