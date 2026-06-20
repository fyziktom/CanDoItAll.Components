# Current-state analysis after v7

## What improved

1. Components now has `stopRuntimeActivity`, `waitForRuntimeIdle`, `applyCommandBatchAndWait`, typed `WebGlRuntimeIdleResult`, and diagnostics for runtime stop/idle.
2. `RunPlayback` now has generation-based playback cancellation and a late-drain stop path.
3. Economy now has `EconomyExperimentReadinessReport`, research-mode policies, behavior expansion profiles, metric/invariant registries, scenario pack hash validation, headless runner, design harness, and golden-oracle tests.
4. Documentation now correctly states that browser/WebGL is observer evidence, not economic truth.
5. Scenario packs are much closer to closed/reproducible artifacts because manifests include required files, per-file hashes, pack hash checks, and extra-file policy.

## What remains risky

1. Pause/stop can still be perceived as delayed because `RunPlayback.StopPlaybackAsync` waits for the playback task before runtime stop. During that wait, browser motion can continue.
2. Idle timeout and command lifecycle semantics are still split across APIs. Some paths can return a successful command with warnings rather than a hard proof failure.
3. Research readiness currently depends on `BrowserRuntimeExercised`, `UIExercised`, and `OracleProofExercised` flags. These must be tied to artifact-backed proof, not just booleans.
4. Design matrix factor levels appear to be recorded in summaries but are not yet proven to change the scenario input or run configuration.
5. Golden oracle cases are currently code-defined tests. That is useful, but research-grade oracle definitions should live in data fixtures so expected behavior cannot drift with helper code.
6. Readiness band classification must handle all diagnostics. Unknown backend or loader diagnostics must not fall through silently.
7. Strict simulation mode is much better, but it still needs broader negative/metamorphic/property tests to avoid hidden economic-noise sources.
8. Browser observer proof needs deterministic hash comparison against headless `WebGlRunDocument` and explicit idle-state assertions.

## Current usability verdict

Use current simulations for:

- exploratory research,
- scenario debugging,
- visual workflow proof,
- headless pipeline validation,
- oracle-development iterations,
- and performance profiling.

Do not yet use them for strong economic claims unless the run has:

- status at least `headless-valid` for headless economic conclusions,
- `oracle-valid` for claim-grade semantic conclusions,
- `browser-observer-valid` for visual playback claims,
- and eventually `research-ready` only when artifact-backed oracle and browser proof are present.
