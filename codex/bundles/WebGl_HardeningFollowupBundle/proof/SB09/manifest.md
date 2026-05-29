# SB09 proof manifest

Large-screen-only hard rule applied; no small-screen screenshots or tuning were produced.

## Production Behavior Artifact Matrix

| Behavior | Producer | Consumer | Evidence |
| --- | --- | --- | --- |
| Asset profiles and variants | `WebGlSandboxAssetCatalogFactory`, JS variant resolver | `WebGlSceneView` runtime | `artifacts/webgl-scene-hardening/browser-summary.json` |
| Drag/move callback | `12-webgl-scene-drag.js` | `TycoonVillage.razor.cs` | `browser-summary.json` phase `drag` |
| Patch/export/import | `13-webgl-scene-patching.js`, `WebGlSceneView` methods | Sandbox proof actions | `browser-summary.json` phases `export-import`, `missing-asset-fallback` |
| Motion primitive | `14-webgl-scene-motion.js` | WebGlSceneView API | `browser-summary.json` phases `motion-active`, `motion-complete` |
| Namespace regression | generated WebGlLib body assets | Browser page | `browser-summary.json` root flags |

## Artifacts

- `artifacts/webgl-scene-hardening/browser-summary.json`
- `artifacts/webgl-scene-hardening/browser-final-proof.json`
- `artifacts/webgl-scene-hardening/browser-final-canvas.png`
- `artifacts/webgl-scene-hardening/browser-console.log`
- `proof/SB09/transcripts`
