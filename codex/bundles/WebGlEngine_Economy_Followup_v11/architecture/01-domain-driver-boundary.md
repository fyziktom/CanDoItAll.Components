# Target architecture: generic core + domain drivers

## Layering

```text
CanDoItAll.Components.WebGlLib
  - Scene model, patches, runtime JS interop, runtime idle, proof snapshots
  - No economy/market/ledger/production-line terms

CanDoItAll.Components.WebGlRunLib
  - Generic action/run/playback contracts
  - Generic domain driver interface only
  - Generic action names: move, pose, symbol, directed-flow, pulse-link, wait, patch

Domain driver packages
  - Economy driver: Economy visual actions -> generic WebGlRun actions
  - Future production-line driver: Manufacturing events -> generic WebGlRun actions
  - Other drivers: logistics, process visualization, memory economy, etc.

CanDoItAll.Economy.*
  - Economic semantics, event handlers, metrics, invariants, readiness, oracles
  - Bridge owns forbidden terms and driver mapping; generic library never imports Economy
```

## Domain driver contract expectations

A domain driver must provide:

- Driver id/version/hash.
- Domain vocabulary allowlist and leak-prevention terms.
- Action kind mapping table to generic action kinds.
- Metadata scrubber: domain metadata in source provenance only, not generic policy metadata.
- Validation result with path-addressed diagnostics.
- Test corpus proving driver output passes generic `WebGlRunDocumentValidator`.

## Research-grade economic run contract

A run may claim research readiness only if:

- Scenario pack manifest and input hashes validate.
- Policy is `ResearchStrict` or explicit equivalent.
- Backend materialization has zero model-adjacent warnings.
- Metric/invariant registry recognizes all definitions.
- External oracle corpus passes or explicit no-oracle status is recorded as not research-ready.
- Reproducibility manifest contains frame hash chain and artifact hashes.
- Browser observer proof is optional for economic truth, but required for visual demo claims.
