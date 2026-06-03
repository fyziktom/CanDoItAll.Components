# SB12 proof manifest

Status: Completed.

## Required proof artifacts

- Failing-first red-team audit: `bundle://proof/SB12/transcripts/failing-first.txt`
- Passing red-team test: `bundle://proof/SB12/transcripts/passing-tests.txt`
- Components build: `bundle://proof/SB12/transcripts/components-build.txt`
- Components tests: `bundle://proof/SB12/transcripts/components-tests.txt`
- Economy build: `bundle://proof/SB12/transcripts/economy-build.txt`
- Economy tests: `bundle://proof/SB12/transcripts/economy-tests.txt`
- Package-mode proof: `bundle://proof/SB12/transcripts/package-mode-proof.txt`
- Source assertions: `bundle://proof/SB12/transcripts/source-assertions.txt`
- Boundary audits: `bundle://proof/SB12/transcripts/boundary-audits.txt`
- Browser artifact audit: `bundle://proof/SB12/transcripts/browser-proof.txt`
- Senior red-team review: `bundle://proof/SB12/transcripts/red-team-review.txt`
- Final validators: `bundle://proof/SB12/transcripts/validator-audits.txt`
- Changed-file hashes: `bundle://proof/SB12/changed-file-hashes.md`
- Final execution report: `bundle://reviews/01-execution-report.md`
- SB04 browser diagnostics: `bundle://proof/SB04/browser/runtime-diagnostics.json`
- SB10 browser screenshot: `bundle://proof/SB10/browser/run-playback-after-import-step-rerender.png`
- SB11 browser diagnostics: `bundle://proof/SB11/browser/performance-proof-diagnostics.json`
- SB11 browser screenshot: `bundle://proof/SB11/browser/performance-proof-browser.png`

## Production Behavior Artifact Matrix

| Signal / behavior | Producer | Consumer | Lifecycle | Negative / assertion proof |
| --- | --- | --- | --- | --- |
| Fail-closed browser/run playback | WebGlRunLib runner and browser adapter | WebGlRunLib tests and Economy replay bridge | Frame conversion, reset, batch apply, and explicit playback apply | `bundle://proof/SB12/transcripts/components-tests.txt` |
| Shared stage ordering | `WebGlRunStageOrderingPolicy` | Validation, frame apply, playback, Economy bridge | Ordered stages flow into diagnostics and command batches | `bundle://proof/SB12/transcripts/source-assertions.txt` |
| Package-mode compatibility | SB12 local prerelease packages | WebGlLib-only sample and Economy WebGlBridge | Pack, restore, build, restore back to project-reference mode | `bundle://proof/SB12/transcripts/package-mode-proof.txt` |
| Browser replay/performance proof | SB04/SB10/SB11 browser artifacts | SB12 browser artifact audit and final report | Route JSON/screenshot artifacts retained under proof tree | `bundle://proof/SB12/transcripts/browser-proof.txt` |
| Proof integrity | `scripts/audit_proof_integrity.py` | Final bundle closure | Completed manifests cite non-empty artifacts | `bundle://proof/SB12/transcripts/validator-audits.txt` |

## Completion rules

All required proof files are non-empty and cite the command, result, and semantic assertion. SB12 is complete.
