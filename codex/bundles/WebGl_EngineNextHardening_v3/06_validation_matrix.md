# Validation Matrix

| Area | Required proof |
| --- | --- |
| Branch rule | `git branch --show-current`; no branch creation in logs |
| JS hygiene | `npm run webgllib:audit-scene-runtime` |
| Asset manifest | `npm run webgllib:build-assets` and `verify-assets` |
| GLB inventory | `npm run webgllib:inventory-glb` |
| WebGlLib build | `dotnet build src/CanDoItAll.Components.WebGlLib/...` |
| WebGlSandbox build | `dotnet build src/CanDoItAll.Components.WebGlSandbox/...` |
| Unit tests | `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/...` |
| Full solution | `dotnet build CanDoItAll.Components.slnx` |
| Tycoon village browser | screenshot, console, proof snapshot |
| Model Lab browser | primitive/model-low/model-high profiles, diagnostics |
| Idle scheduler | render count stops increasing in static scene |
| Motion | command result, motion completed callback, final object position |
| Patch failure | malformed patch returns detailed failure and does not throw |
| Scene document | deterministic round-trip and content hash tests |
| No domain leakage | scan WebGlLib for economy/ledger/account/market/well/community/process terms except documentation boundary files |
