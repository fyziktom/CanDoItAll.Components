# Proof manifest - SB09

Status: completed

Required artifacts:
- `proof/SB09/transcripts/golden-oracle-tests.txt` - passed focused golden oracle suite covering final stores, flows, issues, relationships, metrics, invariants, and deterministic frame hash chains.
- `proof/SB09/artifacts/oracle-diff-sample.json` - generated deliberately broken transfer diff with actual final stores and stable replay hash chain.

Additional proof:
- `proof/SB09/transcripts/golden-oracle-tests-failing-first.txt` - failing-first proof that rule enforcement did not produce the expected issue before implementation.
- `proof/SB09/transcripts/golden-oracle-hardening-tests.txt` - passed full `SimulationEconomicTrustHardeningTests` regression class.
- `proof/SB09/transcripts/source-assertion-golden-oracle-scan.txt` - source assertion scan for rule enforcement, golden oracle cases/hash chains, and oracle coverage labels.
- `proof/SB09/transcripts/anti-stub-audit.txt` - anti-stub audit for SB09 source changes.
- `proof/SB09/transcripts/changed-file-hashes.txt` - SHA256 hashes for SB09 source, proof, doc, and bundle record files.

Implemented:
- Golden oracle scenarios for single transfer, insufficient stock, capacity rejection, shared resource depletion, trade plus fee, relationship update, and rule violation/enforcement.
- Oracle evaluation that compares final stores, flow count, issue count, relationship strength, registered metrics, registered invariants, expected diagnostics, and deterministic frame hash chains across repeated materialization.
- Production rule-enforcement handling for `rule.enforcement.apply`, producing a frame issue that participates in deterministic hashes.
- Readiness `oracleCoverageLabel` metadata and status gating so clean headless runs without valid oracle proof remain exploratory unless explicitly labeled `no-oracle`.
- Economy documentation for golden oracles and oracle coverage classification.

## Production Behavior Artifact Matrix

| Production signal | Producer | Consumer | Lifecycle | Negative-test/proof citation |
|---|---|---|---|---|
| `rule.enforcement.apply` enforcement issue | `SimpleSimulationEventHandlerRegistry.CreateDefault` routes the event to `SimpleSimulationStateTransitionEngine.ApplyRuleEnforcement`. | `SimulationFrame.Issues`, deterministic frame hashing, and the SB09 golden oracle suite. | A scheduled enforcement event is materialized, the handler appends `issue.<event>.enforcement`, and the final frame/hash chain includes that issue. | `proof/SB09/transcripts/golden-oracle-tests-failing-first.txt` failed on `issues.count`; `proof/SB09/transcripts/golden-oracle-tests.txt` passed after implementation. |
| `oracleCoverageLabel` readiness metadata | `EconomyExperimentReadinessReporter.Build` writes `golden-oracle`, `no-oracle`, or empty coverage metadata. | `ResolveStatus` and readiness report consumers. | Valid exercised oracle proof labels the report `golden-oracle`; explicit `no-oracle` allows headless/browser-observer status without oracle proof; unlabeled no-oracle reports remain exploratory. | `proof/SB09/transcripts/golden-oracle-hardening-tests.txt` covers unlabeled exploratory, explicit `no-oracle`, and `golden-oracle` research-ready cases. |
