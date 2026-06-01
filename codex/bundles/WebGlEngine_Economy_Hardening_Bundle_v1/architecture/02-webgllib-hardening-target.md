# WebGlLib Hardening Target

## Runtime correctness

Add a static module audit for `wwwroot/js/runtime/scene/*.js` that detects:

- unresolved imported symbols;
- imported-but-missing exports;
- duplicate module global registrations;
- accidental implicit globals;
- forbidden dependencies from WebGlLib runtime into run/economy modules.

## Patch semantics

Patch application must follow this shape:

```text
normalize patch
validate patch against current scene snapshot
if invalid:
    return failed command result
    do not mutate scene
else:
    classify patch
    apply minimal mutation
    update canonical revision once
    update diagnostics
    schedule render
```

## Patch classification

```text
TransformOnly:
  position/rotation/scale only
  update object transform, links and symbols
  no full rebuild

SymbolOnly:
  symbols only
  rebuild symbols for object
  no full rebuild

VisualReplace:
  asset/color/size/material-affecting metadata
  replace affected object group only

GraphStructure:
  add/remove object or link
  partial graph mutation where safe

SceneRebuild:
  environment/camera/global layout/asset-catalog change
  full rebuild allowed
```

## Revision policy

Codex must choose and document one canonical policy. Preferred:

```text
WebGlSceneModel.Revision = canonical scene model revision
WebGlSceneUiState.Revision = UI-only revision or removed/obsolete bridge
```

The final policy must be implemented consistently in:

- C# patch reducer;
- JS patching;
- WebGlSceneView change detection;
- export/import;
- WebGlSceneDocument hashing;
- tests.

## Resource ownership

Material ownership and texture ownership must be separate.

```text
Shared template:
  geometry shared
  material shared
  texture shared

Instance without tint:
  owns no template resource

Instance with tinted cloned material:
  owns material
  does not own inherited textures unless textures were explicitly cloned

Primitive:
  owns geometry and material
  owns generated texture only if any generated texture exists
```

## WebGlLib-only consumption proof

At least one sample/test must prove a consumer can use WebGlLib for a simple model/scene without referencing WebGlRunLib.
