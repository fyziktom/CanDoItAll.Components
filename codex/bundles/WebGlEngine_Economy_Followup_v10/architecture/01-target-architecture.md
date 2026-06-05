# Target architecture: generic run engine + domain drivers

## Generic Components ownership

Components packages own:

- `WebGlLib`: scene model, assets, patches, runtime interop, runtime idle, command batches, rendering diagnostics.
- `WebGlRunLib`: domain-neutral run timeline, action lifecycle, stage barriers, playback, observer proof contracts, generic extension points.

Components packages must not own:

- Economy vocabulary such as buyer/seller/market/price/investor/elite.
- Production-line vocabulary such as machine/work-order/line station.
- Scenario-specific proof assumptions.

## Domain driver ownership

A domain driver owns:

- Domain action vocabulary.
- Domain visual mapping to generic run actions.
- Domain boundary terms for scanning.
- Domain validation and oracle fixtures.
- Domain-specific docs and samples.

Examples:

- `CanDoItAll.Economy.Simulation.WebGlBridge` = Economy visual/run domain driver.
- Future `CanDoItAll.ProductionLines.WebGlBridge` = manufacturing line driver.

## Generic action vocabulary policy

Generic action kinds should describe visual mechanics, not economic meaning:

- Good: `move-to-object`, `set-pose`, `show-symbol`, `pulse-link`, `directed-flow-visual`, `apply-scene-patch`, `wait`.
- Bad in generic layer: `resource-transfer-visual`, `market-trade`, `buyer-pay`, `machine-cycle`, `work-order-complete`.

## Research-grade economic experiment truth

Economic truth is computed headlessly:

scenario pack -> loader -> backend -> frames/deltas -> metrics/invariants -> oracle -> manifest -> readiness.

Browser proof is observer evidence only:

WebGlRunDocument -> browser playback -> exported scene/diagnostics/hash -> observer band.
