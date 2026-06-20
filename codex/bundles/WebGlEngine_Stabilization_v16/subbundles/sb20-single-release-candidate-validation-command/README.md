# SB20 — Single release-candidate validation command

## Goal

Provide one command Codex and humans can run before merging/freeze.

## Scope

Repository: `CanDoItAll.Components` only.

## Implementation tasks

- Add `scripts/validate-webgl-rc.ps1` or equivalent.
- Command must run dotnet restore/build/test, dotnet pack, npm audits, approval tests, package samples, browser proof if environment supports it, and proof hygiene.
- It must emit machine-readable summary JSON and human-readable markdown.

## Required proof

- `proof/SB20/manifest.md`
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
