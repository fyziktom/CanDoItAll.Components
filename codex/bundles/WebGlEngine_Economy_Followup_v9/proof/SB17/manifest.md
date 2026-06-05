# Proof manifest - SB17

Status: completed

## Scope

SB17 is the final cross-repo red-team closure for the v9 WebGL/Economy hardening bundle. It does not introduce new production behavior. It verifies that the prior subbundles agree across source, tests, browser proof, headless catalog output, artifact inventory, and final readiness claims.

## Required Work Closure

| Requirement | Result | Proof |
| --- | --- | --- |
| Run full focused tests | Passed | `bundle://proof/SB17/transcripts/economy-final-focused-tests.txt`, `bundle://proof/SB17/transcripts/components-final-focused-tests.txt`, `bundle://proof/SB17/transcripts/webgllib-final-focused-tests.txt` |
| Verify browser proofs | Passed | `bundle://proof/SB17/browser-proof-summary.json`, `bundle://proof/SB17/transcripts/browser-proof-verification.txt` |
| Run domain leakage scans | Passed | `bundle://proof/SB17/domain-leakage-scan.txt` |
| Run all three scenarios via headless runner | Completed with expected strict outcome | `bundle://proof/SB17/transcripts/headless-all-three-scenarios.txt`, `bundle://proof/SB17/headless-catalog-run/` |
| Produce final decision matrix | Completed | `bundle://proof/SB17/final-decision-matrix.json`, `bundle://proof/SB17/final-decision-matrix.md` |
| Produce final red-team report | Completed | `bundle://proof/SB17/final-red-team-report.md` |
| Produce artifact inventory | Completed | `bundle://proof/SB17/artifact-inventory.json`, `bundle://proof/SB17/artifact-inventory.md` |

## Production Behavior Artifact Matrix

| Signal or behavior | Producer | Consumer | Lifecycle | Negative or red-team proof |
| --- | --- | --- | --- | --- |
| Runtime stop/idle and command-batch settlement | Components WebGlLib/WebGlRunLib runtime and apply adapter | RunPlayback browser observer and focused tests | Runtime emits stop generation, idle blockers, settlement status, and observer diagnostics during playback | SB02-SB05 browser proofs plus `bundle://proof/SB17/browser-proof-summary.json` |
| Generic executable boundary | Components WebGlRun document/action validators and Economy WebGL bridge | Economy bridge tests, SB12 browser proof, SB17 domain scan | Economy source provenance may retain domain terms; executable Components ids must stay generic | `bundle://proof/SB17/domain-leakage-scan.txt`, `bundle://proof/SB12/browser/multi-goods-browser-assertions.json` |
| Headless readiness and research-ready classification | Economy CLI/readiness reporter/headless manifest writer | Final decision matrix and catalog run summary | Catalog scenarios emit run summary, readiness report, and v4 manifest artifacts | `bundle://proof/SB17/final-decision-matrix.json`, `bundle://proof/SB17/headless-catalog-run/` |
| Oracle coverage separation | Economy CLI and readiness report | Final decision matrix | `--no-oracle` catalog runs cannot claim oracle-valid or research-ready status | `bundle://proof/SB17/final-decision-matrix.md` |
| Artifact hygiene | Bundle proof tree and validator | SB17 inventory and final closure | Proof files are inventoried and empty files are rejected as closure evidence | `bundle://proof/SB17/artifact-inventory.json` |

## Proof Artifacts

- Final red-team report: `bundle://proof/SB17/final-red-team-report.md`
- Final decision matrix: `bundle://proof/SB17/final-decision-matrix.json`
- Human-readable decision matrix: `bundle://proof/SB17/final-decision-matrix.md`
- Browser proof summary: `bundle://proof/SB17/browser-proof-summary.json`
- Domain leakage scan: `bundle://proof/SB17/domain-leakage-scan.txt`
- Artifact inventory: `bundle://proof/SB17/artifact-inventory.json`
- Source assertions: `bundle://proof/SB17/transcripts/source-assertions.txt`
- Anti-stub audit: `bundle://proof/SB17/transcripts/anti-stub-audit.txt`
- Changed-file hashes: `bundle://proof/SB17/transcripts/changed-file-hashes.txt`
- Bundle validator transcript: `bundle://proof/SB17/transcripts/bundle-validator.txt`
- Semantic invariants: `bundle://proof/SB17/semantic-invariants.md`

## Test Summary

- Economy focused final slice: 14 passed, 0 failed.
- Components WebGlRun focused final slice: 20 passed, 0 failed.
- Components WebGlLib focused final slice: 25 passed, 0 failed.
- Browser proof verification: 5 passed, 0 failed.

## Closure

SB17 passes. `multi-goods-elite` is headless-valid and browser-observer-valid, while all scenarios remain exploratory/not research-ready in the final matrix because the final all-catalog run intentionally used `--no-oracle`. The two legacy catalog packs remain strict failures and are explicitly classified as not research-ready rather than hidden as successful research evidence.
