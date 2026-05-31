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

## Status
- Completed.

## Prerequisites
- SB03 and SB04 runtime foundations are complete.

## Exact Source References
- `repo://src/CanDoItAll.Components.WebGlRunLib/Planning/WebGlRunActionPlanBatchCompiler.cs`
- `repo://src/CanDoItAll.Components.WebGlRunLib/Actions/WebGlRunActionPlan.cs`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlSceneCommandBatch.cs`
- `repo://tests/CanDoItAll.Components.WebGlRunLib.Tests`

## Dependency Impact
- Critical bridge between high-level action plans and runtime-executable command batches.

## Validation Depth
- Requires C# positive/negative tests, JS normalization parity proof, and no silent unsupported action fallback.

## Acceptance Checklist
- Sequence actions preserve order.
- Parallel actions do not coalesce across stateful dependencies.
- Unsupported actions produce explicit diagnostics or explicit no-op mappings.

## Proof Required
- `bundle://proof/SB05/manifest.md`
- `bundle://proof/SB05/semantic-invariants.md`
- WebGlRunLib test transcript and JS audit/parity transcript.

## Browser Validation Logging
- Browser validation is not required unless a rendered WebGL route changes.

## Progression Gate
- SB06-SB07 may rely on batch contract proof only after no silent action drop remains.

## Suggested Agent Prompt
- Prove WebGlRun action plans compile into executable command batches with ordered stages and explicit unsupported-action diagnostics.
