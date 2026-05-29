# SB06 — Patch, Motion, and Command Result Hardening

## Goal

Prepare generic scene commands for future run layers without adding run semantics.

## Current risk

C# patch reducer and JS patch runtime can drift. JS public APIs often return `bool`, which is too weak for future run/simulation consumers.

## Implementation tasks

Add generic command result contract:

```text
WebGlSceneCommandResult
```

Fields:

```text
Success
SceneId
CommandKind
Revision
Errors
Warnings
AffectedObjectIds
AffectedLinkIds
Diagnostics
```

Additive APIs are preferred. Keep existing boolean methods if needed, but add detailed methods where useful.

Align C# and JS patch result shapes:

- C# `WebGlScenePatchReducer.Apply` returns a result shape that can be serialized.
- JS `applyPatchDetailed` returns a matching shape.
- Existing `applyPatch` may continue returning `bool` for compatibility.

Motion hardening:

- add motion accepted/completed/cancelled/failed diagnostics;
- include active motion count in runtime diagnostics and proof snapshot;
- add optional motion completed event callback from JS to Blazor;
- add command id / correlation id support;
- keep interpolation generic and object-transform only.

## Acceptance criteria

- Unit tests cover C# patch result shape.
- Browser proof demonstrates `EnqueueMotionAsync` and completion diagnostics.
- Existing `ApplyPatchAsync` and `MoveObjectAsync` still work.
- No domain-specific run logic is added.

