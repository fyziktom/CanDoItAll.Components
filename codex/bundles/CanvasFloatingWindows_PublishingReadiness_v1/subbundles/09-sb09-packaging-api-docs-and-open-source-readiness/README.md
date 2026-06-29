# SB09 Packaging Api Docs And Open Source Readiness

## Status

- `Completed`

## Objective

- Prepare CanvasLib and OverlayLib package metadata, public API approvals, generated assets, and docs for near-term open-source publishing.

## Covered Inputs

- RAW01: Reuse the publishing-prep pattern from basic components.
- RAW03: Refactor, improve, harden, and validate Canvas and floating windows.
- RAW05: Preserve all functionality.
- RAW06: Make the code maintainable, clear, documented, and ready for open source.
- RAW07: Keep runtime implementation pure JavaScript and avoid npm runtime dependency.
- R01, R04, R07, R12, R13, R14.

## Prerequisites

- SB01-SB08 progression gates passed or any open blocker has an explicit owner and approval.
- Generated asset verifier passes or has a documented blocking defect from SB04.
- Package metadata can be inspected through `dotnet build`, `dotnet test`, and pack commands.

## Exact Source References

- repo://src/CanDoItAll.Components.CanvasLib/README.md
- repo://src/CanDoItAll.Components.CanvasLib/CanDoItAll.Components.CanvasLib.csproj
- repo://src/CanDoItAll.Components.OverlayLib/README.md
- repo://src/CanDoItAll.Components.OverlayLib/CanDoItAll.Components.OverlayLib.csproj
- repo://Directory.Build.props
- repo://package.json
- repo://tools/canvaslib/verify-assets.cjs
- repo://tools/canvaslib/build-assets.cjs
- repo://tests/CanDoItAll.Components.BaseLib.Tests/StandardPublishingApprovalTests.cs
- repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/standard-public-api.metadata.approved.json
- repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/standard-project-packability.approved.json
- repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/standard-source-package-inputs.approved.txt

## Deliverables

- README and package metadata alignment for CanvasLib and OverlayLib.
- CanvasLib package/API approval coverage comparable to the standard publishing suite.
- Generated asset verification proof.
- Package content verification proof for CanvasLib and OverlayLib.
- Runtime dependency proof showing no new npm runtime dependency is introduced for Canvas/Floating Windows/Calendar.
- Open-source transfer notes for usage, assets, public contracts, and validation commands.
- Minimal source or doc repairs needed to make public behavior clear without changing behavior.

## Dependency Impact

- SB10 final closure depends on package/API/docs evidence and version consistency.
- Open-source transfer depends on accurate README examples, package metadata, generated asset instructions, and public API snapshots.
- Weak package proof could publish incomplete static assets or accidental public API drift.

## Validation Depth

- Critical publishing closure.
- Package/API approval tests, generated asset verification, build/test/pack proof, and semantic proof.
- Artifact-backed proof manifest required.

## Implementation Steps

1. Compare CanvasLib README version and package metadata; fix drift such as README `0.1.0` versus project `0.1.1` if still present.
2. Review OverlayLib README and package metadata for equivalent publishing clarity.
3. Extend or add approval coverage for CanvasLib public API, package packability, package inputs, generated assets, and static web assets.
4. Run `npm run canvaslib:verify-assets`; run build-assets only if verification instructs it and commit generated outputs as part of the scoped change.
5. Run focused tests, then solution build/test/pack commands required by the existing publishing workflow.
6. Validate README examples and open-source transfer notes against actual package entry points.
7. Confirm npm/Node usage remains limited to Tailwind, generated asset, test, and browser tooling; reject runtime package dependency drift.
8. Confirm no WebGL package/docs/API files changed.
9. Update execution report and create SB09 proof artifacts.

## Scope Exceptions

- UI behavior proof is owned by SB05-SB08.
- Final red-team closure and follow-up split are owned by SB10.

## Do Not Do

- Do not hand-edit generated asset include components; use `tools/canvaslib/build-assets.cjs` when needed.
- Do not expand public API accidentally to make approvals pass.
- Do not publish packages externally.
- Do not edit WebGL package, docs, or source files.
- Do not add npm runtime dependencies for Canvas, floating-window, calendar, or preview implementation.

## Acceptance Checklist

- CanvasLib and OverlayLib package versions, README examples, and metadata agree.
- CanvasLib has explicit package/API approval coverage or a documented approved test home.
- Generated assets verify cleanly.
- Package content includes expected static web assets and excludes unrelated WebGL assets.
- Package/dependency proof shows runtime code is plain browser JavaScript/C# and Razor and npm remains tooling-only for this scope.
- Build/test/pack proof passes.
- Open-source transfer notes list validation commands and known follow-ups.

## Closure Summary

- CanvasLib README now aligns with package version `0.1.1`; OverlayLib README aligns with inherited package version `0.1.0`.
- Added focused CanvasLib/OverlayLib publishing approval tests for public API metadata, project packability metadata, static web assets, and npm runtime dependency policy.
- `npm run canvaslib:verify-assets`, focused approval tests, focused release builds, pack commands, package content inspection, runtime dependency proof, source assertions, and anti-stub audit all passed.
- The broad solution Release build remains recorded as an out-of-scope WebGL sample/test restore exception under `--no-restore`; SB09 did not edit WebGL source, packages, tests, samples, or tools.
- Proof is recorded in `bundle://proof/SB09/manifest.md`.

## Proof Required

- `npm run canvaslib:verify-assets` transcript.
- `dotnet build` transcript.
- Focused approval/test transcript.
- `dotnet pack` transcript for CanvasLib and OverlayLib or the solution packaging command used by the repo.
- Package content manifest.
- API approval diff or approval transcript.
- `bundle://proof/SB09/manifest.md`
- `bundle://proof/SB09/semantic-invariants.md`
- Anti-stub audit transcript.

## Browser Validation Logging

- N/A unless documentation or package fixes alter sandbox-visible output. If they do, reuse the failing SB05-SB08 route and viewport that proves the repair.

## Progression Gate

- SB10 may proceed only after build/test/pack, generated asset verification, package content proof, and README/API alignment pass.
- Reopen SB04 if generated asset proof fails; reopen SB03 or SB07 if public API proof reveals unclear state/window ownership.

## Suggested Agent Prompt

```text
Execute SB09 only. Align CanvasLib and OverlayLib docs, package metadata, generated assets, public API approvals, and package content proof for open-source readiness while preserving behavior and excluding WebGL.
```

