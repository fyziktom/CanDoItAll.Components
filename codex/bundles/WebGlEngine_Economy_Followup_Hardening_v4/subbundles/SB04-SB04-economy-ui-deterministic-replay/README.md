# SB04 Economy UI deterministic replay

## Objective

Implement the next hardening step for the WebGL/Economy engine foundation.

## Scope

- Refactor `EconomySimulationSandboxPage` to use `WebGlRunDocumentRunner` or an equivalent deterministic playback service.
- `Step`, `First`, `Last`, and future seek operations must apply the required sequence from initial scene to target frame, or prove frames are absolute.
- Add UI diagnostics for `appliedFrameIndexes`, `requiresSceneReset`, `runnerState`, and failure reason.
- Ensure `BrowserSceneWasReset` cannot become a stale local boolean that disagrees with runner state.

## Out of scope

- Do not add domain semantics into Components packages.
- Do not rewrite unrelated systems.
- Do not close the subbundle with screenshots only.
- Do not accept empty required proof artifacts.

## Implementation guidance

- Start with a failing-first test or audit where applicable.
- Make the smallest cohesive refactor that fixes the root cause.
- Add source assertions that prove the intended path is used.
- Keep API compatibility where safe; otherwise document the migration.
- Ensure all source-code comments are in English.

## Required proof

- Failing-first browser/component test for seek-to-last with a delta frame.
- Passing component test proving multiple frames are applied for seek-to-last.
- Node route browser proof for Step, Last, First, Apply, Snapshot.
- Console review and runtime diagnostics JSON.

## Refactor gate

Before moving to the next subbundle, record:
- changed files;
- test/build/audit commands;
- proof artifact paths;
- open risks;
- whether public API changed and how users migrate.

## Execution status

Status: Completed

## Implementation notes

- Replaced the sandbox page's single-frame browser apply path with an explicit deterministic replay builder.
- Removed the mutable `BrowserSceneWasReset` field so reset state is derived from `WebGlRunBrowserPlaybackApplyResult`.
- Added UI diagnostics for `appliedFrameIndexes`, `requiresSceneReset`, `runnerState`, and `failureReason`.
- Added a component test proving Last replays every frame from the initial scene through the target delta frame.

## Refactor gate result

- Changed files:
  - `C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Components\Components\SimulationSandbox\EconomySimulationSandboxPage.razor`
  - `C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\EconomySimulationSandboxComponentTests.cs`
- Commands:
  - `dotnet test C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter "FullyQualifiedName~EconomySimulationSandboxPage_LastReplaysAllDeltaFramesFromInitialScene"` failed before the page refactor and passed after it.
  - `dotnet test C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter "FullyQualifiedName~EconomySimulationSandboxComponentTests"` passed 2/2 tests.
  - Browser route proof exercised `/economy/simulation-sandbox` controls: Step, Last, First, Apply frame, Snapshot.
- Proof artifact paths:
  - `proof/SB04/transcripts/failing-first.txt`
  - `proof/SB04/transcripts/passing-component-test.txt`
  - `proof/SB04/transcripts/component-class-tests.txt`
  - `proof/SB04/transcripts/source-assertions.txt`
  - `proof/SB04/transcripts/boundary-audit.txt`
  - `proof/SB04/changed-file-hashes.md`
  - `proof/SB04/browser/runtime-diagnostics.json`
  - `proof/SB04/browser/console-review.json`
  - `proof/SB04/browser/browser-proof-summary.md`
- Open risks: The deterministic UI path intentionally replays from initial scene on every apply for correctness. If future large timelines make this too expensive, the next optimization must prove frame absoluteness or add checkpointed replay.
- Public API change: None.
