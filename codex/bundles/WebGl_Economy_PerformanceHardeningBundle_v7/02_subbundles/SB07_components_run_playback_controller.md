# SB07 - Components reusable run playback controller

## Problem

The sandbox run playback page should not contain reusable orchestration logic.

## Required changes

Create reusable services/classes in `WebGlRunLib`:

```text
IWebGlRunPlaybackController
WebGlRunPlaybackController
IWebGlRunFrameStore
InMemoryWebGlRunFrameStore
IWebGlRunActionPlanner
WebGlRunActionPlanner
WebGlRunPlaybackClock
```

The sandbox page should only:
- create a sample `WebGlRunDocument`,
- instantiate the controller,
- bind buttons to controller commands,
- apply returned command batches to `WebGlSceneView`.

## Validation

- Unit tests for play, pause, step, seek, restart.
- Unit tests that playback speed affects delay only, not deterministic output.
- Sandbox page becomes thin.
