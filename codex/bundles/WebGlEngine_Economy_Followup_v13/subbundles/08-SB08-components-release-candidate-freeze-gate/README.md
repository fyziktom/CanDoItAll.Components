# SB08 - Components release-candidate freeze gate

Declare Components WebGL/Run release-candidate freeze.

Gate requires:
- SB02-SB07 passing.
- WebGlLib-only sample passing.
- WebGlRunLib generic sample passing.
- domain leakage hard gate passing.
- public API snapshots passing.
- package content snapshots passing.
- browser pause/idle proof passing.
- browser observer real-state proof passing.

After this subbundle, do not add generic features in this bundle except bugfixes necessary to keep
the freeze gate green.

Required proof:
- `COMPONENTS_WEBGL_FREEZE_DECISION.md`
- exact allowed post-freeze change categories
- CI command list
- red-team checklist.

