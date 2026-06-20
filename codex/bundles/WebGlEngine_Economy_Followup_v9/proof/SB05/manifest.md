# Proof manifest - SB05

Status: completed

## Scope

SB05 replaces the self-referential RunPlayback observer proof with artifact-backed expected-vs-browser evidence. The sandbox now compares `runDocument` to a distinct browser-loaded document, exports deterministic document/scene/proof-snapshot hashes, records runtime idle diagnostics, and proves completed stage ids, final object positions, and browser console cleanliness from a real Playwright browser run.

## Changed files

Changed-file hashes:

- `bundle://proof/SB05/transcripts/changed-file-hashes.txt`

Production files:

- `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/RunPlayback.razor.cs`
- `repo://src/CanDoItAll.Components.WebGlSandbox/Program.cs`

Test/proof files:

- `bundle://proof/SB05/browser/observer-proof-real-browser-state.mjs`
- `bundle://proof/SB05/semantic-invariants.md`

## Proof artifacts

- Sandbox build proof: `bundle://proof/SB05/transcripts/webglsandbox-build.txt`
- Browser proof transcript: `bundle://proof/SB05/transcripts/observer-proof-playwright.txt`
- Observer report JSON: `bundle://proof/SB05/browser/observer-proof.json`
- Browser assertions/diagnostics JSON: `bundle://proof/SB05/browser/observer-proof-assertions.json`
- Browser screenshot: `bundle://proof/SB05/browser/observer-proof-after.png`
- Browser console log: `bundle://proof/SB05/browser/observer-proof-console.log`
- Browser progress log: `bundle://proof/SB05/browser/observer-proof-progress.log`
- Source assertions: `bundle://proof/SB05/transcripts/source-assertions.txt`
- Anti-stub scan: `bundle://proof/SB05/transcripts/anti-stub-scan.txt`
- Bundle validator transcript: `bundle://proof/SB05/transcripts/bundle-validator.txt`
- Sandbox server logs: `bundle://proof/SB05/transcripts/webgl-sandbox-sb05.out.txt`, `bundle://proof/SB05/transcripts/webgl-sandbox-sb05.err.txt`

## Semantic adequacy gate

- Shallow-pass trap: `WebGlRunObserverProof.Compare(runDocument, runDocument, ...)` could pass without proving the browser loaded the intended document or reached the expected final state.
- Source proof: `source-assertions.txt` verifies the self-compare call is absent, the browser-loaded document path is used, the proof snapshot hash is exported, and the capture path avoids a fragile full-scene browser export.
- Browser proof: `observer-proof-real-browser-state.mjs` drives `/run-playback` through the registered browser bridge, captures a screenshot and console log, and asserts document hashes match, browser document loaded, observer proof valid, runtime idle true with no blockers, completed stages observed, final positions match by object id/vector value, and no disallowed console errors.
- Hash evidence: `observer-proof-assertions.json` records matching expected/browser document hash `sha256:5236a9b9538dbabd056b81d2aa06c55af6116f10902d2b2fe9f5c52f104e2e17`, scene hash `sha256:da1d41d0a712a80f0b9240c1da7481603d9873676a0ee846a9b73b0cdad3e503`, and browser proof snapshot hash `sha256:4b89cd7593be4f916a8e21d7124d6465d9453ef6157086402c6f77551d86a14e`.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative / proof citation |
| --- | --- | --- | --- | --- |
| Distinct browser-loaded run document | `RunPlayback.CaptureBrowserLoadedDocumentAsync` | `WebGlRunObserverProof.Compare` | Created only after browser import succeeds; compared against expected `runDocument` instead of reusing the same object. | `bundle://proof/SB05/transcripts/source-assertions.txt`, `bundle://proof/SB05/browser/observer-proof-assertions.json` |
| Browser hash diagnostics | `RunPlayback.BuildObserverHashDiagnostics` | Browser proof script and diagnostics panel | Exposes expected/browser document hashes, scene content hash, proof snapshot hash, and loaded scene id. | `bundle://proof/SB05/browser/observer-proof-assertions.json` |
| Runtime observer evidence | `RunPlayback.BuildObserverSnapshot` | `WebGlRunObserverProof.Compare` | Carries runtime diagnostics, idle result, completed stage ids, final object positions, and metadata from the real browser run. | `bundle://proof/SB05/browser/observer-proof.json` |
| Development-only detailed circuit errors | `Program.cs` | Sandbox debugging | Enables detailed Blazor Server errors only in Development to prevent silent browser proof disconnects during local proof runs. | `bundle://proof/SB05/transcripts/source-assertions.txt` |

## Closure

SB05 passes. Browser proof route: `http://localhost:5298/run-playback`, requested viewport `1920x1080`, observed RunPlayback final frame `3`, completed stages `run.move.target`, `run.pose.work`, `run.return.anchor`, `run.symbol.hide`, `run.pose.restore`, and `run.symbol.show`; runtime idle was true with blockers `[]`; final object positions matched expected vectors; no disallowed console errors were recorded.
