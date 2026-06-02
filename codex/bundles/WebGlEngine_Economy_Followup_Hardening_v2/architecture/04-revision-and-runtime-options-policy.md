# Scene revision and runtime option policy

## Canonical revision target

- `WebGlSceneModel.Revision` is canonical.
- `WebGlSceneModel.UiState.Revision` is a mirror for backward compatibility only.
- Commit and normalize operations must write both values unless UI state is explicitly excluded from a serialized document.
- When UI state is excluded, the document must still be self-consistent and validators must not create ambiguous revision comparisons.

## Runtime options reset target

The follow-up must choose one policy:

### Policy A — runtime options are external

`WebGlRunBrowserApplyAdapter` imports only the scene. It must warn or document that `WebGlSceneDocument.RuntimeOptions` is ignored by browser reset. Tests must verify this is intentional.

### Policy B — runtime options participate in reset

The runtime wrapper accepts scene document options and calls an import/update API that applies them. Tests must verify profile changes, diagnostics visibility, render mode, and device pixel ratio.

Near-term recommendation: choose Policy A unless there is an immediate need for replay-controlled runtime profiles. Make the policy explicit and validator-backed.
