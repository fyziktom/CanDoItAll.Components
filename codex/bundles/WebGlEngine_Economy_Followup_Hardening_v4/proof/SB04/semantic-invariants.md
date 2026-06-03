# SB04 semantic invariants

- The implementation must preserve Components genericity.
- The implementation must not introduce Economy semantics into WebGlLib or WebGlRunLib.
- Runtime proof must demonstrate behavior, not only compilation.
- Required proof artifacts must be non-empty.

## Completion assertions

- Economy-specific deterministic replay is implemented only in `EconomySimulationSandboxPage`; no WebGlLib or WebGlRunLib source files were modified by SB04.
- The browser apply path uses generic `WebGlRunPlaybackResult` plus `WebGlRunBrowserApplyAdapter.ApplyPlaybackAsync` rather than per-frame Economy logic.
- `Step`, `Last`, `First`, and `Apply frame` all report `requiresSceneReset=True` after apply, so browser state is derived from the replay result instead of a stale local reset flag.
- Live route diagnostics prove `Last` applies `0,1,2` for the shared-well delta timeline.
