# Multi-goods canary closure

Status: passed.

The `multi-goods-elite` scenario pack loads in research-strict mode, materializes headlessly, evaluates six metrics/invariants, and reports `headless-valid` with `researchReady=false` when oracle/browser proof is intentionally not supplied by the CLI run.

Generated artifacts:

- `multi-goods-headless-report.json`
- `multi-goods-readiness-report.json`
- `multi-goods-run/headless-run-manifest.json`
- `multi-goods-run/readiness-report.json`
- `multi-goods-run/metrics-invariants.json`

Validation:

- `dotnet run --no-build --project src\CanDoItAll.Economy.Cli\CanDoItAll.Economy.Cli.csproj -- scenario run --catalog src\CanDoItAll.Economy.Node\SimulationScenarios\EconomySimulationSandbox --scenario multi-goods-elite --output <bundle>\proof\SB10\multi-goods-run --clean --no-oracle`
- `proof/SB10/multi-goods-cli-run.txt`
