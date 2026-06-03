# SB01 current-state inventory

Generated: 2026-06-02T16:08:51.1527863-04:00

## Repositories

### Components
- Root: `repo://` / `C:\repositories\CanDoItAll.Components`
- Branch/status: `## webgl-engine...origin/webgl-engine [ahead 1] ?? codex/bundles/WebGlEngine_Economy_Followup_Hardening_v4/proof/SB01/transcripts/ ?? codex/bundles/WebGlEngine_Economy_Followup_Hardening_v4/scripts/audit_proof_integrity.py`
- HEAD: `a43dfeffd1b2bf220dc34be3a3400a63771ea48c`
- Last commit: `a43dfeff 2026-06-02T15:59:47-04:00 added bundle`
- CodeAnalytics WebGlRunLib snapshot: `snap-20260602200456-4fb1b8eb`
- CodeAnalytics WebGlLib snapshot: `snap-20260602200507-68b70ca1`

#### Components package/TFM signals
```text
Directory.Build.props:8:    <CanDoItAllPackageProofSuffix Condition="'$(CanDoItAllPackageProofSuffix)' == ''"></CanDoItAllPackageProofSuffix>
Directory.Build.props:9:    <Version>$(CanDoItAllPackageBaseVersion)$(CanDoItAllPackageProofSuffix)</Version>
tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj:4:    <TargetFramework>net10.0</TargetFramework>
tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj:11:    <PackageReference Include="coverlet.collector" Version="6.0.4" />
tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj:12:    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj:13:    <PackageReference Include="xunit" Version="2.9.3" />
tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj:14:    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj:4:    <TargetFramework>net10.0</TargetFramework>
tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj:11:    <PackageReference Include="coverlet.collector" Version="6.0.4" />
tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj:12:    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj:13:    <PackageReference Include="xunit" Version="2.9.3" />
tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj:14:    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
src\CanDoItAll.Components.WebGlRunLib\CanDoItAll.Components.WebGlRunLib.csproj:4:    <TargetFramework>net10.0</TargetFramework>
src\CanDoItAll.Components.WebGlLib\CanDoItAll.Components.WebGlLib.csproj:4:    <TargetFramework>net10.0</TargetFramework>
src\CanDoItAll.Components.WebGlLib\CanDoItAll.Components.WebGlLib.csproj:15:    <PackageReference Include="Microsoft.AspNetCore.Components.Web" Version="10.0.4" />
```

### Economy
- Root: `repo://CanDoItAll.Economy/` / `C:\repositories\CanDoItAll.Economy`
- Branch/status: `## main...origin/main`
- HEAD: `6d43b6bc911f2a608ed3ee75bde286d8f83c1121`
- Last commit: `6d43b6bc 2026-06-02T14:24:18-04:00 phase22`
- CodeAnalytics SimulationSandbox snapshot: `snap-20260602200530-25061c55`
- CodeAnalytics Simulation.WebGlBridge snapshot: `snap-20260602200548-7ea5fabf`

#### Economy package/TFM signals
```text
src\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj:3:    <TargetFramework>net10.0</TargetFramework>
src\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj:13:    <ProjectReference Include="..\CanDoItAll.Economy.Simulation.Abstractions\CanDoItAll.Economy.Simulation.Abstractions.csproj" />
src\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj:14:    <ProjectReference Include="..\CanDoItAll.Economy.Simulation.Visualization\CanDoItAll.Economy.Simulation.Visualization.csproj" />
src\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj:15:    <ProjectReference Condition="'$(UseComponentsWebGlRunLibPackage)' != 'true' and Exists('$(ComponentsWebGlRunLibProject)')" Include="$(ComponentsWebGlRunLibProject)" />
src\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj:16:    <PackageReference Condition="'$(UseComponentsWebGlRunLibPackage)' == 'true'" Include="CanDoItAll.Components.WebGlLib" Version="$(ComponentsWebGlLibPackageVersion)" />
src\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj:17:    <PackageReference Condition="'$(UseComponentsWebGlRunLibPackage)' == 'true'" Include="CanDoItAll.Components.WebGlRunLib" Version="$(ComponentsWebGlRunLibPackageVersion)" />
src\CanDoItAll.Economy.SimulationSandbox\CanDoItAll.Economy.SimulationSandbox.csproj:3:    <TargetFramework>net10.0</TargetFramework>
src\CanDoItAll.Economy.SimulationSandbox\CanDoItAll.Economy.SimulationSandbox.csproj:8:    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="10.0.4" />
src\CanDoItAll.Economy.SimulationSandbox\CanDoItAll.Economy.SimulationSandbox.csproj:9:    <ProjectReference Include="..\CanDoItAll.Economy.Simulation.Abstractions\CanDoItAll.Economy.Simulation.Abstractions.csproj" />
src\CanDoItAll.Economy.SimulationSandbox\CanDoItAll.Economy.SimulationSandbox.csproj:10:    <ProjectReference Include="..\CanDoItAll.Economy.Simulation.SimpleAccounts\CanDoItAll.Economy.Simulation.SimpleAccounts.csproj" />
src\CanDoItAll.Economy.SimulationSandbox\CanDoItAll.Economy.SimulationSandbox.csproj:11:    <ProjectReference Include="..\CanDoItAll.Economy.Simulation.Visualization\CanDoItAll.Economy.Simulation.Visualization.csproj" />
src\CanDoItAll.Economy.SimulationSandbox\CanDoItAll.Economy.SimulationSandbox.csproj:12:    <ProjectReference Include="..\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj" AdditionalProperties="UseComponentsWebGlRunLibPackage=$(UseComponentsWebGlRunLibPackage);ComponentsWebGlRunLibPackageVersion=$(ComponentsWebGlRunLibPackageVersion);ComponentsWebGlLibPackageVersion=$(ComponentsWebGlLibPackageVersion)" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:3:    <TargetFramework>net10.0</TargetFramework>
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:9:    <ProjectReference Include="..\CanDoItAll.Economy.BusinessObjects\CanDoItAll.Economy.BusinessObjects.csproj" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:10:    <ProjectReference Include="..\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:11:    <ProjectReference Include="..\CanDoItAll.Economy.Core\CanDoItAll.Economy.Core.csproj" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:12:    <ProjectReference Include="..\CanDoItAll.Economy.Cryptography\CanDoItAll.Economy.Cryptography.csproj" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:13:    <ProjectReference Include="..\CanDoItAll.Economy.Ledger\CanDoItAll.Economy.Ledger.csproj" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:14:    <ProjectReference Include="..\CanDoItAll.Economy.Persistence.EFCore\CanDoItAll.Economy.Persistence.EFCore.csproj" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:15:    <ProjectReference Include="..\CanDoItAll.Economy.Persistence.PostgreSql\CanDoItAll.Economy.Persistence.PostgreSql.csproj" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:16:    <ProjectReference Include="..\CanDoItAll.Economy.Sdk\CanDoItAll.Economy.Sdk.csproj" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:17:    <ProjectReference Include="..\CanDoItAll.Economy.SimulationSandbox\CanDoItAll.Economy.SimulationSandbox.csproj" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:23:    <PackageReference Include="CanDoItAll.Components.BaseLib" Version="$(CanDoItAllComponentsPackageVersion)" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:24:    <PackageReference Include="CanDoItAll.Components.Charts" Version="$(CanDoItAllComponentsPackageVersion)" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:25:    <PackageReference Include="CanDoItAll.Components.Mermaid" Version="$(CanDoItAllComponentsPackageVersion)" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:26:    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.4" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:27:    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.4" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:28:    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.4" />
src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj:29:    <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.8.0" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:3:    <TargetFramework>net10.0</TargetFramework>
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:7:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Core\CanDoItAll.Economy.Core.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:8:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Accounts\CanDoItAll.Economy.Accounts.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:9:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Investments\CanDoItAll.Economy.Investments.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:10:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Investments.Persistence.EFCore\CanDoItAll.Economy.Investments.Persistence.EFCore.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:11:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Markets\CanDoItAll.Economy.Markets.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:12:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Memory\CanDoItAll.Economy.Memory.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:13:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Memory.Regulation\CanDoItAll.Economy.Memory.Regulation.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:14:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Memory.Sandbox\CanDoItAll.Economy.Memory.Sandbox.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:15:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Sandbox\CanDoItAll.Economy.Sandbox.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:16:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.BusinessObjects\CanDoItAll.Economy.BusinessObjects.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:17:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Cli\CanDoItAll.Economy.Cli.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:18:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:19:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Cryptography\CanDoItAll.Economy.Cryptography.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:20:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Ledger\CanDoItAll.Economy.Ledger.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:21:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Node\CanDoItAll.Economy.Node.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:22:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Persistence.EFCore\CanDoItAll.Economy.Persistence.EFCore.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:23:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Persistence.PostgreSql\CanDoItAll.Economy.Persistence.PostgreSql.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:24:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Sdk\CanDoItAll.Economy.Sdk.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:25:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Simulator\CanDoItAll.Economy.Simulator.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:26:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Simulator.Components\CanDoItAll.Economy.Simulator.Components.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:27:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Simulator.Persistence.EFCore\CanDoItAll.Economy.Simulator.Persistence.EFCore.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:28:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Simulation.Abstractions\CanDoItAll.Economy.Simulation.Abstractions.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:29:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Simulation.Ledger\CanDoItAll.Economy.Simulation.Ledger.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:30:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Simulation.SimpleAccounts\CanDoItAll.Economy.Simulation.SimpleAccounts.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:31:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.SimulationSandbox\CanDoItAll.Economy.SimulationSandbox.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:32:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Simulation.Visualization\CanDoItAll.Economy.Simulation.Visualization.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:33:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:34:    <ProjectReference Include="..\..\src\CanDoItAll.Economy.Storage.InMemory\CanDoItAll.Economy.Storage.InMemory.csproj" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:35:    <PackageReference Include="coverlet.collector" Version="6.0.4" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:36:    <PackageReference Include="bunit.web" Version="1.40.0" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:37:    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.4" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:38:    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.4" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:39:    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:40:    <PackageReference Include="xunit" Version="2.9.3" />
tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj:41:    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:3:    <TargetFramework>net10.0</TargetFramework>
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:15:    <PackageReference Include="Microsoft.AspNetCore.Components.Web" Version="10.0.4" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:16:    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="10.0.4" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:20:    <ProjectReference Include="..\CanDoItAll.Economy.Accounts\CanDoItAll.Economy.Accounts.csproj" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:21:    <ProjectReference Include="..\CanDoItAll.Economy.BusinessObjects\CanDoItAll.Economy.BusinessObjects.csproj" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:22:    <ProjectReference Include="..\CanDoItAll.Economy.Cryptography\CanDoItAll.Economy.Cryptography.csproj" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:23:    <ProjectReference Include="..\CanDoItAll.Economy.Investments\CanDoItAll.Economy.Investments.csproj" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:24:    <ProjectReference Include="..\CanDoItAll.Economy.Ledger\CanDoItAll.Economy.Ledger.csproj" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:25:    <ProjectReference Include="..\CanDoItAll.Economy.Markets\CanDoItAll.Economy.Markets.csproj" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:26:    <ProjectReference Include="..\CanDoItAll.Economy.Memory\CanDoItAll.Economy.Memory.csproj" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:27:    <ProjectReference Include="..\CanDoItAll.Economy.Memory.Sandbox\CanDoItAll.Economy.Memory.Sandbox.csproj" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:28:    <ProjectReference Include="..\CanDoItAll.Economy.Sandbox\CanDoItAll.Economy.Sandbox.csproj" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:29:    <ProjectReference Include="..\CanDoItAll.Economy.Sdk\CanDoItAll.Economy.Sdk.csproj" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:30:    <ProjectReference Include="..\CanDoItAll.Economy.SimulationSandbox\CanDoItAll.Economy.SimulationSandbox.csproj" AdditionalProperties="UseComponentsWebGlRunLibPackage=$(UseComponentsWebGlPackages);ComponentsWebGlRunLibPackageVersion=$(ComponentsWebGlRunLibPackageVersion);ComponentsWebGlLibPackageVersion=$(ComponentsWebGlLibPackageVersion)" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:31:    <ProjectReference Include="..\CanDoItAll.Economy.Storage.InMemory\CanDoItAll.Economy.Storage.InMemory.csproj" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:32:    <ProjectReference Condition="'$(UseComponentsWebGlPackages)' != 'true' and Exists('$(ComponentsWebGlLibProject)')" Include="$(ComponentsWebGlLibProject)" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:33:    <ProjectReference Condition="'$(UseComponentsWebGlPackages)' != 'true' and Exists('$(ComponentsWebGlRunLibProject)')" Include="$(ComponentsWebGlRunLibProject)" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:34:    <PackageReference Include="CanDoItAll.Components.BaseLib" Version="$(CanDoItAllComponentsPackageVersion)" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:35:    <PackageReference Include="CanDoItAll.Components.Charts" Version="$(CanDoItAllComponentsPackageVersion)" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:36:    <PackageReference Include="CanDoItAll.Components.Mermaid" Version="$(CanDoItAllComponentsPackageVersion)" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:37:    <PackageReference Condition="'$(UseComponentsWebGlPackages)' == 'true'" Include="CanDoItAll.Components.WebGlLib" Version="$(ComponentsWebGlLibPackageVersion)" />
src\CanDoItAll.Economy.Components\CanDoItAll.Economy.Components.csproj:38:    <PackageReference Condition="'$(UseComponentsWebGlPackages)' == 'true'" Include="CanDoItAll.Components.WebGlRunLib" Version="$(ComponentsWebGlRunLibPackageVersion)" />
```

## Previous proof artifacts
- Empty proof artifacts found across previous bundles: 19
```text
repo://codex\bundles\WebGlEngine_Economy_Followup_Hardening_v2\proof\SB02\transcripts\node-server.stderr.txt
repo://codex\bundles\WebGlEngine_Economy_Followup_Hardening_v2\proof\SB03\transcripts\components-sandbox-server-after-restart.stderr.txt
repo://codex\bundles\WebGlEngine_Economy_Followup_Hardening_v2\proof\SB03\transcripts\components-sandbox-server.stderr.txt
repo://codex\bundles\WebGlEngine_Economy_Followup_Hardening_v2\proof\SB05\transcripts\webglsandbox-server-stderr.txt
repo://codex\bundles\WebGlEngine_Economy_Followup_Hardening_v2\proof\SB08\transcripts\webglsandbox-server-stderr.txt
repo://codex\bundles\WebGlEngine_Economy_Followup_Hardening_v2\proof\SB11\browser\economy-sandbox-console-errors.log
repo://codex\bundles\WebGlEngine_Economy_Followup_Hardening_v2\proof\SB11\browser\run-playback-console-errors.log
repo://codex\bundles\WebGlEngine_Economy_Followup_Hardening_v2\proof\SB11\transcripts\components-webglsandbox-server.err.txt
repo://codex\bundles\WebGlEngine_Economy_Followup_Hardening_v2\proof\SB11\transcripts\economy-node-release-server.err.txt
repo://codex\bundles\WebGlEngine_Economy_Hardening_Bundle_v1\proof\SB02\transcripts\webglsandbox-host.stderr.txt
repo://codex\bundles\WebGlEngine_Economy_Hardening_Bundle_v1\proof\SB03\transcripts\webglsandbox-host-failing-first.stderr.txt
repo://codex\bundles\WebGlEngine_Economy_Hardening_Bundle_v1\proof\SB03\transcripts\webglsandbox-host-final.stderr.txt
repo://codex\bundles\WebGlEngine_Economy_Hardening_Bundle_v1\proof\SB03\transcripts\webglsandbox-host-passing.stderr.txt
repo://codex\bundles\WebGlEngine_Economy_Hardening_Bundle_v1\proof\SB04\transcripts\webglsandbox-host-sb04-failing.stderr.txt
repo://codex\bundles\WebGlEngine_Economy_Hardening_Bundle_v1\proof\SB04\transcripts\webglsandbox-host-sb04-passing.stderr.txt
repo://codex\bundles\WebGlEngine_Economy_Hardening_Bundle_v1\proof\SB05\transcripts\webglsandbox-5284.err.log
repo://codex\bundles\WebGlEngine_Economy_Hardening_Bundle_v1\proof\SB09\transcripts\webglsandbox-server.err.log
repo://codex\bundles\WebGlEngine_Economy_Hardening_Bundle_v1\proof\SB13\transcripts\economy-node-server.err.txt
repo://codex\bundles\WebGlEngine_Economy_Hardening_Bundle_v1\proof\SB13\transcripts\webglsandbox-server.err.txt
```

## Refactor gate decision
- SB01 entry gate: Pass. Prepared-stage validator passed and SB01 has no prerequisite subbundle.
- SB01 closure condition: proof integrity script added, failing-first and passing transcripts captured, source assertions and baseline hashes recorded.
