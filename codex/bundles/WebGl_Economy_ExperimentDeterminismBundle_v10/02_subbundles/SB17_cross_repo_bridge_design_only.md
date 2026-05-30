# SB17 — Cross-repo bridge design only

## Rule

Do not implement direct Components dependency inside Economy yet.

## Tasks

1. Document future bridge:
   - `EconomyVisualAction` -> `WebGlRunAction`
   - `EconomyVisualFrame` -> `WebGlSceneDocument` or patches
   - input pack provenance -> `WebGlRunDocument.Metadata`
2. Define mapping table.
3. Do not add project references between repos.

## Done criteria

- Future bridge can be implemented later without changing current boundaries.
