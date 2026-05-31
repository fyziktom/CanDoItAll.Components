# SB04 - Components ActionPlan to StagedBatch

## Problem

`WebGlRunActionPlanner` currently emits flat lists of patches and motions. Sequence actions set metadata but do not clearly produce staged command batches.

## Goal

Add a reusable converter:

```csharp
WebGlRunActionPlan -> WebGlSceneCommandBatch
```

## Required behavior

- Sequence action steps become deterministic stages.
- Parallel action steps can share a stage where safe.
- Wait action becomes a stage wait.
- Patches that change pose/symbol before a move must not be coalesced across the move.
- Multiple motions for the same object in a sequence must preserve order.
- Metadata must include:
  - action id
  - parent action id
  - stage id
  - stage index
  - visual action id if present
  - source event id if present

## Suggested files

```text
src/CanDoItAll.Components.WebGlRunLib/Planning/WebGlRunActionPlanBatchBuilder.cs
src/CanDoItAll.Components.WebGlRunLib/Planning/WebGlRunStageIdPolicy.cs
```

## Tests

Add fixture test for a shared-resource sequence without Economy references.
