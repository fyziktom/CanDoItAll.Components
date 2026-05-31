# SB05 — WebGlRun plan to command batch contract

## Goal
Guarantee that `WebGlRunActionPlan` always becomes runtime-executable commands.

## Required
- `WebGlRunActionPlanBatchCompiler` must emit `WebGlSceneCommandBatch` with stages, patches, and motions.
- Sequence actions must preserve order.
- Parallel actions may coalesce only when no stateful dependency exists.
- No unsupported action kind should silently become wait unless explicitly mapped as no-op.

## Validation
- Use C# tests for planner output.
- Use JS audit fixture for equivalent batch normalization.
- Add parity fixture: C# normalized batch JSON equals JS normalized batch JSON for key cases.
