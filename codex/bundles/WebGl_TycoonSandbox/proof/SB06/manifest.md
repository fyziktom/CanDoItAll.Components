# SB06 Proof Manifest

## Scope

Additive `WebGlSceneView`, scene CSS, generated asset includes, and separate JS namespace `window.CanDoItAll.webglScene`.

## Changed Source

- `repo://src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/01-webgl-scene.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/03-webgl-scene-assets.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/04-webgl-scene-symbols.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/05-webgl-scene-interaction.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/06-webgl-scene-camera.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/07-webgl-scene-overlays.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js`
- `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/css/scene/webgl-scene.css`
- `repo://tools/webgllib/asset-manifest.json`
- `repo://tools/webgllib/build-assets.cjs`
- `repo://tools/webgllib/verify-assets.cjs`
- `repo://src/CanDoItAll.Components.WebGlLib/Components/Shared/Assets/WebGlLibBodyAssets.razor`
- `repo://src/CanDoItAll.Components.WebGlLib/Components/Shared/Assets/WebGlLibHeadAssets.razor`
- Hashes: `bundle://proof/SB09/transcripts/changed-file-hashes.json`

## Command Proof

- failing-first baseline: `repo://artifacts/webgl-symbolic-tycoon-sandbox/01_INVENTORY.md` records only `window.CanDoItAll.webglWorkbench` before this subbundle.
- passing JS syntax proof: `bundle://proof/SB06/transcripts/webgl-scene-js-syntax.txt`
- passing asset verification: `bundle://proof/SB09/transcripts/npm-webgllib-verify-assets.txt`
- passing build proof: `bundle://proof/SB06/transcripts/webgllib-runtime-build.txt`
- passing browser proof: `bundle://proof/SB08/browser/webgl-tycoon-village-final-proof.json`
- anti-stub audit: `bundle://proof/SB09/transcripts/anti-stub-audit.txt`

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| `window.CanDoItAll.webglScene` runtime | `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/01-webgl-scene.js` | `repo://src/CanDoItAll.Components.WebGlLib/Components/Scene/WebGlSceneView.razor` | Body assets load module, Blazor invokes create/update/dispose, runtime renders canvas and emits callbacks | `bundle://proof/SB08/browser/webgl-tycoon-village-final-proof.json` confirms both scene and workbench namespaces exist and canvas exports non-empty image data. |
| Selection and hover events | Scene runtime interaction module | `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/TycoonVillage.razor` | Browser pointer events hit-test objects, JS invokes .NET callbacks, inspector updates | `bundle://proof/SB08/browser/webgl-tycoon-village-final-proof.json` records selected and hovered `building.house-b`. |

