# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository scope

CanDoItAll.Components is a Blazor UI component library styled with Tailwind CSS. It owns reusable component packages, their static assets, package metadata, tests, and maintained visual samples (Sandbox). It does **not** own application-specific workflows or the file-provider/editor packages maintained by `CanDoItAll.FileTools` (separate repo).

The primary consumer of these packages is the `CanDoItAll` application, a sibling repo. Public-interface changes here (component parameters, removed/renamed types, changed CSS class names) are felt there first — write `CHANGELOG.md` entries with that consumer in mind, and treat "breaking" as "breaks CanDoItAll," not just "breaks in theory."

A repo-root `AGENTS.md` also exists (it points at a `CanDoItAll.SharedInfo` clone for cross-repo shared standards). It is not used for this workflow — this file is the source of truth here; ignore `AGENTS.md`.

## Rules

We are working toward **2.0.0**. Two rule sets apply: rules for day-to-day development, and rules for refactoring. Read both before touching component or Tailwind code.

### Development rules

1. **Changelog every change.** Every change gets an entry in `CHANGELOG.md`, filed under: version (currently the `[Unreleased]` section heading toward `2.0.0`) → sub-project (package name, e.g. BaseLib, CanvasLib, Tailwind) → `Public interface` or `Internal` heading. Use Added / Changed / Removed. Entries must be concise but tell a consumer *how to update their code* — not just what happened. Mark anything breaking explicitly (`**Breaking:**`) and say what replaces it.
2. **Centralized color.** All raw/unique color values (hex, `oklch()`, `rgb()` literals, named CSS colors) are defined exactly once, in `Tailwind/theme.css`. No other CSS file may introduce a new raw color value. Deriving from an existing token — `color-mix()`, opacity/alpha modifiers, `light-dark()` pairing of existing tokens — is fine anywhere; inventing a new base color is not.
3. **CSS namespacing convention** *(target — being rolled out, not fully migrated yet; the reference block lives at the top of `Tailwind/input-test.css`, ahead of `theme.css`/`input-base.css`)*:
   - `--cus-` — custom colors
   - `--color-chrome-` — chrome colors
   - `--color-` — Tailwind color utilities
   - `--tw-prose-` — Tailwind typography plugin
   - `--tone-` — component colors
   - `--ui-` — component variables
   - `--role-` — role variables
   - `.ui-` — UI components
   - `.role-` — role settings
   - `.form-` — Tailwind forms plugin
   - `.prose` — Tailwind typography plugin

   New or touched CSS should follow this scheme. When refactoring a file that still uses an older prefix (e.g. `--cad-`), migrate it to the matching namespace above as part of that change rather than leaving it mixed.
4. **No application-specific semantics in shared components** *(target — not fully applied yet)*. Variant/enum members and other public component vocabulary must describe a generic role or style, never a specific consuming application's concept (e.g. no `TabsVariant.WorkspaceTertiary`). When you find one, replace it with a generic equivalent and record the rename/removal in `CHANGELOG.md` as breaking.
5. Tailwind folder/file naming mirrors `src/<Package>/Components/<Group>/<Component>.razor`, in lowercase-kebab (e.g. `buttons/copy-button.css` for `Buttons/CopyButton.razor`).
6. A Tailwind selector with no current component owner moves to that group's `compatibility/` subfolder instead of being deleted, until its removal is proven safe.

### Refactoring rules

1. **A refactor that introduces or changes a structural or naming convention must add or update the corresponding rule in this file's Development Rules, in the same change.** Don't apply a new pattern silently — write the rule down so the next refactor (by Claude or a human) converges on it instead of reinventing or contradicting it. This rule applies to itself: it was written because the Tailwind file-layout-by-component-group convention and the `compatibility/` subfolder convention were both applied before being documented here.
2. Every refactor gets a `CHANGELOG.md` entry per the changelog rule above, even when it only touches `Internal` structure and no `Public interface` changed (e.g. reorganizing Tailwind source files without changing any emitted class name).
3. When a rule is only partially applied across the codebase, mark it `(target — ...)` in this file rather than omitting it, and keep pushing existing code toward it opportunistically as you touch nearby files. Don't treat a target rule as optional for *new* code just because old code doesn't conform yet.
4. Prefer moving now-unused code to a `compatibility/` location over deleting it outright, until ownership and safe removal are confirmed — this mirrors the BaseLib compatibility-shim policy in `docs/standard-components-compatibility-policy.md`. (The BaseLib component-shim instance of this pattern was retired 2026-08-25 by explicit maintainer override of the SB12 gate; the rule still applies to future cases, e.g. Tailwind CSS.)

## Commands

```powershell
# Restore / build / test .NET solution
dotnet restore CanDoItAll.Components.slnx --configfile NuGet.config
dotnet build CanDoItAll.Components.slnx --configuration Release --no-restore
dotnet test CanDoItAll.Components.slnx --configuration Release --no-build

# Run a single test project or a filtered subset
dotnet test tests/CanDoItAll.Components.BaseLib.Tests --filter "FullyQualifiedName~ClassName.MethodName"

# Tailwind (must be rebuilt after any Tailwind/** input change)
npm ci
npm ci --prefix Tailwind
npm run build:tailwind          # writes src/CanDoItAll.Components.BaseLib/wwwroot/css/output.css
npm run watch:tailwind          # watch mode, run from Tailwind/ or via this script

# Verify generated/static assets after a build
npm run assets:verify

# Sandbox (visual catalog / regression host — check components here before/after changes)
dotnet run --project samples/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj
npm run watch:sandbox           # dotnet watch on the Sandbox

# Package all packable projects under src/ (restores, builds, tests, packs; runs tailwind:build first)
.\tools\deployment\nugets\Build-NuGets.ps1
.\tools\deployment\nugets\Build-NuGets.ps1 -Version "0.2.0" -PrereleaseSuffix "-preview.1"
```

Other notable npm scripts (see `package.json` for the full list): `canvaslib:verify-assets`, `webgllib:verify-assets`, `webgllib:audit-boundary`, `webglrunlib:audit-boundary`, `gantt:test-routing`, `gantt:test-grid`, `screenshots*` (Playwright-based visual diffing), `components-usage` (usage inventory).

## Architecture

### Package layering and dependency direction

```
Common (framework-neutral primitives, no Blazor deps)
  -> BaseLib (standard Blazor component contracts, StyledComponentBase)
    -> OverlayLib (draggable/resizable floating windows over ordinary pages)
    -> CanvasLib (stateful interactive canvas/workbench framework)
      -> Gantt, Mermaid, Charts, WebGlLib, WebGlRunLib (domain packages built on CanvasLib)
Sandbox -> all of the above (visual catalog / regression proof, never the other way)
```

`Common` must never depend on BaseLib, AppComponents, sandbox projects, WebGL, Canvas, or app services — it's the dependency-light floor every other package can assume. Domain packages (e.g. Gantt) depend on CanvasLib/BaseLib/Common but never on a product/consuming application, EF, HTTP, or persistence — they emit strongly-typed mutation *requests* and let the host accept/reject/transform them rather than owning two-way bound mutable state.

### Component base and styling contract

- `StyledComponentBase` (BaseLib) exposes `Class`, `Style`, `AdditionalAttributes` and merges them via `ComponentAttributes`/`CssClassBuilder` (Common). `ComponentAttributeExtensions` is a compatibility wrapper only — do not duplicate merge logic elsewhere.
- Contract behavior (merge order, empty-attribute cleanup, null handling) is pinned by `tests/CanDoItAll.Components.Common.Tests` and `tests/CanDoItAll.Components.BaseLib.Tests`.
- `docs/standard-components-compatibility-policy.md` records BaseLib compatibility shims that were kept for old call sites during publishing prep; the last 21 (`ProfileTagChip`, `SheetCard`, `ProfileField`, etc.) were removed 2026-08-25 ahead of the SB12 gate at explicit maintainer request — see the doc's closure note for replacements. Any future shim added there follows the same don't-remove-without-the-SB12-gate rule unless similarly waived.

### CanvasLib vs OverlayLib

Two different floating-window mechanisms exist and are not interchangeable:
- `OverlayWindow` (OverlayLib) — a bounded, draggable/resizable tool window over an ordinary page, respects the page's safe-top area.
- `CanvasFloatingWindow` (CanvasLib) — restricted to a `CanvasWorkbench` stage, placed in `CanvasWorkbench.OverlayContent`, and its geometry/visibility is tracked as `CanvasWorkbenchWindowState` inside `CanvasWorkbenchUiState` (alongside selection, viewport, layout state) rather than as a separate window system.

Choose CanvasLib only when the task is genuinely spatial (dependency graphs, plan authoring, dense calendars, network inspection) — not to decorate an ordinary settings/list/form page; start with BaseLib for those, and add OverlayLib only when a page-local tool must stay open over existing content.

CanvasLib's mental model: keep domain truth in .NET (map records to `CanvasWorkbenchSurface`/nodes/links/chrome + `CanvasWorkbenchUiState`), render with `CanvasWorkbench`, and treat component callbacks (`SelectionChanged`, `NodesMoved`, `NodeEdited`, `ContextActionRequested`, `ClipboardRequested`) as application commands — the browser runtime draws/manages the stage but is never the source of truth. See `docs/canvas/README.md` for the full guide.

### Tailwind styling

- `Tailwind/` is the single styling source for BaseLib's shared CSS; it is not for application/product-specific styling. Output builds to `src/CanDoItAll.Components.BaseLib/wwwroot/css/output.css`.
- Folder structure mirrors `src/CanDoItAll.Components.BaseLib/Components/` (lowercase-kebab), one CSS file per component (e.g. `buttons/copy-button.css` ↔ `Buttons/CopyButton.razor`). A `<folder>/compatibility/` subfolder holds selectors with no current component owner — check there before assuming a class is dead.
- Use `@apply` for reusable layout/sizing/spacing/typography/responsive behavior; keep raw CSS only where Tailwind can't express it safely (theme tokens/custom properties, `:focus-visible`/ARIA state selectors tied to variables, `color-mix()`/gradients/shadows that must stay themeable, component geometry exposed as part of the component's API, browser-integration rules like `scrollbar-gutter`). See `docs/standard-components-tailwind-policy.md`.
- `npm run apply -- <path>` (in `Tailwind/`) converts plain CSS to `@apply` via an early-stage tool (`css-to-tailwindcss4`, v0.0.x) — always review its diff manually; it's not idempotent on partially-converted files and can collapse multiple `@media` blocks into unreadable arbitrary-value lines. Workflow: `apply` one folder → `npm run build` → `npm run format` → read full diff.
- Styling changes need screenshot proof (desktop + narrow mobile) via the Sandbox for the affected component group, plus interaction proof for controls like tabs/dropdowns/menus — not just source inspection.

### Component proof expectations

- Reusable BaseLib changes need small/medium/large viewport coverage in the Sandbox when behavior is viewport-sensitive.
- Other libraries (Canvas-based, WebGL, etc.) target large-screen application use by default; preserve existing responsive behavior unless cross-viewport work is explicitly in scope.
- Prefer shared component parameters and Sandbox proof routes over application-specific structural CSS.
- **There are two Sandbox hosts, and both need to be checked.** `CanDoItAll.Components.Sandbox` runs server-side (SSR, via `dotnet run`/`watch:sandbox`); `CanDoItAll.Components.SandboxWasm` runs client-side (Blazor WebAssembly, via `watch:sandbox-wasm`) and is the one built and published to GitHub Pages (`npm run build:sandbox-wasm-static`, `npm run deploy:sandbox-wasm-gh-pages`). A change can render or behave correctly under SSR and break under WASM (or vice versa) — verify both before calling a Sandbox-visible change done, not just whichever one is already running.
- Screenshot-diffing tooling exists for visual regression checks: `npm run screenshots` (Sandbox, `tools/screenshots/`) and `npm run screenshots-app` (the main `CanDoItAll` app, `tools/screenshots/screenshots-app.config.json`). Both capture with Playwright, diff against the previous stored run with `odiff-bin`, and write a Markdown report — each to its own sibling storage repo (`storageRepoPath` in the respective config). Reach for these to verify a styling/layout change didn't regress other pages/components, not just the one you touched; see `tools/screenshots/README.md` for the full workflow.

### Repo layout

- `src/` — the packable component libraries (BaseLib, Common, CanvasLib, OverlayLib, Charts, Gantt, Mermaid, QRCode, WebGlLib, WebGlRunLib). Only these are packed; samples and tests never are.
- `samples/` — Sandbox apps (visual catalog/regression host) and focused viewers (e.g. WebGlLibOnlyViewer).
- `tests/` — one test project per library that has automated coverage.
- `tools/<area>/` — automation scripts (Node-based asset builders/auditors, screenshot tooling, packaging). New automation goes in a lowercase `tools/<area>` directory; `tools/pack-packages.ps1`, `scripts/pack-release.ps1`, and `scripts/webgl-engine` are compatibility paths — don't add new tooling beside them.
- `docs/` — architecture decisions (`docs/architecture/`, `docs/webgl/`), the Canvas guide (`docs/canvas/README.md`), and the foundation/compatibility/Tailwind policy docs referenced above.
- Generated packages/proof output belongs under ignored `artifacts/` or `output/` directories — never commit generated output, local settings, credentials, runtime state, or browser artifacts.

### Package versioning

All packages share one version, set via `CanDoItAllPackageBaseVersion` in `Directory.Build.props`. `Build-NuGets.ps1` prints the effective version; use `-Version` to override for one invocation (local/validation builds) without editing the committed value, which should only change for real releases.
