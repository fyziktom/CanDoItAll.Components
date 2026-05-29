# SB06 - Components generic action/event mapping

Repository: `CanDoItAll.Components`

## Goal

Define generic action plans in `WebGlRunLib` so later economy visual actions can be mapped without hardcoding wells/markets.

## Add contracts

```text
WebGlRunEvent
WebGlRunAction
WebGlRunActionKind
WebGlRunActionPlan
WebGlRunActionMapping
WebGlRunObjectBinding
WebGlRunActionCompiler
```

## Generic action kinds

```text
move-to-object
move-to-position
return-to-anchor
set-asset
set-pose
show-symbol
hide-symbol
pulse-link
resource-transfer-visual
wait
apply-scene-patch
```

## Required compiler behavior

A generic action plan can compile to:
- `WebGlScenePatch`
- `WebGlObjectMotionCommand`
- ordered frame patches/motions
- command metadata for later diagnostics

## Sandbox proof

Extend generic run playback demo:
- object moves to target;
- object returns to start/home anchor;
- object changes asset/pose while "doing work";
- symbol appears and disappears.
