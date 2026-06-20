# SB15 Semantic Invariants

Status: completed

## Invariants

- `scenario run --all` must enumerate every valid catalog descriptor and produce a manifest for each attempted scenario, including `multi-goods-elite`.
- Headless CLI exit codes must preserve economic truth: nonzero is acceptable when one or more catalog scenarios fail strict readiness, but the status line and artifacts must identify each scenario result.
- Headless run manifests must list the exact approved volatile artifact names, and volatile artifacts must be excluded from deterministic artifact-set hashes.
- Manifest diff categories must use the stable vocabulary `scenario`, `model`, `policy`, `oracle`, and `runtime`.
- Repeated deterministic headless runs of the same `multi-goods-elite` catalog scenario must produce equivalent manifests under `scenario manifest-diff`.

## Boundary

This subbundle is Economy headless and CLI-only. It does not change Components runtime, browser observer behavior, or visual mapping semantics.

## Proof

- `bundle://proof/SB15/cli-catalog-run-summary.json`
- `bundle://proof/SB15/manifest-diff-proof.json`
- `bundle://proof/SB15/transcripts/headless-cli-focused-tests.txt`
