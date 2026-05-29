# SB07 — Scene Document and Persistence Semantics

## Goal

Make WebGL scene save/load safe and deterministic before it is used by generic runs or economy visualizations.

## Current issue

`WebGlSceneDocument` exists and has content hashing, but the semantics need to be stronger before downstream run layers depend on it.

## Tasks

1. Split hashes:
   - `SceneContentHash`: stable hash of scene content.
   - `DocumentHash`: optional hash of the whole saved document.
   - `ContentHash` may remain as backward-compatible alias if needed.

2. Define volatile fields excluded from `SceneContentHash`:
   - `SavedAtUtc`
   - document source
   - selected objects
   - hovered object
   - transient camera if configured
   - runtime diagnostics
   - run/economy metadata

3. Add deterministic normalization:
   - sort metadata recursively,
   - optionally sort assets/objects/links/layers by ID for hash mode,
   - preserve display order separately if needed.

4. Add schema migration placeholder:
   - `IWebGlSceneDocumentMigrator`
   - `WebGlSceneDocumentMigrationResult`
   - no migrations needed yet, but the boundary should exist.

5. Validation:
   - recursively reject run/economy/ledger/account metadata in generic document unless it is under a neutral `source.` namespace.
   - validate duplicate object IDs.
   - validate links reference existing objects.
   - validate asset references and fallback references.

6. Add tests:
   - same scene saved twice has same scene content hash.
   - changing `SavedAtUtc` does not change scene content hash.
   - changing object position changes scene content hash.
   - invalid link endpoint fails validation.

## Done criteria

- Scene document can be used as a stable generic artifact.
- It does not become a run/economy persistence format.
