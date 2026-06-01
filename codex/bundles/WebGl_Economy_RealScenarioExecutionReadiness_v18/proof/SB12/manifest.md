# SB12 Manifest

Final validation completed on 2026-06-01.

Components proof:

- `components-dotnet-build.txt`: `dotnet build CanDoItAll.Components.slnx`
- `components-npm-audit-scene-runtime.txt`: `npm run webgllib:audit-scene-runtime`
- `components-npm-build-assets.txt`: `npm run webgllib:build-assets`
- `components-npm-verify-assets.txt`: `npm run webgllib:verify-assets`
- `components-webgllib-tests.txt`: `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj`
- `components-webglrunlib-tests.txt`: `dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj`

Economy proof:

- `economy-dotnet-build.txt`: `dotnet build CanDoItAll.Economy.slnx`
- `economy-boundary-audit.txt`: `powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/audit-simulation-boundaries.ps1`
- `economy-required-filtered-tests.txt`: `dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj --filter "RealProbe|SimulationSandbox|WebGlBridge|Snapshot"`
- `economy-full-tests.txt`: `dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj`

Known validation warnings:

- Components scene runtime audit passes with 11 pre-existing line-count warnings.
- Economy build passes with existing `ncalc` compatibility warnings and OpenTelemetry advisory warnings from the adjacent IPFS project.
