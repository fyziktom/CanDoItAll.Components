# SB05 semantic invariants

Status: Completed 2026-06-02.

| Invariant ID | Expected behavior | Shallow-pass trap | Negative proof | Positive proof | Source files | Downstream dependency |
| --- | --- | --- | --- | --- | --- | --- |
| SB05-INV-001 | Strict patch mode is the default and rejects invalid scene id, strict revision, object target, added-object id, or added-link endpoint without mutating scene state. | Only validating errors without asserting object/link/revision state after failure. | `bundle://proof/SB05/transcripts/failing-first-patch-transaction-tests.txt` plus browser strict assertion JSON show the old contract lacked metadata and would not prove transaction mode. | `passing-focused-csharp-patch-transaction-tests.txt`, `components-webgllib-tests.txt`, `browser-strict-runtime-assertions.json`. | `WebGlScenePatchReducer.cs`, `WebGlScenePatchPolicy.cs`, `13-webgl-scene-patching.js`, `35-webgl-scene-patch-validation.js` | SB11 browser route proof and package consumers. |
| SB05-INV-002 | `permissive-invalid-links` skips invalid added links, records warnings, applies valid object/link changes, and identifies affected and skipped ids. | Treating a warning string as sufficient proof while affected/skipped ids are absent from command results. | Failing-first permissive test failed because explicit `patchTransactionMode=permissive-invalid-links` was still treated as strict failure. | Focused C# tests and `browser-warning-runtime-assertions.json` prove affected object/link ids and `skippedLinkIds`. | `WebGlScenePatchPolicy.cs`, `WebGlScenePatchResult.cs`, `37-webgl-scene-patch-policy.js` | Downstream playback diagnostics and browser proof. |
| SB05-INV-003 | Patch result metadata includes transaction mode, missing-link endpoint mode, patch classification, and skipped-link ids when applicable in both C# and JS. | Adding a C# metadata property while browser command results still omit JS metadata. | Failing-first strict test failed because `WebGlScenePatchResult` had no metadata; browser assertions would fail if JS metadata were missing. | `source-policy-assertions.txt`, focused tests, and strict/warning browser JSON. | C# patching files, JS `13` and `37` runtime modules, sandbox route fields | SB06/SB07 validation policy and dynamic-object decisions. |
| SB05-INV-004 | Patch policy changes keep the WebGlLib runtime modular and generic. | Passing behavioral tests while making runtime modules exceed hard line-count limits or adding domain semantics. | Scene-runtime audit initially failed the hard line-count gate after inline helper growth; helper split repaired it. | Final `components-webgllib-scene-runtime-audit.txt`, `components-webgllib-boundary-audit.txt`, and placeholder scan. | `13-webgl-scene-patching.js`, `37-webgl-scene-patch-policy.js`, `WebGlScenePatchPolicy.cs` | R14 genericity and later package/browser proof. |

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative test |
| --- | --- | --- | --- | --- |
| `patchTransactionMode` | Patch metadata, C#/JS policy resolvers | Reducer/runtime results, browser UI proof | Defaults to `strict`; explicit `permissive-invalid-links` selects invalid-link warning behavior | Failing-first permissive test expected success but old code failed. |
| `patchClassification` | C# policy helper and JS classifier | Results, diagnostics, browser assertion fields | Recorded for successful and failed patch commands | Strict failing-first metadata assertion failed before property/policy support. |
| `skippedLinkIds` | Permissive invalid-link handlers | Result metadata and browser proof fields | CSV list of invalid added-link ids skipped by warning mode | Browser warning JSON asserts `proof.bad-link.warn` is skipped while `proof.good-link.warn` is affected. |
| Browser bad-link proof controls | Tycoon Village sandbox route | Playwright proof | Route-only proof controls invoke production `ApplyPatchDetailedAsync` | Console and runtime-state JSON guard against screenshot-only proof. |

## Raw Requirement Closure

| Requirement | Closure |
| --- | --- |
| R06 | Solved for SB05. Strict and permissive invalid-link patch modes are named, documented, tested in C#, aligned in JS, and proven through `/tycoon-village` browser actions. |
| R12 | Partially advanced. SB05 adds large-screen `/tycoon-village` browser proof for strict/warning patch behavior; SB11 still owns final large+narrow multi-route UI proof. |
| R14 | Maintained locally. WebGlLib boundary audit passes and patch policy remains generic, with no Economy/domain terms. |
