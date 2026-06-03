# Semantic Invariants for SB06

## Invariant SB06-oracles-bind-economic-outcomes

Source: SimpleAccounts transition engine and deterministic hash helpers.

Expected behavior: known-answer cases produce expected final store quantities, flow counts, issue counts, metric values, and stable frame hashes; negative cases fail for expected strict reasons.

Passing result: `GoldenOracleSuite_ProvesKnownFinalStoresFlowsIssuesMetricsAndHashes` and `GoldenOracleSuite_NegativeScenariosFailForExpectedReasons` passed.

Why this prevents simulator-noise contamination: model primitives now have executable expected outcomes rather than relying on visual inspection.

