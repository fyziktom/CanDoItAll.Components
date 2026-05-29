# Single File Execution Prompt

Copy/paste this to Codex:

```text
You are working in fyziktom/CanDoItAll.Components.

IMPORTANT BRANCH RULE:
Work in the currently checked-out branch only. The expected branch is webgl-engine. Do not create a new branch, do not switch branches, and do not push to any new branch. Start by running git status --short --branch and git branch --show-current. If the branch is not webgl-engine, stop and report it.

Implement the next WebGL engine hardening from the provided bundle. Keep WebGlLib completely generic and domain-neutral. Do not add economy, ledger, account, market, well, community, entrepreneur, governance, process, or game-specific semantics into WebGlLib.

Execute subbundles in order:
SB01 inventory and evidence gate
SB02 JS runtime module cleanliness
SB03 command result and patch hardening
SB04 refactoring gate 1
SB05 resource lifetime and asset cache
SB06 model import visibility and recipes
SB07 scene document persistence semantics
SB08 refactoring gate 2
SB09 render scheduler and clock boundary
SB10 interaction, motion, scene commands
SB11 layers visibility and scene indexing
SB12 generic WebGlRunLib contracts only if previous gates pass
SB13 economy repo boundary plan as docs only, no economy code in Components
SB14 refactoring gate 3
SB15 validation and final report

Run validation:
npm run webgllib:audit-scene-runtime
npm run webgllib:inventory-glb
npm run webgllib:build-assets
npm run webgllib:verify-assets
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
dotnet build CanDoItAll.Components.slnx

Add browser proof for /tycoon-village and /model-lab. If WebGlRunLib/run-playback is implemented, add proof for that too.

Keep all source-code comments in English.
```
