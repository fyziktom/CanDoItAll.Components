# SB03 — Components scene document hashing and validation

## Goal

Make `WebGlSceneDocument` reliable enough for future run snapshots.

## Tasks

1. Split scene document hashes:
   - `ContentHash`: stable scene content without volatile UI/runtime state.
   - `DocumentHash`: full serialized document hash.
2. Add validation for:
   - duplicate object ids;
   - duplicate link ids;
   - dangling links;
   - missing asset ids;
   - invalid vector values;
   - run-layer metadata keys.
3. Add options:
   - include/exclude UI state;
   - include/exclude runtime options;
   - include/exclude diagnostics.
4. Add tests for stable hash behavior:
   - changing camera hover/selection must not change content hash;
   - changing object position must change content hash.

## Validation

- New unit tests.
- Existing scene document serializer tests still pass.
