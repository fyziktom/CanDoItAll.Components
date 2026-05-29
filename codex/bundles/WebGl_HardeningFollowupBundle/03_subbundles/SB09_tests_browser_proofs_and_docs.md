# SB09 - Tests, browser proofs, and docs

## Goal

Convert proof-of-concept into maintainable component infrastructure.

## Tasks

1. Add or extend test project.
2. Unit-test:
   - `WebGlAssetCatalogValidator`,
   - `DefaultWebGlSymbolPolicy`,
   - asset variant resolver,
   - scene patch reducer.
3. Add browser proof script or documented Playwright scenario:
   - default primitive village,
   - model variant village,
   - selection,
   - drag,
   - motion,
   - export/import scene,
   - old workbench namespace still exists.
4. Update docs:
   - `src/CanDoItAll.Components.WebGlLib/README.md`
   - `src/CanDoItAll.Components.WebGlSandbox/README.md`
   - root README package role table if needed.
5. Create final implementation report.

## Acceptance criteria

- All builds pass.
- Browser proof JSON is stored under `artifacts/webgl-scene-hardening`.
- Docs explain the boundary between WebGlLib and future WebGlRunLib.
