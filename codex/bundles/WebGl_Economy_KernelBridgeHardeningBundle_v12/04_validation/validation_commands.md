# Validation Commands

## Components

```powershell
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj
npm run webgllib:audit-scene-runtime
```

## Economy

```powershell
dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj
pwsh ./scripts/audit-simulation-boundaries.ps1
```

## Cross-repo

```powershell
# Components must not reference Economy
rg "CanDoItAll\.Economy" src/CanDoItAll.Components.* tests -g "*.csproj" -g "*.cs" -g "*.js"

# Economy low-level projects must not reference WebGL/Components
rg "CanDoItAll\.Components|WebGl" src/CanDoItAll.Economy.Simulation.Abstractions src/CanDoItAll.Economy.Simulation.SimpleAccounts src/CanDoItAll.Economy.Simulation.Visualization -g "*.csproj" -g "*.cs"

# WebGL desktop-only policy
rg "mobile|tablet|phone|small-screen|medium-screen" docs src codex -g "*.md" -g "*.cs" -g "*.js"
```

Any match must be either intentionally allowed in docs/audits/tests or removed.
