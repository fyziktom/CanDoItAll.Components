# Proof Manifest - SB09

## Status

Completed.

## Changed Files

- Economy sandbox page and CSS.
- Node scenario registration.
- Component tests.
- Hashes: `proof/SB12/transcripts/changed-file-hashes.txt`.

## Command Transcripts

- `proof/SB09/transcripts/economy-runtime-fixture-path-scan.txt`
- `proof/SB03/transcripts/economy-sandbox-focused-tests-release-after-restore.txt`

## Browser Artifacts

- Route: `http://127.0.0.1:56429/economy/simulation-sandbox`
- Viewport: `1440x1000`
- Screenshot: `proof/SB09/browser/economy-sandbox-browser-proof.png`
- Diagnostics JSON: `proof/SB09/browser/economy-sandbox-browser-diagnostics.json`
- Console log: `proof/SB09/browser/economy-sandbox-console.log`
- Server logs: `proof/SB09/browser/economy-node-browser-proof.out.log`, `proof/SB09/browser/economy-node-browser-proof.err.log`

## Source Assertions

- Scenario selector displays scenario title/id, version, pack hash, and validity.
- Summary and diagnostics include scenario id/title/version/pack hash.
- Browser proof exercised login, apply frame, step, back, and scenario switch to `farmer-land`.

## Gate Decision

Passed. Operator UI exposes the new scenario/provenance diagnostics and remains browser-live.
