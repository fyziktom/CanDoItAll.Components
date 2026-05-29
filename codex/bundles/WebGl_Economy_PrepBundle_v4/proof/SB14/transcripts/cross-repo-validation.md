# SB14 Cross-Repo Validation Transcript

Invariant ID: SB14-cross-repo-validation

Command: `dotnet build CanDoItAll.Components.slnx -p:UseSharedCompilation=false`
ExitCode: 0
Result: Components solution build passed with 0 warnings and 0 errors.

Command: `dotnet test tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj -p:UseSharedCompilation=false -v:minimal`
ExitCode: 0
Result: Components WebGlLib tests passed: 18 total.

Command: `npm run webgllib:audit-scene-runtime`
ExitCode: 0
Result: Scene runtime audit passed with expected size warnings only.

Command: `npm run webgllib:model-diagnostics`
ExitCode: 0
Result: Diagnostics written for 43 model assets.

Command: `npm run webgllib:build-assets` and `npm run webgllib:verify-assets`
ExitCode: 0
Result: WebGlLib assets were up to date and verified.

Command: Browser Playwright validation for `/model-lab` and `/run-playback`
ExitCode: 0
Result: Model Lab rendered `models 1`, `meshes 9`, `fallbacks 0`; run playback rendered frame 2; both had nonblank pixel proof.

Command: `dotnet build CanDoItAll.Economy.slnx -p:UseSharedCompilation=false`
ExitCode: 0
Result: Economy solution build passed with existing dependency warnings.

Command: `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj -p:UseSharedCompilation=false -v:minimal`
ExitCode: 0
Result: Economy tests passed: 427 total.

Command: `powershell -ExecutionPolicy Bypass -File .\scripts\audit-simulation-boundaries.ps1`
ExitCode: 0
Result: Economy simulation boundary audit passed.

Command: No-coupling scans for Components WebGL libraries and new Economy Simulation projects
ExitCode: 0
Result: No forbidden cross-repo references found.
