# Semantic invariants - SB09

- Components remains domain-neutral; golden oracle semantics and rule-enforcement behavior live in Economy.
- Golden oracles must be independent of WebGL projection, browser replay, and UI timing.
- Each golden oracle must assert known final stores, flows, issues, relationships when applicable, metrics, invariants, expected diagnostics when applicable, and frame hash chains.
- Frame hash chains must be stable across repeated materialization of the same research-grade scenario.
- A deliberately broken expected value must produce a path-addressed oracle diff with expected and actual values.
- Rule enforcement events must produce production frame issues rather than test-seeded evidence.
- Scenario classification must not rise above exploratory without valid exercised oracle proof or an explicit `no-oracle` coverage label.

## Production Behavior Artifact Matrix

| Production signal | Producer | Consumer | Lifecycle | Negative-test/proof citation |
|---|---|---|---|---|
| `rule.enforcement.apply` enforcement issue | `SimpleSimulationEventHandlerRegistry.CreateDefault` and `SimpleSimulationStateTransitionEngine.ApplyRuleEnforcement`. | Final simulation frames, deterministic frame hashing, and golden oracle issue assertions. | Scheduled enforcement events become production issues with `sourceEventId` and `enforcementTargetId` metadata before final frame hashing. | `proof/SB09/transcripts/golden-oracle-tests-failing-first.txt` and `proof/SB09/transcripts/golden-oracle-tests.txt`. |
| `oracleCoverageLabel` readiness metadata | `EconomyExperimentReadinessReporter.Build`. | `ResolveStatus`, readiness report consumers, and classification tests. | Reports with valid oracle proof carry `golden-oracle`; explicit `no-oracle` marks intentional no-oracle coverage; unlabeled no-oracle reports stay exploratory. | `proof/SB09/transcripts/golden-oracle-hardening-tests.txt`. |
