# SB04 JS API Parity Report

Approved JS API manifest:

- `tests/CanDoItAll.Components.WebGlLib.Tests/fixtures/approvals/webgllib-webglscene-js-api.approved.json`

Approved JS facade surface:

- `tests/CanDoItAll.Components.WebGlLib.Tests/fixtures/approvals/webgllib-webglscene-js-surface.approved.txt`

Hardening added:

- `Webgl_sceneview_csharp_facade_invokes_only_approved_js_api_methods` scans `WebGlSceneView` partial files for `CanDoItAll.webglScene.*` interop literals.
- The test fails if a C# wrapper invokes a JS method absent from the approved manifest.
- Runtime import graph audit passed in the RC transcript.

Validation:

- `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj --no-restore`
- `npm run webgllib:audit-scene-runtime-imports`
