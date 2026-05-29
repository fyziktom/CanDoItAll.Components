# Single-file Execution Prompt

You are working in `C:\repositories\CanDoItAll.Components`.

Do not create a new branch. Work in the branch that is already checked out. At start run:

```powershell
git branch --show-current
git status --short
```

Do not run `git checkout -b`, `git switch -c`, or any equivalent. If the current branch is unexpected, stop and report.

Perform a second hardening pass on the generic WebGL scene runtime and WebGL sandbox.

Focus areas:

1. Add `tools/webgllib/audit-scene-runtime.cjs` and `npm run webgllib:audit-scene-runtime`.
2. Refactor JS scene runtime files if any are too broad, especially lifecycle/state/shell/notification/disposal responsibilities.
3. Fix resource ownership and disposal for GLB templates, cloned materials, primitive fallbacks, symbols, links, decorations, and repeated create/dispose cycles.
4. Add model import diagnostics and import normalization options for invisible or badly converted GLB files.
5. Split large sandbox asset catalog files by responsibility.
6. Add or refine a model lab proof page for inspecting individual assets/variants and diagnostics.
7. Improve command results for patch/import/motion and align JS patch result semantics with C# reducer semantics.
8. Add a generic `WebGlSceneDocument` serialization contract, but do not build a run engine.
9. Improve render scheduling so idle scenes sleep instead of running an endless RAF loop.
10. Add tests and browser proof evidence under `artifacts/webgl-runtime-hardening-v2/`.

Keep `WebGlLib` domain-neutral. Do not add economy, process, agent, game, pathfinding, physics, replay, run lifecycle, or storage-provider semantics.

Validation:

```powershell
npm install
npm run webgllib:build-assets
npm run webgllib:verify-assets
npm run webgllib:audit-scene-runtime
dotnet build src/CanDoItAll.Components.WebGlLib/CanDoItAll.Components.WebGlLib.csproj
dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj
dotnet build CanDoItAll.Components.slnx
dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj
```

Browser proof must confirm `/tycoon-village`, primitive fallback, model diagnostics, motion, patch/export/import, idle render scheduler, and that `window.CanDoItAll.webglWorkbench` still exists.

All source code comments must be in English.

