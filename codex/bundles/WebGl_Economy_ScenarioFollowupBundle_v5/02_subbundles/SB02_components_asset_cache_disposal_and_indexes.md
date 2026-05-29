# SB02 - Components asset cache disposal and scene index hardening

Repository: `CanDoItAll.Components`

## Problems

- Asset cache helper exists but lifecycle must explicitly dispose cached templates.
- Patch operations can mutate objects/links without keeping scene indexes synchronized.

## Tasks

1. Initialize asset cache through `createAssetCache`.
2. Call `disposeAssetCache(state)` during scene lifecycle dispose.
3. Add diagnostics for cache disposal in proof snapshots.
4. Add `syncSceneIndexes(state, reason)` helper.
5. Call index sync after:
   - object add;
   - object remove;
   - object replace;
   - link add;
   - link remove;
   - layer visibility changes.
6. Add tests/proofs:
   - add object via patch, then move it;
   - add hidden-layer object and ensure it is not rendered;
   - remove object and confirm related links are removed and indexes are clean.

## Validation

```powershell
npm run webgllib:audit-scene-runtime
dotnet test tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore
dotnet build CanDoItAll.Components.slnx --no-restore
```
