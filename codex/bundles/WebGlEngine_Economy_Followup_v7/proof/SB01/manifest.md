# Proof manifest - SB01

Status: completed

## Required artifacts

- `bundle://proof/SB01/proof-integrity-report.md`
- `bundle://proof/SB01/transcripts/current-file-inventory.txt`
- `bundle://proof/SB01/transcripts/proof-hygiene-scan.txt`
- `bundle://proof/SB01/transcripts/v6-components-completed-validator.txt`
- `bundle://proof/SB01/transcripts/run-playback-pause-before-playwright.txt`
- `bundle://proof/SB01/browser/run-playback-pause-before.cjs`
- `bundle://proof/SB01/browser/run-playback-pause-before.json`
- `bundle://proof/SB01/browser/run-playback-pause-before.png`

## Changed-file and artifact hashes

See `bundle://proof/SB01/transcripts/sb01-artifact-hashes.txt`.

## Commands

| Command | Transcript | Result |
|---|---|---|
| `python scripts/validate_bundle.py --stage prepared --profile initiative` | prepared-stage console output in execution context | Passed |
| `python scripts/validate_bundle.py --stage completed --profile initiative` from v6 bundle | `bundle://proof/SB01/transcripts/v6-components-completed-validator.txt` | Passed |
| `node proof/SB01/browser/run-playback-pause-before.cjs ...` | `bundle://proof/SB01/transcripts/run-playback-pause-before-playwright.txt` | Failed one baseline assertion, intentionally retained as pre-SB02 finding |

## Semantic adequacy

- Shallow-pass trap: accepting v6 report text or prepared v7 manifests as proof would miss current browser behavior.
- Adversarial negative proof: `run-playback-pause-before.json` records the immediate post-pause UI lag while browser runtime eventually reports idle.
- Semantic positive proof: the same artifact shows public browser runtime diagnostics draining active motions, queued motions, and queued command stages to zero.
- Anti-stub audit: `proof-hygiene-scan.txt` records no zero-length text proof/report files in Components after SB01 capture and marks old Economy zero-length logs invalid for v7 proof.

## Production Behavior Artifact Matrix

No new production signal, state, record, or event was introduced by SB01.

