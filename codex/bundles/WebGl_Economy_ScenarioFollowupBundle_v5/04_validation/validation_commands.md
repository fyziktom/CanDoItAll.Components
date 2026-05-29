# Validation commands

## Components repo

```powershell
git branch --show-current
npm install
npm run webgllib:audit-scene-runtime
npm run webgllib:inventory-glb
npm run webgllib:build-assets
npm run webgllib:verify-assets
dotnet build src\CanDoItAll.Components.WebGlLib\CanDoItAll.Components.WebGlLib.csproj --no-restore
dotnet build src\CanDoItAll.Components.WebGlRunLib\CanDoItAll.Components.WebGlRunLib.csproj --no-restore
dotnet build src\CanDoItAll.Components.WebGlSandbox\CanDoItAll.Components.WebGlSandbox.csproj --no-restore
dotnet test tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore
dotnet build CanDoItAll.Components.slnx --no-restore
```

## Economy repo

```powershell
git branch --show-current
pwsh scripts\audit-simulation-boundaries.ps1
dotnet build src\CanDoItAll.Economy.Simulation.Abstractions\CanDoItAll.Economy.Simulation.Abstractions.csproj --no-restore
dotnet build src\CanDoItAll.Economy.Simulation.SimpleAccounts\CanDoItAll.Economy.Simulation.SimpleAccounts.csproj --no-restore
dotnet build src\CanDoItAll.Economy.Simulation.Ledger\CanDoItAll.Economy.Simulation.Ledger.csproj --no-restore
dotnet build src\CanDoItAll.Economy.Simulation.Visualization\CanDoItAll.Economy.Simulation.Visualization.csproj --no-restore
dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --no-restore
dotnet build CanDoItAll.Economy.slnx --no-restore
```

## Required browser proofs

Components sandbox:
- `/tycoon-village`
- `/model-lab`
- `/run-playback`

Economy visual proofs are not required yet because no WebGL bridge should be introduced in this wave.
