# SB02 - Components WebGlRunLib contract refactor

## Problem

`WebGlRunContracts.cs` and `WebGlRunActions.cs` are already broad files. They will become hard to maintain as soon as action planning, playback, mapping, validation, and state tracking grow.

## Required refactor

Split `CanDoItAll.Components.WebGlRunLib` contracts into separate files:

```text
Identity/
  WebGlRunId.cs

Documents/
  WebGlRunDocument.cs
  WebGlRunTimeline.cs
  WebGlRunFrame.cs

Playback/
  WebGlRunPlaybackState.cs
  WebGlRunPlaybackCommand.cs
  WebGlRunPlaybackOptions.cs
  WebGlRunPlaybackResult.cs
  WebGlRunPlaybackController.cs

Actions/
  WebGlRunEvent.cs
  WebGlRunAction.cs
  WebGlRunActionTarget.cs
  WebGlRunActionPlan.cs
  WebGlRunActionKinds.cs
  WebGlRunAnchorKeys.cs

Planning/
  WebGlRunPlanningContext.cs
  WebGlRunActionPlanner.cs
  WebGlRunTargetResolver.cs
  WebGlRunVisualStateResolver.cs

Catalogs/
  WebGlVisualStateCatalog.cs
  WebGlPoseDefinition.cs
  WebGlSymbolDefinition.cs
  WebGlActionBinding.cs
```

## Rules

- Preserve public API names unless there is a clear reason to rename.
- Add XML docs only where they clarify boundary semantics.
- Source-code comments must be in English.
- No Economy-specific action names.

## Validation

- Existing WebGlRunLib tests still pass.
- New file line-count audit passes.
