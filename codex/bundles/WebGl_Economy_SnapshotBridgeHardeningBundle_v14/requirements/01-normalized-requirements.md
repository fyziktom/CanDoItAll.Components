# Normalized Requirements

| ID | Requirement | Owner |
|---|---|---|
| RQ-001 | Work only in the currently checked-out branches in Components and Economy. | SB01 |
| RQ-002 | Keep Components and WebGlRunLib generic, with no Economy references or example-domain vocabulary in runtime code. | SB01, SB02, SB10 |
| RQ-003 | Keep WebGL desktop / large-screen only; do not add mobile, tablet, small-screen, medium-screen, or touch-first scope. | SB02, SB15 |
| RQ-004 | Keep JS runtime modules modular, acyclic, under hard line thresholds, and free of TypeScript migration scope. | SB02 |
| RQ-005 | Prove command stage waits, stage runner diagnostics, per-object motion queues, cancellation, object removal, and reset/dispose behavior. | SB03 |
| RQ-006 | Ensure action plans compile to predictable generic WebGL command batches and stage barriers without silent action drops. | SB04 |
| RQ-007 | Harden Economy WebGlBridge projection so non-trivial visual frames produce initial scene data, executable stages, mapping context, and diagnostics. | SB05 |
| RQ-008 | Keep Economy bridge dependencies clean and avoid developer-specific checkout paths as the only reference strategy. | SB06 |
| RQ-009 | Add renderer-neutral simulation snapshot contracts with deterministic hash and serializer. | SB07 |
| RQ-010 | Add snapshot store, JSON export/import, diff, and roundtrip tests that preserve deterministic hash. | SB08 |
| RQ-011 | Attach optional visual/WebGL state metadata to simulation snapshots without making snapshots WebGL-dependent. | SB09 |
| RQ-012 | Remove example-domain leakage from generic Economy abstractions, bridge, policies, and runtime paths except allowlisted fixtures/tests/docs. | SB10 |
| RQ-013 | Harden visual mapping schema and strict loader so multiple probes can map without hardcoded asset, pose, or symbol choices. | SB11 |
| RQ-014 | Add only an Economy-side SimulationSandbox design or skeleton, not a finished demo. | SB12 |
| RQ-015 | Add performance and scalability proof for bridge/runtime foundation sizes before richer demos. | SB13 |
| RQ-016 | Prove a snapshot-driven analysis flow can answer why a visual state looks bad without inspecting runtime internals. | SB14 |
| RQ-017 | Close the bundle with command transcripts, changed-file hashes, source assertions, proof manifests, raw-note closure, and final validators. | SB15 |
