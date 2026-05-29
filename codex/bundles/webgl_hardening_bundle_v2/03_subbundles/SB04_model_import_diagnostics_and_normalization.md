# SB04 — Model Import Diagnostics and Normalization

## Goal

Make invisible or badly converted GLB models diagnosable without blocking fallback rendering.

## Current context

External GLB alternatives are now present and attempted, but some are invisible. Treat this as a diagnostics problem first, not as a reason to abandon the asset pipeline.

## Implementation tasks

Add C# DTOs:

```text
WebGlModelImportOptions
WebGlModelDiagnostics
```

Add JS module:

```text
src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/18-webgl-scene-model-diagnostics.js
```

Model diagnostics should detect:

- no scene in GLB;
- no mesh nodes;
- all mesh nodes invisible;
- zero/near-zero bounds;
- extreme bounds;
- NaN/Infinity transform values;
- all materials transparent or opacity near zero;
- unit scale mismatch hints;
- model placed far from expected origin;
- camera clipping/far distance risk.

Import options should support:

- unit scale;
- fit mode: `fit-bounds`, `original-scale`, `fixed-scale`;
- center mode: `center-bottom`, `center-bounds`, `preserve-origin`;
- rotation offset;
- position offset;
- force double sided material;
- normalize material visibility;
- debug bounds rendering;
- disable tint for imported materials.

## Sandbox proof

Add or improve `/model-lab`:

- select a single asset/variant;
- switch asset quality profile;
- toggle debug bounds;
- show computed bounds, mesh count, material diagnostics, and warnings;
- show fallback status;
- export proof snapshot.

## Acceptance criteria

- Invisible models produce clear diagnostics instead of silent failure.
- Primitive fallback still renders when model loading or normalization fails.
- High-detail models are not enabled by default.
- Browser proof includes at least one model-high attempt and diagnostics output.

