# Current Review Summary

## Components repo

Observed state:
- `CanDoItAll.Components.WebGlRunLib` is present in the solution.
- WebGL scene runtime has a module audit with thresholds:
  - warning threshold: 220 JS lines
  - failure threshold: 320 JS lines
  - facade threshold: 180 JS lines
- Runtime is organized as ES modules rather than TypeScript/classes. This is acceptable; do not introduce TypeScript.
- Stage-aware command batching exists in C# and JavaScript.
- A command stage runner exists and is advanced from the render loop.
- A per-object motion queue exists.

Main concerns:
- Some JS modules are still near the warning threshold, especially motion and batch normalization.
- Stage runner and motion queue are not yet proven with a real multi-step simulation bridge path.
- Stage runner has basic timing, but needs richer diagnostics and cancellation behavior before a full sandbox.
- `resolveRenderReason` should explicitly consider `commandStageRunner.queue` and `commandStageRunner.waitSeconds`, not only indirect `renderRequested` scheduling.
- Motion queue behavior needs tests for replace, append, cancel queued item, cancel active item, dispose, and object removal.

## Economy repo

Observed state:
- `CanDoItAll.Economy.Simulation.WebGlBridge` exists in the solution.
- Bridge correctly lives in Economy repo.
- Bridge references `Simulation.Abstractions`, `Simulation.Visualization`, and Components `WebGlRunLib`.
- Initial scene projection now maps nodes/links/layers/symbols.
- Action stage projection maps normalized visual actions through WebGL run action planning and command batch compilation.
- Input pack strict validation exists and rejects placeholder hashes in strict mode.
- Economy boundary audit checks for forbidden references and example-domain leaks.

Main concerns:
- Bridge still uses a direct sibling checkout project reference to Components. This is acceptable locally, but needs conditional dev/package strategy before CI/release.
- Snapshot support for paused simulation analysis is not yet visible as a first-class concept.
- Future `SimulationSandbox` is not yet present.
- Economy `WebGlBridge` currently creates run documents, but needs a clear `SimulationRunSnapshot -> VisualFrame -> WebGlRunDocument/Frame` integration contract.
- Traceability is improving, but we need a full provenance chain from input pack hashes to simulation frame hashes to visual frame IDs to WebGL run frame/stage IDs.

## Key recommendation

Next wave should not build the final demo yet.
It should stabilize the generic bridge pipeline and add snapshot capability.
