# SB02 — External WebGL engine benchmark matrix

## Goal

Convert lessons from Three.js, PlayCanvas, Babylon.js, and regl into concrete Components RC acceptance criteria.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Create a benchmark matrix mapping external engine concepts to current Components capabilities.
- Identify gaps: instancing/LOD readiness, asset lifecycle, ECS-like domain separation, viewer/sample proof, profiler diagnostics, command lifecycle.
- Do not add features yet; produce architecture notes and traceability to later subbundles.

## Required proof

- `proof/SB02/manifest.md`
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
