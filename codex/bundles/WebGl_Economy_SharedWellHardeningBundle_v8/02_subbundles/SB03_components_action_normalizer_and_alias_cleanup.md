# SB03 — Components: action normalizer and alias cleanup

## Problem
`WebGlRunAction` has multiple compatibility aliases. This is risky.

## Required work
- Add `WebGlRunActionNormalizer`.
- Convert aliases into canonical fields immediately.
- Internal logic must use only:
  - `ActionKind`;
  - `SubjectObjectId`;
  - `Target.ObjectId`;
  - `Target.AnchorKey`;
  - `PoseKey`;
  - `SymbolKey`.
- Add validation warnings for ambiguous inputs where both aliases are set differently.
- Add tests for alias conflict detection.

## Acceptance
No planner/controller code should repeatedly branch over both `Kind`/`ActionKind` or `ObjectId`/`SubjectObjectId`.
