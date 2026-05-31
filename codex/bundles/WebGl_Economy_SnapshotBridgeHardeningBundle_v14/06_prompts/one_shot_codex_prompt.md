# One-shot Codex prompt

You are working in two already-cloned repositories:

1. `CanDoItAll.Components`
2. `CanDoItAll.Economy`

Do not create a new branch. Work only in the currently checked-out branches.

Implement the follow-up hardening bundle `CanDoItAll_WebGl_Economy_SnapshotBridgeHardeningBundle_v14`.

Critical requirements:

- Keep Components generic. Do not add any Economy references or economy vocabulary to Components.
- Keep WebGL desktop/large-screen only. Do not add small/medium/mobile/tablet optimization work.
- Keep JavaScript runtime modular and maintainable. Do not introduce TypeScript.
- The combined simulation + visualization sandbox belongs in the Economy repo, not Components.
- Add first-class Economy simulation snapshot contracts, store, serializer, hash, and snapshot analysis probe.
- Harden `Simulation.WebGlBridge` so it produces traceable executable WebGL run documents and can attach visual state metadata to snapshots.
- Validate shared-resource and finite-resource probes without hardcoding example-specific concepts into generic models.
- Run the validation commands in `04_validation/validation_commands.md`.
- Record proof under the relevant bundle/proof folders.
