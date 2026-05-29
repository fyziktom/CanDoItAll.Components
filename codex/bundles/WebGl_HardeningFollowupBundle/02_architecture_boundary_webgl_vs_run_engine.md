# 02 - Architecture boundary: WebGlLib vs WebGlRunLib

## Core decision

Do not turn `WebGlLib` into a complete game engine.

`WebGlLib` should be a reusable rendering/interaction substrate. A future `WebGlRunLib` or consuming domain package should drive time, simulation, game rules, persistence semantics, and path planning.

## Belongs in `CanDoItAll.Components.WebGlLib`

These are general render-layer features:

```text
WebGlSceneModel
WebGlSceneObject
WebGlSceneLink
WebGlSceneCamera
WebGlSceneEnvironment
WebGlAssetCatalog
WebGlAssetDefinition
WebGlAssetVariant
WebGlAssetPerformanceHint
WebGlStatusSymbol
WebGlScenePatch
WebGlObjectMotionCommand
WebGlSceneProofSnapshot
WebGlRuntimeDiagnostics
```

Runtime features:

```text
load scene
export current scene
apply patch
set object transform
enqueue transform interpolation
hover/select/pick
drag object on ground plane
camera pan/zoom/orbit/focus
world-to-screen overlays
asset fallback/variants/LOD selection
proof snapshot
```

Why these belong in `WebGlLib`:
- Any run/game/simulation wrapper needs the renderer to support incremental transform updates.
- Drag and picking are UI mechanics, not domain semantics.
- Asset variants and performance hints are rendering concerns.
- Motion interpolation is a renderer primitive, not a simulation rule.

## Does not belong in `WebGlLib`

These belong above the wrapper:

```text
simulation clock semantics
run lifecycle
scenario definitions
domain entity model
economy/game rules
agent planning/pathfinding
physics/collision
resource economy
state persistence provider
event sourcing
save slots
replay logs
network synchronization
domain-specific symbol policy
```

## Proposed later package

For the second phase, add a new optional package:

```text
src/CanDoItAll.Components.WebGlRunLib/
```

References:

```text
WebGlRunLib -> WebGlLib
WebGlLib -> no WebGlRunLib dependency
```

Possible contracts:

```csharp
public sealed class WebGlRunModel
{
    public string RunId { get; set; } = string.Empty;
    public WebGlSceneModel Scene { get; set; } = new();
    public WebGlRunClock Clock { get; set; } = new();
    public List<WebGlRunTrack> Tracks { get; set; } = [];
    public Dictionary<string, string> Metadata { get; set; } = [];
}

public sealed class WebGlRunFrame
{
    public double TimeSeconds { get; set; }
    public List<WebGlScenePatch> Patches { get; set; } = [];
    public List<WebGlObjectMotionCommand> Motions { get; set; } = [];
}
```

This layer can then be reused by:
- generic simulations,
- economy simulator,
- process visualization,
- game-like prototypes,
- educational visualizations.

## Practical rule

When in doubt:

```text
Can it be used by any 3D scene? -> WebGlLib
Does it describe how a run evolves over time? -> WebGlRunLib
Does it describe economy/process/game meaning? -> consuming domain package
```
