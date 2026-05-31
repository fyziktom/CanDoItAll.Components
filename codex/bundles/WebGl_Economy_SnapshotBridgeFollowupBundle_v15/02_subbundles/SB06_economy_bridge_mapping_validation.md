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
