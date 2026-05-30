# Components review findings

## Good progress

- `CanDoItAll.Components.WebGlRunLib` exists and is listed in the solution.
- `WebGlRunDocument`, `WebGlRunTimeline`, `WebGlRunFrame`, playback state and frame source contracts exist.
- `WebGlRunAction`, target, anchor, pose, symbol and action binding contracts exist.
- `WebGlRunPlaybackController` exists and can apply frames through an `IWebGlRunFrameApplier`.
- WebGL scene runtime has a large-screen-only policy document.
- JS runtime has command batching, scene indexes, render scheduler, asset cache and runtime audit.

## Main concerns

1. Sequential actions can be broken by motion deduplication.
   A shared-well sequence may contain multiple motions for the same actor:
   home -> well -> admin desk -> home.
   Current batching can drop duplicate motions per object unless the batch is explicitly staged or duplicates are enabled.

2. C# and JS batch normalizers are separate implementations.
   They must be parity-tested with JSON fixtures so they do not drift.

3. Generic WebGlRun action semantics need explicit stage boundaries.
   A sequence action must be represented as ordered stages, not a flat list of actions that can be coalesced.

4. `syncLinksForObject` still risks O(movingObjects * links) behavior if it scans all links per object.
   A map from object id to related link groups exists conceptually; make sure the runtime actually uses it.

5. The public JS façade is still near the thin façade threshold. Keep it thin and move new functions inward.

6. The large-screen-only rule exists, but it needs to become a validation gate for future bundles.
