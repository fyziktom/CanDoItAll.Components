# SB12 — Generic WebGlRunLib Contracts

## Goal

Prepare a generic layer above WebGlLib for run/playback scenarios without implementing economy logic.

## Decision

Create this project only if WebGlLib hardening gates pass:

```text
src/CanDoItAll.Components.WebGlRunLib/
```

If the team decides not to create the project yet, create only docs and skeletons under `docs/webgl/run-layer`.

## Project dependency rule

```text
CanDoItAll.Components.WebGlRunLib -> CanDoItAll.Components.WebGlLib
CanDoItAll.Components.WebGlLib -> no WebGlRunLib
```

## Initial contracts

- `WebGlRunDocument`
- `WebGlRunId`
- `WebGlRunTimeline`
- `WebGlRunFrame`
- `WebGlRunFramePatch`
- `WebGlRunPlaybackState`
- `WebGlRunPlaybackCommand`
- `IWebGlRunPlaybackController`
- `IWebGlRunFrameSource`
- `IWebGlRunSceneProjector<TFrame>`
- `IWebGlRunSnapshotStore`

## What this layer may do

- apply scene patches over time,
- playback/pause/seek,
- load/save generic run documents,
- manage current frame cursor,
- expose generic run diagnostics.

## What this layer must not do

- decide economy outcomes,
- execute ledger transactions,
- update simple accounts,
- calculate markets,
- choose agent actions,
- implement pathfinding or physics.

## Sandbox proof

Add a generic `run-playback` sandbox page:
- uses a tiny scene,
- contains a timeline of patches,
- moves an object over several frames,
- supports play/pause/step/seek,
- does not reference economy.

## Done criteria

- WebGlRunLib is generic.
- It demonstrates playback over WebGlSceneModel.
- No economy terms in public API.
