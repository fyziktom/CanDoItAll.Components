# Proof manifest - SB05

Status: completed

## Scope

JS API and C# interop parity freeze v2 for `window.CanDoItAll.webglScene` and `WebGlSceneView`.

## Artifacts

- JS API approval manifest: `repo://tests/CanDoItAll.Components.WebGlLib.Tests/fixtures/approvals/webgllib-webglscene-js-api.approved.json`
- Freeze/parity tests: `repo://tests/CanDoItAll.Components.WebGlLib.Tests/WebGlLibFreezeApprovalTests.cs`
- Final changed-file hashes: `bundle://proof/SB22/changed-file-hashes.txt`
- RC transcript proving the test run: `repo://artifacts/webgl-engine-rc-v16/validate-release-candidate.transcript.txt`
- RC summary: `repo://artifacts/webgl-engine-rc-v16/validation-summary.md`

## Commands

- `dotnet test tests\CanDoItAll.Components.WebGlLib.Tests\CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore /p:UseSharedCompilation=false`
- `powershell -ExecutionPolicy Bypass -File scripts\validate-webgl-rc.ps1 -SkipBrowserProof -PackageProofSuffix '-rcv16.codex'`

## Result

- WebGlLib tests passed: 69 passed, 0 failed.
- Approval manifest now records method, parameter shape, result shape, missing-runtime behavior, lifecycle behavior, idle/settled behavior, and failure behavior.
- Negative unapproved-method probe and C# facade parity test are present.

## Production Behavior Artifact Matrix

| Signal | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Approved JS API manifest | `WebGlLibFreezeApprovalTests` | reviewers and RC wrapper | Read during WebGlLib test run | `Webgl_scene_js_api_approval_rejects_deliberate_unapproved_method_probe` |
| C# interop method literals | `WebGlSceneView*` files | parity test | scanned during freeze approval | unapproved method fails the parity assertion |
