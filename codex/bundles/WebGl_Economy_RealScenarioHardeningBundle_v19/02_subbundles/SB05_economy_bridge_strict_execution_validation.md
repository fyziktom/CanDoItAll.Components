# SB05 - Economy bridge strict execution validation

Codex must harden `EconomyWebGlRunValidator` so strict mode fails when:

- a stage has no command and is not an explicit wait,
- a stage lacks source visual action id,
- a stage lacks source simulation frame id,
- a stage lacks input pack hash,
- a motion refers to an object not in initial scene,
- a patch refers to an object not in initial scene,
- a fallback object is used while fallback is disabled,
- no-op pose/symbol fallback is used while disabled.

The validator should report structured messages with path/code/severity.

## Status

Completed.

## Goal

Make Economy WebGL bridge strict validation reject incomplete, fallback-dependent, or unresolved executable run documents with structured diagnostics.

## Prerequisites

- SB01 must pass.
- Current Economy bridge validator and tests must be inspected before editing.

## Owned Requirements

- R05 Economy Bridge Strictness.

## Dependency Impact

Blocks SB08 real-scenario runner readiness from accepting invalid bridge output.

## Validation Depth

Critical behavior gate: negative tests must fail shallow validation and passing tests must prove structured diagnostics.

## Proof Required

- Economy test transcript covering every listed strict failure.
- Source assertions for path/code/severity diagnostics.
- Proof manifest and semantic invariant contract.

## Progression Gate

Pass only when each listed invalid input produces a structured diagnostic and strict mode rejects the run.
