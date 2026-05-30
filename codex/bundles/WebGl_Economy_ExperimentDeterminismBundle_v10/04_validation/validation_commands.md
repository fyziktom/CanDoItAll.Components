# Validation commands

## Components

```powershell
git branch --show-current
dotnet build CanDoItAll.Components.slnx --no-restore -p:UseSharedCompilation=false
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore -p:UseSharedCompilation=false
dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore -p:UseSharedCompilation=false
npm run webgllib:audit-scene-runtime
npm run webgllib:build-assets
npm run webgllib:verify-assets
```

## Economy

```powershell
git branch --show-current
.\scriptsudit-simulation-boundaries.ps1
dotnet build CanDoItAll.Economy.slnx --no-restore -p:UseSharedCompilation=false
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj --no-restore -p:UseSharedCompilation=false
```

## Required new proof

Each critical subbundle must create:

```text
proof/SBxx/manifest.md
proof/SBxx/semantic-invariants.md
proof/SBxx/transcripts/*.txt
proof/SBxx/source-assertions.txt
proof/SBxx/changed-file-hashes.txt
```

## Large-screen policy

Any browser proof for WebGL must use a desktop viewport of `1440x900` or larger. Do not add small/medium/mobile/tablet proof.
