# Semantic Invariants for SB02

## Invariant SB02-idle-contract-observable

Source: `WebGlSceneView`, `WebGlSceneViewBrowserRuntime`, and runtime JS.

Expected behavior: host code can ask the browser runtime whether it is idle and receive structured diagnostics instead of guessing from UI state.

Passing result: unit tests cover the new diagnostics contract, and the browser proof consumes the runtime idle result after pause.

Why this prevents simulator-noise contamination: playback code can distinguish accepted commands from settled visual state, which keeps replay correctness separate from animation timing.

