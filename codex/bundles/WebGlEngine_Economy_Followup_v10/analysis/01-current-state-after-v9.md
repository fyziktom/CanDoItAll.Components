# Current-state analysis after previous bundle

## What appears fixed

- Generic boundary checking in Components is now configurable through `WebGlRunGenericBoundaryOptions`, so economy-specific forbidden terms no longer live directly in the default generic validator.
- Economy supplies its own strict generic boundary term list through `EconomyWebGlMappingBoundary`.
- Runtime pause/stop is no longer only a C# flag; `RunPlayback` calls a `WebGlRunPlaybackStopCoordinator`, which immediately calls the runtime stop function before cancel/drain/final stop.
- Runtime idle machinery exists in JS and C# wrappers.
- `multi-goods-elite` was added to runtime scenarios and test fixtures.
- Design matrix materialization now copies scenario sources and applies factors to effective sources, with no-effect failure.
- Scenario pack manifest validation now checks strict hashes, required files, file hashes, extra files, and pack hash.

## What remains unsafe or incomplete

- `ResourceTransferVisual` still exists as a public generic WebGlRun action kind. This is a domain-shaped concept in a generic library.
- Browser observer proof must not rely on expected/fallback final positions. It needs actual browser-exported scene state and document hash.
- Readiness booleans such as `BrowserRuntimeExercised`, `UIExercised`, and `OracleProofExercised` need to become computed from artifact evidence.
- The third scenario should become a canary proving genericity, not only another scenario fixture.
- Domain-driver boundaries should be formalized before adding production-line or other domains.
