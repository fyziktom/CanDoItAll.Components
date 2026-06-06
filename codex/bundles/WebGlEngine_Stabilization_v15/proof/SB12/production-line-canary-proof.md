# SB12 Production-Line Canary Proof

Canary location:

- `tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunValidatorTests.cs`

Test:

- `Production_line_canary_maps_driver_vocabulary_to_generic_scene_and_run_contracts`

Coverage:

- station/buffer/conveyor/token objects are fixture-only driver vocabulary;
- action mappings resolve to generic `ShowSymbol`, `DirectedFlowVisual`, and `MoveToObject`;
- generated `WebGlRunDocument` validates through generic validators;
- no production source gains production-line semantics.

Validation:

- WebGlRunLib tests passed in the RC transcript.
