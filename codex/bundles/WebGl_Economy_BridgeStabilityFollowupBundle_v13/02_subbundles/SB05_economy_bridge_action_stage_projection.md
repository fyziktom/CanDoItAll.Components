# SB05 — Economy bridge action/stage projection

## Problem
The bridge currently maps actions but discards mapped actions when building frames; stages contain metadata only.

## Tasks
- Add `EconomyWebGlActionStageProjector`.
- Map `EconomyVisualAction` -> `WebGlRunAction`.
- Use `WebGlRunActionPlanner` and `WebGlRunActionPlanBatchCompiler`.
- Emit frame command batches/stages with actual patches and motions.
- Do not duplicate `input.Actions` into every visual frame; partition them by `Timeline.StepIndex` and time.

## Tests
- `MoveToTarget` creates motion command.
- `ShowSymbol` creates symbol patch.
- `ChangePose` creates pose/asset patch.
- sequence produces ordered stages.
- global action with step index only appears in that frame.
