# Semantic Invariants for SB04

## Invariant SB04-strict-mode-is-explicit-and-preserved

Source: scenario definitions, normalizer, loader, and SimpleAccounts transition options.

Expected behavior: strict and research-grade runs require an explicit expansion profile, preserve mode through normalization, and promote model-adjacent failures to errors.

Passing result: `StrictScenarioWithoutExpansionProfileFailsValidation` and unknown-handler strict/exploratory tests passed in the focused suite.

Why this prevents simulator-noise contamination: implicit defaults can no longer silently turn a strict economic experiment into an exploratory one.

