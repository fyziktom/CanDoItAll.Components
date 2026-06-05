# Proof manifest - SB02

Status: completed

## Artifacts

- Negative scanner probe: `proof/SB02/transcripts/domain-boundary-audit-negative-probe.txt`
- Broad boundary scan: `proof/SB02/transcripts/domain-boundary-audit-webglrunlib.txt`
- Config: `tools/webgllib/domain-boundary-audit.config.json`
- CI workflow: `.github/workflows/domain-leakage.yml`

## Result

Passed. Domain terms now come from an explicit registry, CI points at v11, and the audit covers generic source, tests, docs, tools/workflows, and active bundle artifacts with reasoned allowlists.
