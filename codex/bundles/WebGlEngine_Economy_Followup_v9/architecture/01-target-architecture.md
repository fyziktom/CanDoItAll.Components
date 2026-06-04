# Target architecture for research-grade economic simulation

## Source-of-truth principle

Economic truth must come from headless deterministic artifacts:

```text
scenario pack + policy + behavior profile + backend materialization
  -> event stream
  -> frames/deltas
  -> metrics/invariants
  -> oracle corpus comparison
  -> readiness report
  -> reproducibility manifest
```

Browser/WebGL is observer evidence only:

```text
headless WebGlRunDocument
  -> browser route loads expected document
  -> runtime applies frames
  -> observer proof validates stage completion, final positions, idle diagnostics and console health
```

Browser proof can support a visual demo claim, but it must never rewrite headless artifacts or determine economic correctness.

## Generic boundary

`CanDoItAll.Components` must remain generic. It may expose scene/run primitives such as object, link, symbol, stage, command, motion, barrier, runtime idle and proof snapshot. It must not contain economic concepts such as market, buyer, seller, investor, monopoly or elite as production behavior.

Boundary audits should be configurable. Economy-specific forbidden terms belong in tests/audit fixtures or Economy integration tests, not hardcoded production behavior inside Components.

## Third scenario target

The third scenario must not be a slightly renamed shared-well/farmer-land scenario. It should stress a different economic shape:

- multiple categories of goods/resources,
- exchange or market-like clearing/swap events,
- investment or capital contribution from wealthy entities to smaller entities,
- return/claim/dividend/repayment-like flows represented generically,
- concentration metrics such as top wealth share, HHI and Gini-like proxy,
- policy shocks such as fee/tax/subsidy/capital access constraint,
- visualization of portfolio, exchange flows and capital dependency.

The implementation should first try to express this using generic event/action primitives. Add new core types only when generic representation becomes ambiguous or unsafe.
