# Current-State Analysis After v2 Completion

## What improved

The v2 implementation moved the codebase forward substantially.

1. `CanDoItAll.Components` now treats `WebGlRunLib` as an optional generic run/playback/action/stage layer over `WebGlLib`.
2. `CanDoItAll.Economy` now has a runtime-safe scenario content location under the Node project, and Node copies those scenario files to output and publish.
3. The Economy simulation sandbox page no longer hardcodes `tests/CanDoItAll.Economy.Tests/Fixtures/...`; it injects `IEconomySimulationScenarioCatalog`.
4. `FileSystemEconomySimulationScenarioCatalog` includes basic path traversal guardrails.
5. `WebGlRunDocumentValidator` now rejects mixed frame-level commands and staged commands.
6. `EconomyWebGlRunValidator` now tracks dynamically-added objects across ordered patch stages and checks command source metadata.
7. Resource ownership and texture-retention diagnostics were improved in WebGlLib.

## New review verdict

The system is no longer primarily blocked by missing scaffolding. It is now blocked by stability, consumer ergonomics, and semantic fail-safe behavior.

The main remaining risk is that several safeguards exist only as validators or host-local registrations, while the public runtime APIs still allow unsafe or surprising behavior when a consumer does not call the validator first or does not host inside `CanDoItAll.Economy.Node`.

## Important current gaps

### F01 Proof integrity is still fragile

The v2 bundle reports completion and many tests, but the compare view shows many transcript artifacts as zero-line additions. Some proof files may be placeholders or command transcripts captured without meaningful content. This does not prove the implementation is wrong, but it means a v3 audit must separate real executable proof from empty or diagnostic-only artifacts.

### F02 Component service registration is host-specific

`EconomySimulationSandboxPage` injects `IEconomySimulationSandboxSessionService` and `IEconomySimulationScenarioCatalog`, but the only production registration found is inside `EconomyNodeServiceRegistration`. The BUnit test manually registers both. A consuming app that uses `CanDoItAll.Economy.Components` outside Node has no obvious reusable extension.

### F03 Scenario catalog is file/path-based and not yet a scenario-pack contract

The catalog provides descriptors with absolute `ExperimentJsonPath`, and session loading still takes a string path. This is better than test fixture lookup but still not a portable scenario-pack abstraction. Exports still preserve local path fields.

### F04 UI cannot really choose scenarios

The page has `DefaultScenarioId` and loads one scenario. It uses the catalog to resolve default, but does not render a scenario picker, scenario manifest details, or invalid/missing-scenario recovery UX.

### F05 Session service is sync-over-async

`EconomySimulationSandboxSessionService` calls async snapshot store operations through `.AsTask().GetAwaiter().GetResult()`. This is risky for server UI, future remote stores, and any future storage provider that has real I/O latency.

### F06 Frame apply can still silently drop commands if callers bypass validation

`WebGlRunDocumentValidator` rejects mixed direct+staged frames, but `WebGlRunFrameApplyResult.FromFrame` still silently ignores direct frame-level commands when stages exist. The apply method should enforce the policy itself and surface an error, not rely on validators having been called upstream.

### F07 Browser apply should fail fast on reset failure

`WebGlRunBrowserApplyAdapter.ApplyAsync` can record a reset error but still apply the command batch afterward. A frame that requires a scene reset should not mutate the existing browser scene if no initial scene is supplied or import fails.

### F08 Scene document runtime options are not applied on browser reset

`WebGlSceneViewBrowserRuntime.ImportSceneAsync` imports only `sceneDocument.Scene` and ignores `sceneDocument.RuntimeOptions`. This weakens the value of `WebGlSceneDocument` and may cause mismatched runtime behavior across exported/imported scenes.

### F09 Provenance boundary is too coarse

Generic validation skips `source.*` metadata entirely. That is pragmatic, but it allows arbitrary domain data and possibly policy semantics to be hidden under `source.*`. The boundary needs a typed provenance policy that allows traceability but not hidden runtime behavior.

### F10 Resource/performance budgets are still diagnostic, not policy

Asset cache/resource hardening improved, but there is no explicit large-simulation budget policy for loaded assets, scene objects, active motions, queued stages, retained textures, or renderer context lifecycle. This will matter for large Economy and production-line simulations.

## Senior architect conclusion

The implementation is directionally correct. v3 should avoid another broad feature expansion. It should focus on making current public APIs safe by default, reducing host-specific assumptions, and creating executable proof that prevents silent failure modes.
