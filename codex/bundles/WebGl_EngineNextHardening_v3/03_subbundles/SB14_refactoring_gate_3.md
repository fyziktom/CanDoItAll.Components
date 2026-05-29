# SB14 — Refactoring Gate 3

Stop after SB09-SB13.

## Required checks

```powershell
npm run webgllib:audit-scene-runtime
npm run webgllib:inventory-glb
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet build CanDoItAll.Components.slnx
```

If `WebGlRunLib` was created:

```powershell
dotnet build src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj
```

## Browser checks

- `/tycoon-village`
- `/model-lab`
- generic `/run-playback` if implemented

Record:
- screenshots
- console logs
- proof snapshots
- diagnostics JSON
- render idle proof

## Review questions

1. Did any WebGlLib API gain economy/process/game-specific names?
2. Did any JS runtime file exceed thresholds?
3. Are command failures detailed and non-throwing?
4. Does a static scene become idle?
5. Can a scene document round-trip deterministically?
6. Are model visibility issues visible in diagnostics?
7. Is future WebGlRunLib still generic?
8. Are Economy repo boundaries documented but not implemented in Components?

Write:

```text
artifacts/webgl-engine-next-hardening/reviews/refactoring-gate-3.md
```
