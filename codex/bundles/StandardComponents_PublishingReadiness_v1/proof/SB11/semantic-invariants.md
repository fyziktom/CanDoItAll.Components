# SB11 Semantic Invariants

## SB11-INV-001 Full Standard Visual Matrix Passes

- Invariant ID: `SB11-INV-001`
- Source raw note: RAW10 requires real Playwright validation and screenshots one component/action state at a time, and RAW01 requires publishing readiness.
- Expected behavior: every standard sandbox route in the SB11 route list renders meaningful content at max-desktop, desktop, tablet, and mobile widths without page horizontal overflow, visible viewport escape, clipped visible text, or browser console errors.
- Disallowed shallow implementation: run only a desktop smoke test, inspect source without screenshots, reuse stale screenshots, or ignore console errors and local clipping failures.
- Failing-first test: the first SB11 matrix reported 165 failures, separating real visual defects from false-positive clipping checks before repairs were accepted.
- Passing test: `bundle://proof/SB11/transcripts/sb11-visual-matrix.txt` proves 51 routes, 4 viewports, 102 screenshots, 817 checks, 0 failures, and 0 console errors; `bundle://proof/SB11/transcripts/sb11-source-assertions.txt` prints `SB11-INV-001`.
- Changed source files: `bundle://scripts/verify-sb11-visual-matrix.mjs` SHA-256 `6fb8cc21d59fef12f1acbfcd43b39727a02e7dba7a549c36cb899dfd226e5e12`; `bundle://proof/SB11/data/sb11-visual-matrix.json` SHA-256 `22b28e2fe0955226c06a1d138bce2e1e83f50fd572149f6ad134242d8bfe659f`.
- Production assertions: final matrix screenshots are generated from `matrix-final`, and MCP screenshots cover the routes that originally showed visible defects.
- Red-team negative case: the verifier rejects page overflow, visible element escape, clipped visible text, missing meaningful content, stale screenshot roots, and console errors.
- Downstream dependency check: SB12 can use the matrix as the final visual-readiness gate for standard components while keeping Canvas/WebGL deferred.

## SB11-INV-002 ListDetailShell Uses Container-Owned Layout

- Invariant ID: `SB11-INV-002`
- Source raw note: RAW02 and RAW10 require hardening real implementation behavior where components do not use available space correctly.
- Expected behavior: `ListDetailShell` decides when to split panes through a component-owned container query, not through viewport breakpoints that can force a split inside narrow hosts.
- Disallowed shallow implementation: keep viewport-only grid breakpoints or rely on parent pages to resize the list/detail shell correctly.
- Failing-first test: SB11 visual review found list/detail content squeezed by available-space mismatch in constrained layouts.
- Passing test: `bundle://proof/SB11/transcripts/sb11-source-assertions.txt` proves the container query shell and prints `SB11-INV-002`; `bundle://proof/SB11/transcripts/sb11-visual-matrix.txt` proves no final clipped text or viewport escape.
- Changed source files: `repo://src/CanDoItAll.Components.BaseLib/Components/Lists/ListDetailShell.razor` SHA-256 `500814d20b9837baac359d55235be3e31bdec373432c81302945713cc038ca56`; `repo://Tailwind/data-display/list-detail.css` SHA-256 `93f7d178a9eb03cfd8426ff8314791f7c5001f6a75ae41453e99b2aed750c6cc`; `repo://Tailwind/input.css` SHA-256 `e53bee746a006f1349df53584c080ecddcdaa8e35ef4b51a2d5ca4ca4583cd29`.
- Production assertions: the shell emits `cda-list-detail-shell`, the CSS owns `container-type: inline-size`, and the split only activates when the component container is wide enough.
- Red-team negative case: restoring viewport-only split grid or removing the container query fails source assertions and reopens visual matrix clipping.
- Downstream dependency check: package-input approvals were refreshed and locked-mode BaseLib tests passed after this source change.

## SB11-INV-003 Long Navigation And Tabs Labels Stay Readable

- Invariant ID: `SB11-INV-003`
- Source raw note: RAW10 calls out wrapping text overflow and weird layout behavior in dropdowns/navigation-like components.
- Expected behavior: TreeView labels wrap instead of truncating, wrap-mode tabs wrap long labels, and scroll-mode tabs can grow to label width without clipping text.
- Disallowed shallow implementation: hide overflow with `truncate`, test only short labels, or make scroll tabs clip long text inside fixed-width labels.
- Failing-first test: SB11 MCP screenshots and matrix notes captured long navigation and tabs labels before repairs.
- Passing test: `bundle://proof/SB11/transcripts/sb11-source-assertions.txt` proves TreeView/Tabs wrapping rules and prints `SB11-INV-003`; MCP screenshots `bundle://proof/SB11/screenshots/mcp/sb11-mcp-navigation-long-desktop.png`, `bundle://proof/SB11/screenshots/mcp/sb11-mcp-navigation-long-mobile.png`, and `bundle://proof/SB11/screenshots/mcp/sb11-mcp-navigation-tabs-long-mobile.png` show the repaired states.
- Changed source files: `repo://Tailwind/navigation/treeview.css` SHA-256 `cd9e0f3532f369dcdd3087cff766a38d1ad00c64ff1cf02c9a5f2d605ea24679`; `repo://Tailwind/navigation/tabs.css` SHA-256 `aebcc1b87f1c8440f2fa303aa7ab07daf80fb2a7594f1ae02fbaba9f6e9198cb`.
- Production assertions: TreeView labels use normal wrapping and scroll tabs use width rules that preserve full label text.
- Red-team negative case: reintroducing `truncate` into TreeView labels or fixed-width scroll tabs fails source assertions and the matrix clipped-text check.
- Downstream dependency check: SB08 navigation verifier was rerun after repairs and passed.

## SB11-INV-004 Inputs And Summary Tiles Wrap In Constrained Layouts

- Invariant ID: `SB11-INV-004`
- Source raw note: RAW05 asks for Tailwind-owned component styling rather than custom CSS hacks, and RAW10 requires visual inspection for wrapping/overflow.
- Expected behavior: PrefixedField adornments remain readable with long tokens on mobile by stacking at full width, and SummaryTile label/value/helper text wraps inside constrained grid tracks without negative letter spacing.
- Disallowed shallow implementation: hide long prefixes with truncation, keep negative tracking, or fix only the sandbox layout while leaving component CSS brittle.
- Failing-first test: SB11 mobile input and hero summary screenshots showed constrained text behavior requiring repair.
- Passing test: `bundle://proof/SB11/transcripts/sb11-source-assertions.txt` proves PrefixedField and SummaryTile rules and prints `SB11-INV-004`; `bundle://proof/SB11/screenshots/mcp/sb11-mcp-inputs-long-mobile.png` proves the long-input mobile state.
- Changed source files: `repo://Tailwind/forms/fields.css` SHA-256 `f5ed1a37b1dfe49b6c0bb790bf97941f5be4056ea08420724d5236060e286e20`; `repo://Tailwind/surfaces/cards.css` SHA-256 `bc9ab11af251073b0175273c2e1005b07009dcff6b736535f4a0e81b94b114b3`; `repo://src/CanDoItAll.Components.BaseLib/wwwroot/css/output.css` SHA-256 `6e56fcbf39d2efd637500bc3360474966bfbcee937e362cd6e6bdef7005f85db`.
- Production assertions: generated `output.css` was rebuilt from Tailwind inputs, and summary values no longer rely on negative letter spacing.
- Red-team negative case: restoring adornment truncation, single-line mobile prefixes, or negative tracking fails source assertions and matrix clipped-text checks.
- Downstream dependency check: SB06 input verifier reran after repairs and passed.

## SB11-INV-005 Earlier Interaction Verifiers Still Pass

- Invariant ID: `SB11-INV-005`
- Source raw note: RAW09 requires refactoring checkpoints that re-analyze and validate after implementation.
- Expected behavior: SB06-SB09 focused browser verifiers still pass after final visual repairs, proving styling hardening did not regress input, action/feedback, layout/navigation/overlay, data-display/chart/Mermaid behavior.
- Disallowed shallow implementation: close the final visual matrix without rerunning earlier interaction/open-state checks.
- Failing-first test: SB11 repairs touched shared Tailwind/navigation/layout inputs that could regress earlier subbundle behavior if not rerun.
- Passing test: `bundle://proof/SB11/transcripts/sb11-sb06-inputs-verifier.txt`, `bundle://proof/SB11/transcripts/sb11-sb07-actions-feedback-verifier.txt`, `bundle://proof/SB11/transcripts/sb11-sb08-layout-navigation-overlays-verifier.txt`, and `bundle://proof/SB11/transcripts/sb11-sb09-data-display-charts-mermaid-verifier.txt` all pass; `bundle://proof/SB11/transcripts/sb11-source-assertions.txt` prints `SB11-INV-005`.
- Changed source files: `bundle://proof/SB11/data/sb11-visual-review-notes.json` SHA-256 `4bb36fa9902eadc70f0756d41f851d703db23b5e57d595b77b8d3597c6e58725`.
- Production assertions: reruns report SB06 22 checks and 0 console errors, SB07 37 checks/0 failures/0 console errors, SB08 67 checks/0 failures/0 console errors, and SB09 57 checks/0 failures/0 console errors.
- Red-team negative case: any earlier verifier failure blocks SB11 closure and prevents SB12 transfer readiness.
- Downstream dependency check: SB12 can cite both the broad matrix and earlier focused interaction proof.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| Visual matrix report | `bundle://scripts/verify-sb11-visual-matrix.mjs` creates `bundle://proof/SB11/data/sb11-visual-matrix.json`. | SB12 transfer audit consumes the matrix as final route/screenshot coverage. | `bundle://proof/SB11/transcripts/sb11-visual-matrix.txt` proves 817 checks, 102 screenshots, and 0 failures/console errors. | The verifier rejects overflow, viewport escape, clipped visible text, missing meaningful content, stale screenshot roots, and console errors. |
| Container-owned ListDetailShell layout | `repo://src/CanDoItAll.Components.BaseLib/Components/Lists/ListDetailShell.razor` emits the shell and `repo://Tailwind/data-display/list-detail.css` owns the container query. | Sandbox data-display and final matrix routes render the shell under constrained layouts. | `bundle://proof/SB11/transcripts/sb11-source-assertions.txt` and locked-mode BaseLib tests prove the source/package-input lifecycle. | Removing the container query or restoring viewport-only split fails source assertions and visual matrix checks. |
| Long-label wrapping rules | `repo://Tailwind/navigation/treeview.css`, `repo://Tailwind/navigation/tabs.css`, `repo://Tailwind/forms/fields.css`, and `repo://Tailwind/surfaces/cards.css` own the visual fixes. | Components consume the generated CSS at `repo://src/CanDoItAll.Components.BaseLib/wwwroot/css/output.css`. | `bundle://proof/SB11/transcripts/sb11-tailwind-build.txt` proves regenerated CSS and `bundle://proof/SB11/transcripts/sb11-visual-matrix.txt` proves final rendering. | Source assertions reject truncation/negative tracking and the matrix rejects clipped visible text. |

## Semantic Gate Decision

Pass. SB11 includes failing visual observations, production Tailwind/component repairs, a complete visual matrix, MCP screenshots for repaired states, focused verifier reruns, build/test proof, anti-stub audit proof, and changed-file hashes.
