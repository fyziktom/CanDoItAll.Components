# Components WebGL Engine RC Freeze Manifest

Status: Release candidate frozen for the generic Components WebGL engine.
Date: 2026-06-05
Scope: CanDoItAll.Components only.

## Frozen APIs

| Surface | Freeze artifact | Status |
|---|---|---|
| WebGlLib public C# API | repo://tests/CanDoItAll.Components.WebGlLib.Tests/fixtures/approvals/webgllib-public-api.approved.txt | Frozen |
| WebGlRunLib public C# API | repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/fixtures/approvals/webglrunlib-public-api.approved.txt | Frozen |
| WebGlLib package content | repo://tests/CanDoItAll.Components.WebGlLib.Tests/fixtures/approvals/webgllib-package-content.approved.txt | Frozen |
| WebGlRunLib package content | repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/fixtures/approvals/webglrunlib-package-content.approved.txt | Frozen |
| JS runtime API | repo://tests/CanDoItAll.Components.WebGlLib.Tests/fixtures/approvals/webgllib-webglscene-js-api.approved.json | Frozen |
| Run driver manifest schema | repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/fixtures/approvals/webglrunlib-domain-driver-manifest-schema.approved.txt | Frozen |

## RC Validation Proof

| Gate | Proof |
|---|---|
| Build | bundle://proof/SB16/transcripts/dotnet-build-final.txt |
| WebGlLib tests | bundle://proof/SB16/transcripts/dotnet-test-webgllib-final.txt |
| WebGlRunLib tests | bundle://proof/SB16/transcripts/dotnet-test-webglrunlib-final.txt |
| Pack | bundle://proof/SB16/transcripts/dotnet-pack-final.txt |
| Runtime idle policy | bundle://proof/SB06/transcripts/runtime-idle-policy-final.txt |
| Resource ownership | bundle://proof/SB12/transcripts/resource-ownership-final.txt |
| Command batch/motion/stage audits | bundle://proof/SB13/transcripts/command-batch-parity-final.txt; bundle://proof/SB13/transcripts/motion-queue-final.txt; bundle://proof/SB13/transcripts/stage-runner-final.txt |
| Domain source hard gate | bundle://proof/SB16/transcripts/domain-boundary-generic-source-hard-gate.txt |
| Domain public API hard gate | bundle://proof/SB16/transcripts/domain-boundary-generic-public-api-hard-gate.txt |
| Domain package hard gate | bundle://proof/SB16/transcripts/domain-boundary-package-content-hard-gate.txt |
| Browser observer proof | bundle://proof/SB14/browser-observer-proof.json; bundle://proof/SB14/screenshots/run-playback-1920x1080.png |
| Anti-stub audit | bundle://proof/SB16/transcripts/anti-stub-audit-owned-final.txt |

## Deferred Items

No required v14 bundle item is deferred. Future generic-engine changes must update the approval baselines and repeat the release checks in repo://docs/webgl/components-webgl-engine-rc-freeze.md.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| Runtime idle policy fields | repo://src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeIdleOptions.cs; repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/41-webgl-scene-runtime-idle-state.js | bundle://proof/SB06/transcripts/runtime-idle-policy-final.txt; bundle://proof/SB14/browser-observer-proof.json | repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/40-webgl-scene-runtime-idle.js | bundle://proof/SB06/transcripts/failing-first-or-closed-gap.txt |
| Generic package boundary | repo://Directory.Build.props; repo://samples/CanDoItAll.Components.WebGlRunLibGenericSample/CanDoItAll.Components.WebGlRunLibGenericSample.csproj | bundle://proof/SB11/transcripts/webglrunlib-sample-package-build.txt; bundle://proof/SB16/transcripts/dotnet-pack-final.txt | repo://tests/CanDoItAll.Components.WebGlRunLib.Tests/ComponentsPackageScopeTests.cs | bundle://proof/SB02/transcripts/failing-first-or-closed-gap.txt |
| Domain driver boundary | repo://src/CanDoItAll.Components.WebGlRunLib/DomainDrivers/WebGlRunDomainMappingDriver.cs | bundle://proof/SB16/transcripts/dotnet-test-webglrunlib-final.txt | repo://tools/webgllib/domain-boundary-audit.config.json | bundle://proof/SB10/transcripts/domain-boundary-audit.txt |
| Browser observer proof | bundle://proof/SB14/browser-proof-runner.mjs | bundle://proof/SB14/browser-observer-proof.json | repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs | bundle://proof/SB14/transcripts/failing-first-or-closed-gap.txt |

## Signoff

RC signoff passes. The generic Components WebGL engine is frozen for this bundle with no consuming-app project changes.

