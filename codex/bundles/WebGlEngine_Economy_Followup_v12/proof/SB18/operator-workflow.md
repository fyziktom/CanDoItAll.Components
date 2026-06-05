# Operator workflow

Canonical validation sequence used for v12:

1. Components generic validation:
   `dotnet test tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore --filter "WebGlRunValidatorTests" -m:1 -p:UseSharedCompilation=false -p:BuildInParallel=false`

2. Components boundary audit:
   `node tools\webgllib\audit-webglrunlib-boundary.cjs --config tools\webgllib\domain-boundary-audit.config.json`

3. Economy targeted validation:
   `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --no-restore --filter "EconomyWebGlBridgeTests|EconomyWebGlBridgeReferenceTests|EconomyWebGlInitialSceneProjectorSplitTests|SimulationEconomicTrustHardeningTests|EconomyWebGlBridgeStrictMappingTests|EconomyWebGlBridgeStrictMappingDynamicTests|SimulationMetamorphicPropertyTests|EconomyPerformanceProbeTests" -m:1 -p:UseSharedCompilation=false -p:BuildInParallel=false`

4. Economy boundary audit:
   `.\scripts\audit-simulation-boundaries.ps1`

5. Multi-goods canary CLI:
   `dotnet run --no-build --project src\CanDoItAll.Economy.Cli\CanDoItAll.Economy.Cli.csproj -- scenario run --catalog src\CanDoItAll.Economy.Node\SimulationScenarios\EconomySimulationSandbox --scenario multi-goods-elite --output <bundle>\proof\SB10\multi-goods-run --clean --no-oracle`

6. Browser observer proof:
   start `CanDoItAll.Components.WebGlSandbox` on `http://127.0.0.1:5327`, then run `proof\SB14\browser-observer-real-state.mjs`.

7. Bundle validator:
   `python scripts\validate_bundle.py --stage completed`
