# SB10 proof manifest

Status: completed

## Semantic assertion

`WebGlSceneView` successful external imports now update component-local lifecycle state as well as browser runtime state. The component records the imported scene/options key and the parameter scene key/id it replaced, prevents stale replaced parameters from issuing a JS update on the next render, and clears the external-import guard once a genuinely new parameter payload takes over. Parent-owned parameter state is not mutated outside the component.

## Required proof artifacts

- `transcripts/failing-first.txt`
- `transcripts/passing-tests.txt`
- `transcripts/source-assertions.txt`
- `transcripts/boundary-audit.txt`
- `transcripts/browser-proof.txt`
- `changed-file-hashes.md`
- `browser/run-playback-after-import-step-rerender.png`

## Results

- Failing-first proof: `transcripts/failing-first.txt` records the stale parameter update calling `CanDoItAll.webglScene.update` before the lifecycle fix.
- Passing tests: `transcripts/passing-tests.txt` records the focused lifecycle test, full WebGlLib tests, and WebGlRun browser adapter tests passing.
- Browser proof: `transcripts/browser-proof.txt` records real `/run-playback` import, frame apply, re-render, diagnostics, and screenshot proof.
- Source assertions: `transcripts/source-assertions.txt` proves all public external import methods mark successful imports in component lifecycle state and that run-layer browser reset uses the document-detailed import path.
- Boundary audit: `transcripts/boundary-audit.txt` proves no Economy dependency or vocabulary entered Components source.
- Validator audits: `transcripts/validator-audits.txt` records the prepared-stage bundle validator and proof-integrity audit passing after SB10 proof was added.
- Changed hashes: `changed-file-hashes.md` records hashes for SB10 source, test, docs, and browser screenshot.

## Production Behavior Artifact Matrix

| Behavior | Producer | Consumer | Lifecycle | Negative or parity proof |
| --- | --- | --- | --- | --- |
| External scene import lifecycle key | `WebGlSceneView.MarkExternalImportApplied` | `OnParametersSet`, `OnAfterRenderAsync`, public import methods | Successful import records the imported scene/options key, updates `pendingSceneKey` and `appliedSceneKey`, and remembers the replaced parameter key/id | `transcripts/failing-first.txt` and `transcripts/passing-tests.txt` |
| Stale parameter overwrite guard | `WebGlSceneView.ShouldKeepExternalImport` | Blazor parameter/render lifecycle | Replaced parameter key or scene id is treated as stale after import; imported runtime scene is preserved until the parent sends the imported payload or a genuinely new scene id | `transcripts/passing-tests.txt` |
| Browser replay import persistence | `WebGlSceneViewBrowserRuntime.ImportSceneAsync` -> `ImportSceneDocumentDetailedAsync` | WebGlRun browser adapter and sandbox run playback | Browser reset imports a document scene, applies a frame, then survives a Snapshot-triggered re-render with frame/object state intact | `transcripts/browser-proof.txt`, `browser/run-playback-after-import-step-rerender.png` |

## Refactor Gate

- Changed Components files: `WebGlSceneView.razor`, `WebGlSceneViewExternalImportLifecycleTests.cs`, and `src/CanDoItAll.Components.WebGlLib/README.md`.
- Changed Economy files: none.
- Public API changed: no signatures changed. Behavioral contract clarified: successful imports update component-local lifecycle state; parent-owned parameters remain external, and callers that want parameter state to reflect import should still sync their own model.
- Test/build/audit commands: see `transcripts/failing-first.txt`, `transcripts/passing-tests.txt`, `transcripts/source-assertions.txt`, `transcripts/boundary-audit.txt`, `transcripts/browser-proof.txt`, and `transcripts/validator-audits.txt`.
- Proof artifact paths: this manifest, `semantic-invariants.md`, `changed-file-hashes.md`, `transcripts/`, and `browser/run-playback-after-import-step-rerender.png`.
- Open risks: no known SB10 gap. The stale-parameter guard intentionally treats the replaced scene id as stale after import; a caller that intentionally wants to restore the exact replaced scene id should send a distinct new scene id or call an import method explicitly.

## Completion rules

This manifest cannot be marked completed unless all required proof files are non-empty and cite the command, result, and semantic assertion.
