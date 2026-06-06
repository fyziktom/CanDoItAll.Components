# Components WebGL Engine RC Freeze

This document records the release-candidate contract for the generic WebGL substrate in `CanDoItAll.Components`.

## Frozen Surfaces

The following surfaces are frozen for normal consumers:

- `CanDoItAll.Components.WebGlLib` public C# API captured by `tests/CanDoItAll.Components.WebGlLib.Tests/fixtures/approvals/webgllib-public-api.approved.txt`.
- `CanDoItAll.Components.WebGlRunLib` public C# API captured by `tests/CanDoItAll.Components.WebGlRunLib.Tests/fixtures/approvals/webglrunlib-public-api.approved.txt`.
- `window.CanDoItAll.webglScene` method names and result-shape expectations captured by `tests/CanDoItAll.Components.WebGlLib.Tests/fixtures/approvals/webgllib-webglscene-js-api.approved.json`.
- WebGlLib and WebGlRunLib package contents captured by the package-content approval snapshots.
- Generic run-driver manifest and validator behavior captured by the WebGlRunLib approval and validator tests.

Changes to these surfaces should be treated as compatibility events. Update the approval file, add a migration note, and run the domain-boundary hard gates before accepting the change.

## Package Consumption

Use package references for consumer smoke tests once a package build is available.

For the WebGlRunLib generic sample:

```xml
<UseComponentsWebGlRunLibPackage>true</UseComponentsWebGlRunLibPackage>
<ComponentsWebGlRunLibPackageVersion>0.1.0-v14.20260605.2</ComponentsWebGlRunLibPackageVersion>
```

The sample still supports project-reference mode for nearby development. The `ValidateComponentsWebGlRunLibReference` target rejects ambiguous package/project configuration.

## Runtime Idle Modes

`WebGlRuntimeIdlePolicyModes` exposes three modes:

- `semanticOnly`: command stages, motions, and resource disposal must settle; visual render scheduling can remain active.
- `visualStrict`: semantic and visual blockers must both be clear.
- `allowFinalRenderDrain`: semantic idle is required and a final scheduled render can drain before success.

Browser proof that asserts a final visual state should use `visualStrict`. Component flows that only need command completion may use `semanticOnly` explicitly.

## Generic Driver Boundary

`WebGlRunLib` remains generic. Domain terms, transformations, and rule vocabularies belong in a consuming driver. The pass-through driver only passes approved generic action kinds and maps unknown action kinds to `Wait`.

Source provenance is intentionally opaque. Generic validators allow stable reference tokens such as `source.provenanceMode` and `source.traceMapRef`, but they do not interpret consumer-specific meaning.

## Companion Docs

- `docs/webgl/external-consumer-quickstart.md` describes package-mode scene and run hosting.
- `docs/webgl/api-change-request-template.md` is required before changing a frozen API, JS facade, action-kind, package, or driver-manifest surface.
- `docs/webgl/post-freeze-change-governance.md` defines allowed post-freeze changes and proof requirements.

## Required Release Checks

Before changing or releasing these packages, run:

```powershell
npm run webgl:validate-rc
```

The command wraps the focused checks below and emits an artifact manifest under `artifacts/webgl-engine-rc-v15`.

```powershell
npm run webgllib:test-runtime-idle-policy
npm run webgllib:test-resource-ownership
npm run webgllib:audit-command-batch-parity
npm run webgllib:audit-motion-queue
npm run webgllib:audit-stage-runner
node tools/webgllib/domain-boundary-auditor.cjs --profile generic-source-hard-gate
node tools/webgllib/domain-boundary-auditor.cjs --profile generic-public-api-hard-gate
node tools/webgllib/domain-boundary-auditor.cjs --profile package-content-hard-gate
dotnet build CanDoItAll.Components.slnx /p:UseSharedCompilation=false
dotnet test tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore /p:UseSharedCompilation=false
dotnet test tests\CanDoItAll.Components.WebGlRunLib.Tests\CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-restore /p:UseSharedCompilation=false
dotnet pack CanDoItAll.Components.slnx --configuration Release --output artifacts\packages
```

For browser-visible changes, also run the `/run-playback` browser observer proof and capture a screenshot plus the JSON observer report.
