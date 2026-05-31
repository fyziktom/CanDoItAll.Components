# SB05 Proof - Economy bridge action stage projection

## Scope

Replaced metadata-only bridge projection with action-stage projection that partitions visual actions by timeline step and emits real generic WebGL command batches.

## Changed-file hashes

- `5cdec64859a1bed8639d47c3f5041cd8411780842e6632eed8a57d2bb5c8724a  C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\EconomyWebGlActionStageProjector.cs`
- `003401a1a0f2ab054c54cc9ffc3d326d071b5a463581c0cccdb7ba5ab3f1fb6d  C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\EconomyWebGlRunProjector.cs`
- `04b29ff21086e2a11fdbaa724712982a4e576d63e153b640b9749743b52e66c8  C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\EconomyWebGlRunValidator.cs`
- `12ae68fd038f30dc43220e399d68268da4e0ad3e1d4f333495c1679979e5291c  C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\EconomyWebGlBridgeTests.cs`

## Validation transcript

- `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter EconomyWebGlBridge --no-restore`: pass, 6 tests.
- `dotnet test .\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --no-restore`: pass, 483 tests.

## Semantic invariants

- Global input actions are partitioned by `Timeline.StepIndex` and are not replayed into every frame.
- Stages contain command batches with actual patch/motion work, not metadata-only placeholders.
- Source metadata remains generic: `source.visualActionId` and `source.eventId`.
