# SB02 - Components runtime stage barrier hardening

Codex must harden:

- barrier timeout diagnostics,
- cancellation behavior,
- wait-for-object-motions behavior when object id is missing,
- wait-for-render-idle behavior when symbols animate forever,
- wait-for-event behavior so manual-step cannot leak across unrelated batches.

Add tests or audit fixtures for:

- stage A motion -> barrier waits for object motion -> stage B pose patch,
- stage B does not execute until A motion is complete,
- cancel clears queue, active barrier and journal state,
- command journal remains bounded.

## Status

Completed.

## Goal

Harden generic WebGL stage barrier behavior so executable playback does not run early, hang forever, or leak events between batches.

## Prerequisites

- SB01 branch and boundary guard must pass.
- Components runtime files and WebGlLib tests must be inspected before editing.

## Owned Requirements

- R02 Components Runtime Hardening.

## Dependency Impact

Unlocks SB03 run document controller proof and later Economy browser/run-document readiness.

## Validation Depth

Behavior-changing critical foundation: include failing-first or adversarial coverage, passing tests/audit, source assertions, bounded journal proof, and anti-stub audit.

## Proof Required

- WebGlLib test or audit transcript covering the listed barrier cases.
- Source assertions for barrier timeout, cancellation, object-motion missing id, render-idle forever animation, wait-for-event isolation, and journal bound.
- Proof manifest and semantic invariant contract.

## Progression Gate

Pass only when stage B cannot execute before stage A motion completes, cancellation clears active state, unrelated manual-step events cannot release other batches, and the journal remains bounded.
