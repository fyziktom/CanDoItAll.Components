# 00 - Review findings

## Reviewed branch

```text
repo: fyziktom/CanDoItAll.Components
base: main
head: codex/webgl-symbolic-tycoon-sandbox
```

`compare_commits` ukázal, že branch je o 2 commity před `main` a přidává nový `CanDoItAll.Components.WebGlSandbox`, nové scene DTO, asset/symbol/interaction/interop kontrakty a samostatnou scene JS runtime vrstvu.

## Co Codex udělal dobře

1. **Additive architecture**
   - `WebGlSceneView` byl přidán vedle existujícího `WebGlWorkbench`.
   - Runtime namespace je separátní: `window.CanDoItAll.webglScene`.
   - `WebGlWorkbench` namespace má zůstat zachovaný.

2. **Domain-neutral contracts**
   - `WebGlSceneModel` obsahuje scene id, asset catalog, environment, camera, ui state, interaction, layers, objects and links.
   - `WebGlSceneObject` je obecný a neobsahuje economy/process-specific pojmy.
   - `WebGlStatusSymbol` je obecný: `SemanticKind`, `Intensity`, `Color`, `Scale`, `HeightOffset`, `EffectKey`.

3. **GLB + fallback strategy**
   - Runtime umí GLB/GLTF load přes `GLTFLoader`.
   - Pokud model chybí nebo selže, renderuje primitive fallback.
   - Sandbox report uvádí 4 loaded GLB assets a 12 fallback objects.

4. **Standalone sandbox**
   - `CanDoItAll.Components.WebGlSandbox` je v solution.
   - Nezávisí na `CanDoItAll.Modules.Processes`.

## Hlavní hardening nálezy

### HF-01: Stažené modely nejsou využité jako asset variants

Sandbox catalog explicitně používá jen:

```text
1gears.glb
gears.glb
lowpoly_person_boxing.glb
question_box.glb
```

Domy a stromy jsou primitives. To je dobrý fallback, ale teď potřebujeme přidat stažené modely jako alternativní asset pack / variants, aby šlo porovnat výkon a vizuál.

### HF-02: `WebGlAssetVariant` existuje, ale runtime ho nepoužívá

DTO má `Id`, `DisplayName`, `Uri`, `Color`, `Scale`, `Metadata`, ale `syncAssetVisual` resolvuje pouze base asset `uri`. Chybí `PreferredVariantId`, `QualityTier`, `MaxTriangleCount`, runtime selector variant a proof snapshot metriky.

### HF-03: Drag/move kontrakty jsou nedokončené

`WebGlInteractionOptions` obsahuje `AllowDragOnGroundPlane` a `WebGlSceneView` deklaruje `ObjectsMoved`, ale JS interaction vrstva zatím řeší hover, click selection a double-click focus. Nevolá `OnObjectsMoved`.

### HF-04: Static scene update dělá full rebuild

`updateState` volá `rebuildScene(state)` při každé změně. To je v pořádku pro první proof, ale pro run/timeline nesmí každý tick rebuildovat celou scénu.

### HF-05: Render loop běží pořád

`startRenderLoop` volá `render(state)` každý animation frame. To je OK pro symbol effects, ale pro větší tycoon-like scénu musí být možné přepínat:
- continuous,
- on-demand,
- continuous only while animations/run playback active.

### HF-06: Create failure nepropaguje RuntimeError dostatečně

`webglScene.create` při exception vrací `false`, ale `WebGlSceneView.OnAfterRenderAsync` jen neuloží `appliedSceneKey` a skončí. Mělo by to explicitně vyvolat `RuntimeError`.

### HF-07: Některé soubory už potřebují refaktor

Aktuální velikosti nejsou katastrofální, ale je vhodné nastavit limit dřív, než se runtime rozroste:

```text
01-webgl-scene.js              563 lines
03-webgl-scene-assets.js       362 lines
sandbox-webgl.css              396 lines
TycoonVillage.razor            244 lines
WebGlSandboxVillageSceneFactory.cs 221 lines
```

### HF-08: Chybí persistent/export/import scene state

Existuje proof snapshot, ale ne scene state export pro pozdější uložení layoutu a jeho obnovu.

### HF-09: Chybí run/timeline boundary

Potřebujeme rozhodnout, co patří do `WebGlLib` a co do budoucího `WebGlRunLib`.

Doporučení:
- `WebGlLib`: rendering, scene commands, transform tween primitive, patch API.
- `WebGlRunLib`: timeline, run clock, simulation events, persistence adapter, domain-neutral scenario playback.

### HF-10: Test coverage je spíše browser proof, ne unit testy

Doplnit:
- unit tests for asset catalog validation,
- symbol policy normalization tests,
- scene patch reducer tests,
- Playwright smoke tests for sandbox route,
- regression guard that `window.CanDoItAll.webglWorkbench` still exists.
