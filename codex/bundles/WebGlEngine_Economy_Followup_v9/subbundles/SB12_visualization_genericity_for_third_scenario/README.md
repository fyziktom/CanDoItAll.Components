# SB12 — Visualization genericity for third scenario

## Goal

Render exchange/investment scenario without new domain-specific Components concepts.

## Scope

Repository scope: **Both repos**  
Priority: **P1**

Status: **Completed**

## Required implementation work

- Map portfolios/entities/exchange edges to generic objects, links, symbols and stages.
- Avoid object kinds like buyer/seller/investor in WebGlRunLib; keep them in Economy input metadata only.
- Add visual load and idle browser proof.


## Required proof

- browser observer proof for third scenario
- generic validation passes

## Executed proof

- `proof/SB12/failing-first-bridge-bound-readiness.txt` records the pre-change bridge-bound visual mapping diagnostics and missing browser observer exercise.
- `proof/SB12/transcripts/generic-visualization-tests.txt` passes 42/42 focused Economy tests for renderer binding, strict generic projection, readiness classification, and SB11 visual event coverage.
- `proof/SB12/transcripts/real-run-export.txt` exports the real `multi-goods-elite` artifacts, including `webgl.run-document.json`.
- `proof/SB12/browser/multi-goods-browser-assertions.json` verifies the generated run id, 23 objects, 12 links, one generic stage, matching document hashes, runtime idle, final positions, no genericity failures, and no disallowed console messages.
- `proof/SB12/browser/multi-goods-browser-after.png` and `proof/SB12/browser/multi-goods-browser-console.log` provide the required browser screenshot and console evidence.
- `proof/SB12/transcripts/webgl-sandbox-build.txt` builds `CanDoItAll.Components.WebGlSandbox` with 0 warnings and 0 errors.


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
