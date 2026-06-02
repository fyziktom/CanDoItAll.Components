# Semantic Invariants - SB03

## Status

Completed.

## Invariants

- Runtime scenarios are manifest-backed packs with stable ids, versions, content hashes, and pack hashes.
- Invalid or missing manifests remain visible as invalid catalog descriptors.

## Adversarial Negative Proof

Focused tests cover missing-manifest invalid descriptors and path traversal rejection.

## Semantic Positive Proof

Both `shared-well` and `farmer-land` load through the runtime catalog and appear in browser proof.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Scenario manifest | Node scenario pack | Catalog, session service, UI | Startup/runtime | Missing manifest marked invalid |
