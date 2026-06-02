# Proof Manifest - SB04

## Status

Completed.

## Changed Files

- Economy sandbox contracts and session service.
- Scenario catalog implementations.
- Session tests.
- Hashes: `proof/SB12/transcripts/changed-file-hashes.txt`.

## Command Transcripts

- `proof/SB04/transcripts/pathless-session-api-source-scan.txt`
- `proof/SB03/transcripts/economy-sandbox-focused-tests-release-after-restore.txt`

## Browser Artifacts

N/A. Browser proof for pathless scenario selection is under SB09.

## Source Assertions

- Public APIs include `LoadScenario`, `TryLoadScenario`, `ExportSessionAsync`, and `ImportSessionAsync`.
- Session export includes scenario id/title/version/pack hash and import resolves catalog scenarios without relying on the original file path.
- Pack-hash mismatch rejects import.

## Gate Decision

Passed. Portable scenario references now survive moved exports while rejecting stale packs.
