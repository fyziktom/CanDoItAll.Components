# SB03 - Components executable run document controller

Codex must verify or implement a reusable generic controller that can execute:

- seek frame,
- apply frame stages,
- export runtime snapshot,
- pause/resume,
- step forward/back in document timeline,
- report current stage/action ids.

It must not know Economy.

It must output enough data for Economy to attach runtime state into `SimulationRunSnapshot.VisualState`.

## Status

Completed.

## Goal

Verify or implement an Economy-free executable run document controller that supports timeline control and runtime snapshot export.

## Prerequisites

- SB01 must pass.
- SB02 barrier semantics must be trusted for stage application.

## Owned Requirements

- R03 Components Run Document Controller.

## Dependency Impact

Feeds SB08 real-scenario runner output and SB09 snapshot runtime attachment.

## Validation Depth

Critical foundation: tests must prove real controller behavior, not only DTO construction.

## Proof Required

- WebGlRunLib test transcript.
- Source assertions for seek, apply stages, snapshot export, pause/resume, step forward/back, and stage/action id reporting.
- Proof manifest and semantic invariant contract.

## Progression Gate

Pass only when the controller remains generic, reports current stage/action identifiers, and exports runtime state shape sufficient for Economy to attach later.
