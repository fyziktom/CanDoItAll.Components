# SB09 Proof Manifest

## Status

- Result: Passed with an explicit out-of-scope solution-build exception.
- Date: 2026-06-29.
- Scope: CanvasLib and OverlayLib package/API/docs/open-source readiness without WebGL package or source work.

## Source Changes

- `repo://src/CanDoItAll.Components.CanvasLib/README.md`
- `repo://src/CanDoItAll.Components.OverlayLib/README.md`
- `repo://tests/CanDoItAll.Components.BaseLib.Tests/CanvasOverlayPublishingApprovalTests.cs`
- `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/canvas-overlay-project-packability.approved.json`
- `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/canvas-overlay-public-api.metadata.approved.json`
- `repo://tests/CanDoItAll.Components.BaseLib.Tests/fixtures/approvals/canvas-overlay-static-web-assets.approved.txt`

## Command Proof

- Approval update pass: `bundle://proof/SB09/transcripts/dotnet-test-canvas-overlay-approvals-update.txt`
- Approval steady-state pass: `bundle://proof/SB09/transcripts/dotnet-test-canvas-overlay-approvals.txt`
- Generated asset verification: `bundle://proof/SB09/transcripts/npm-canvaslib-verify-assets.txt`
- Focused release build: `bundle://proof/SB09/transcripts/dotnet-build-canvas-overlay-release.txt`
- Package creation: `bundle://proof/SB09/transcripts/dotnet-pack-canvas-overlay.txt`
- Package content manifest: `bundle://proof/SB09/transcripts/package-content-manifest.txt`
- Runtime dependency proof: `bundle://proof/SB09/transcripts/runtime-dependency-proof.txt`
- Source/package/docs assertions: `bundle://proof/SB09/transcripts/source-assertions-package-docs.txt`
- Anti-stub audit: `bundle://proof/SB09/transcripts/anti-stub-audit.txt`
- Changed-file hashes: `bundle://proof/SB09/transcripts/changed-file-hashes.txt`
- Semantic adequacy: `bundle://proof/SB09/transcripts/semantic-adequacy.txt`
- Representative SHA-256: `918A002A8372A4434846594AF007433BF0EF4B1F7FC417DE0C86084492AD2A6E`
- Failing-first: N/A process/no production behavior change for package/docs/API approval coverage; SB09 changed documentation, approval fixtures, and proof artifacts without changing runtime behavior.
- Passing transcript: `bundle://proof/SB09/transcripts/dotnet-test-canvas-overlay-approvals.txt`

## Package Artifacts

- `bundle://proof/SB09/packages/CanDoItAll.Components.CanvasLib.0.1.1.nupkg`
- `bundle://proof/SB09/packages/CanDoItAll.Components.OverlayLib.0.1.0.nupkg`

## Build Exception

- Broad command `dotnet build CanDoItAll.Components.slnx --configuration Release --no-restore` is recorded in `bundle://proof/SB09/transcripts/dotnet-build-release.txt`.
- That command failed only on out-of-scope WebGL sample/test restore gaps for `Microsoft.AspNetCore.Components.Analyzers 10.0.4`.
- In-scope CanvasLib and OverlayLib release builds passed with 0 warnings and 0 errors, and both packages packed successfully.
- SB09 did not edit WebGL package, source, sample, test, or tool files.

## Gate Decision

- SB10 may proceed.
- Final closure must preserve the documented WebGL exclusion and cite the focused package/build proof rather than treating the broad WebGL restore exception as Canvas/Overlay package failure.
