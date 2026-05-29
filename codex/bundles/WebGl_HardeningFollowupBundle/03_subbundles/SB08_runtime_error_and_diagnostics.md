# SB08 - Runtime error and diagnostics hardening

## Goal

Make failures visible and testable.

## Tasks

1. Fix create failure path:
   - JS should call `OnRuntimeError` when create fails after dotNetRef is available.
   - Blazor should emit `RuntimeError` if JS returns `false`.
2. Add diagnostics for:
   - failed asset URI,
   - unsupported format,
   - GLTFLoader warning count if feasible,
   - missing fallback asset,
   - failed patch command.
3. Extend `WebGlRuntimeDiagnostics`.
4. Add UI display in sandbox.
5. Add negative proof scenario:
   - intentionally missing model URI,
   - fallback used,
   - runtime remains healthy,
   - diagnostics reports missing asset.

## Acceptance criteria

- No silent create failure.
- Missing GLB never breaks whole scene.
- Diagnostics are enough for Codex/browser tests.
