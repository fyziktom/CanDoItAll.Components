# Normalized Requirements

## Hard Constraints

| ID | Requirement | Owning subbundles |
|---|---|---|
| RQ-001 | Work in the currently checked-out branches of `CanDoItAll.Components` and `CanDoItAll.Economy`; do not create or switch branches. | SB01 |
| RQ-002 | Keep `CanDoItAll.Components.*` free of Economy references. | SB01, SB14, SB19 |
| RQ-003 | Keep low-level Economy abstractions free of WebGL and Components references. | SB09, SB13, SB14, SB19 |
| RQ-004 | Keep WebGL runtime proof desktop / large-screen only; do not add mobile, tablet, small-screen, or medium-screen optimization work. | SB15, SB19 |
| RQ-005 | Preserve generic engine behavior; example-specific shared-well and farmer-land data must live in fixtures, scenario factories, docs, or tests, not core generic runtime code. | SB09, SB10, SB11, SB12, SB17 |

## Components Requirements

| ID | Requirement | Owning subbundles |
|---|---|---|
| RQ-C-001 | Staged command batches execute through an asynchronous stage runner and respect `waitSeconds` as an execution barrier. | SB02 |
| RQ-C-002 | Stage progress exposes diagnostics for current batch/stage, completed stages, failed stages, and queued stages. | SB02 |
| RQ-C-003 | Stage execution can be cancelled on scene import or runtime disposal. | SB02, SB06 |
| RQ-C-004 | `queueMode=append` creates a per-object sequential queue instead of competing active motions for the same object. | SB03 |
| RQ-C-005 | Motion queue diagnostics expose active, queued, max queue length, and cancelled motion counts. | SB03 |
| RQ-C-006 | `WebGlRunActionPlan` can be converted to deterministic staged `WebGlSceneCommandBatch` output without Economy references. | SB04 |
| RQ-C-007 | C# and JS batch normalizer behavior is fixture-driven and comparable for stage, wait, coalescing, and duplicate-motion cases. | SB05 |
| RQ-C-008 | `WebGlRunPlaybackController` reports explicit playback result state, deterministic backwards seek/replay behavior, invalid timeline diagnostics, and run-source provenance. | SB06 |

## Economy Requirements

| ID | Requirement | Owning subbundles |
|---|---|---|
| RQ-E-001 | Experiment input pack strict mode rejects placeholder hashes, verifies `sha256:<64 lowercase hex>` document hashes, verifies pack hash, checks missing/extra hash entries, and prevents path escape. | SB07 |
| RQ-E-002 | A high-level experiment input-pack loader returns a traceable run input with pack, scenario, placement, parameters, run plan, visual mapping, invariants, event stream, hashes, and diagnostics. | SB08 |
| RQ-E-003 | Generic model leakage is audited and refactored so example-specific terms stay out of generic source. | SB09 |
| RQ-E-004 | State transition behavior uses a deterministic event handler registry rather than hardcoded handler lambdas in the engine core. | SB10 |
| RQ-E-005 | Transition diagnostics cover normalization warnings, missing actors/stores, capacity rejection, insufficient stock, unknown handlers, and negative stock behavior. | SB11 |
| RQ-E-006 | Metric and invariant evaluation covers resource totals, concentration, rule violations, access cost, depletion, relationship aggregates, burden, and transfer volume. | SB12 |
| RQ-E-007 | Economy visual mapping is serializable and WebGL-neutral. | SB13 |
| RQ-E-008 | A bridge adapter design or compile-only skeleton isolates the only project that may reference both Economy visualization and Components WebGlRunLib. | SB14 |

## Proof And Closure Requirements

| ID | Requirement | Owning subbundles |
|---|---|---|
| RQ-P-001 | Each executed subbundle records entry/closure gate status in `reviews/01-execution-report.md`. | SB01-SB19 |
| RQ-P-002 | Critical behavior-changing subbundles record artifact-backed proof under `proof/SBxx/`. | SB01-SB19 |
| RQ-P-003 | Final validation runs the Components tests/audits, Economy tests/audits, cross-repo reference scans, anti-stub audit, and bundle validator. | SB19 |

