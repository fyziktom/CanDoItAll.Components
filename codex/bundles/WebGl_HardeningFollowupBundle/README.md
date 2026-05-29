# CanDoItAll.Components WebGL hardening follow-up bundle v1

Tento bundle navazuje na větev:

```text
codex/webgl-symbolic-tycoon-sandbox
```

Cíl není přidávat ekonomickou doménu. Cíl je zpevnit obecný `WebGlLib`, doplnit asset varianty včetně dříve stažených GLB modelů, refaktorovat runtime/sandbox tam, kde Codex vytvořil příliš dlouhé nebo málo rozdělené soubory, a připravit čisté rozhraní pro budoucí druhou fázi: obecný "run" / light-game layer nad WebGL wrapperem.

Nejdůležitější soubor pro Codex:

```text
01_codex_master_prompt.md
```

Rychlá copy-paste verze:

```text
10_single_file_execution_prompt.md
```

## Shrnutí review

Pozitiva:
- Implementace je správně additive vedle starého `WebGlWorkbench`.
- Přibyl samostatný `CanDoItAll.Components.WebGlSandbox` bez dependency na procesy.
- Existuje obecný `WebGlSceneModel`, asset catalog, symboly nad objekty, hover/selection a proof snapshot.
- Browser proof prošel a report uvádí 20 objektů, 9 symbolů, 4 načtené GLB assety a 12 fallback objektů.

Hlavní mezery:
- Stažené alternativní modely nejsou zapojené jako asset variants / asset packs.
- `AllowDragOnGroundPlane` a `ObjectsMoved` kontrakty existují, ale runtime zatím neumí reálný drag/move.
- Runtime při update dělá full rebuild scény; pro run/timeline bude potřeba patch/diff API.
- Render loop běží pořád; pro větší scény je potřeba režim on-demand vs continuous.
- Asset variants DTO existuje, ale runtime je zatím nevyužívá.
- Některé soubory jsou už na hraně udržitelnosti (`01-webgl-scene.js`, `03-webgl-scene-assets.js`, sandbox CSS/page/factory).
- Error propagation při create failure není dostatečná: JS vrátí `false`, ale Blazor komponenta z toho neudělá `RuntimeError`.
- Chybí základní unit tests pro validators/policies a Playwright smoke pro `WebGlSceneView`.

## Doporučená strategie

Do `WebGlLib` patří:
- declarative scene DTO,
- asset catalog + variants + performance metadata,
- GLB loading/fallback/LOD selection,
- selection/hover/drag picking,
- scene patch/command API,
- transform interpolation primitive,
- camera and overlay bridge,
- deterministic proof snapshots.

Do budoucího `WebGlRunLib` nebo doménového run layeru patří:
- simulation clock,
- scenario/run lifecycle,
- world model semantics,
- persistence adapters,
- path planning,
- physics/collision,
- economy/game-specific rules,
- domain-specific symbol policies.

Tento bundle záměrně řeší jen obecné hardening kroky a run-readiness primitives.
