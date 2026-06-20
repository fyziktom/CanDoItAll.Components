# Semantic Invariants for SB05

## Invariant SB05-store-resolution-is-policy-driven

Source: scheduled events, normalized events, and SimpleAccounts mutation context.

Expected behavior: strict mode rejects ambiguous store matches unless the event carries an explicit resolution policy such as exact store id or shared pool.

Passing result: `StoreResolution_StrictModeFailsAmbiguityAndExactPolicyPasses` and the shared-pool golden oracle passed.

Why this prevents simulator-noise contamination: resource flows cannot be attributed to an arbitrary matching store when the scenario model is under-specified.

