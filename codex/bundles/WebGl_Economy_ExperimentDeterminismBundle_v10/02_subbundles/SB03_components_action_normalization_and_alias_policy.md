# SB03 — Components: action normalization and alias policy

## Problem

`WebGlRunAction` exposes aliases such as `Kind`, `ActionKind`, `ObjectId`, `SubjectObjectId`, `TargetObjectId`, and `Target.ObjectId`. This is compatible but risky.

## Tasks

1. Add `WebGlRunActionNormalizer`.
2. Normalize aliases into a canonical form:
   - `ActionKind`
   - `SubjectObjectId`
   - `Target.ObjectId` or explicit `Target.Position`
   - `PoseKey`
   - `SymbolKey`
   - `Timeline`
   - stage metadata
3. Add validation:
   - missing subject
   - unsupported action kind
   - conflicting aliases
   - missing target for target-based actions
4. Add tests for alias equivalence and conflict warnings.

## Done criteria

- Planner consumes normalized actions only.
- Original aliases remain for compatibility but are never used directly in planning logic.
