# SB02 - Components: ordered action stages

## Problem
A sequence like home -> well -> admin -> home can be corrupted by motion deduplication/coalescing.

## Tasks
- Add `WebGlRunActionStage` and `WebGlRunActionStageBatch` or equivalent.
- Add `SequenceId`, `ParentActionId`, `StageIndex`, `OrderIndex`, `ExecutionPolicy`.
- Execution policies:
  - `preserve-order`
  - `parallel`
  - `coalesce-within-stage`
- Ensure multiple motions for the same object are allowed across stages.
- Ensure duplicate motion dedupe applies only inside safe parallel stages.

## Tests
- Actor moves home -> well -> admin -> home in one visual scenario and all motions survive.
- Coalescing still works for independent objects in the same stage.
