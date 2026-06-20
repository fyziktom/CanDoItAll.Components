# Economic simulation readiness

## Safe today

- Exploratory runs.
- Scenario authoring and debugging.
- Headless pipeline development.
- Visual/WebGL smoke testing.
- Comparing run manifests when readiness status is not overclaimed.

## Not safe yet without new gates

- Research-grade conclusions from UI/browser proof.
- Treating browser animation as economic evidence.
- Trusting readiness bands that are backed only by record-shaped evidence, not real artifacts.
- Treating multi-goods-elite as proof of generality before oracle/metamorphic closure.

## Required pipeline for claim-oriented runs

```text
scenario pack
  -> ResearchStrict load
  -> backend frames/deltas
  -> metrics/invariants
  -> artifact-backed evidence resolver
  -> external oracle corpus
  -> reproducibility manifest
  -> readiness report
  -> optional browser observer proof
```

A run can be called research-ready only when the above pipeline passes and the final report has no unclassified diagnostics and no non-allowlisted warnings.
