# SB11 — Production-line generic canary sample

## Goal

Create a non-Economy generic sample that approximates future manufacturing visualization without domain terms in shipped engine.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Add a sample/proof route or console/browser proof using neutral ids: nodes, route, queue-zone, token, status symbols, directed flow.
- Do not add station/machine/work-order/WIP/conveyor semantics to generic packages.
- Use a test-only domain-driver fixture or neutral pass-through driver.
- Prove selection, small control callback, flow motion, status update, and queue overlay are generic.

## Required proof

- `proof/SB11/manifest.md`
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
