# Validation commands

## Components

```powershell
git branch --show-current
npm install
npm run webgllib:build-assets
npm run webgllib:verify-assets
npm run webgllib:audit-scene-runtime
dotnet build CanDoItAll.Components.slnx --no-restore -p:UseSharedCompilation=false
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore -p:UseSharedCompilation=false
dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore -p:UseSharedCompilation=false
```

## Economy

```powershell
git branch --show-current
pwsh scripts/audit-simulation-boundaries.ps1
dotnet build CanDoItAll.Economy.slnx --no-restore -p:UseSharedCompilation=false
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj --no-restore -p:UseSharedCompilation=false
```

## Large-screen WebGL proof only

Run browser proof only at:

```text
1440x900
1920x1080
```

Do not add mobile/tablet proof requirements.
