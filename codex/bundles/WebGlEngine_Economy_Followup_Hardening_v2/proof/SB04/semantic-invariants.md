# SB04 semantic invariants

Status: Completed 2026-06-02.

| Invariant ID | Expected behavior | Shallow-pass trap | Negative proof | Positive proof | Source files | Downstream dependency |
| --- | --- | --- | --- | --- | --- | --- |
| SB04-INV-001 | `WebGlSceneModel.Revision` is canonical; `UiState.Revision` mirrors it whenever UI state is included. | Only asserting the top-level revision while leaving stale UI revision drift in serialized documents. | `bundle://proof/SB04/transcripts/failing-first-revision-policy-tests.txt` failed because normalization kept UI revision `2` while scene revision was `7`. | `bundle://proof/SB04/transcripts/passing-revision-policy-tests.txt` and `components-webgllib-tests.txt`. | `WebGlSceneRevisionPolicy.cs`, `WebGlSceneDocumentSerializerTests.cs` | SB05 patch transaction proofs and SB11 browser document import/export proof. |
| SB04-INV-002 | UI-excluded serialization keeps the canonical scene revision and removes UI-state revision ambiguity. | Treating `IncludeUiState=false` as proof that revision consistency does not matter. | The same failing-first serializer run showed divergent UI revision could change document identity before normalization mirrored it. | `Serialize_without_ui_state_keeps_canonical_scene_revision_without_ui_mirror_conflict` passes in `passing-revision-policy-tests.txt`. | `WebGlSceneDocumentNormalizer.cs`, `WebGlSceneRevisionPolicy.cs`, `WebGlSceneDocumentSerializerTests.cs` | Package-mode scene document proof in SB09. |
| SB04-INV-003 | `WebGlSceneDocument.RuntimeOptions` are external to browser reset; reset imports only the scene and default runtime options. | Documenting that the concrete browser runtime ignores runtime options while the adapter still forwards them to other runtimes. | `bundle://proof/SB04/transcripts/failing-first-runtime-options-reset-test.txt` failed because the fake runtime received `RenderMode=continuous`. | `bundle://proof/SB04/transcripts/passing-runtime-options-reset-test.txt` and `components-webglrunlib-tests.txt`. | `WebGlRunBrowserApplyAdapter.cs`, `WebGlRunBrowserApplyAdapterTests.cs` | SB11 browser reset proof and package consumer reset semantics. |
| SB04-INV-004 | Components layers remain generic and free of Economy/domain semantics after policy hardening. | Passing serializer/adapter tests while accidentally adding domain-specific branching or fixture-only paths. | Boundary audits would fail on forbidden references or domain terms; placeholder scan would fail on stub markers. | `components-webgllib-boundary-audit.txt`, `components-webglrunlib-boundary-audit.txt`, and `changed-file-placeholder-scan.txt`. | `docs/webgl/run-layer-boundary.md`, Components WebGL production files | R14 genericity and all later subbundles. |

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative test |
| --- | --- | --- | --- | --- |
| Revision mirror write | `WebGlSceneRevisionPolicy.Commit` | Normalizer, reducer, serializer | Called by `Normalize` and live scene mutation paths; writes both revision fields | Failing-first `Normalize_mirrors_canonical_scene_revision_to_ui_state` observed stale UI revision. |
| Canonical identity hashing | `WebGlSceneDocumentSerializer.Serialize` after normalization | Scene document persistence and package consumers | Normalizes before computing content/document hashes | Failing-first hash assertion showed UI revision drift created different document hashes. |
| Scene-only reset import | `WebGlRunBrowserApplyAdapter.CreateSceneResetDocument` | Browser runtime adapter | Created for each reset request; runtime options and diagnostics reset to defaults | Failing-first fake-runtime test observed non-default runtime options. |
| Runtime-options warning | `WebGlRunBrowserApplyAdapter.ApplyAsync` | Playback caller and snapshot | Added only when non-default document runtime options are stripped | Passing fake-runtime test asserts warning and preserved source document options. |

## Raw Requirement Closure

| Requirement | Closure |
| --- | --- |
| R04 | Solved for SB04. Canonical revision policy is documented, `Normalize` mirrors through `Commit`, conflicting-revision tests fail first and pass after implementation, and WebGlLib full tests pass. |
| R05 | Solved for SB04. Browser reset treats document runtime options as external, strips them before runtime import, emits a warning for non-default reset options, and fake-runtime tests prove the policy. |
| R14 | Maintained locally. Components boundary audits and placeholder scan pass for the changed WebGL policy surface. |
