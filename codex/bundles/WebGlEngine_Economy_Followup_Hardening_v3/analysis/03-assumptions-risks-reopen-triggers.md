# Assumptions, Risks, And Reopen Triggers

## Assumptions

- `CanDoItAll.Components` remains on branch/ref `webgl-engine`.
- `CanDoItAll.Economy` remains on branch/ref `main`.
- Codex has both repositories available locally and can build them together.
- Comments in source code must be in English.
- The v3 work should not add domain-specific terms or logic to Components packages.
- The v3 work should not add new large features unless needed to make current APIs safe and provable.

## Critical Path Risks

1. **Validator-only safety.** Validators exist, but public apply APIs may still accept unsafe input if called directly.
2. **Host-specific registration.** Node registration works, but component package consumers need reusable registration.
3. **Path portability.** Absolute file paths in session exports and APIs can fail in package, Docker, and moved workspace scenarios.
4. **Proof theater.** Empty transcripts, screenshot-only proof, or "build passed" without behavior assertions can hide regressions.
5. **Large-scene pressure.** Current demos are small; future production-line and Economy scenarios may stress assets, motions, and command stages.

## Validation Risks

- BUnit tests with loose JS interop do not prove the browser WebGL runtime path.
- Browser screenshots do not prove frame command semantics unless diagnostics are captured and asserted.
- Package-mode builds with global NuGet caches can accidentally consume stale packages.
- Scenario catalog tests using repo-root helpers do not prove published Node/content layout.

## Reopen Triggers

- Any critical subbundle lacks failing-first proof.
- A proof transcript is empty or contains only command headers.
- The implementation relies on `tests/` paths outside tests.
- `WebGlRunFrameApplyResult.FromFrame` can still return success for mixed command frames.
- Browser apply continues after reset failure.
- `source.*` values are used to drive generic runtime behavior.
