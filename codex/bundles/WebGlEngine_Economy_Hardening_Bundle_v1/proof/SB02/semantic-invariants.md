# SB02 Semantic Invariants

Subbundle: `SB02-webgllib-js-runtime-correctness`

## Invariants

| Invariant ID | Requirement | Expected behavior | Disallowed shallow implementation | Negative proof | Positive proof |
| --- | --- | --- | --- | --- | --- |
| SB02-IMPORT-001 | REQ-002 | Every named import in `wwwroot/js/runtime/scene/*.js` resolves to an export from its sibling module, and scene symbols exported by sibling modules cannot be used without importing them. | Only checking parseable JavaScript or only checking import specifier paths. | `bundle://proof/SB02/transcripts/failing-first-audit-scene-runtime-imports.txt` rejects `resolveObjectPosition` usage without import; `bundle://proof/SB02/transcripts/audit-scene-runtime-imports-self-test.txt` rejects intentionally missing exports and unimported scene symbols. | `bundle://proof/SB02/transcripts/passing-audit-scene-runtime-imports.txt` passes for 33 real scene runtime modules after the import fix. |
| SB02-RUNTIME-001 | REQ-002, REQ-015 | The real sandbox browser route creates the WebGL scene, supports a draggable object, applies a transform patch through the public runtime API, and disposes state/canvas without console or page errors. | Closing on static audit only, or using a fake DOM fixture instead of a real Blazor/WebGL route. | The browser transcript records `pageErrors=[]` and empty filtered console messages; a failed create/drag/patch/dispose path would make the transcript fail or show runtime errors. | `bundle://proof/SB02/transcripts/browser-tycoon-village-runtime-proof.json` records scene creation, real mouse drag of `agent.runner`, successful `applyPatchDetailed`, snapshot counts, and disposal. |
| SB02-BOUNDARY-001 | REQ-001, REQ-002 | SB02 keeps `WebGlLib` generic: runtime/package files do not import or mention Economy, ledger, market, production-line, or Vernon concepts. | Adding a special-case domain guard inside production runtime code, or moving simulator concepts into `WebGlLib`. | `bundle://proof/SB02/transcripts/sb02-anti-stub-and-boundary-scan.txt` would surface forbidden domain terms in production runtime/package files. | The same scan records zero forbidden production-runtime/package matches; audit-tool denylist terms are intentional negative checks. |
| SB02-AUDIT-001 | REQ-002 | The import audit is CI-ready and documented for maintainers. | A local-only one-off script that is not exposed through npm or README notes. | Missing script/docs would be caught by the runtime import assertion scan. | `package.json` exposes `webgllib:audit-scene-runtime-imports`; WebGlLib README lists it beside the existing runtime audit. |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Scene runtime import/export audit | `tools/webgllib/audit-scene-runtime-imports.cjs` | npm/CI/manual validation | Parses scene runtime modules, validates imports/exports, checks unimported scene symbol usage, root registration, and forbidden runtime dependencies. | Failing-first and self-test transcripts reject realistic bad module states. |
| `resolveObjectPosition` runtime dependency | `02-webgl-scene-core.js` export consumed by `11-webgl-scene-graph.js` | Transform-only update path | Loaded with scene graph module and used by `updateObjectRuntimeTransform`. | Failing-first audit caught the missing import before the fix. |
| Browser proof transcript/screenshot | `/tycoon-village` route | Bundle progression gate and later SB13 comparison | Generated after starting WebGlSandbox, exercising scene create/drag/patch/dispose, and stopping the host. | Empty filtered console/page-error lists and disposal state checks reject shallow visual-only proof. |

## Reopen Triggers

- `npm run webgllib:audit-scene-runtime-imports` fails or is removed from `package.json`.
- A new scene runtime module uses an exported symbol from another scene module without importing it.
- `/tycoon-village` browser proof shows create, drag, patch, or dispose regressions.
- Production `WebGlLib` runtime/package files gain Economy, ledger, market, production-line, or Vernon-specific concepts.
- SB03/SB04 changes invalidate the transform patch or incremental update assumptions recorded here.
