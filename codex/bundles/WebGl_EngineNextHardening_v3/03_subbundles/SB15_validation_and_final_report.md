# SB15 — Validation and Final Report

## Goal

Produce reliable evidence and a final implementation report.

## Required validation

```powershell
git status --short --branch
npm install
npm run webgllib:audit-scene-runtime
npm run webgllib:inventory-glb
npm run webgllib:build-assets
npm run webgllib:verify-assets
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet build CanDoItAll.Components.slnx
```

## Browser proof

Run sandbox and verify:
- `/tycoon-village`
- `/model-lab`
- optional `/run-playback`

Collect:
- console logs
- screenshots
- proof snapshots
- runtime diagnostics
- model diagnostics
- idle render metrics

## Final report

Create:

```text
artifacts/webgl-engine-next-hardening/IMPLEMENTATION_REPORT.md
```

Must include:
- branch used,
- no-new-branch confirmation,
- files changed,
- features implemented,
- features deliberately deferred,
- WebGlLib vs WebGlRunLib boundary,
- Economy future boundary,
- validation results,
- known model issues,
- remaining risks.
