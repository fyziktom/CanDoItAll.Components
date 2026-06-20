# Semantic Invariants for SB12

## Invariant SB12-headless-runner-is-deterministic

Source: `EconomyHeadlessExperimentRunner`.

Expected behavior: identical input packs produce identical run hashes and output artifacts, while changed economic inputs produce different run hashes.

Passing result: `HeadlessRunner_ProducesDeterministicRunHashesAndBatchConfidence` passed, and batch summaries report L4 confidence for strict headless runs.

Why this prevents simulator-noise contamination: experiments can be compared through deterministic headless artifacts without depending on browser rendering state.

