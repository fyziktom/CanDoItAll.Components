# SB12 Proof - Economy WebGL bridge traceability

## Scope

Bridge run projection now records generic source provenance while avoiding domain-specific metadata names in WebGL command payloads.

## Changed-file hashes

- `003401a1a0f2ab054c54cc9ffc3d326d071b5a463581c0cccdb7ba5ab3f1fb6d  C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\EconomyWebGlRunProjector.cs`
- `04b29ff21086e2a11fdbaa724712982a4e576d63e153b640b9749743b52e66c8  C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\EconomyWebGlRunValidator.cs`
- `f768061e7c2e45722c7a17266bd7a6de5a6964f592f30507561a7ab5599d76ab  C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\EconomyWebGlBridgeContracts.cs`

## Validation transcript

- Economy WebGL bridge tests: pass, 6 tests.
- Full Economy tests: pass, 483 tests.

## Semantic invariants

- Command metadata uses generic source keys.
- Run metadata includes mapping and diagnostic context without requiring Components-side domain knowledge.
- Validator fails empty or untraceable command stages.
