# SB05 Proof Manifest

## Scope

Subbundle: `05-sandbox-taxonomy-and-standard-coverage-expansion`

Inputs closed:

- RAW03: keep this bundle focused on standard components, not WebGL/Canvas implementation.
- RAW04: analyze missing sandbox components and logical grouping.
- RAW10: require real Playwright route and screenshot proof, including mobile behavior.

## Source Changes

- `repo://src/CanDoItAll.Components.Sandbox/SandboxCatalogRegistry.cs`
  - Added standard/deferred proof scope helpers and kept Canvas out of standard route/example counts.
- `repo://src/CanDoItAll.Components.Sandbox/Components/Layout/MainLayout.razor`
  - Split sidebar navigation into standard proof groups and deferred proof scope.
- `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Home.razor`
  - Updated catalog counts and group list to standard-only proof scope.
- `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Coverage.razor`
  - Added sandbox route `groups/coverage` with stable test IDs for standard groups, focused routes, and deferred scope.
- `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Foundations.razor`
  - Replaced invalid icon demo token that rendered as oversized material text.
- `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Navigation.razor`
  - Wrapped group-demo tabs on mobile.
- `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/NavigationTabs.razor`
  - Contained narrow wrap/scroll tab shells with `w-full min-w-0`.
- `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/LayoutComposition.razor`
  - Collapsed Row/Column comparison layout at narrow widths and contained shared hero surfaces.
- `repo://src/CanDoItAll.Components.Sandbox/wwwroot/sandbox.css`
  - Bounded nested sandbox grid track minimums with `min(100%, ...)` and added icon tile containment.

## Semantic Contract

- `bundle://proof/SB05/semantic-invariants.md`

## Proof Scripts And Data

- Coverage generator: `bundle://scripts/build-sb05-coverage.mjs`
  - SHA-256 `890C8735B684E3E61BF8E30D0BFEAFB8C59D601E21B5067FDBE5D21A5D95ACAE`.
- Semantic verifier: `bundle://scripts/verify-sb05.mjs`
  - SHA-256 `0EC6291CA61C45BF79C44334DE38CE9347BFE2ACD355A38DC17F4583BA2F7299`.
- Coverage JSON: `bundle://proof/SB05/data/standard-component-coverage.json`
  - 169 standard component rows.
  - 53 `covered`, 91 `planned-route`, 25 `documented-exception`.
  - CanvasLib/WebGlLib/WebGlRunLib/WebGlSandbox listed as deferred projects, not standard rows.
- Coverage markdown: `bundle://proof/SB05/data/standard-component-coverage.md`
- Visual smoke JSON: `bundle://proof/SB05/data/sb05-visual-smoke.json`
- Changed-file hashes: `bundle://proof/SB05/transcripts/sb05-changed-file-hashes.txt`
- Screenshot hashes: `bundle://proof/SB05/transcripts/sb05-screenshot-hashes.txt`

## Validation

- Passing transcript: `bundle://proof/SB05/transcripts/sb05-verifier.txt`.
- Failing-first: N/A process/non-production proof normalization; SB05 negative coverage and route-smoke cases are documented in `bundle://proof/SB05/semantic-invariants.md`.
- Sandbox build: `proof/SB05/transcripts/sb05-components-sandbox-build.txt`
  - `Build succeeded`, `0 Warning(s)`, `0 Error(s)`.
- Source assertions: `proof/SB05/transcripts/sb05-source-assertions.txt`
  - Standard groups exclude Canvas.
  - Coverage route is linked.
  - Every coverage JSON row has owner group/route.
  - Deferred Canvas/WebGL source is excluded from standard rows.
- Route smoke: `proof/SB05/transcripts/sb05-route-smoke.txt`
  - 14 standard/index/focused routes captured at `1366x900` and `390x844`.
  - Every row ended with `PASS`, `pageHorizontalOverflow=false`, and `overflowCount=0`.
- Playwright MCP visual proof: `proof/SB05/transcripts/sb05-playwright-mcp-visual.txt`
  - Sandbox route `groups/coverage` at `1366x900` and `390x844`.
  - Sandbox route `groups/layout/composition` at `1366x900` and `390x844`.
  - All four captures visually inspected and passed.
- Semantic verifier: `proof/SB05/transcripts/sb05-verifier.txt`
  - `SB05-INV-001` through `SB05-INV-005` passed.
- Anti-stub audit: `proof/SB05/transcripts/sb05-anti-stub-audit.txt`
  - No `TODO`, `NotImplemented`, explicit stub, placeholder, or fake implementation matches in SB05 changed implementation files.

## Screenshots

- `proof/SB05/screenshots/mcp/sb05-coverage-1366.png`
- `proof/SB05/screenshots/mcp/sb05-coverage-390.png`
- `proof/SB05/screenshots/mcp/sb05-layout-composition-1366.png`
- `proof/SB05/screenshots/mcp/sb05-layout-composition-390.png`

## Visual Repairs From Proof

- Foundations icon demo no longer uses `triangle_exclamation`, which rendered as a long material icon text token.
- Sandbox demo grids no longer overflow narrow nested frames because track minimums are bounded.
- Navigation group tabs wrap on mobile while the dedicated tabs lab remains the scroll-mode proof surface.
- Tabs lab narrow shells now respect the mobile sandbox frame width.
- Layout composition Row/Column demos collapse at narrow widths and no longer overflow mobile screenshots.

## Raw Note Closure

- "standard components and not webgl and canvas": solved for sandbox taxonomy and generated coverage scope; Canvas/WebGL implementation work remains explicitly deferred.
- "components sandbox might missing some components": solved by generated coverage ownership for 169 standard component rows with owner route or documented exception.
- "splitting of components into groups": solved by standard group ownership across Foundations, Inputs, Actions, Navigation, Feedback, Layout, Data Display, Charts, Mermaid, and Overlays.
- "real validations with playwright mcp and screenshots": solved for SB05 route/index proof with Playwright MCP screenshots and route smoke; full one-by-one component screenshots continue in SB06-SB11.
