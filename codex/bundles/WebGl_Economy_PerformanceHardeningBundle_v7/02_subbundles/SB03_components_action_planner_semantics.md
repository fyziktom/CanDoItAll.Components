# SB03 - Components generic action planner semantics

## Goal

Harden the generic action planner so it can support scenarios like shared well community without hardcoding that scenario.

## Required functionality

1. Ensure `WebGlRunActionPlanner` supports:
   - sequence
   - parallel
   - move-to-object
   - move-to-position
   - return-to-anchor
   - change-pose / set-pose
   - show-symbol / hide-symbol / update-symbol
   - wait
   - apply-scene-patch
2. Add explicit `WebGlRunActionPlanResult` or extend `WebGlRunActionPlan` with:
   - action id
   - validation warnings/errors
   - generated patches
   - generated motions
   - target resolution diagnostics
   - dropped/ignored steps
3. Add target resolution rules:
   - explicit position wins
   - object anchor next
   - object center fallback
   - named fallback anchor from object metadata
4. Add no-op fallback behavior:
   - missing pose -> warning and no-op patch
   - missing symbol -> warning and no-op patch
   - missing target -> error, no motion

## Performance concern

Action planning must be pure and linear over the number of action steps. It must not scan all scene objects repeatedly for every step; build an index once per planning context.
