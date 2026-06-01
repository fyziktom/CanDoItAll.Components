# CanDoItAll WebGL + Economy Real Scenario Hardening Bundle v19

This follow-up bundle reviews the current pushed state after the previous executable bridge readiness work and prepares the next hardening phase.

Primary goal:

- keep `CanDoItAll.Components` generic and Economy-free,
- keep joined simulation + visualization in `CanDoItAll.Economy`,
- harden the current headless real-scenario path,
- prepare a first safe large-screen integration test without turning Components into an Economy runtime,
- keep scenario examples generic: shared finite resource, finite spatial/resource ownership, small producer/community trade.

This bundle assumes both repositories are already cloned locally and that Codex must work in the currently checked-out branches.

Hard rules:

1. Do not create a new branch.
2. Do not move Economy-specific code into Components.
3. Do not optimize WebGL for small/medium/mobile/tablet.
4. Do not implement a final UI demo yet unless explicitly requested later.
5. All source-code comments must be in English.
6. Every proof transcript must contain real output; empty transcript files are not acceptable.

Main spreadsheet:

`05_spreadsheets/implementation_matrix.xlsx`

## Validation Summary

Bundle readiness gate: Passed after structural repair.

Execution status: SB01 through SB15 completed.

Final closure gate: Passed.

Browser validation analytics: Not started. Browser proof is limited to the planned large-screen smoke path; this bundle must not optimize WebGL for small, medium, mobile, or tablet screens.

## Execution Notes

- Bundle repair added the standard plan, traceability, execution report, validator, and proof folders required by the CanDoItAll bundle workflow.
- SB01 captured cross-repo branch/commit baseline and mapped source-boundary findings to downstream subbundles.
- SB02 hardened Components stage barriers, manual-step isolation, cancellation reset behavior, timeout diagnostics, missing object-motion ids, render-idle behavior, and bounded journal proof.
- SB03 exposed generic runtime snapshot export through the playback-controller interface and added backward-step document-runner support.
- SB04 passed the Components scene runtime audit, confirmed no TypeScript migration, and preserved the generic runtime boundary.
- SB05 hardened Economy strict bridge validation with structured diagnostics for missing commands/source fields, unresolved motion/patch targets, and disabled fallback usage.
- SB06 added a reusable Economy bridge diagnostics aggregator, kept bridge projectors separated, and refactored action-stage projection responsibilities.
- SB07 marked remaining runtime-specific visual mapping fields as bridge-bound follow-ups and proved low-level Economy reference boundaries remain renderer-neutral.
- SB08 added a generic Economy real-scenario runner, required `real-scenario-runs/shared-well` and `farmer-land` artifact exports, and strict invalid run-document rejection before export.
- SB09 attached visual runtime state to snapshots, added separate `snapshot.dataState` and `snapshot.visualRuntime` hashes, and proved real scenario artifacts serialize the new runtime fields.
- SB10 moved snapshot analysis pressure categories into reusable production analyzers, added resource scarcity and unresolved visual mapping facets, and passed the production domain-term scan.
- SB11 hardened Economy backend selection with deterministic hint ordering, structured missing-backend diagnostics, ledger descriptor-only readiness, fake backend coverage, and backend-neutral sandbox contracts.
- SB12 added an artifact-backed Economy real scenario readiness reporter, generated `real-scenario-readiness-report.json`, proved both required probes run headlessly with zero strict failures, identified SB13 browser playback actions, and preserved the no-final-UI boundary.
- SB13 prepared the large-screen-only browser smoke plan at `1440x900` or larger, selected the generated `shared-well` run document and expected stage ids, and intentionally deferred browser proof because the existing route does not load generated artifacts without adding a new harness.
- SB14 extended performance probes with required Economy headless scale counts, deterministic hash timing, WebGL data/runtime scale counts, and bounded stage queue/journal proof.
- SB15 closed the bundle with Components and Economy build/test/audit transcripts, proof integrity checks, renderer-neutral cleanup from final validation, and fake-proof resistance review.
- Subbundles must run in order unless the dependency map explicitly allows a later inspection-only task.
- Components must remain generic and Economy-free throughout all phases.
