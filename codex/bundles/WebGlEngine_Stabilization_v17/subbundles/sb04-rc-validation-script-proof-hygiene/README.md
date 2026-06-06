# SB04 — RC validation script proof hygiene

## Purpose

Strengthen webgl:validate-rc so every step produces non-empty, hash-cited, assertion-bearing artifacts and fails on empty/no-op proof.

## Inputs to inspect

- Components repository, branch `webgl-engine`
- Existing approval tests and fixtures
- Existing RC validation script
- Existing package/sample projects
- Domain-boundary config
- Current WebGlSceneView and JS runtime surface

## Tasks

1. Reproduce current state or create a failing-first proof when applicable.
2. Implement the smallest generic fix that satisfies the subbundle.
3. Do not modify Economy.
4. Keep public APIs stable unless this subbundle explicitly says otherwise.
5. Update docs and approval snapshots only with a clear reason.
6. Produce proof artifacts under `proof/SB04/`.



## Done criteria

- Code builds.
- Relevant tests pass.
- Domain-boundary hard gates pass.
- Proof manifest exists and references non-empty files.
- No unchecked domain terms appear in generic source.
- A short QA note states why this does not make Components economy-specific.

## Required proof files

- `proof/SB04/manifest.md`
- `proof/SB04/transcripts/*.txt`
- `proof/SB04/changed-file-hashes.txt`
- optional browser JSON/screenshots if this subbundle touches runtime/browser behavior
