# Semantic Invariants for SB03

## Invariant SB03-pause-cancels-host-and-runtime-work

Source: `RunPlayback.razor.cs` and WebGlRun playback controller.

Expected behavior: pause/stop cancels host-side playback work, drains late applies, waits for runtime idle, and reasserts paused UI state.

Passing result: WebGlRunLib playback tests passed, WebGL sandbox build passed, and the browser proof shows runtime work drained after pause.

Why this prevents simulator-noise contamination: logical frame playback no longer advances by relying on unsettled visual work as proof of state.

