# Validation commands

## Components repo

```powershell
git branch --show-current
git status --short
npm install
npm run webgllib:build-assets
npm run webgllib:verify-assets
npm run webgllib:audit-scene-runtime
dotnet build CanDoItAll.Components.slnx --no-restore -p:UseSharedCompilation=false
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore -p:UseSharedCompilation=false
```

## Economy repo

```powershell
git branch --show-current
git status --short
dotnet build CanDoItAll.Economy.slnx --no-restore -p:UseSharedCompilation=false
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj --no-restore -p:UseSharedCompilation=false
```

After adding the Economy boundary script:

```powershell
./scripts/audit-simulation-boundaries.ps1
```

## Anti-branch rule

Codex must include the current branch name in both implementation reports and must state that it did not create a new branch.
