# SB05 Proof Manifest

Subbundle: `SB05-resource-ownership-asset-cache`
Status: `Completed`
Prepared by: Codex
Completed at UTC: `2026-06-02T02:26:23Z`

## Changed File Hashes

| Repo | Path | Before SHA-256 | After SHA-256 | Why changed |
| --- | --- | --- | --- | --- |
| CanDoItAll.Components | `repo://CanDoItAll.Components/package.json` | `dc1262ffb12d7f9f9291ce31f57b9dc08b2196ecb1a67680189ba28e8a3b2bab` | `8c9cc166d4cec0fa5c5926cd96e01d70b748a99c0859dc231738721a5cb4cbc6` | Added CI-ready resource ownership test script. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md` | `21029a42366f59cbe0646baf3f9aaf8499adc0c5fdea81fe43e8b67973bada69` | `50b5ec0c58f5f84b66921e8f34068e84895fcb200ab7d258cd89b9d5489e0bcf` | Documented state-local asset cache policy, texture ownership split, and resource diagnostics counters. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs` | `c630447877610c86d1ecb5200a4cb48862093f9ad0c7f8525181242c43d1d44e` | `5f01b310a2077598e5b5acd2ee07bc41d85967d6591cea8958c068faa9d3e12a` | Added typed runtime diagnostics for cache mode and retained shared textures. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneProofSnapshot.cs` | `8232a63aca5d97ec5aefc74464f12dcde9e66b0a2b8fe0bed198de22993304a9` | `510a7790de953e3235864985c08cddfe1c579825347ab3d2189dc25216cb3790` | Added proof snapshot fields for cache mode and retained shared textures. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js` | `5d9bac417fe6dc31593f7ecf3214bca84be3422aac56692f3dbdd5085edfdba7` | `a2ad5306f8f41b140ea863dc72a70187df91f26f1ad9617ab4147d66366567d1` | Exposed cache mode and retained shared texture count in diagnostics snapshots. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js` | `7b16fb915f4799b7d419eac57bbe578317649eb485a2c7dbeaefd88b2ff3782d` | `1ea4a1f6425af4de06e000f8a31ddc708eca5be0feb1fb55c57ee7874728aa15` | Exposed cache mode and retained shared texture count in proof snapshots and metadata. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/16-webgl-scene-models.js` | `6167f37e7a9312d7c05aad92981a474c657aa4dd68b9593cfac68283604dc7a7` | `c308d5f01d786cee70dfa66a2c348bad459dffed321a47779bfba68fdb5a733a` | Marked cached templates as resource owners and cloned/tinted model instances as material-owned but texture-shared. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/17-webgl-scene-resources.js` | `44debc4f49766f56d314518fc310a14dbb5dc0d0819363340facb1942876b637` | `4861091e144fb080699e9671a6bdac8dc824b7f46457768f6b0e9730ef33c0da` | Split geometry, material, and texture ownership; retained shared textures; de-duped template resource disposal. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/20-webgl-scene-command-results.js` | `f48d67a7b5933e5546ca2f4da6b12344e9f5f2292e07620fd2f872c826640725` | `ad2a93c2c074bf5fbfc4b0343560d198b1ffb538225aa20bd607b74bf6035f2f` | Included resource/cache counters in command-result diagnostics. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/21-webgl-scene-asset-cache.js` | `966075d5c38c388889dea77c1d6b8a3e57696ecaf3ddc72e8a69ef2344fa75f0` | `f24d5939b11c0766a09d70d06b1a33aa821994acac72a503b8e2e331670b4e4a` | Passed diagnostics into cached template disposal and surfaced state-local cache mode. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/25-webgl-scene-diagnostics.js` | `92161e23111e68d019735b89e80631bf96031c62145b9aba0ed1245f4547dc76` | `a2703b350f02e6c12ac6c31d92e973e57b29cd3d8dcb4eb31e0b63935bd0b788` | Initialized cache mode and retained shared texture diagnostics. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlLib.Tests/WebGlRuntimeDiagnosticsTests.cs` | `efce187c59a7a900c82b69325cf9b64afda73beb7b30a5a9822d977693dc511e` | `3d4f476b2f80b1e1072c134bb53196fc4041c9201c532543670b4b0ebd4f7cc4` | Added DTO round-trip test for cache/resource diagnostics. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/tools/webgllib/test-resource-ownership.mjs` | `NEW` | `d912858243ac46d25e4cebe1369386c812edfe3c04e2fa6f6df1a7e489f52189` | Added focused JS proof for texture ownership, duplicate disposal de-dupe, and template cache disposal. |

## Commands

| Command | Working directory | Transcript path | Result |
| --- | --- | --- | --- |
| Inline JS resource ownership proof before fix | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB05/transcripts/failing-first-resource-ownership.json` | Expected failure; cloned material disposal also disposed shared texture and cache template disposal counted a template without disposing geometry/material/texture. |
| `npm run webgllib:test-resource-ownership` | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB05/transcripts/passing-resource-ownership-test.txt` | Pass; tinted instances retain shared texture maps, duplicate template resources dispose once, cache disposal releases template resources. |
| Browser textured cache/resource proof | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB05/transcripts/passing-browser-resource-cache-proof.json` | Pass; two textured GLB instances used one state-local cache entry, profile switches had no unexpected missing assets, one-instance removal retained shared textures, final dispose released template textures. |
| Browser console log capture | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB05/transcripts/browser-console-sb05-resource-cache.log` | Pass; Blazor info entries only, no errors/warnings. |
| `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj --filter "Diagnostics|Asset|Patch|Document|Revision"` | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB05/transcripts/passing-focused-diagnostics-asset-tests.txt` | Pass; 29 focused tests, 0 failures. |
| `npm run webgllib:audit-scene-runtime-imports` | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB05/transcripts/passing-audit-scene-runtime-imports.txt` | Pass; 36 scene runtime modules audited. |
| `npm run webgllib:audit-scene-runtime` | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB05/transcripts/passing-audit-scene-runtime.txt` | Pass with existing line-count warnings only; no hard failures. |
| SB05 source assertion scan | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB05/transcripts/sb05-source-assertions.txt` | Pass; records ownership flags, cache diagnostics, DTO fields, and docs/test references. |
| `dotnet build CanDoItAll.Components.slnx` | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB05/transcripts/components-solution-build.txt` | Pass; 0 warnings, 0 errors. |
| `dotnet test CanDoItAll.Components.slnx --no-build` | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB05/transcripts/components-solution-test-no-build.txt` | Pass; WebGlLib 40 tests and WebGlRunLib 28 tests passed. |
| `dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj` | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB05/transcripts/webglsandbox-build.txt` | Pass; 0 warnings, 0 errors. |
| SB05 anti-stub and boundary scan | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB05/transcripts/sb05-anti-stub-and-boundary-scan.txt` | Pass; no placeholders and no forbidden terms in touched production code files. |

## Proof Artifact Hashes

| Artifact | SHA-256 |
| --- | --- |
| `bundle://proof/SB05/transcripts/failing-first-resource-ownership.json` | `686e0ade211bd27c0978ee79de94e5ce1685bd8c34ea7a28a39aa0e4c85fb6fe` |
| `bundle://proof/SB05/transcripts/passing-resource-ownership-test.txt` | `4762c87898b0ff1c87d3808a5e04261d227f3e37313e6862baf7567520b80646` |
| `bundle://proof/SB05/transcripts/passing-browser-resource-cache-proof.json` | `05da3f9dd1847790f6a13a2d5802b9807de43050dd5cad2971cccb6445f541ab` |
| `bundle://proof/SB05/transcripts/browser-console-sb05-resource-cache.log` | `6dd7bb3fa30afc3720964002e8281d6b4352a303a8c1f3e5088a48307a8a0e22` |
| `bundle://proof/SB05/browser/sb05-textured-cache-proof.png` | `f400b84a3f9b55f9e2db7cde1e5b8c77a1a390d32bfc46a0d418ee8d7a96ac40` |
| `bundle://proof/SB05/transcripts/passing-focused-diagnostics-asset-tests.txt` | `9cd40cf9435c7e944c33311a2e72a9e60f9bd8537390084ea2e427b52b41ae10` |
| `bundle://proof/SB05/transcripts/passing-audit-scene-runtime-imports.txt` | `fdd6301a0f0c313cc1003e165ffbf1592969afd8eac954e4984e3733954f2564` |
| `bundle://proof/SB05/transcripts/passing-audit-scene-runtime.txt` | `340b80a0a23fb6dd993d4dc11f05923ad1860f33ac8c5eac540f97a8d748f0ee` |
| `bundle://proof/SB05/transcripts/sb05-source-assertions.txt` | `b3e59c99d154af51dd3ae0b2b12d70a1a7ff79173e397f2e369bcfe6471e6a6a` |
| `bundle://proof/SB05/transcripts/components-solution-build.txt` | `55d906d7d02640b48266491273ae4baed8a640451fc736bfb9ee39d9bf46f46e` |
| `bundle://proof/SB05/transcripts/components-solution-test-no-build.txt` | `49d3c046d1a60ecb96543e74a5f55b6a439bc4fa76f296fdff2c2b7c854660e9` |
| `bundle://proof/SB05/transcripts/webglsandbox-build.txt` | `2daa9c4b8190a2f2ec7b3ccf13eb9def9e720f83ca1bda44c2f5f91c3b87fb67` |
| `bundle://proof/SB05/transcripts/sb05-anti-stub-and-boundary-scan.txt` | `4d1c959c8b70fc8449163e2715f8f247ab7196870737ed43539f3f57cc62e673` |
| `bundle://proof/SB05/semantic-invariants.md` | `a5193b4763e5f917ee01e81a89e294223af98e3a9e015e8e29147bf8500aa05b` |
| `bundle://proof/SB05/refactor-gate.md` | `c72035c329d08ffc4c6827582c53b10f2310e39085836419b7edffd00ac6cf2a` |

## Source Assertions

| Assertion | Source path | Line/range or search term | Verified by |
| --- | --- | --- | --- |
| Resource ownership now separates geometry, material, and texture flags for templates and instances. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/17-webgl-scene-resources.js` | lines 16-22, 26, 37, 58, 78-84, 136 | `bundle://proof/SB05/transcripts/sb05-source-assertions.txt`, `bundle://proof/SB05/transcripts/passing-resource-ownership-test.txt` |
| Tinted model instances are marked as non-owning for shared geometry/material/texture references, then cloned materials are marked material-owned and texture-shared. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/16-webgl-scene-models.js` | lines 46, 104, 124 | JS ownership test and browser proof. |
| Texture disposal only runs when ownership says textures are owned; otherwise shared textures are retained and counted. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/17-webgl-scene-resources.js` | lines 78-84, 148, 164, 176 | Failing-first and passing JS/browser proofs. |
| State-local cache disposal passes diagnostics into template tree disposal and reports cache mode. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/21-webgl-scene-asset-cache.js` | lines 48, 66 | `bundle://proof/SB05/transcripts/passing-browser-resource-cache-proof.json` |
| Runtime diagnostics and command results expose cache mode, retained shared textures, and texture disposal. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/20-webgl-scene-command-results.js` | core lines 275, 283; command results lines 61-63 | Browser proof and focused DTO tests. |
| Proof snapshots and C# DTOs expose cache/resource counters. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Interop/WebGlRuntimeDiagnostics.cs`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneProofSnapshot.cs` | proof lines 64-65, 81, 87; DTO lines 91, 107, 109; proof snapshot lines 111, 113 | `bundle://proof/SB05/transcripts/passing-focused-diagnostics-asset-tests.txt` |
| A durable JS script tests ownership, retention, duplicate disposal de-dupe, and cache template disposal. | `repo://CanDoItAll.Components/tools/webgllib/test-resource-ownership.mjs`, `repo://CanDoItAll.Components/package.json` | script lines 53, 65, 87-88, 100, 113; package script `webgllib:test-resource-ownership` | `bundle://proof/SB05/transcripts/passing-resource-ownership-test.txt` |
| WebGlLib docs name the state-local cache policy and counters. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md` | line 74 | This manifest and source assertion scan. |

## Semantic Adequacy Gate

| Item | Required evidence | Status | Artifact |
| --- | --- | --- | --- |
| Shallow-pass trap named | A shallow fix could increment counters while still disposing shared textures, or count cache templates as disposed without disposing their resources. | Pass | `bundle://proof/SB05/semantic-invariants.md` |
| Adversarial negative proof | Bad pre-fix ownership and cache-disposal behavior is captured. | Pass | `bundle://proof/SB05/transcripts/failing-first-resource-ownership.json` |
| Semantic positive proof | JS and browser proof exercise real intended texture ownership, cache hits/misses, profile switches, intentional fallback, and final template disposal. | Pass | `bundle://proof/SB05/transcripts/passing-resource-ownership-test.txt`, `bundle://proof/SB05/transcripts/passing-browser-resource-cache-proof.json` |
| Anti-stub audit | TODO/NotImplemented/fixture-only and lower-layer boundary scan. | Pass | `bundle://proof/SB05/transcripts/sb05-anti-stub-and-boundary-scan.txt` |
| Raw-note closure | REQ-006 and REQ-007 closed for SB05; REQ-015 browser/refactor proof slice closed without domain leakage. | Pass | This manifest, `bundle://proof/SB05/semantic-invariants.md`, `bundle://proof/SB05/refactor-gate.md` |
| Downstream smoke | Components solution and WebGlSandbox still build/test. | Pass | `bundle://proof/SB05/transcripts/components-solution-build.txt`, `bundle://proof/SB05/transcripts/components-solution-test-no-build.txt`, `bundle://proof/SB05/transcripts/webglsandbox-build.txt` |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `ownsTexture` ownership state | `17-webgl-scene-resources.js` and `16-webgl-scene-models.js` | Instance disposal, template disposal, JS test, browser proof | Templates own geometry/material/texture; model instances do not own shared template resources; cloned tint materials own material but not texture. | Failing-first transcript records the old shared texture disposal. |
| `retainedSharedTextureCount` | `retainMaterialTextures` | Runtime diagnostics, command results, C# DTO, browser proof | Increments when shared texture maps are intentionally retained during material clone disposal. | Passing browser proof requires retained count to rise while texture disposal remains unchanged. |
| `assetCacheMode` | `21-webgl-scene-asset-cache.js` and diagnostics snapshots | Browser proof, C# DTO, proof snapshots | Reports `state-local`; cache is per runtime state and disposed with that state. | Browser proof asserts state-local mode before and after dispose. |
| Template disposal counters | `disposeAssetCache` and `disposeSceneObjectTree` | Browser proof, JS ownership test, diagnostics | Template promises are resolved at dispose; template resources are disposed once and counted. | Failing-first proof records template count without resource disposals. |
| Textured browser proof scene | `/tycoon-village` with public JS import/patch/dispose APIs | SB05 closure gate and later SB13 memory/perf proof | Loads two `question_box.glb` markers, removes one, switches primitive/model profiles, applies explicit missing asset, then disposes. | Browser proof rejects unexpected missing assets and requires final template texture disposal. |

## Browser Validation Analytics

| Route | Viewport | Actions | Screenshot/log artifacts | Result |
| --- | --- | --- | --- | --- |
| `http://127.0.0.1:5284/tycoon-village` | 1440x1000 | Imported two textured `question_box.glb` marker objects through public runtime APIs, removed one tinted instance, switched primitive and high profiles, applied explicit missing asset patch, disposed scene/cache, and copied browser console log. | `bundle://proof/SB05/transcripts/passing-browser-resource-cache-proof.json`, `bundle://proof/SB05/browser/sb05-textured-cache-proof.png`, `bundle://proof/SB05/transcripts/browser-console-sb05-resource-cache.log` | Pass; cache mode stayed `state-local`, first load had 1 miss and at least 1 hit, profile switching increased hits without unexpected missing assets, remove retained shared textures with `disposedTextureCount=0`, intentional missing id was the only missing asset, and final dispose raised `disposedTextureCount` from 0 to 2 with `disposedTemplateCount=1`. |

## Refactor Gate Result

- Touched files reviewed: resource ownership, model loading, asset cache, diagnostics snapshots, proof snapshots, command results, C# diagnostics DTOs, focused tests, JS test tool, package script, and WebGlLib README.
- Duplicates removed: ownership/disposal rules are centralized in `17-webgl-scene-resources.js`; cache disposal remains in `21-webgl-scene-asset-cache.js`; model loading only marks the ownership semantics.
- Layering checked: touched production code files have no WebGlRunLib, Economy, ledger, market, production-line, Vernon, or Smith terms; README mentions WebGlRunLib only as boundary documentation.
- Fixture-specific code removed: browser proof uses public runtime APIs and a repository GLB as textured data; production code contains no proof object ids or route checks.
- Docs/tests updated: WebGlLib README, `WebGlRuntimeDiagnosticsTests`, `tools/webgllib/test-resource-ownership.mjs`, package script, proof manifest, semantic invariants, execution report, and traceability.
- Remaining refactor risk: low for SB05; line-count warnings remain existing broader runtime refactor debt for SB07, and no global/shared cache was introduced.
