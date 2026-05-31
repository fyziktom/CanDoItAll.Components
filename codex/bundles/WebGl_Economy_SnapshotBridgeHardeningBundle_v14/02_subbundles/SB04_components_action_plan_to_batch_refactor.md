# SB04 - Components action plan to batch refactor

Goal:
- Ensure action plans become WebGL command batches in a predictable, generic way.

Tasks:
1. Review `WebGlRunActionPlanBatchBuilder`.
2. Ensure sequence/parallel semantics are preserved.
3. Ensure `Wait` actions produce stage barriers when needed, not just warnings.
4. Ensure `PulseLink` and `ResourceTransferVisual` either emit supported generic commands or produce explicit diagnostic actions.
5. Add tests that a sequence with move -> pose -> symbol -> move generates ordered stages.
6. Add C# <-> JS batch-normalizer parity fixtures.

Acceptance:
- Planner/compiler never silently drops a visual action without a diagnostic.

## Status

Completed.

## Prerequisites

SB03 stage runner and motion queue proof.

## Validation Depth

Add or verify WebGlRunLib tests and C# to JS parity fixtures for sequence/parallel semantics, wait barriers, unsupported action diagnostics, and ordered move/pose/symbol/move stages.

## Progression Gate

SB05 may proceed only after planner/compiler proof shows generic command batches preserve ordering and unsupported actions surface explicit diagnostics.
