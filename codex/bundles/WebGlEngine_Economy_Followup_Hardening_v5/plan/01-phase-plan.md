# Phase plan

| Subbundle | Title | Dependencies | Gate |
| --- | --- | --- | --- |
| SB01 | Current-state, pause reproducer, and proof integrity baseline | - | No implementation until a failing-first pause/browser proof and proof-hygiene inventory exist. |
| SB02 | WebGlLib runtime stop API | - | Calling stop twice is idempotent; queued stages and active/queued motions drop to zero. |
| SB03 | RunPlayback playback state-machine and pause fix | SB01, SB02 | Browser proof: click Play, wait for motion, click Pause, scene stops within bounded time and no further frame/stage/motion progress occurs. |
| SB04 | WebGlRun runner lifecycle contracts | SB03 | Runner state reflects canceled/paused/stopped operation and does not mark canceled frames completed. |
| SB05 | Multi-frame ApplyPlayback transaction/cancellation semantics | SB03 | Failures report target frame, last applied frame, cancellation reason, and do not continue applying later frames. |
| SB06 | Economy deterministic replay performance strategy | SB05 | Forward Step applies only necessary delta frames; Seek/Back uses full replay with reset. Browser proof covers both. |
| SB07 | Scenario source contract cleanup | SB01 | Runtime UI and tests no longer depend on ExperimentJsonPath except in legacy compatibility paths. |
| SB08 | Scenario manifest file-hash and pack-hash hardening | SB07 | Changing any scenario file causes catalog validation failure unless manifest is regenerated intentionally. |
| SB09 | Large simulation performance budgets and resource stress | SB01 | Budget test outputs machine-readable metrics and fails on regression thresholds. |
| SB10 | Proof integrity validator | SB01 | Bundle cannot close with zero-byte/blank transcripts or browser screenshots without JSON assertions. |
| SB11 | Docs, migration, and troubleshooting | SB01 | Docs include a concrete pause bug troubleshooting checklist and host integration recipe. |
| SB12 | Final cross-repo red-team closure | SB02-SB11 | No open P0/P1 issues, pause proof passes, performance budget captured, proof validator passes. |
