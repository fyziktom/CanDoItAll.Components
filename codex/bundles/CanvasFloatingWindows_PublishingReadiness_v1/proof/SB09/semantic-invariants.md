# SB09 Semantic Invariants

## SB09-INV-PACKAGE-DOCS

- CanvasLib README package version must match `CanDoItAll.Components.CanvasLib.csproj` `PackageVersion`.
- OverlayLib README package version must match the inherited `Directory.Build.props` base package version unless OverlayLib explicitly overrides it.
- README examples must name the generated head/body asset components and show the runtime asset flags needed by consuming hosts.
- Evidence: `bundle://proof/SB09/transcripts/source-assertions-package-docs.txt`.

## SB09-INV-API-APPROVALS

- CanvasLib and OverlayLib public API metadata, packability metadata, and static web asset inventory must be covered by approval fixtures comparable to the standard publishing suite.
- Steady-state approval tests must pass without updating fixtures.
- Evidence: `bundle://proof/SB09/transcripts/dotnet-test-canvas-overlay-approvals.txt`.

## SB09-INV-GENERATED-ASSETS

- CanvasLib generated include components and public static asset outputs must remain synchronized with the source asset graph.
- Asset verification must pass without hand-editing generated include components.
- Evidence: `bundle://proof/SB09/transcripts/npm-canvaslib-verify-assets.txt`.

## SB09-INV-PACKAGE-CONTENTS

- CanvasLib package must include its README, net10 assembly, workbench runtime entry, calendar interop, and workbench shell CSS.
- OverlayLib package must include its README, net10 assembly, overlay runtime JS, and overlay CSS.
- Neither package may include WebGL static assets.
- Evidence: `bundle://proof/SB09/transcripts/package-content-manifest.txt`.

## SB09-INV-RUNTIME-DEPENDENCIES

- Canvas, floating-window, calendar, and preview runtime code must remain C# and Razor plus plain browser JavaScript.
- Root npm packages must remain tooling-only; CanvasLib/OverlayLib runtime JS must not depend on `import`, `require`, or npm runtime modules.
- Evidence: `bundle://proof/SB09/transcripts/runtime-dependency-proof.txt`.

## Reopen Decisions

- No source reopen remains for SB09.
- The broad solution Release build is not accepted as a package failure because its only failing projects are WebGL sample/test projects excluded from this bundle. The focused CanvasLib/OverlayLib Release build and pack commands passed.


## Validator Contract Summary

- Invariant ID: `SB09-INV-PACKAGE-DOCS`
- Source raw note: RAW01, RAW03, RAW05, RAW06, RAW07.
- Expected behavior: CanvasLib and OverlayLib package docs, approvals, generated assets, package contents, and runtime dependency policy must be publication-ready without WebGL scope.
- Disallowed shallow implementation: README-only proof, missing approval fixtures, missing package static assets, npm runtime dependency creep, or accidental WebGL package inclusion.
- Failing-first test: N/A process/no production behavior change; SB09 changed docs, approval fixtures, and proof artifacts.
- Passing test: `bundle://proof/SB09/transcripts/dotnet-test-canvas-overlay-approvals.txt`
- Changed source files: `repo://src/CanDoItAll.Components.CanvasLib/README.md`, `repo://src/CanDoItAll.Components.OverlayLib/README.md`, and `repo://tests/CanDoItAll.Components.BaseLib.Tests/CanvasOverlayPublishingApprovalTests.cs`.
- Production assertions: `bundle://proof/SB09/transcripts/source-assertions-package-docs.txt`
- Red-team negative case: `bundle://proof/SB09/transcripts/package-content-manifest.txt` and `bundle://proof/SB09/transcripts/runtime-dependency-proof.txt` reject missing package assets, WebGL entries, npm runtime dependencies, and import/require runtime paths.
- Downstream dependency check: SB10 final closure can depend on `bundle://proof/SB09/manifest.md`.


