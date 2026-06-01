# SB05 Semantic Invariants

| Invariant ID | Assertion |
|---|---|
| SB05-strict-mapping | Unresolved subjects, targets, poses, and symbols fail by default unless explicit diagnostic fallback is enabled. |
| SB05-stage-traceability | Every emitted stage carries visual action id, source event id field, simulation frame id, and input pack hash. |
| SB05-wait-marker | Commandless wait stages are explicit wait markers, not silently dropped stages. |

## Shallow-pass trap

A shallow bridge could pass by dropping bad actions or metadata-only waits. The validator now rejects commandless stages unless they are explicit wait markers with traceability.

## Adversarial negative proof

`economy-webgl-bridge-tests.txt` includes strict mapping tests for unresolved subjects/targets and missing visual-state mappings.

## Semantic positive proof

`economy-webgl-bridge-tests.txt` passes, including explicit diagnostic fallback and wait-only stage preservation.

## Anti-stub audit

The projection uses real `EconomyVisualAction` to `WebGlRunAction` planning and command batch compilation; no final demo UI or hard-coded example path is used.

