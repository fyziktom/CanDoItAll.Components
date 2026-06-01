# Simulation + Visualization Join Readiness Report

## Current Completed Chain

- Input packs load through `SimulationExperimentInputPackLoader` and strict hash validation is exercised in `SimulationSandboxSessionTests.HeadlessExecutableProbe_LoadsStrictInputProjectsExecutesSnapshotsAndAnalyzes`.
- Backend selection and materialization run through `EconomySimulationSandboxWorkflow`, `DefaultEconomySimulationBackendSelector`, and `SimpleAccountsEconomySimulationBackend`.
- Visual frames are built by `EconomyVisualizationPipeline` and `EconomyVisualFrameMapper`.
- WebGL run documents are projected by `EconomyWebGlRunProjector`, `EconomyWebGlInitialSceneProjector`, and `EconomyWebGlActionStageProjector`.
- Generic playback execution is proven through `WebGlRunPlaybackController` and `WebGlRunFrameApplyResult.FromFrame` in the headless executable probe.
- Snapshots and analysis run through `EconomySnapshotPipeline`, `SimulationSnapshotBuilder`, and `SimulationSnapshotAnalysisService`.

## Missing Runtime Execution Pieces

- There is no Blazor UI loop that continuously applies `WebGlRunPlaybackResult.FramesToApply` to the browser runtime.
- There is no browser-side pause/seek synchronization between `EconomySimulationSandboxSession` and the WebGL canvas.
- Runtime execution is proven headlessly, but not yet with `IJSRuntime` or actual WebGL scene mutation in an Economy page.

## Missing Sandbox/Session Pieces

- `EconomySimulationSandboxSessionService` is headless and supports load/project/step/seek/pause/resume/snapshot/analyze.
- There is no persisted session registry or multi-user session store.
- Future ledger backend support is prepared through `IEconomySimulationBackendRegistry`; no ledger backend session test is included in this bundle.

## Missing Snapshot/Analysis Pieces

- Snapshot services are reusable production services and include data, visual-state, and full hashes.
- `FileSimulationSnapshotStore` is covered by tests, but the sandbox session does not yet save snapshots to a durable store automatically.
- Analysis facets are generic; richer narrative ranking for UI display remains out of scope.

## Visual Mapping Gaps

- Strict bridge diagnostics now distinguish unresolved mappings, missing pose/symbol mappings, fallback object use, and no-op fallbacks.
- Real fixture probes require explicit `AllowNoOpPoseFallback` and `AllowNoOpSymbolFallback` because current fixture mappings are intentionally incomplete for some admin/risk visual states.
- Abstractions and Visualization contain no direct WebGL/GLB/Components coupling per SB06 proof.

## Expected First Demo Scope

- Load one strict input pack.
- Use `EconomySimulationSandboxSessionService` to project a session.
- Bind current visual frame and `WebGlRunDocument` to a desktop WebGL view.
- Provide controls for pause, resume, step, seek, snapshot, and analyze.
- Show bridge warnings when fallback options are enabled.

## Explicit Non-goals

- No mobile/small-screen WebGL work.
- No new Components dependency from Components back to Economy.
- No final Blazor demo page in this bundle.
- No ledger backend implementation in this bundle.
- No large asset optimization or GLB delivery pipeline in this bundle.

## Blocker List

- Browser runtime integration still needs a UI-owned apply loop for `WebGlRunFrameApplyResult`.
- Fixture visual mappings should be completed for admin/risk pose and symbol states before strict no-fallback demos.
- Session persistence and snapshot store wiring need a follow-up bundle.

## Next Bundle Proposal

- Build the desktop Economy sandbox page around `EconomySimulationSandboxSessionService`.
- Add a browser playback adapter that turns `WebGlRunPlaybackResult` into WebGL runtime calls.
- Add fixture mapping completion for shared-resource and finite-resource probes.
- Add session persistence and file snapshot store integration.
- Add Playwright proof for load, play, pause, seek, snapshot, and analyze on the real page.
