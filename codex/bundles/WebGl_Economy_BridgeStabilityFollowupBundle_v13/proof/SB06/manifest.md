# SB06 Proof - Economy bridge dependency strategy

## Scope

Hardened the bridge reference to Components WebGlRunLib with a local project default and package-mode opt-in.

## Changed-file hashes

- `cc7983cfb0ef52ce5717cd7ff4903dce363dabc668742d0b533f118c54a1463c  C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj`
- `65878bce5ed6d3c2937246a6d6ff09fb707413d991b572ed14da0a535c18be12  C:\repositories\CanDoItAll.Economy\scripts\audit-simulation-boundaries.ps1`
- `12ae68fd038f30dc43220e399d68268da4e0ad3e1d4f333495c1679979e5291c  C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\EconomyWebGlBridgeTests.cs`

## Validation transcript

- `powershell -ExecutionPolicy Bypass -File .\scripts\audit-simulation-boundaries.ps1`: pass.
- `dotnet build .\CanDoItAll.Economy.slnx`: pass, 44 warnings, 0 errors.
- Full Economy tests: pass, 483 tests.

## Semantic invariants

- Local sibling development remains easy when the Components project exists.
- Package consumption is explicit through `UseComponentsWebGlRunLibPackage`.
- A missing local project now fails with a clear MSBuild error instead of an opaque restore/build failure.
