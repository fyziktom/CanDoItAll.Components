# SB10 — Interaction, Motion, and Scene Commands

## Goal

Prepare generic scene commands for future run/playback layers without making WebGlLib a game engine.

## Tasks

1. Extend basic motion carefully:
   - position target
   - optional rotation target
   - optional scale target
   - duration or speed
   - easing
   - replace existing or queue mode
   - cancel by object/motion id
   - completion callback
   - deterministic command id support

2. Keep out of WebGlLib:
   - pathfinding
   - collision
   - steering behavior
   - resource rules
   - agent decision logic
   - economic time semantics

3. Interaction improvements:
   - drag constraints: axis lock, ground plane, optional bounds rectangle.
   - snap-to-grid option.
   - modifier-key multi-select behavior documented.
   - keyboard accessibility basics for selection/focus.

4. Scene command consistency:
   - all commands should have simple and detailed variants.
   - detailed variants must return `WebGlSceneCommandResult`.
   - failures never throw from JS façade unless interop itself fails.

5. Tests:
   - enqueue position motion.
   - enqueue transform motion.
   - cancel motion.
   - patch while motion active either cancels or updates deterministically according to documented rule.

## Done criteria

- Generic motion is useful for tycoon-like movement.
- Complex movement remains above WebGlLib.
