# WebGL + Economy hardening v7 execution report

Date: 2026-05-30

## Branch inventory

Components repository:

- Path: `C:\repositories\CanDoItAll.Components`
- Branch: `webgl-engine`
- HEAD before work: `cdc3c7d06bde5ed09ccc38f6a56e7bee09931e76`
- Branch action: no branch was created or switched.

Economy repository:

- Path: `C:\repositories\CanDoItAll.Economy`
- Branch: `main`
- HEAD before work: `1cc65cdcbc94ae6f72a8b4a1fe31b396691550e3`
- Branch action: no branch was created or switched.

## Implemented scope

- Split broad `WebGlRunLib` contracts/actions/planning into focused folders for identity, documents, playback, actions, catalogs, and planning.
- Added generic action target/visual-state resolution diagnostics and sequential batch metadata.
- Hardened C# and JS command batch normalization so ordered/sequential semantics are not coalesced or deduped incorrectly.
- Added link-group indexing by object id for runtime link updates and added frame/link/cache diagnostics to runtime snapshots and proof snapshots.
- Split JS link helpers into `27-webgl-scene-links.js` to keep the scene graph module below the hard audit threshold.
- Added the large-screen-only docs note and a simple unsupported-size warning in the WebGL sandbox.
- Added user-requested SB17 for the missing `Run Play` menu entry, updated the home card, bundle index, metadata, and implementation workbook.
- Added Economy scenario loading, generic event stream compilation, frame artifact deltas, deterministic hashing additions, consistency validation, minimal ledger deltas, and visual intention aliases without adding WebGL coupling.

## Validation results

- `dotnet build CanDoItAll.Components.slnx --no-restore -p:UseSharedCompilation=false`: passed, 0 warnings, 0 errors.
- `dotnet test tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore -p:UseSharedCompilation=false`: passed, 27 tests.
- `dotnet test tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore -p:UseSharedCompilation=false`: passed, 7 tests.
- `npm run webgllib:build-assets`: passed, assets already up to date.
- `npm run webgllib:verify-assets`: passed, sources/public outputs/include components in sync.
- `npm run webgllib:audit-scene-runtime`: passed with 8 warning-level line-count notes and 0 failures.
- `dotnet build CanDoItAll.Economy.slnx --no-restore -p:UseSharedCompilation=false`: passed, 0 errors, existing package compatibility/vulnerability warnings.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\audit-simulation-boundaries.ps1`: passed.
- `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --no-restore -p:UseSharedCompilation=false --filter FullyQualifiedName~SimulationPreparationTests`: passed, 10 tests.
- Full `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --no-restore -p:UseSharedCompilation=false`: 432 passed, 1 failed due local PostgreSQL permission denied while terminating a backend in `PostgreSqlLedgerStore_ConcurrentDoubleSpendConfirmsOnlyOneTransaction`.

## Browser proof

All browser proofs were captured at desktop/large-screen viewports only. Raw files are under `C:\repositories\CanDoItAll.Components\artifacts\webgl-runtime-hardening-v7`.

- `run-playback-1440x900.png` and `run-playback-1440x900-proof.json`: `/run-playback`, canvas nonblank, `navHasRunPlay: true`, 2 visible objects, 1 visible link, 0 missing assets.
- `tycoon-village-1440x900.png` and `tycoon-village-1440x900-proof.json`: `/tycoon-village`, canvas nonblank, 20 visible objects, 5 visible links, 9 symbols, 0 missing assets.
- `model-lab-high-1920x1080.png` and `model-lab-high-1920x1080-proof.json`: `/model-lab` high-detail GLB profile, canvas nonblank, `modelInstanceCount: 1`, 9 visible meshes, 0 model warnings, 0 model errors.
- `home-run-play-link-proof.json`: overview and navigation both expose `/run-playback`.
- `browser-console-errors.log`: 0 browser console errors.

## Residual risk

The remaining full-suite Economy failure is environmental and outside this bundle's simulation hardening path: the PostgreSQL integration test cannot delete its database because the current local PostgreSQL user lacks permission to terminate another backend.
