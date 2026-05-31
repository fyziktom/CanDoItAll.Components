# SB09 Proof - Remove example-specific leakage

## Scope

Kept Components generic and confined example-specific fixture names to Economy fixtures/tests/proof.

## Changed-file hashes

- `0ad1a3e6b3d683dc611e33f52498f9aa50c2418d3128067edbcb15602c26e286  C:\repositories\CanDoItAll.Components\tools\webgllib\audit-scene-runtime.cjs`
- `65878bce5ed6d3c2937246a6d6ff09fb707413d991b572ed14da0a535c18be12  C:\repositories\CanDoItAll.Economy\scripts\audit-simulation-boundaries.ps1`

## Validation transcript

- `powershell -ExecutionPolicy Bypass -File .\scripts\audit-simulation-boundaries.ps1`: pass.
- `npm run webgllib:audit-scene-runtime`: pass, existing file-size warnings only.
- `dotnet build .\CanDoItAll.Components.slnx`: pass, 0 warnings, 0 errors.

## Semantic invariants

- Components generic runtime code has no Economy project reference.
- Fixture examples are readiness probes, not generic abstractions.
- Bridge-specific source terms stay in Economy-side code.
