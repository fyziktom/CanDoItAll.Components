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
2. **Centralized color.** All raw/unique color values (hex, `oklch()`, `rgb()` literals, named CSS colors) are defined exactly once, in `Tailwind/theme.css`. No other CSS file may introduce a new raw color value. Deriving from an existing token — `color-mix()`, opacity/alpha modifiers, `light-dark()` pairing of existing tokens — is fine anywhere; inventing a new base color is not. Narrow exemption: a fallback literal that is only used when a `theme.css`-defined token can't resolve — a JS default passed to a CSS custom-property read (e.g. `readTokens`'s `tokenMap`, rule 8 below), or the second argument of a `var(--ui-<pkg>-*, <literal>)` read in a non-BaseLib package's own scoped CSS (rule 9 below) — is allowed outside `theme.css`, since it mirrors a token that already lives there; it is not a second source of truth.
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

   `--color-chrome-*` (defined in `Tailwind/theme.css`) is the theme-aware neutral scale for structural application chrome: page backgrounds, surfaces, borders, headings, muted text, dividers, and metadata. It inverts at each `data-cad-theme` boundary (step 50 <-> 950, 100 <-> 900, ... 500 stays fixed), so a nested theme scope must restate its own mapping rather than inherit one — see the "Duplicate required for theme nesting" comments in `theme.css`. Use it instead of Tailwind's raw `slate-*` utilities/tokens in BaseLib and Sandbox markup and CSS. Values that must stay fixed across both themes (shadows, the permanently-dark nav rail) consume `--color-neutral-*` directly instead, since routing them through `--color-chrome-*` would incorrectly invert them in dark mode.
4. **No application-specific semantics in shared components** *(target — not fully applied yet)*. Variant/enum members and other public component vocabulary must describe a generic role or style, never a specific consuming application's concept (e.g. no `TabsVariant.WorkspaceTertiary`). When you find one, replace it with a generic equivalent and record the rename/removal in `CHANGELOG.md` as breaking.
5. **Standardized appearance properties.** A component's visual presentation should be driven primarily by three standardized parameter roles, not by ad hoc or application-specific enum members:
   - `Size` — physical dimension scale (e.g. `Small`/`Medium`/`Large`), per existing convention (`ButtonSize`, etc.).
   - `Density` — spacing/scale of a component's own chrome (e.g. `Normal`/`Compact`, some components may add `Comfortable`). `Normal` is always the default. Each component family defines its own `<Component>Density` enum next to its other primitives (mirrors the existing per-family `Size` convention) rather than sharing one repo-wide enum.
   - `Variant` — a genuinely distinct visual/stylistic treatment (e.g. `Filled`/`Outlined`/`Text`), never a consuming application's concept (ties into rule 4 above).

   When a component's existing `Variant` (or similar) enum mixes spacing concerns with app-specific shell/theme concepts, split it: fold the spacing-only members into a new `Density` parameter and remove the app-specific members, replacing app-level shell theming with consumer-supplied `Class`/`Style`.
6. Tailwind folder/file naming mirrors `src/<Package>/Components/<Group>/<Component>.razor`, in lowercase-kebab (e.g. `buttons/copy-button.css` for `Buttons/CopyButton.razor`).
7. A Tailwind selector with no current component owner moves to that group's `compatibility/` subfolder instead of being deleted, until its removal is proven safe.
8. **Shared JS theme-token module.** JS that needs a theme-aware color (not a one-off literal) reads it from `window.CanDoItAll.themeTokens` (`src/CanDoItAll.Components.BaseLib/wwwroot/js/theme/theme-tokens.js`) rather than hand-rolling `getComputedStyle`/`getPropertyValue` per file:
   - `readTokens(hostElement, tokenMap)` resolves CSS custom properties from `hostElement` — never `document.documentElement` — because `ThemeHost` can stamp `data-cad-theme` on any subtree, and a component nested inside one must read its own ancestor's value, not the page root's.
   - `watchTheme(hostElement, onChange)` observes the nearest `data-cad-theme` ancestor and invokes `onChange` when it changes, so a component's rendered colors stay live across a theme flip instead of only refreshing on remount. Always store and call the returned `disconnect()` on component teardown.
   - Any package that already depends on `CanDoItAll.Components.BaseLib` (per the dependency layering in this file's Architecture section) can reach the module via `_content/CanDoItAll.Components.BaseLib/js/theme/theme-tokens.js`, composed with a plain `<script src="@Assets[...]">` tag ahead of the consuming script — the same pattern `GanttChartBodyAssets.razor` already uses for `CanvasRuntimeBodyAssets`. `gantt-chart.js` is the reference implementation.
   - **Degrade, don't throw, when the module is absent.** A consuming package's color-reading code must fall back to its pre-adoption behavior (an inline `getComputedStyle`/`getPropertyValue` read, no live reactivity) when `window.CanDoItAll.themeTokens` isn't loaded, rather than throwing — mirroring how `IncludeBaseLibStyles` already lets a consumer opt out of BaseLib's CSS without breaking. Pair the script tag with its own `Include*` opt-out parameter on the asset-composition component (see `GanttChartBodyAssets.IncludeThemeTokens`) so the degrade path is reachable deliberately, not just accidentally.
9. **`--<pkg>-*` / `--ui-<pkg>-*` two-layer tokens for non-BaseLib packages' own scoped CSS.** A package below BaseLib in the dependency graph but above it (CanvasLib, Gantt, WebGlLib, ...) that needs its own component-scoped color palette (in a `.razor.css` file, which sits outside Tailwind's source scan — see the Tailwind styling section) follows this pattern rather than hardcoding raw colors directly, per rule 2:
   - The component's own scoped CSS keeps its existing internal prefix (e.g. Gantt's `--gantt-*`, read by its own JS via the theme-tokens module in rule 8 above) — this name never changes, so it stays a stable read target for that package's own code.
   - Each `--<pkg>-<token>` is declared exactly once, as `var(--ui-<pkg>-<token>, <original literal>)` — the literal is the fallback for when BaseLib's CSS isn't loaded (`IncludeBaseLibStyles="false"`), not a second source of truth.
   - `--ui-<pkg>-<token>` is defined in `Tailwind/theme.css` (using the target `--ui-` "component variables" namespace from rule 3, even though the rest of `theme.css` is still mid-migration off `--cad-*`) — in the light block (`:root, [data-cad-theme="light"]`) always, and additionally in the dark block (`[data-cad-theme="dark"]`) **only** for tokens that represent structural chrome (a surface/background/border/text color) rather than paint atop an already-colored element (an accent, a badge fill, a stroke on a colored shape). Paint tokens declared once, matching how `--cad-tone-*` solid colors and `--cad-nav-*` already behave in this file.
   - `--ui-<pkg>-*` is the **public override surface**: since custom properties cross scoped-CSS boundaries and cascade normally, a consumer (or a future `CanDoItAll` app) changes the package's palette by editing/overriding `--ui-<pkg>-*` in `theme.css` alone — the component's own `.razor.css` never needs a touch. `GanttChart.razor.css`/`Tailwind/theme.css`'s `--ui-gantt-*` block is the reference implementation.
   - This intentionally means a consumer who both keeps a `data-cad-theme` ancestor *and* sets `IncludeBaseLibStyles="false"` gets the package's un-themed literal fallback regardless of the attribute's value for structural tokens — the override surface requires the definer (`theme.css`) to be loaded. This is a deliberate, documented tradeoff, not a bug.

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
