# SB08 — Refactoring Gate 2

Stop feature work after SB05-SB07.

## Required checks

```powershell
npm run webgllib:audit-scene-runtime
npm run webgllib:inventory-glb
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet build CanDoItAll.Components.slnx
```

## Review focus

- asset cache lifetime
- resource disposal diagnostics
- model import recipe boundaries
- scene document hash determinism
- no run/economy leakage into WebGlLib

Write:

```text
artifacts/webgl-engine-next-hardening/reviews/refactoring-gate-2.md
```
