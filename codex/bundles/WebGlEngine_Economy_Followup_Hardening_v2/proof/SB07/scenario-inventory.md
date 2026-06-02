# SB07 Economy scenario dynamic-object inventory

Status: Completed 2026-06-02.

## Policy

Economy WebGL bridge validation supports dynamic object references across command order. An object introduced by a scene patch is considered known for later patches, stages, and frames. A motion in the same stage as the object creation remains invalid because stage commands can be batched together and should not depend on intra-stage object creation order.

## Current scenario inventory

The shipped Economy runtime scenarios and test fixtures are effectively static with respect to WebGlRun scene objects. The scan in `bundle://proof/SB07/transcripts/economy-scenario-dynamic-object-scan.txt` found no `addObjects` or `removeObjectIds` WebGlRun patch payloads under:

- `../CanDoItAll.Economy/src/CanDoItAll.Economy.Node/SimulationScenarios`
- `../CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/Fixtures`

Current examples project visual nodes from the initial visual frame into the initial scene and generate staged patches/motions against those object ids.

## Extension path

Future scenarios may introduce dynamic visual objects by emitting a scene patch in an earlier stage or frame, then targeting the new object from a later stage or frame. Validators will reject ambiguous same-stage motion-to-new-object references and unresolved references after removals.
