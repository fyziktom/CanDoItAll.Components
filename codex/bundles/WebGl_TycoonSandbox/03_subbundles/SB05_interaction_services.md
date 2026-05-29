# SB05 - Interaction contracts and services

## Goal

Add generic interaction contracts without breaking workbench-specific events.

## Required files

```text
src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlSceneSelectionState.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlSceneSelectionChangedEventArgs.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlSceneHoverChangedEventArgs.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlObjectMovedEventArgs.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlSceneCommand.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlSceneCommandResult.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlCameraCommand.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Interaction/WebGlInteractionOptions.cs
```

## Requirements

Support:

- hover object id
- selected object ids
- primary selected object id
- click selection
- optional multi-select
- optional drag on ground plane
- camera pan/zoom/orbit
- fit-view
- focus-object
- reset-camera
- context action id

## Acceptance criteria

- `WebGlSceneView` exposes typed callbacks:
  - `SelectionChanged`
  - `HoverChanged`
  - `ObjectsMoved`
  - `StateChanged`
  - `RuntimeReady`
  - `RuntimeError`
- Existing `WebGlWorkbench` callbacks still compile and behave unchanged.
