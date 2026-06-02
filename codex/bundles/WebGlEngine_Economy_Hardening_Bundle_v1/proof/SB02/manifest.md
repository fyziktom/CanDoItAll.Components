# SB02 Proof Manifest

Subbundle: `SB02-webgllib-js-runtime-correctness`
Status: `Completed`
Prepared by: Codex
Completed at UTC: `2026-06-02T00:04:00Z`

## Changed File Hashes

| Repo | Path | Before SHA-256 | After SHA-256 | Why changed |
| --- | --- | --- | --- | --- |
| CanDoItAll.Components | `repo://CanDoItAll.Components/package.json` | `0a9a15088632f35eb7794c42116ec203836f18a1cfa67420c6fccdb170122ddc` | `dc1262ffb12d7f9f9291ce31f57b9dc08b2196ecb1a67680189ba28e8a3b2bab` | Added CI-ready `webgllib:audit-scene-runtime-imports` script. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md` | `9b24c1b9c4c462ccc3d94a1a5145de9f1f04be50aefe5ba981a3b603e1e7e0a0` | `e5509b6a30c5a5ed7d633bdb82ac6c9cbdfd0e33ce92f3117209f3d64f1facab` | Documented the new import/export runtime audit. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/11-webgl-scene-graph.js` | `56200ce156ce064c78aec7f21f2bb5d814b02f4bd9e44b0464bd40a5de2759ba` | `52f10af04a2d64a0b3d9f7d4aee2acfbb1dad59f98113bf0fb5a95fa43f90f11` | Imported `resolveObjectPosition` used by transform-only runtime updates. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/tools/webgllib/audit-scene-runtime-imports.cjs` | `NEW` | `dd5b6a3b434412958ec5a62622f4b5418b3f37957f40b13e220d7bb320f5f301` | Added static scene-runtime import/export, unimported symbol, registration, and forbidden-boundary audit. |

## Commands

| Command | Working directory | Transcript path | Result |
| --- | --- | --- | --- |
| `npm run webgllib:audit-scene-runtime-imports` before import fix | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB02/transcripts/failing-first-audit-scene-runtime-imports.txt` | Expected failure; audit rejected unimported `resolveObjectPosition` in `11-webgl-scene-graph.js`. |
| `node tools/webgllib/audit-scene-runtime-imports.cjs --self-test` | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB02/transcripts/audit-scene-runtime-imports-self-test.txt` | Pass; self-test rejects both missing exported imports and unimported scene symbols. |
| `npm run webgllib:audit-scene-runtime-imports` after fix | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB02/transcripts/passing-audit-scene-runtime-imports.txt` | Pass; 33 scene runtime modules audited. |
| `npm run webgllib:audit-scene-runtime` | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB02/transcripts/passing-audit-scene-runtime.txt` | Pass with existing runtime audit warnings. |
| `dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj` | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB02/transcripts/webglsandbox-build.txt` | Pass; 0 warnings, 0 errors. |
| Browser proof on `/tycoon-village` | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB02/transcripts/browser-tycoon-village-runtime-proof.json` | Pass; scene created, real mouse drag moved `agent.runner`, transform patch succeeded, dispose removed state/canvas, console/page errors clean. |
| `rg` anti-stub and boundary scans | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB02/transcripts/sb02-anti-stub-and-boundary-scan.txt` | Pass; no TODO/FIXME/NotImplemented in touched files and no forbidden domain matches in production runtime/package files. |

## Proof Artifact Hashes

| Artifact | SHA-256 |
| --- | --- |
| `bundle://proof/SB02/transcripts/failing-first-audit-scene-runtime-imports.txt` | `8f2b42ef6a83f057c0f74c3394b8294b310f9dc2148480d2f242f8d50f832333` |
| `bundle://proof/SB02/transcripts/audit-scene-runtime-imports-self-test.txt` | `27f555fc40e7c26eb22e813180191e67ff33e73bbf972170adc3ecb7226a27b9` |
| `bundle://proof/SB02/transcripts/passing-audit-scene-runtime-imports.txt` | `23aa51020c7984b4eaaad2f58da50ee5c6d013b2a88e015bfa222180467ab98a` |
| `bundle://proof/SB02/transcripts/passing-audit-scene-runtime.txt` | `2473ce9040a2d1890db174d94d87c786337a58151b8051722b0d542330f85899` |
| `bundle://proof/SB02/transcripts/webglsandbox-build.txt` | `1703ee7fe3fb6e63b1e3b546f04cefc4fb0fd68e984fa62014719ec2d23321b9` |
| `bundle://proof/SB02/transcripts/browser-tycoon-village-runtime-proof.json` | `ee15cce66fb0ee49406943a9ba64c7215240e7bd4336c85c0080b8f07719fa15` |
| `bundle://proof/SB02/browser/tycoon-village-sb02.png` | `2936c27245c752b3bc05d1a3c1e693d63f604841ecfd794af1f7d2421e784e4b` |
| `bundle://proof/SB02/transcripts/sb02-anti-stub-and-boundary-scan.txt` | `6398f406192b371ff5074a95071ae2ad6a2da7010b050b156f3f8c212b9f3be9` |
| `bundle://proof/SB02/refactor-gate.md` | `450e829b4e214d505d2833dc8eadc4d4e277a713c6300a89b7747ff96dbb7560` |

## Source Assertions

| Assertion | Source path | Line/range or search term | Verified by |
| --- | --- | --- | --- |
| `resolveObjectPosition` is imported where `updateObjectRuntimeTransform` uses it. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/11-webgl-scene-graph.js` | `resolveObjectPosition` | `bundle://proof/SB02/transcripts/sb02-anti-stub-and-boundary-scan.txt` |
| Static audit rejects scene runtime imports that name missing exports. | `repo://CanDoItAll.Components/tools/webgllib/audit-scene-runtime-imports.cjs` | `collectExports`, `collectImports`, self-test | `bundle://proof/SB02/transcripts/audit-scene-runtime-imports-self-test.txt` |
| Static audit rejects scene symbols used without importing the owning module. | `repo://CanDoItAll.Components/tools/webgllib/audit-scene-runtime-imports.cjs` | `checkUnimportedSceneSymbols` | `bundle://proof/SB02/transcripts/failing-first-audit-scene-runtime-imports.txt` |
| Static audit enforces one global scene registration and forbids WebGlRunLib/domain dependencies from scene runtime modules. | `repo://CanDoItAll.Components/tools/webgllib/audit-scene-runtime-imports.cjs` | `checkRootRegistration`, `checkForbiddenRuntimeDependencies` | `bundle://proof/SB02/transcripts/passing-audit-scene-runtime-imports.txt`, `bundle://proof/SB02/transcripts/sb02-anti-stub-and-boundary-scan.txt` |
| The new audit is documented and scriptable. | `repo://CanDoItAll.Components/package.json`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md` | `webgllib:audit-scene-runtime-imports` | `bundle://proof/SB02/transcripts/sb02-anti-stub-and-boundary-scan.txt` |

## Semantic Adequacy Gate

| Item | Required evidence | Status | Artifact |
| --- | --- | --- | --- |
| Shallow-pass trap named | A syntax-only import scan could pass even while a module uses a scene symbol from another runtime file without importing it. | Pass | `bundle://proof/SB02/semantic-invariants.md` |
| Adversarial negative proof | Bad case fails for the right reason. | Pass | `bundle://proof/SB02/transcripts/failing-first-audit-scene-runtime-imports.txt`, `bundle://proof/SB02/transcripts/audit-scene-runtime-imports-self-test.txt` |
| Semantic positive proof | Real intended behavior passes. | Pass | `bundle://proof/SB02/transcripts/passing-audit-scene-runtime-imports.txt`, `bundle://proof/SB02/transcripts/browser-tycoon-village-runtime-proof.json` |
| Anti-stub audit | TODO/NotImplemented/fixture-only scan. | Pass | `bundle://proof/SB02/transcripts/sb02-anti-stub-and-boundary-scan.txt` |
| Raw-note closure | REQ-002 and SB02 share of REQ-015 closed literally. | Pass | This manifest; `bundle://reviews/01-execution-report.md` |
| Downstream smoke | Dependent sandbox path still works. | Pass | `bundle://proof/SB02/transcripts/webglsandbox-build.txt`, `bundle://proof/SB02/transcripts/browser-tycoon-village-runtime-proof.json` |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Scene runtime import/export audit | `tools/webgllib/audit-scene-runtime-imports.cjs` | npm/CI/manual validation | Runs over `wwwroot/js/runtime/scene/*.js`; writes an ignored JSON report and exits nonzero on failures. | Failing-first transcript rejects unimported `resolveObjectPosition`; self-test rejects missing exported imports and unimported scene symbols. |
| Runtime `resolveObjectPosition` import | `11-webgl-scene-graph.js` | Browser runtime transform update path | Imported from `02-webgl-scene-core.js` when the module loads; used during transform-only updates. | Failing-first audit identified the missing import before the fix. |
| Browser runtime smoke artifact | `/tycoon-village` sandbox route | SB02/SB13 proof readers | Route creates the scene, accepts real mouse drag, applies transform-only patch, emits diagnostics, captures screenshot, then disposes state/canvas. | Console/page-error lists are empty after filtering normal Blazor info messages. |

## Browser Validation Analytics

| Route | Viewport | Actions | Screenshot/log artifacts | Result |
| --- | --- | --- | --- | --- |
| `http://127.0.0.1:5278/tycoon-village` | 1440x1000 | Created scene, selected `agent.runner`, dragged it with real Playwright mouse input, applied `applyPatchDetailed` transform-only patch, captured diagnostics/snapshot, disposed runtime state/canvas, navigated away. | `bundle://proof/SB02/transcripts/browser-tycoon-village-runtime-proof.json`, `bundle://proof/SB02/browser/tycoon-village-sb02.png` | Pass; object moved from `(1.2,0,-1.6)` to `(1.05,0,2.19)`, patch affected `agent.runner`, `replacedObjectGroupCountDelta=0`, `pageErrors=[]`, filtered console messages empty. |

## Refactor Gate Result

- Touched files reviewed: `package.json`, `src/CanDoItAll.Components.WebGlLib/README.md`, `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/11-webgl-scene-graph.js`, `tools/webgllib/audit-scene-runtime-imports.cjs`.
- Duplicates removed: no duplicate import-audit paths introduced.
- Layering checked: production runtime/package files have zero forbidden domain matches; audit denylist terms are intentional.
- Fixture-specific code removed: self-test fixture is only behind `--self-test`.
- Docs/tests updated: npm script and WebGlLib README updated.
- Remaining refactor risk: low; SB03/SB04 own transaction and rebuild-performance semantics.
