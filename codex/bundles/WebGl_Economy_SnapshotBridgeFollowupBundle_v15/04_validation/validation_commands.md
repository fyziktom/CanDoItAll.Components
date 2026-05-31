# Validation commands

Run from the currently checked-out branches. Do not create a new branch.

## Components

```powershell
dotnet build .\CanDoItAll.Components.slnx
dotnet test .\tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet test .\tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj
npm install
npm run audit:webgllib
```

If npm script names differ, run the exact WebGL runtime audit tool directly:

```powershell
node .\tools\webgllib\audit-scene-runtime.cjs
```

## Economy

```powershell
dotnet build .\CanDoItAll.Economy.slnx
dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj
pwsh .\scripts\audit-simulation-boundaries.ps1
```

## Required proof artifacts

- changed-file hashes
- source assertions
- runtime audit transcript
- test transcript
- boundary audit transcript
- snapshot export/import proof
- bridge projection proof
- shared-resource and finite-resource readiness probes
