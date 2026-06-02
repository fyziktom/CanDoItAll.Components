# Proof Manifest - SB06

## Status

Completed.

## Changed Files

- WebGlRun frame apply result and browser apply adapter.
- WebGlRun tests.
- Hashes: `proof/SB12/transcripts/changed-file-hashes.txt`.

## Command Transcripts

- `proof/SB06/transcripts/components-webglrunlib-tests-release.txt`
- `proof/SB08/transcripts/components-webglrunlib-boundary-audit.txt`

## Browser Artifacts

Browser behavior is covered by SB09.

## Source Assertions

- Mixed direct/staged frames produce an error result with an inert command batch and `blockedByPolicy` metadata.
- Browser apply does not apply batches when frame validation already failed.
- Reset failure or missing initial scene fails fast without applying the frame batch.

## Gate Decision

Passed. Public frame apply APIs fail safely even if caller skips prior validation.
