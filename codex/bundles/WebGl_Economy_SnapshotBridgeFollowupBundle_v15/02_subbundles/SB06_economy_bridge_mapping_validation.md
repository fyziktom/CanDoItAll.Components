# SB06 — Economy bridge mapping validation

## Goal
Prevent bad visual mapping from silently producing misleading WebGL output.

## Required
- Unresolved subject object is error by default.
- Unresolved target object is error by default.
- Diagnostic fallback object is allowed only when `allowDiagnosticFallback=true`.
- Every node-object binding must be deterministic.
- Every action must retain source event id and visual action id.
- Every stage must retain source frame id and input pack hash.

## Validation
- Negative tests for missing node, missing target, missing symbol, missing pose.
- Positive test for explicit diagnostic fallback.

## Status
- Completed.

## Prerequisites
- SB05 batch contract proof is complete.
- SB01 boundary baseline is still valid.

## Exact Source References
- `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\EconomyWebGlRunProjector.cs`
- `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\EconomyWebGlActionStageProjector.cs`
- `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\EconomyWebGlBridgeContracts.cs`
- `C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\EconomyWebGlBridgeTests.cs`

## Dependency Impact
- Critical Economy bridge guardrail for SB07 and SB12.

## Validation Depth
- Requires adversarial unresolved mapping tests, explicit fallback positive test, metadata traceability proof, and boundary audit.

## Acceptance Checklist
- Unresolved subject and target objects fail by default.
- Missing symbol and pose fail unless allowed by explicit diagnostic fallback.
- Every emitted action/stage retains source event, visual action, frame, and input-pack hash metadata.

## Proof Required
- `bundle://proof/SB06/manifest.md`
- `bundle://proof/SB06/semantic-invariants.md`
- Economy test transcript and boundary audit transcript.

## Browser Validation Logging
- Browser validation is not required unless bridge output is validated through a live route.

## Progression Gate
- SB07 and SB12 may proceed only after strict mapping behavior is proven.

## Suggested Agent Prompt
- Harden Economy WebGL bridge mapping validation so unresolved mappings fail by default and traceability metadata survives projection.
