# SB01 Semantic Invariants

## Invariant Inventory Completeness

- Invariant ID: `SB01-INV-INVENTORY`
- Source raw note: RAW01, RAW02, and RAW03 require the prior publishing-readiness workflow to be reused for Canvas and floating windows.
- Expected behavior: The bundle has a fresh source-only inventory and publishing map that name CanvasLib, OverlayLib, Sandbox, tests, canvas asset tooling, routes, and package surfaces.
- Disallowed shallow implementation: A prepared bundle that only lists subbundle names without checking the current repo source tree.
- Failing-first test: N/A process/no production behavior change; shallow-pass risk is rejected by `bundle://proof/SB01/transcripts/inventory-generation.txt` and `bundle://inventories/current-state-data.json`.
- Passing test: `bundle://proof/SB01/transcripts/prepared-validator.txt` and `bundle://proof/SB01/transcripts/semantic-adequacy.txt` include `SB01-INV-INVENTORY`.
- Changed source files: Bundle-only artifacts under `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1`.
- Production assertions: `bundle://proof/SB01/transcripts/source-reference-assertions.txt` verifies the repo references used by later phases exist.
- Red-team negative case: A stale inventory that includes bin/obj output is rejected; `bundle://proof/SB01/transcripts/inventory-generation.txt` records source-only inventory excluding bin/obj.
- Downstream dependency check: SB02-SB10 can use `bundle://inventories/canvas-floating-windows-publishing-map.md` as the scope map.

## Invariant WebGL Scope Exclusion

- Invariant ID: `SB01-INV-WEBGL-SCOPE`
- Source raw note: RAW04 says "do not do webgl part yet."
- Expected behavior: WebGL implementation, package, runtime, and docs files are not modified by this bundle execution.
- Disallowed shallow implementation: Mentioning WebGL as out of scope while changing WebGL source or package files.
- Failing-first test: N/A process/no production behavior change; WebGL boundary is enforced by changed-file assertion.
- Passing test: `bundle://proof/SB01/transcripts/webgl-exclusion-source-assertion.txt` and `bundle://proof/SB01/transcripts/semantic-adequacy.txt` include `SB01-INV-WEBGL-SCOPE`.
- Changed source files: Bundle-only artifacts under `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1`.
- Production assertions: `bundle://proof/SB01/transcripts/webgl-exclusion-source-assertion.txt` shows no changed file path matches WebGL.
- Red-team negative case: Repo-wide WebGL matches exist, but they are existing source files outside the changed-file set and cannot be counted as in-scope proof.
- Downstream dependency check: SB04, SB08, SB09, and SB10 must continue to assert no WebGL changed-file drift.

## Invariant Pure JavaScript Runtime Boundary

- Invariant ID: `SB01-INV-RUNTIME-CONSTRAINT`
- Source raw note: RAW07 requires avoiding npm dependency for main Canvas, floating windows, calendar, and related implementation, keeping those runtimes in pure JavaScript.
- Expected behavior: The bundle contract requires pure browser JavaScript plus C# and Razor for Canvas, floating-window, calendar, preview, and related interactive runtime implementation.
- Disallowed shallow implementation: Allowing npm package dependency drift while using the phrase "pure JS" only in the final summary.
- Failing-first test: N/A process/no production behavior change; this phase records the constraint for downstream source and package assertions.
- Passing test: `bundle://proof/SB01/transcripts/prepared-validator.txt` and `bundle://proof/SB01/transcripts/semantic-adequacy.txt` include `SB01-INV-RUNTIME-CONSTRAINT`.
- Changed source files: `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/requirements/01-normalized-requirements.md`, `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/traceability/01-requirement-traceability.md`, and affected subbundle README files.
- Production assertions: SB04, SB06, SB07, SB09, and SB10 now require source/package assertions that npm remains tooling-only.
- Red-team negative case: A later package change adding a runtime dependency must reopen SB04 or SB09 and cannot be closed by browser screenshots alone.
- Downstream dependency check: SB04 owns the first runtime dependency assertion; SB09 and SB10 close package and final red-team assertions.

