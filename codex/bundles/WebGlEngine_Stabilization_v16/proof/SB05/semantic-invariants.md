# Semantic invariants - SB05

- Components-only scope is preserved; implementation stayed under `repo://src`, `repo://tests`, `repo://tools`, `repo://scripts`, `repo://docs`, and this bundle.
- WebGlLib JS API additions or removals must update `webgllib-webglscene-js-api.approved.json`.
- `WebGlSceneView` C# facade methods may invoke only approved JS API method names.
- JS API entries must declare parameter shape, result shape, missing-runtime result, lifecycle behavior, idle/settled behavior, and failure behavior.
- Public API drift is intentional, approved, and represented in freeze snapshots.

## Production Behavior Artifact Matrix

| Behavior | Producer | Consumer | Lifecycle | Proof |
|---|---|---|---|---|
| JS API freeze | `webgllib-webglscene-js-api.approved.json` | `WebGlLibFreezeApprovalTests` | test-time approval gate | 69-test WebGlLib pass in `repo://artifacts/webgl-engine-rc-v16/validation-summary.md` |
| C# interop parity | `WebGlSceneView*` literal scan | `Webgl_sceneview_csharp_facade_invokes_only_approved_js_api_methods` | test-time source scan | same RC summary |
