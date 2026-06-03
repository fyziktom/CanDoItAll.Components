# SB10 semantic invariants

- The implementation must preserve Components genericity.
- The implementation must not introduce Economy semantics into WebGlLib or WebGlRunLib.
- Runtime proof must demonstrate behavior, not only compilation.
- Required proof artifacts must be non-empty.

## Production Behavior Artifact Matrix

| Behavior | Producer | Consumer | Lifecycle | Negative or parity proof |
| --- | --- | --- | --- | --- |
| External scene import lifecycle key | `WebGlSceneView.MarkExternalImportApplied` | `OnParametersSet`, `OnAfterRenderAsync`, public import methods | Successful import updates component-local `pendingSceneKey` and `appliedSceneKey` to the imported scene/options key | `transcripts/failing-first.txt`, `transcripts/passing-tests.txt` |
| Stale parameter overwrite guard | `WebGlSceneView.ShouldKeepExternalImport` | Blazor render lifecycle | Replaced parameter key or scene id cannot overwrite the runtime-imported scene during a later stale render | `transcripts/passing-tests.txt` |
| Browser import/apply/re-render persistence | Run playback sandbox through `WebGlSceneViewBrowserRuntime` | Browser runtime, WebGlRun browser adapter | Import scene document, apply frame, trigger re-render, and preserve frame/object state | `transcripts/browser-proof.txt`, `browser/run-playback-after-import-step-rerender.png` |

## Completion assertions

- Successful `ImportSceneAsync`, `ImportSceneDetailedAsync`, `ImportSceneDocumentAsync`, and `ImportSceneDocumentDetailedAsync` calls mark imported scene/options lifecycle state.
- A stale parent parameter update with the replaced scene id no longer calls `CanDoItAll.webglScene.update`.
- A genuinely new scene id still updates through the normal parameter-driven render path.
- Real browser proof shows document import, frame apply, and re-render preserve frame 1 and 26 visible objects.
