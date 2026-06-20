# Current-state summary after previous bundle

- Components has `WebGlLib`, `WebGlRunLib`, and `WebGlSandbox`.
- `RunPlayback` has Play/Pause/Cancel controls, but pause currently only affects C# state and CTS.
- WebGlLib JS runtime exposes `clearMotions` publicly, but does not expose `cancelCommandStageRunner` publicly.
- WebGlRun browser apply supports fail-closed frame apply and explicit multi-frame playback apply.
- Economy sandbox has a scenario selector, runtime scenario catalog, manifests, and deterministic replay through `ApplyPlaybackAsync`.
- Remaining hardening is centered on cancellation semantics, replay performance, scenario pack portability, and proof integrity.
