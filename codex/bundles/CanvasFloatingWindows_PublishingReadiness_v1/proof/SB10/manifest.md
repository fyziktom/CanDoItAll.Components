# SB10 Proof Manifest

## Status

- Result: Passed.
- Date: 2026-06-29.
- Scope: Final red-team closure and transfer gate for Canvas/Floating Windows publishing readiness.

## Command Proof

- Final focused tests: `bundle://proof/SB10/transcripts/dotnet-test-final-in-scope.txt`
- Final generated asset verification: `bundle://proof/SB10/transcripts/npm-canvaslib-verify-assets-final.txt`
- Proof inventory audit: `bundle://proof/SB10/transcripts/proof-inventory-audit.txt`
- WebGL exclusion source assertion: `bundle://proof/SB10/webgl-exclusion-source-assertion.txt`
- Runtime dependency red-team: `bundle://proof/SB10/runtime-dependency-red-team.txt`
- Changed-file hashes: `bundle://proof/SB10/transcripts/changed-file-hashes.txt`
- Anti-stub audit: `bundle://proof/SB10/transcripts/anti-stub-audit.txt`
- Semantic adequacy: `bundle://proof/SB10/transcripts/semantic-adequacy.txt`
- Completed-stage validator: `bundle://reviews/completed-validation.txt`
- Representative SHA-256: `3BE303EF891E6017B9F09B81F5BA9841797821E743BAB94AD51DD70CBBFDC389`
- Failing-first: N/A process/no production behavior change for final closure artifacts; SB10 performs red-team audit and transfer proof without changing runtime behavior.
- Passing transcript: `bundle://proof/SB10/transcripts/dotnet-test-final-in-scope.txt`

## Review Artifacts

- Final proof audit: `bundle://proof/SB10/final-proof-audit.md`
- Raw-note closure: `bundle://proof/SB10/raw-note-closure.md`
- Fake-proof resistance: `bundle://proof/SB10/fake-proof-resistance.md`
- Open-source transfer checklist: `bundle://proof/SB10/open-source-transfer-checklist.md`
- Semantic invariants: `bundle://proof/SB10/semantic-invariants.md`

## Gate Decision

- Bundle may be marked complete.
- WebGL remains excluded and should be handled by a separate future bundle.

