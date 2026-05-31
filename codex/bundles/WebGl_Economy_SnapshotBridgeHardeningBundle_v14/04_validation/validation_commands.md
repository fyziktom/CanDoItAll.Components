# Validation commands

## Components repo

```powershell
git branch --show-current
dotnet build .\CanDoItAll.Components.slnx
dotnet test .\tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet test .\tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj
npm run webgllib:audit-scene-runtime
```

## Economy repo

```powershell
git branch --show-current
dotnet build .\CanDoItAll.Economy.slnx
dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj
pwsh .\scripts\audit-simulation-boundaries.ps1
```

## Required proof

- changed-file hashes,
- command transcripts,
- source assertion notes,
- snapshot JSON roundtrip proof,
- bridge projection proof,
- JS runtime audit output.
