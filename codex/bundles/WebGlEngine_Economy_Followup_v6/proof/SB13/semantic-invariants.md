# Semantic Invariants for SB13

## Invariant SB13-docs-state-ui-proof-is-not-economic-proof

Source: `docs/simulation/experiment-readiness.md`.

Expected behavior: user guidance distinguishes economic validity from projection, runtime, and UI proof, and documents how strict warnings affect confidence.

Passing result: the documentation file includes readiness bands, L0-L5 meanings, strict setup, headless artifacts, and troubleshooting for pause/runtime idle and scenario pack hashes.

Why this prevents simulator-noise contamination: operators are told not to treat browser screenshots or animation success as economic-model proof.

## Invariant SB13-proof-files-are-substantive

Source: `scripts/validate_proof_integrity.py`.

Expected behavior: proof manifests and semantic invariant files fail validation if they remain empty, template-like, or missing required evidence/result/changed-file sections.

Passing result: the proof-integrity transcript records the validator result after manifests were replaced with concrete evidence.

Why this prevents simulator-noise contamination: bundle closure cannot rely on empty proof files or screenshots alone.

