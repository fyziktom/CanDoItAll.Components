# SB08 Semantic Invariants

| Invariant ID | Shallow-pass trap | Adversarial negative proof | Semantic positive proof | Anti-stub audit |
| --- | --- | --- | --- | --- |
| SB08-INV-01 | A snapshot with one monolithic hash could appear deterministic while hiding runtime noise. | `SimulationSnapshotBuilderTests` changes runtime diagnostics and expects the data hash to remain stable while full hash changes. | Builder emits `snapshot.data`, `snapshot.visualState`, and `snapshot.full`. | Hash assertions inspect production snapshot provenance, not test-local strings. |
| SB08-INV-02 | A diff that only compares frame hash would miss relationship and visual-state changes. | Store tests mutate a relationship and attach visual state, then assert specific diff paths. | `SimulationSnapshotDiff.Compare` reports relationship and visual-state differences. | The same snapshot is exported/imported and then compared after mutation. |
| SB08-INV-03 | An analyzer hardcoded to example terms could pass one fixture only. | Analysis probe asserts the summary does not contain example-specific resource/site terms. | Generic facets report admin burden, issues, concentration, relationships, pending events, visual stages, and invariants. | `SimulationSnapshotAnalysisService` composes production facet classes. |
