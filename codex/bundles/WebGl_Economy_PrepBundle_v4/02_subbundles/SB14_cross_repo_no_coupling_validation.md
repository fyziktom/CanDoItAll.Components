# SB14 — Cross-repo no-coupling validation

## Goal

Prove both repos are prepared but not coupled.

## Components validation

- `dotnet build CanDoItAll.Components.slnx`
- `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj`
- `npm run webgllib:build-assets`
- `npm run webgllib:verify-assets`
- `npm run webgllib:audit-scene-runtime`
- optional browser proof for WebGlSandbox.

## Economy validation

- `dotnet build CanDoItAll.Economy.slnx`
- `dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj`
- dependency boundary audit script.

## Coupling checks

Fail if:
- Economy references Components.WebGlLib or WebGlRunLib;
- Components references Economy;
- WebGlLib contains economy/domain keywords;
- simple-account backend references ledger/business objects/sdk.
