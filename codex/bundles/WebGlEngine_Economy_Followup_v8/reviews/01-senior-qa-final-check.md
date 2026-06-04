# Senior QA final preparation check

Status: prepared and internally reviewed.

## QA verdict

This bundle is ready for Codex execution. The current implementation is strong enough for exploratory economic simulation work, but not yet strong enough for unsupervised research claims. The follow-up bundle therefore focuses on removing simulator noise sources rather than adding new demo features.

## Non-negotiable quality bars

- Pause/stop proof must show a settled browser runtime, not only a clicked button or changed label.
- Research readiness must be backed by artifacts and hashes, not only boolean flags.
- Economic truth must be headless and deterministic; browser/WebGL remains observer evidence.
- Design matrix factor levels must mutate scenario input or be rejected as passive labels.
- Unknown diagnostics, metric kinds, invariant kinds, and oracle drift must fail strict/research-grade gates.
- Proof files must not be empty, stale, or screenshot-only.

## QA concerns intentionally preserved for Codex

- The current `RunPlayback.StopPlaybackAsync` ordering may still allow visible motion before JS runtime stop is requested.
- Runtime idle semantics are implemented but need consistent hard-fail treatment across all proof paths.
- The design matrix harness appears to record factor levels without proving that they change the scenario source consumed by the runner.
- Golden oracle coverage is currently primarily code-local and should be externalized into a corpus that is harder to accidentally update with the implementation.
