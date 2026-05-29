# 06 - Validation checklist

Run these commands:

```powershell
npm install
npm run webgllib:build-assets
npm run webgllib:verify-assets
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
dotnet build CanDoItAll.Components.slnx
```

Browser checks:

```text
/tycoon-village
/asset-catalog
```

Proof scenarios:

1. Scene loads with primitive asset profile.
2. Scene loads with GLB mixed asset profile.
3. Scene loads with high-detail asset profile, or gracefully falls back.
4. Click selects object.
5. Hover updates object id.
6. Drag a draggable object.
7. `ObjectsMoved` callback fires.
8. Enqueue smooth motion command.
9. Motion reaches target.
10. Export scene state.
11. Reload/import scene state.
12. Workbench namespace still exists.
13. Scene namespace exists.
14. Missing asset does not crash runtime.
15. Runtime diagnostics report missing asset/fallback.
16. No console errors except documented Three.js GLB extension warnings.

Performance checks:

```text
Initial load time
Loaded model count
Fallback count
Average frame time during static scene
Average frame time during active motion
Max device pixel ratio respected
```
