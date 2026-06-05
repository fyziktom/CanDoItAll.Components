# SB17 - Final cross-repo red-team closure

Status: Completed

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

- `proof/SB17/final-red-team-report.md`
- `proof/SB17/transcripts/bundle-validator.txt`
- `proof/SB17/artifact-inventory.json`
- `proof/SB17/final-decision-matrix.json`
- `proof/SB17/browser-proof-summary.json`
- `proof/SB17/domain-leakage-scan.txt`

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

## Closure notes

- Focused final tests passed: Economy 14/14, WebGlRun 20/20, WebGlLib 25/25.
- Browser proof verification passed for SB02-SB05 and SB12, including screenshot and console-log checks.
- Domain leakage scan passed with zero blocking affected-source matches.
- Final catalog run produced three scenario manifests: `multi-goods-elite:headless-valid`, `shared-well:failed`, and `farmer-land:failed`.
- Final decision matrix keeps every scenario exploratory/not research-ready because the final catalog run used `--no-oracle`; `multi-goods-elite` is also browser-observer-valid through SB12 proof.
