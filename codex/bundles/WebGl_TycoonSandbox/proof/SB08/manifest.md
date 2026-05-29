# SB08 Proof Manifest

## Scope

Generic tycoon village demo scene and browser-visible validation.

## Changed Source

- `repo://src/CanDoItAll.Components.WebGlSandbox/WebGlSandboxVillageSceneFactory.cs`
- `repo://src/CanDoItAll.Components.WebGlSandbox/WebGlSandboxAssetCatalogFactory.cs`
- `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/TycoonVillage.razor`
- `repo://src/CanDoItAll.Components.WebGlSandbox/wwwroot/sandbox-webgl.css`
- Hashes: `bundle://proof/SB09/transcripts/changed-file-hashes.json`

## Browser Proof

- failing-first baseline: `repo://artifacts/webgl-symbolic-tycoon-sandbox/01_INVENTORY.md` records `CanDoItAll.Components.WebGlSandbox` was absent.
- passing desktop screenshot: `bundle://proof/SB08/browser/webgl-tycoon-village-final-desktop.png`
- passing mobile screenshot: `bundle://proof/SB08/browser/webgl-tycoon-village-final-mobile.png`
- passing symbol-visible screenshot: `bundle://proof/SB08/browser/webgl-tycoon-village-symbols-visible.png`
- passing interaction and snapshot proof: `bundle://proof/SB08/browser/webgl-tycoon-village-final-proof.json`
- anti-stub audit: `bundle://proof/SB09/transcripts/anti-stub-audit.txt`

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Village scene factory | `repo://src/CanDoItAll.Components.WebGlSandbox/WebGlSandboxVillageSceneFactory.cs` | `/tycoon-village` Blazor page | Creates scene model, passes it to `WebGlSceneView`, renders in runtime | `bundle://proof/SB08/browser/webgl-tycoon-village-final-proof.json` proves production route counts, not a manually seeded test object. |
| Proof snapshot panel | Runtime proof module | `repo://src/CanDoItAll.Components.WebGlSandbox/Components/Pages/TycoonVillage.razor` | Snapshot button requests runtime snapshot and displays object/symbol/asset/fallback counts | Browser proof records the runtime snapshot and UI text values. |

