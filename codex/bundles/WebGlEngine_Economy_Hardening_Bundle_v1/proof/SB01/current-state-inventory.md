# SB01 Current-State Inventory

Generated UTC: 2026-06-01T23:34:58.0913198Z

## Git refs

- Components branch: webgl-engine
- Components HEAD: 70bb17fb31467f91dbaa4aea91283c5ba5f7e9f1
- Economy branch: main
- Economy HEAD: 36ba8ec7f2dc6bfd1f197297f29b7ac465c09d0f

## Git status

### Components
```text
## webgl-engine...origin/webgl-engine [ahead 1]
?? codex/bundles/WebGlEngine_Economy_Hardening_Bundle_v1/proof/SB01/changed-file-baseline.md
?? codex/bundles/WebGlEngine_Economy_Hardening_Bundle_v1/proof/SB01/transcripts/
```

### Economy
```text
## main...origin/main
```

## CodeAnalytics snapshots

- Components scoped snapshot: snap-20260601231917-d9c63db7 (5 scoped projects, 155 documents, forced refresh).
- Economy scoped snapshot: snap-20260601232114-ff83ac75 (WebGlBridge + Economy tests, 2 scoped projects, 109 documents, forced refresh; workspace warnings recorded in tool output and baseline transcripts).

## Components solution projects
```text
Project(s)
----------
src\CanDoItAll.Components.BaseLib\CanDoItAll.Components.BaseLib.csproj
src\CanDoItAll.Components.CanvasLib\CanDoItAll.Components.CanvasLib.csproj
src\CanDoItAll.Components.Charts\CanDoItAll.Components.Charts.csproj
src\CanDoItAll.Components.Common\CanDoItAll.Components.Common.csproj
src\CanDoItAll.Components.Mermaid\CanDoItAll.Components.Mermaid.csproj
src\CanDoItAll.Components.OverlayLib\CanDoItAll.Components.OverlayLib.csproj
src\CanDoItAll.Components.Sandbox\CanDoItAll.Components.Sandbox.csproj
src\CanDoItAll.Components.WebGlLib\CanDoItAll.Components.WebGlLib.csproj
src\CanDoItAll.Components.WebGlRunLib\CanDoItAll.Components.WebGlRunLib.csproj
src\CanDoItAll.Components.WebGlSandbox\CanDoItAll.Components.WebGlSandbox.csproj
tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj
tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj
```

## Economy solution projects
```text
Project(s)
----------
examples\CanDoItAll.Economy.AppHost.DcpProxy\CanDoItAll.Economy.AppHost.DcpProxy.csproj
examples\CanDoItAll.Economy.AppHost\CanDoItAll.Economy.AppHost.csproj
examples\CanDoItAll.Economy.Components.Demo\CanDoItAll.Economy.Components.Demo.csproj
examples\CanDoItAll.Economy.ConsoleDemo\CanDoItAll.Economy.ConsoleDemo.csproj
examples\CanDoItAll.Economy.MarketSandbox.Demo\CanDoItAll.Economy.MarketSandbox.Demo.csproj
examples\CanDoItAll.Economy.Simulator.App\CanDoItAll.Economy.Simulator.App.csproj
integrations\CanDoItAll.Modules.Economy\CanDoItAll.Modules.Economy.csproj
src\CanDoItAll.Economy.Accounts\CanDoItAll.Economy.Accounts.csproj
src\CanDoItAll.Economy.BusinessObjects\CanDoItAll.Economy.BusinessObjects.csproj
src\CanDoItAll.Economy.Cli\CanDoItAll.Economy.Cli.csproj
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj
src\CanDoItAll.Economy.Core\CanDoItAll.Economy.Core.csproj
src\CanDoItAll.Economy.Cryptography\CanDoItAll.Economy.Cryptography.csproj
src\CanDoItAll.Economy.Investments.Persistence.EFCore\CanDoItAll.Economy.Investments.Persistence.EFCore.csproj
src\CanDoItAll.Economy.Investments\CanDoItAll.Economy.Investments.csproj
src\CanDoItAll.Economy.Ledger\CanDoItAll.Economy.Ledger.csproj
src\CanDoItAll.Economy.Markets\CanDoItAll.Economy.Markets.csproj
src\CanDoItAll.Economy.Memory.Regulation\CanDoItAll.Economy.Memory.Regulation.csproj
src\CanDoItAll.Economy.Memory.Sandbox\CanDoItAll.Economy.Memory.Sandbox.csproj
src\CanDoItAll.Economy.Memory\CanDoItAll.Economy.Memory.csproj
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj
src\CanDoItAll.Economy.Persistence.EFCore\CanDoItAll.Economy.Persistence.EFCore.csproj
src\CanDoItAll.Economy.Persistence.PostgreSql\CanDoItAll.Economy.Persistence.PostgreSql.csproj
src\CanDoItAll.Economy.Sandbox\CanDoItAll.Economy.Sandbox.csproj
src\CanDoItAll.Economy.Sdk\CanDoItAll.Economy.Sdk.csproj
src\CanDoItAll.Economy.Simulation.Abstractions\CanDoItAll.Economy.Simulation.Abstractions.csproj
src\CanDoItAll.Economy.Simulation.Ledger\CanDoItAll.Economy.Simulation.Ledger.csproj
src\CanDoItAll.Economy.Simulation.SimpleAccounts\CanDoItAll.Economy.Simulation.SimpleAccounts.csproj
src\CanDoItAll.Economy.Simulation.Visualization\CanDoItAll.Economy.Simulation.Visualization.csproj
src\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj
src\CanDoItAll.Economy.SimulationSandbox\CanDoItAll.Economy.SimulationSandbox.csproj
src\CanDoItAll.Economy.Simulator.Components\CanDoItAll.Economy.Simulator.Components.csproj
src\CanDoItAll.Economy.Simulator.Persistence.EFCore\CanDoItAll.Economy.Simulator.Persistence.EFCore.csproj
src\CanDoItAll.Economy.Simulator\CanDoItAll.Economy.Simulator.csproj
src\CanDoItAll.Economy.Storage.InMemory\CanDoItAll.Economy.Storage.InMemory.csproj
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj
```

## Current-state decision record

- Components WebGlLib/WebGlRunLib/WebGlSandbox are present and build on webgl-engine.
- Economy CanDoItAll.Economy.Simulation.WebGlBridge and CanDoItAll.Economy.SimulationSandbox are present and build through the Economy solution.
- The bundle observation that the repos may have advanced since preparation is current: the refreshed Components snapshot has more scoped WebGlRunLib/WebGlSandbox documents than the cached May 30 snapshot.
- The SB02-SB13 implementation phases remain necessary because the audit is a baseline only; it does not close runtime patching, incremental rendering, resource ownership, or final browser stress proof.
