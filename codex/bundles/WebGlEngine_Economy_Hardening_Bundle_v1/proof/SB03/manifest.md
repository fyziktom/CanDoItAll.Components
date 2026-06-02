# SB03 Proof Manifest

Subbundle: `SB03-patch-transactions-and-revisions`
Status: `Completed`
Prepared by: Codex
Completed at UTC: `2026-06-02T00:53:20Z`

## Changed File Hashes

| Repo | Path | Before SHA-256 | After SHA-256 | Why changed |
| --- | --- | --- | --- | --- |
| CanDoItAll.Components | `repo://CanDoItAll.Components/codex/bundles/WebGlEngine_Economy_Hardening_Bundle_v1/architecture/06-scene-revision-policy.md` | `NEW` | `61fcb3f9b41801f67182d093987150ecd2e6854180682a6ac15c11c1ea540c54` | Added ADR for canonical scene revision, UI revision mirror, hash policy, and transactional patch rules. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md` | `e5509b6a30c5a5ed7d633bdb82ac6c9cbdfd0e33ce92f3117209f3d64f1facab` | `0a7e8d3a9c8ad7ad1377662324782e12a4872dc99b4477ea089c2f14ac3506d1` | Documented canonical revision and scene/document hash behavior after SB02 docs. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneRevisionPolicy.cs` | `NEW` | `1b2064b99ecbfadd41effe3ef81ee9ba54b5103906ce926a7b41f7c37bf7b52f` | Centralized C# canonical revision resolution, next revision, commit, and normalize helpers. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Patching/WebGlScenePatchReducer.cs` | `92b0f28c818de1aca5eab932ffcacdc41fa3b7be6af8602a4e794c8a4c905fb5` | `8c14849a408f01a765cb418a46475d9d9accceb70b88bed8afecefe502d8e40e` | Made reducer preflight object/link failures, use canonical revision, avoid revision bumps on no-op patches, and clean links/layers on object removal. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocumentHasher.cs` | `d635039586c20cd217330d2a5c79dc2173d5fced20eebf85c84ca525f65470e7` | `ab59512ca239bcf368bb090134774cf75a6e1d4e98577f60868638695bed148c` | Kept UI-only revision out of scene content hash while preserving canonical scene revision. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocumentNormalizer.cs` | `7c090b58043357e8525059dd2893c658b3d5504439bb2d91ea3f7e64172a1828` | `614129b2dfa1a0bdd6868a2ba25c3be6bca2439c19db7dffd369740f8da0ee4f` | Normalized legacy documents through canonical revision policy. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js` | `f50839ae4c5c7cd12f533bf23fd8c2ba789ba4d129455c554a7e8c2752f8531d` | `f6a5fda284469e6db46ed8c78e79706039092c4f267e53595dd98df8c0e51010` | Removed local revision helpers after splitting revision policy into its own runtime module. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/10-webgl-scene-lifecycle.js` | `62ef45d7e5739649fb15ab41b428dc88b88fa8c6f058c3d6d4d2d9e4be512e49` | `edb0b6a97b5ad092e0c5081717836d38b556f27ed03cfc0adcac2123c0f27238` | Exported scenes now report canonical revision and mirror it into exported UI state. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js` | `a35ad08bad28a0fab2ba09af475e0f8063f696e284eccea96b80b1cd22b89aa3` | `eb7d3e21379c06f3777fd96fce2749dbe969e88f1d1559ce063b5fc9d2f54edd` | Added JS preflight validation, canonical revision commit, and layer cleanup on object removal. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/20-webgl-scene-command-results.js` | `e63d43560981db543ce78d4e96c70648cf0e57bc6cb5f386083230f3b25191db` | `554b195ce4fcfc272e8d5704e49022f23bec06ecc833460d3bbae085a15f467e` | Command results report canonical revision. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/34-webgl-scene-revisions.js` | `NEW` | `96bc7d9fd94f2471c1d98acd1caf4d5198a507995637744787186f8700242a60` | Added JS canonical revision helper. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/35-webgl-scene-patch-validation.js` | `NEW` | `832534a4047e05adbd2d17d440894bd8a8944d1ada6d3996070f646d1bb14df2` | Added JS preflight validation helper. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlLib.Tests/WebGlScenePatchReducerTests.cs` | `60b5452a605a17f82addfff866f3c6c831b21371bad4c842e1d5c12c2067d560` | `c79412b18979a4bf88c50ebc3c39be1bf10a8bd4ae9906e1cafe41cbe42ad261` | Added parity tests for canonical revision, transactional failure, link/layer cleanup, and revision increments. |
| CanDoItAll.Components | `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlLib.Tests/WebGlSceneDocumentSerializerTests.cs` | `7597e7092626a14ef555d241d369e3c775bb792bd050d15d076df41b190a486e` | `5989dde5874aec71fdd9e885677e6bf67f378d6c932669788f48291ac127baa0` | Added hash-policy test proving scene content hash uses scene revision and ignores UI revision. |

## Commands

| Command | Working directory | Transcript path | Result |
| --- | --- | --- | --- |
| `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj --filter "Patch|Document|Revision"` before fix | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB03/transcripts/failing-first-dotnet-patch-document-revision.txt` | Expected failure; 5 failing tests captured canonical revision, transaction, cleanup, and hash gaps. |
| Browser bad-link patch before JS fix | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB03/transcripts/failing-first-browser-bad-link-partial-commit.json` | Expected failure; bad patch returned failed result but still moved `agent.runner` and advanced revision. |
| `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj --filter "Patch|Document|Revision"` after fix | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB03/transcripts/passing-dotnet-patch-document-revision.txt` | Pass; 22 tests, 0 failures. |
| Browser bad-link and good-link patch after JS fix | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB03/transcripts/passing-browser-bad-link-transaction-proof.json` | Pass; failed patch leaves snapshot unchanged, valid patch moves one object, adds one link, and advances revision once. |
| `npm run webgllib:audit-scene-runtime-imports` | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB03/transcripts/passing-audit-scene-runtime-imports.txt` | Pass; 35 scene runtime modules audited. |
| `npm run webgllib:audit-scene-runtime` | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB03/transcripts/passing-audit-scene-runtime.txt` | Pass with existing line-count warnings only. |
| `dotnet build src/CanDoItAll.Components.WebGlSandbox/CanDoItAll.Components.WebGlSandbox.csproj` | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB03/transcripts/webglsandbox-build.txt` | Pass; 0 warnings, 0 errors. |
| SB03 anti-stub and boundary scan | `C:\repositories\CanDoItAll.Components` | `bundle://proof/SB03/transcripts/sb03-anti-stub-and-boundary-scan.txt` | Pass; no stub placeholders and no forbidden domain/run-layer terms in touched production files. |

## Proof Artifact Hashes

| Artifact | SHA-256 |
| --- | --- |
| `bundle://proof/SB03/transcripts/failing-first-dotnet-patch-document-revision.txt` | `a81a61d6d21998d1a26b82930137511e349ff8e6cb982d93daeed225b0025701` |
| `bundle://proof/SB03/transcripts/failing-first-browser-bad-link-partial-commit.json` | `1b6443dfd68043cadc8d9579fa42179c55ae00c381a5e0fee1d51329ce29b890` |
| `bundle://proof/SB03/transcripts/passing-dotnet-patch-document-revision.txt` | `e06fdce4c130c38357b330e7823c0910253f738a8a72bda558ea2239a0975b55` |
| `bundle://proof/SB03/transcripts/passing-browser-bad-link-transaction-proof.json` | `81d60990929472a59919569e9447f0eb0724cf2a6d47977f8a1042daabefaaa5` |
| `bundle://proof/SB03/transcripts/passing-audit-scene-runtime-imports.txt` | `19b289493523d9cfa5c33f45586772c5716e528009ab0a52ec85769e1b44bc17` |
| `bundle://proof/SB03/transcripts/passing-audit-scene-runtime.txt` | `c7647c3707589c79f4db18e98f214d66581f3fb083d7f44ff4fd466e395af066` |
| `bundle://proof/SB03/transcripts/webglsandbox-build.txt` | `caa89865077d91ffb1708d62ef4f0c4b247e738258391d1254c7ef942ebc70a9` |
| `bundle://proof/SB03/transcripts/sb03-anti-stub-and-boundary-scan.txt` | `93121bf88827776b6746db396a8108fd4d82ab93348f26116bace682941fad76` |
| `bundle://proof/SB03/browser/tycoon-village-sb03-passing.png` | `dab6bcc0c1bed30e3aa99b7abfeeec82003f6582530716aae9b684a717299fe5` |
| `bundle://proof/SB03/refactor-gate.md` | `f90979f1dd93664150c38240e0fd7619af81f14eb8582a985796508aed82faf7` |
| `bundle://proof/SB03/semantic-invariants.md` | `f4480dac43ba5fed6db7327f076a39abee8a391ce809aced59c394f80273b2b3` |
| `bundle://architecture/06-scene-revision-policy.md` | `61fcb3f9b41801f67182d093987150ecd2e6854180682a6ac15c11c1ea540c54` |

## Source Assertions

| Assertion | Source path | Line/range or search term | Verified by |
| --- | --- | --- | --- |
| C# revision resolution prefers `Scene.Revision`, falls back to legacy UI revision, and commits both values on successful mutations. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneRevisionPolicy.cs` | lines 5, 17, 20, 29 | `bundle://proof/SB03/transcripts/passing-dotnet-patch-document-revision.txt` |
| C# reducer validates before mutation and rejects object patches that target removed/missing objects. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Patching/WebGlScenePatchReducer.cs` | lines 10, 114, 149, 163 | Failing-first and passing dotnet transcripts. |
| C# reducer cleans layer membership and records removed link ids when removing an object. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Patching/WebGlScenePatchReducer.cs` | lines 38, 43, 266, 280 | `bundle://proof/SB03/transcripts/passing-dotnet-patch-document-revision.txt` |
| Scene content hash ignores UI revision but normalized documents resolve canonical scene revision. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocumentHasher.cs`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocumentNormalizer.cs` | hasher line 21; normalizer line 15 | `bundle://proof/SB03/transcripts/passing-dotnet-patch-document-revision.txt` |
| JS runtime validates patches before mutation and commits canonical revision on successful mutation. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/13-webgl-scene-patching.js`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/35-webgl-scene-patch-validation.js` | patching lines 34, 104, 234; validation lines 3, 10, 67 | `bundle://proof/SB03/transcripts/passing-browser-bad-link-transaction-proof.json`, `bundle://proof/SB03/transcripts/passing-audit-scene-runtime-imports.txt` |
| JS command results and exported scenes use canonical revision. | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/20-webgl-scene-command-results.js`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/10-webgl-scene-lifecycle.js` | command result lines 15 and 54; lifecycle lines 136 and 145 | Browser passing proof and runtime import audit. |
| Revision/hash contract is documented. | `repo://CanDoItAll.Components/codex/bundles/WebGlEngine_Economy_Hardening_Bundle_v1/architecture/06-scene-revision-policy.md`, `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlLib/README.md` | ADR lines 7, 15, 17, 19; README line 80 | This manifest and semantic invariant file. |
| Tests cover canonical revision, transactional no-partial-commit, cleanup, and hash policy. | `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlLib.Tests/WebGlScenePatchReducerTests.cs`, `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlLib.Tests/WebGlSceneDocumentSerializerTests.cs` | reducer tests lines 8, 201, 241; serializer test line 111 | `bundle://proof/SB03/transcripts/passing-dotnet-patch-document-revision.txt` |

## Semantic Adequacy Gate

| Item | Required evidence | Status | Artifact |
| --- | --- | --- | --- |
| Shallow-pass trap named | A patch could report failure after partially mutating earlier operations, or tests could check only success flags/revisions without checking object/link/layer snapshots. | Pass | `bundle://proof/SB03/semantic-invariants.md` |
| Adversarial negative proof | Bad case fails for the right reason before the fix. | Pass | `bundle://proof/SB03/transcripts/failing-first-dotnet-patch-document-revision.txt`, `bundle://proof/SB03/transcripts/failing-first-browser-bad-link-partial-commit.json` |
| Semantic positive proof | Real intended behavior passes after the fix. | Pass | `bundle://proof/SB03/transcripts/passing-dotnet-patch-document-revision.txt`, `bundle://proof/SB03/transcripts/passing-browser-bad-link-transaction-proof.json` |
| Anti-stub audit | TODO/NotImplemented/fixture-only and boundary scan. | Pass | `bundle://proof/SB03/transcripts/sb03-anti-stub-and-boundary-scan.txt` |
| Raw-note closure | REQ-003, REQ-004, and REQ-008 closed for SB03 scope without adding domain semantics. | Pass | This manifest, `bundle://proof/SB03/semantic-invariants.md`, `bundle://architecture/06-scene-revision-policy.md` |
| Downstream smoke | Dependent sandbox path still builds and browser route exercises real runtime. | Pass | `bundle://proof/SB03/transcripts/webglsandbox-build.txt`, `bundle://proof/SB03/transcripts/passing-browser-bad-link-transaction-proof.json` |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Canonical scene revision | `WebGlSceneRevisionPolicy.cs`, `34-webgl-scene-revisions.js` | C# patches, JS patches, command results, exports, document normalization | Resolved on validate/export/result; committed once on successful mutating patches; mirrored to UI revision for compatibility. | Failing-first dotnet transcript shows previous wrong revision values. |
| Patch preflight result | C# reducer validation and JS `validatePatchForApply` | `Apply`, `applyPatchDetailed`, browser/runtime command results | Whole-patch validation runs before mutation for scene/base/object/link failures. | Failing-first browser transcript records failed patch with mutation before the JS preflight fix. |
| Scene consistency cleanup | C# reducer and JS patching runtime | Scene objects, links, layers, runtime link groups | Removing an object also removes dependent links and layer object ids and records affected link ids. | Failing-first dotnet transcript catches stale cleanup behavior. |
| Scene content hash | `WebGlSceneDocumentHasher` | Save/load consumers and proof comparisons | Hashes normalized scene content including canonical scene revision while clearing UI-only revision/selection/hover. | Failing-first dotnet transcript catches UI revision influencing scene content hash. |

## Browser Validation Analytics

| Route | Viewport | Actions | Screenshot/log artifacts | Result |
| --- | --- | --- | --- | --- |
| `http://127.0.0.1:5279/tycoon-village` before fix | 1280x900 | Applied bad patch with valid object transform plus invalid link endpoint. | `bundle://proof/SB03/transcripts/failing-first-browser-bad-link-partial-commit.json` | Expected failure; result was failed, but `failedButMutated=true`, runner moved from `(1.2,0,-1.6)` to `(1.7,0,-1.1)`, and revision changed from 0 to 1. |
| `http://127.0.0.1:5281/tycoon-village` after fix | 1280x900 | Applied the same bad patch, then a valid patch that moved `agent.runner` and added `link.runner.plaza.proof`; captured diagnostics and screenshot. | `bundle://proof/SB03/transcripts/passing-browser-bad-link-transaction-proof.json`, `bundle://proof/SB03/browser/tycoon-village-sb03-passing.png` | Pass; bad patch kept hash/snapshot/revision unchanged and affected ids empty; good patch moved one object, added one link, advanced scene/UI revision by 1, `pageErrors=[]`, filtered console messages empty. |

## Refactor Gate Result

- Touched files reviewed: C# patch reducer, scene revision policy, document normalizer/hasher, JS patching, JS revision/validation helpers, JS lifecycle export, JS command results, tests, WebGlLib README, and ADR.
- Duplicates removed: revision helpers split into C#/JS policy modules; JS validation split out of patching module.
- Layering checked: production patch/revision files have zero forbidden domain/run-layer matches.
- Fixture-specific code removed: browser proof uses public runtime APIs on the real sandbox route; tests are generic scene tests.
- Docs/tests updated: ADR, WebGlLib README, reducer tests, serializer tests, and proof artifacts updated.
- Remaining refactor risk: low; runtime line-count audit warnings remain existing broader WebGlLib refactor debt for SB04/SB07.
