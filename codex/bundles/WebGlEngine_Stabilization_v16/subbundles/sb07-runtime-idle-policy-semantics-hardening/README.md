# SB07 — Runtime idle policy semantics hardening

## Goal

Make semantic vs visual vs final-render-drain behavior explicit and testable.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Document exact meaning of `SemanticOnly`, `VisualStrict`, and `AllowFinalRenderDrain`.
- Add tests for pending motions, queued stages, active barriers, render-loop scheduled frame, continuous mode, and disposed runtime.
- Add negative proof where `AllowFinalRenderDrain` can pass but `VisualStrict` must fail.
- Ensure default policy choices are explicit in C# and JS.

## Required proof

- `proof/SB07/manifest.md`
- changed-file list for this subbundle
- tests/build/audit transcripts relevant to the subbundle
- semantic invariants file
- zero-byte proof transcript scan
- explicit note if no code was changed

## Done criteria

- Public/generic boundaries remain intact.
- No Economy or domain repository files are changed.
- All new source comments are in English.
- The subbundle can be reviewed independently.
