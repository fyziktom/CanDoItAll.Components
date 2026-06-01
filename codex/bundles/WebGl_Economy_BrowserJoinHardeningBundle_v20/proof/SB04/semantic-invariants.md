# SB04 Semantic Invariants

## Invariant ID

SB04-bounded-runtime-snapshot

## Shallow-pass trap

A snapshot DTO can exist but omit live stage, motion, barrier, journal, warning, and error state or grow without bound.

## Adversarial negative proof

`Adapter_bounds_runtime_snapshot_lists` proves oversized active/queued motion IDs are capped to 100 items and command journal tail is capped to the most recent 12 entries.

## Semantic positive proof

`Adapter_applies_frame_to_runtime_and_returns_counts_and_snapshot` proves applied runtime diagnostics populate current batch/stage IDs, active/queued motion IDs, barrier blockers, and command journal tail.

## Anti-stub audit

`bundle://proof/SB04/transcripts/anti-stub-audit.txt` confirms no placeholder code in runtime snapshot production sources.

## Raw-note literal closure

- current frame index: covered by adapter snapshot result.
- active/queued stages: covered by current, active, queued stage fields.
- active/queued motions: covered and bounded.
- command journal tail: covered and bounded.
- stage barrier state: covered by `WebGlRunStageBarrierSnapshot`.
- runtime errors/warnings: covered by snapshot lists.
