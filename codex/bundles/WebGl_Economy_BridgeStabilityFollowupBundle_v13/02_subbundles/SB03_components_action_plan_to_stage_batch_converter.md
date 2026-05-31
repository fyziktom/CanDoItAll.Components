# SB03 — Components action-plan to staged command batch converter

## Problem
`WebGlRunActionPlanner` emits flat `Patches` and `Motions`. Sequences currently only set metadata and append actions flat. The bridge needs a stable way to convert generic `WebGlRunActionPlan` into staged `WebGlSceneCommandBatch`.

## Tasks
- Add `WebGlRunActionPlanBatchCompiler`.
- Convert sequence actions into ordered stages.
- Convert parallel actions into a coalescible stage.
- Preserve source action ids in metadata.
- Ensure command batch normalizer does not coalesce ordered semantics.

## Tests
- sequence with two motions for same object creates two ordered stages.
- parallel actions can coalesce independent patches.
- C# normalized batch and JS normalized batch parity fixture matches.
