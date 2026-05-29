# SB09 — Validation and Evidence

## Goal

Finish with repeatable proof and evidence.

## Required commands

```powershell
npm install
npm run webgllib:build-assets
npm run webgllib:verify-assets
npm run webgllib:audit-scene-runtime
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
dotnet build CanDoItAll.Components.slnx
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
```

## Browser proof

Use a large desktop viewport. Small-screen tuning is out of scope unless the user asks later.

Validate:

- `/tycoon-village` loads;
- optional `/model-lab` loads if implemented;
- primitive profile works;
- model-low/model-high profile attempts do not crash;
- invisible/bad models have diagnostics;
- fallback is visible when model is bad;
- idle scheduler does not render endlessly;
- motion command moves object smoothly;
- patch/export/import work;
- no console errors except known documented GLTF extension warnings;
- `window.CanDoItAll.webglWorkbench` still exists.

## Evidence folder

Write evidence to:

```text
artifacts/webgl-runtime-hardening-v2/
```

Expected files:

```text
01_INVENTORY.md
IMPLEMENTATION_REPORT.md
VALIDATION.md
runtime-audit.txt
browser-summary.json
browser-console.log
browser-proof.json
model-lab-proof.json optional
```

## Acceptance criteria

- Implementation report clearly lists what was done and what remains.
- Any known broken GLB conversions are named explicitly with diagnostics.
- The report confirms no new branch was created.

