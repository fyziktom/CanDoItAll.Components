# SB04 — Refactoring Gate 1

Stop feature work and review the result of SB01-SB03.

## Required checks

1. Run:
   ```powershell
   npm run webgllib:audit-scene-runtime
   dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
   dotnet build CanDoItAll.Components.slnx
   ```

2. Review:
   - runtime module line counts,
   - command result duplication,
   - patch command failure behavior,
   - branch rule compliance,
   - no economy/process/game semantics in WebGlLib.

3. Create:
   ```text
   artifacts/webgl-engine-next-hardening/reviews/refactoring-gate-1.md
   ```

## Rule

If this gate finds architectural drift, fix it before continuing.
