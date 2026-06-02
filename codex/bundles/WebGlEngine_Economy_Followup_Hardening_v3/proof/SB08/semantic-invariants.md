# Semantic Invariants - SB08

## Status

Completed.

## Invariants

- WebGlLib remains independent of WebGlRunLib and Economy.
- WebGlRunLib remains generic and accepts only bounded traceability provenance under `source.*`.

## Adversarial Negative Proof

Validator tests reject policy-like `source.*` metadata and domain leakage where source provenance is not allowed.

## Semantic Positive Proof

WebGlLib and WebGlRunLib boundary audits pass.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| `source.*` metadata | Domain bridge | WebGlRun validation | Document validation | Policy/source smuggling rejected |
