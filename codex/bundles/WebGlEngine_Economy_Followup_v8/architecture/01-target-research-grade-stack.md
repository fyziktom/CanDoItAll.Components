# Target research-grade experiment architecture

```text
Scenario Pack
  - scenario.manifest.json
  - experiment.json
  - scenario.definition.json
  - parameters.json
  - placement.json
  - run.plan.json
  - visual.mapping.json
  - expected.invariants.json
  - oracle/*.json
  - all files hashed and closed by manifest
        ↓
Scenario Loader + Policy
  - ResearchStrict by default for claims
  - explicit behavior expansion profile
  - no unknown event/metric/invariant fallback
        ↓
Headless Economic Runner
  - event stream
  - frames/deltas
  - metrics/invariants
  - oracle result
  - reproducibility manifest
  - run hash + frame hash chain
        ↓
Readiness Report
  - scenario band
  - simulation band
  - metrics band
  - oracle band
  - performance band
  - projection/runtime/UI/browser bands as observer evidence
        ↓
Browser Observer
  - loads generated WebGlRunDocument
  - proves render/playback/idle
  - compares document hash and runtime diagnostics
  - never mutates economic headless truth
```

## Boundary rules

- Economy owns economic semantics, scenarios, policies, metrics, invariants, oracles, and experiment design.
- Components owns generic visualization/runtime semantics, command lifecycle, idle status, and browser proof primitives.
- Browser/WebGL is an observer, not a simulator source of truth.
- A research claim requires artifact-backed proof. Booleans alone are never enough.
- Any unclassified diagnostic is a defect in the readiness system.
- Any design factor that does not change scenario input/configuration is invalid for comparison.
