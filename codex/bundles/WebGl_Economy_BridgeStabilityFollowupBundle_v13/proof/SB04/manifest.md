# SB04 Proof - Economy bridge initial scene projector

## Scope

Added Economy-side initial scene projection that converts the first visual frame into generic WebGL scene objects and links, while populating the bridge node-object map.

## Changed-file hashes

- `9c31669de3b3d6572309e6c7e0820b8d473e1e08a137b6a61432e1799c2cee18  C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\EconomyWebGlInitialSceneProjector.cs`
- `f768061e7c2e45722c7a17266bd7a6de5a6964f592f30507561a7ab5599d76ab  C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\EconomyWebGlBridgeContracts.cs`
- `12ae68fd038f30dc43220e399d68268da4e0ad3e1d4f333495c1679979e5291c  C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\EconomyWebGlBridgeTests.cs`

## Validation transcript

- `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter EconomyWebGlBridge --no-restore`: pass, 6 tests.
- `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --no-restore`: pass, 483 tests.

## Semantic invariants

- Visual nodes become stable generic `WebGlSceneObject` entries.
- Visual links become stable generic `WebGlSceneLink` entries.
- Bridge context owns node-object mappings and does not require Components to know simulation concepts.
