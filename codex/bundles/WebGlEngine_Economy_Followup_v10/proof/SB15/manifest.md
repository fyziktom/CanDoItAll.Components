# Proof manifest SB15

Status: pass

Required proof: CI fails on empty critical proof; proof manifest must cite non-empty artifacts.

Artifacts attached:
- `proof-validator-positive.txt` - strengthened validator passes the real bundle.
- `proof-validator-negative-empty-artifact.txt` - negative proof copied the bundle to a temp directory, zeroed a cited proof transcript, and verified the validator failed with `proof-artifact-empty`.
- `source-scan-proof-integrity-gates.txt` - source scan proving validator checks missing/empty artifacts, skipped scans, stale screenshots, screenshot assertion pairing, and CI workflow invocation.
- `changed-file-hashes.txt` - SHA-256 hashes for the validator and CI workflow changes.
- `anti-stub-scan.txt` - anti-stub scan for the validator and CI workflow changes.

Result:
Pass. The bundle validator now enforces closed proof manifests (`Status: pass/completed`) by resolving cited bundle artifacts, rejecting missing/empty/blank artifacts, rejecting scan transcripts that say the scan was skipped, and requiring screenshots to be paired with fresh machine-readable assertion or diagnostic artifacts. The Components CI workflow now runs this validator on push and pull request.

Production Behavior Artifact Matrix:

| Behavior | Production artifact | Proof artifact |
| --- | --- | --- |
| Empty cited proof artifact fails validation | `scripts/validate_bundle.py` | `proof-validator-negative-empty-artifact.txt` |
| Closed proof manifests must cite non-empty artifacts | `scripts/validate_bundle.py` | `proof-validator-positive.txt`, `source-scan-proof-integrity-gates.txt` |
| Bundle proof gate runs in CI | `.github/workflows/domain-leakage.yml` | `source-scan-proof-integrity-gates.txt` |
