# SB05 Semantic Invariants

## SB05-INV-001 Standard And Deferred Proof Scope Are Split

- Invariant ID: `SB05-INV-001`
- Source raw note: RAW03 and RAW04.
- Expected behavior: standard publishing proof includes BaseLib, Charts, Mermaid, and overlay-facing standard surfaces; Canvas/WebGL stays visible only as deferred scope.
- Disallowed shallow implementation: keep Canvas in the same standard group count or hide the scope boundary only in documentation.
- Failing-first test: route smoke and coverage review initially treated Canvas as a normal sandbox group.
- Passing test: `proof/SB05/transcripts/sb05-verifier.txt` prints `SB05-INV-001`.
- Changed source files: `src/CanDoItAll.Components.Sandbox/SandboxCatalogRegistry.cs`, `Components/Layout/MainLayout.razor`, `Components/Pages/Home.razor`.
- Production assertions: `StandardGroups` excludes `SandboxGroupKey.Canvas`; `DeferredGroups` exposes Canvas separately; the sidebar has a deferred proof scope section.
- Red-team negative case: if Canvas appears in `standard-component-coverage.json` component rows, the verifier fails.
- Downstream dependency check: SB06-SB09 route proof can now target standard groups without mixing Canvas/WebGL implementation work.

## SB05-INV-002 Coverage Index Is A Stable Proof Route

- Invariant ID: `SB05-INV-002`
- Source raw note: RAW04 and RAW10.
- Expected behavior: sandbox route `groups/coverage` lists standard group owners, focused routes, and deferred scope with stable test hooks.
- Disallowed shallow implementation: produce only a JSON file without a real browser route or test IDs.
- Failing-first test: no `groups/coverage` route existed before SB05.
- Passing test: `proof/SB05/transcripts/sb05-verifier.txt` prints `SB05-INV-002`.
- Changed source files: `src/CanDoItAll.Components.Sandbox/Components/Pages/Coverage.razor`.
- Production assertions: the route includes `sandbox-coverage-index`, `sandbox-coverage-group-*`, and `sandbox-coverage-deferred` hooks.
- Red-team negative case: removing the route or hooks fails the verifier before downstream visual hardening can proceed.
- Downstream dependency check: SB11 can use the route matrix and coverage index as a visual proof starting point.

## SB05-INV-003 Every Standard Component Has An Owner Route Or Exception

- Invariant ID: `SB05-INV-003`
- Source raw note: RAW04.
- Expected behavior: every scanned standard `.razor` component in BaseLib, Charts, Mermaid, and OverlayLib has an owner group route or documented compatibility/asset exception.
- Disallowed shallow implementation: count only components already shown in the sandbox and silently ignore the rest.
- Failing-first test: SB01 inventory showed sampled sandbox coverage rather than one-to-one coverage ownership.
- Passing test: `proof/SB05/transcripts/sb05-verifier.txt` prints `SB05-INV-003`.
- Changed source files: `codex/bundles/StandardComponents_PublishingReadiness_v1/scripts/build-sb05-coverage.mjs`.
- Production assertions: `proof/SB05/data/standard-component-coverage.json` has 169 component rows: 53 covered, 91 planned-route, and 25 documented-exception.
- Red-team negative case: a component row with missing owner group, missing route, invalid status, or leaked Canvas/WebGL path fails the verifier.
- Downstream dependency check: SB06-SB09 can harden planned-route rows by group without redoing the taxonomy.

## SB05-INV-004 Route Smoke Covers Standard And Focused Routes

- Invariant ID: `SB05-INV-004`
- Source raw note: RAW10.
- Expected behavior: every standard group route plus the sandbox root, `groups/coverage`, `groups/navigation/tabs`, and `groups/layout/composition` can load at desktop and mobile widths without page overflow.
- Disallowed shallow implementation: rely on source inspection or a single desktop screenshot.
- Failing-first test: route smoke initially failed on Foundations, Navigation mobile tabs, Tabs Lab mobile shells, and Layout Composition mobile Row/Column surfaces.
- Passing test: `proof/SB05/transcripts/sb05-route-smoke.txt` contains desktop and mobile `PASS` rows for every route in `standard-component-coverage.json`.
- Changed source files: `Components/Pages/Foundations.razor`, `Navigation.razor`, `NavigationTabs.razor`, `LayoutComposition.razor`, and `wwwroot/sandbox.css`.
- Production assertions: visual repairs replaced an invalid icon token, bounded demo grid minimums, wrapped mobile navigation tabs, contained tabs lab shells, and collapsed layout composition rows at narrow widths.
- Red-team negative case: a route with page overflow, clipped ordinary elements, HTTP failure, or too little rendered content fails route smoke.
- Downstream dependency check: the route matrix is now safe enough for SB06-SB09 visual hardening to use.

## SB05-INV-005 Playwright MCP Screenshots Back The Visual Gate

- Invariant ID: `SB05-INV-005`
- Source raw note: RAW10.
- Expected behavior: Playwright MCP screenshots prove the group index and a representative focused route at desktop and mobile widths.
- Disallowed shallow implementation: mark screenshots captured without inspecting overflow metrics and saved proof images.
- Failing-first test: the route smoke failures reopened styling/layout fixes before screenshots were accepted.
- Passing test: `proof/SB05/transcripts/sb05-playwright-mcp-visual.txt` and `proof/SB05/data/sb05-visual-smoke.json` record four passed captures.
- Changed source files: `proof/SB05/screenshots/mcp/*.png`, `proof/SB05/data/sb05-visual-smoke.json`.
- Production assertions: coverage index exposes 10 standard rows; layout composition exposes 4 comparison panels; all four captures have `pageHorizontalOverflow=false` and `overflowCount=0`.
- Red-team negative case: missing screenshot files, non-MCP tool marker, or a capture without visual inspection fails `verify-sb05.mjs`.
- Downstream dependency check: SB11 can expand this screenshot matrix to every standard route and scenario.
