# SB13 — Instancing/LOD design checkpoint

## Goal

Decide what must be generic now and what belongs in backlog.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Compare current repeated-object performance to thresholds.
- Document whether we need an `InstanceGroup` generic contract now or later.
- Document LOD/quality-profile behavior for repeated assets.
- Reject domain-specific performance hacks.

## Required proof

- `proof/SB13/manifest.md`
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
