# Proof manifest SB01

Status: Completed

## Scope

Branch, dirty-state, boundary, audit, and baseline build inventory before production code edits.

## Changed Files

No production files were changed in SB01. Bundle repair and proof files are hashed in:

- `bundle://proof/SB01/source-assertions/changed-file-hashes.txt`

## Command Transcripts

- Prepared validator: `bundle://proof/SB01/transcripts/prepared-validator.txt`
- Components branch/status: `bundle://proof/SB01/transcripts/components-branch-status.txt`
- Economy branch/status: `bundle://proof/SB01/transcripts/economy-branch-status.txt`
- Components direct Economy reference scan: `bundle://proof/SB01/transcripts/components-direct-economy-reference-scan.txt`
- Components broad Economy text scan: `bundle://proof/SB01/transcripts/components-economy-reference-scan.txt`
- Economy Components/WebGL reference scan: `bundle://proof/SB01/transcripts/economy-components-reference-scan.txt`
- Economy boundary audit: `bundle://proof/SB01/transcripts/economy-boundary-audit.txt`
- Components build: `bundle://proof/SB01/transcripts/components-build.txt`
- Components npm audit alias check: `bundle://proof/SB01/transcripts/components-runtime-audit.txt`
- Components direct WebGL runtime audit: `bundle://proof/SB01/transcripts/components-runtime-audit-direct.txt`
- Economy build: `bundle://proof/SB01/transcripts/economy-build.txt`

## Source Assertions

- Components branch is `webgl-engine`; Economy branch is `main`.
- Components direct reference scan found no `CanDoItAll.Economy` using/project/package references in `src` or `tests` (exit code 1 means no matches).
- Economy boundary audit passed with exit code 0.
- Components build passed with 0 warnings and 0 errors.
- Economy build passed with 44 existing package warnings and 0 errors.
- `npm run audit:webgllib` is not defined, so the bundle fallback was used: `node .\tools\webgllib\audit-scene-runtime.cjs`.
- Direct WebGL runtime audit passed with 9 warning-threshold file-size warnings and no hard failures.

## Failures / Blockers

- No SB01 blocker.
- Follow-up context: the missing npm alias is recorded; direct node audit is the authoritative SB01 runtime-audit proof.
