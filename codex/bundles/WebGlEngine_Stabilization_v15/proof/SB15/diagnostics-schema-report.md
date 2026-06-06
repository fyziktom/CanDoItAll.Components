# SB15 Diagnostics Schema Report

Stable diagnostics surfaces proved:

- `WebGlRuntimeDiagnostics`
- `WebGlRuntimeIdleResult`
- `WebGlSceneProofSnapshot`
- `WebGlRunRuntimeSnapshot`
- `WebGlRunObserverProofReport`

Validation:

- WebGlLib diagnostics tests passed.
- WebGlRunLib observer proof validation passed.
- Browser observer JSON recorded runtime idle, command batch, proof snapshot, hashes, completed stage ids, and final object positions.

Artifacts:

- `proof/SB16/browser/browser-observer-proof.json`
- `artifacts/webgl-engine-rc-v15/artifact-manifest.json`
