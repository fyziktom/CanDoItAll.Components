# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository scope

CanDoItAll.Components is a Blazor UI component library styled with Tailwind CSS. It owns reusable component packages, their static assets, package metadata, tests, and maintained visual samples (Sandbox). It does **not** own application-specific workflows or the file-provider/editor packages maintained by `CanDoItAll.FileTools` (separate repo).

The primary consumer of these packages is the `CanDoItAll` application, a sibling repo. Public-interface changes here (component parameters, removed/renamed types, changed CSS class names) are felt there first — write `CHANGELOG.md` entries with that consumer in mind, and treat "breaking" as "breaks CanDoItAll," not just "breaks in theory."

There is a shared top-level `AGENTS.md` in a sibling `CanDoItAll.SharedInfo` repo, holding reviewed cross-repo shared standards, and an `$apply-candoitall-shared-standards` Codex skill command (checking a `CANDOITALL_SHAREDINFO_ROOT` env var and nearby sibling locations) for applying those standards.

## Rules

We are working toward **2.0.1**. Two rule sets apply: rules for day-to-day development, and rules for refactoring. Read both before touching component or Tailwind code.

### Development rules

1. **Changelog every change.** Every change gets an entry in `CHANGELOG.md`, filed under: version (currently the `[Unreleased]` section heading toward `2.0.1`) → sub-project (package name, e.g. BaseLib, CanvasLib, Tailwind) → `Public interface` or `Internal` heading. Use Added / Changed / Removed. Entries must be concise but tell a consumer *how to update their code* — not just what happened. Mark anything breaking explicitly (`**Breaking:**`) and say what replaces it. Hard-wrap each entry's prose to roughly 100–120 characters per line (continuation lines indented to align under the bullet's text, not re-indented to the margin) rather than writing it as one unbroken line — a single 300+ character line renders as one unbroken span in diff viewers, which is slow and glitchy to review.
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

   `--color-chrome-*` (defined in `Tailwind/theme.css`) is the theme-aware neutral scale for structural application chrome: page backgrounds, surfaces, borders, headings, muted text, dividers, and metadata. It inverts at each `data-ui-theme` boundary (step 50 <-> 950, 100 <-> 900, ... 500 stays fixed), so a nested theme scope must restate its own mapping rather than inherit one — see the "Duplicate required for theme nesting" comments in `theme.css`. Use it instead of Tailwind's raw `slate-*` utilities/tokens in BaseLib and Sandbox markup and CSS. Values that must stay fixed across both themes (shadows, the permanently-dark nav rail) consume `--color-neutral-*` directly instead, since routing them through `--color-chrome-*` would incorrectly invert them in dark mode.

   `--cus-*`/semantic `--color-*` *(target — the raw palette and semantic-mapping `@theme` blocks have been ported into `theme.css` verbatim from `input-test.css`'s prototype; nothing in the file consumes them yet)*: `theme.css` now carries a `/*CUSTOM COLORS, Should not be used directly*/ @theme { ... }` block (the `--cus-lavender-blue-*`, `--cus-pervenche-*`, `--cus-iris-*`, `--cus-photon-barrier-*`, etc. raw OKLCH ramps) followed by a `/*SEMANTIC COLORS */ @theme { ... }` block (`--color-info-*`, `--color-success-*`, `--color-warn-*`, `--color-danger-*`, `--color-primary-*`, `--color-secondary-*`, `--color-tertiary-*`, plus the existing `--color-chrome-*` mapping), matching `input-test.css` exactly. This is a landing step only: the existing `--tone-*` tokens (and any `--role-*`/`.role--*` forwarding layer) still need to be pointed at these new semantic colors in a follow-up change before they affect anything rendered. `Tailwind/input-test.css` remains the prototype reference for this convention.
4. **No application-specific semantics in shared components** *(target — not fully applied yet)*. Variant/enum members and other public component vocabulary must describe a generic role or style, never a specific consuming application's concept (e.g. no `TabsVariant.WorkspaceTertiary`). When you find one, replace it with a generic equivalent and record the rename/removal in `CHANGELOG.md` as breaking.
5. **Standardized appearance properties.** A component's visual presentation should be driven primarily by four standardized parameter roles, not by ad hoc or application-specific enum members:
    - `Size` — physical dimension scale (e.g. `Small`/`Medium`/`Large`), per existing convention (`ButtonSize`, etc.).
    - `Density` — spacing/scale of a component's own chrome (e.g. `Normal`/`Compact`, some components may add `Comfortable`). `Normal` is always the default. Each component family defines its own `<Component>Density` enum next to its other primitives (mirrors the existing per-family `Size` convention) rather than sharing one repo-wide enum. `ButtonDensity` is the reference implementation.
    - `Variant` — a genuinely distinct visual/stylistic treatment (e.g. `Filled`/`Outlined`/`Text`), never a consuming application's concept (ties into rule 4 above).
    - `Rounded` — corner radius scale (`None`/`Default`/`Medium`/`Large`), increasing in that order (`None` is square). `Default` is the smallest non-zero radius and the parameter's default value. Each component family defines its own `<Component>Rounded` enum next to its other primitives, mirroring the `Size`/`Density` convention. `ButtonRounded` is the reference implementation.

   When a component's existing `Variant` (or similar) enum mixes spacing concerns with app-specific shell/theme concepts, split it: fold the spacing-only members into a new `Density` parameter and remove the app-specific members, replacing app-level shell theming with consumer-supplied `Class`/`Style`.
6. Tailwind folder/file naming mirrors `src/<Package>/Components/<Group>/<Component>.razor`, in lowercase-kebab (e.g. `buttons/copy-button.css` for `Buttons/CopyButton.razor`).
7. A Tailwind selector with no current component owner moves to that group's `compatibility/` subfolder instead of being deleted, until its removal is proven safe.
8. **Shared JS theme-token module.** JS that needs a theme-aware color (not a one-off literal) reads it from `window.CanDoItAll.themeTokens` (`src/CanDoItAll.Components.BaseLib/wwwroot/js/theme/theme-tokens.js`) rather than hand-rolling `getComputedStyle`/`getPropertyValue` per file:
    - `readTokens(hostElement, tokenMap)` resolves CSS custom properties from `hostElement` — never `document.documentElement` — because `ThemeHost` can stamp `data-ui-theme` on any subtree, and a component nested inside one must read its own ancestor's value, not the page root's.
    - `watchTheme(hostElement, onChange)` observes the nearest `data-ui-theme` ancestor and invokes `onChange` when it changes, so a component's rendered colors stay live across a theme flip instead of only refreshing on remount. Always store and call the returned `disconnect()` on component teardown.
    - Any package that already depends on `CanDoItAll.Components.BaseLib` (per the dependency layering in this file's Architecture section) can reach the module via `_content/CanDoItAll.Components.BaseLib/js/theme/theme-tokens.js`, composed with a plain `<script src="@Assets[...]">` tag ahead of the consuming script — the same pattern `GanttChartBodyAssets.razor` already uses for `CanvasRuntimeBodyAssets`. `gantt-chart.js` is the reference implementation.
    - **Degrade, don't throw, when the module is absent.** A consuming package's color-reading code must fall back to its pre-adoption behavior (an inline `getComputedStyle`/`getPropertyValue` read, no live reactivity) when `window.CanDoItAll.themeTokens` isn't loaded, rather than throwing — mirroring how `IncludeBaseLibStyles` already lets a consumer opt out of BaseLib's CSS without breaking. Pair the script tag with its own `Include*` opt-out parameter on the asset-composition component (see `GanttChartBodyAssets.IncludeThemeTokens`) so the degrade path is reachable deliberately, not just accidentally.
9. **`--<pkg>-*` / `--ui-<pkg>-*` two-layer tokens for non-BaseLib packages' own scoped CSS.** A package below BaseLib in the dependency graph but above it (CanvasLib, Gantt, WebGlLib, ...) that needs its own component-scoped color palette (in a `.razor.css` file, which sits outside Tailwind's source scan — see the Tailwind styling section) follows this pattern rather than hardcoding raw colors directly, per rule 2:
    - The component's own scoped CSS keeps its existing internal prefix (e.g. Gantt's `--gantt-*`, read by its own JS via the theme-tokens module in rule 8 above) — this name never changes, so it stays a stable read target for that package's own code.
    - Each `--<pkg>-<token>` is declared exactly once, as `var(--ui-<pkg>-<token>, <original literal>)` — the literal is the fallback for when BaseLib's CSS isn't loaded (`IncludeBaseLibStyles="false"`), not a second source of truth.
    - `--ui-<pkg>-<token>` is defined in `Tailwind/theme.css` (using the target `--ui-` "component variables" namespace from rule 3, even though the rest of `theme.css` is still mid-migration off `--cad-*`) — in the light block (`:root, [data-ui-theme="light"]`) always, and additionally in the dark block (`[data-ui-theme="dark"]`) **only** for tokens that represent structural chrome (a surface/background/border/text color) rather than paint atop an already-colored element (an accent, a badge fill, a stroke on a colored shape). Paint tokens declared once, matching how `--tone-*` solid colors and `--ui-nav-*` already behave in this file.
    - `--ui-<pkg>-*` is the **public override surface**: since custom properties cross scoped-CSS boundaries and cascade normally, a consumer (or a future `CanDoItAll` app) changes the package's palette by editing/overriding `--ui-<pkg>-*` in `theme.css` alone — the component's own `.razor.css` never needs a touch. `GanttChart.razor.css`/`Tailwind/theme.css`'s `--ui-gantt-*` block is the reference implementation.
    - This intentionally means a consumer who both keeps a `data-ui-theme` ancestor *and* sets `IncludeBaseLibStyles="false"` gets the package's un-themed literal fallback regardless of the attribute's value for structural tokens — the override surface requires the definer (`theme.css`) to be loaded. This is a deliberate, documented tradeoff, not a bug.
    - **Exception: `--ui-nav-*` is BaseLib-internal, not a `<pkg>` instance of this pattern.** `SideMenu.razor`, `HelpPopover.razor`, and `TagEditor.razor` are BaseLib's own components (not a package below BaseLib in the dependency graph), so they have no `--<pkg>-*` layer of their own and no `IncludeBaseLibStyles` fallback-literal concern — `Tailwind/navigation/side-menu.css` and `Tailwind/feedback/help-popover.css` read `--ui-nav-*` directly. It still follows this rule's other conventions (declared once in the light block, since it's the theme-fixed "permanently-dark nav rail" case from rule 3) and is still the public override surface for the nav rail's palette; it just isn't gated behind a package dependency layer the way `--ui-gantt-*`/`--ui-mermaid-*`/`--ui-canvas-*`/`--ui-calendar-*` are.
    - **One package can host more than one `--ui-<pkg>-*` namespace when it has more than one component-scoped palette.** CanvasLib's `CanvasCalendar` (`--zy-cal-*` internal prefix, JS-injected stylesheet in `calendar/core/01-foundation.js` plus `<canvas>`-painted colors in `zy-canvas-primitives.js`) reads `--ui-calendar-*`, a sibling of — not an instance of — `CanvasWorkbench`'s `--ui-canvas-*` in the same package, since the two components have unrelated palettes. `Tailwind/theme.css`'s `--ui-calendar-*` block is the reference implementation for a canvas-painted (not just DOM-CSS) consumer of this pattern.
10. **Dynamic-`import()` packages still compose `theme-tokens.js` through a `*BodyAssets` component.** A package whose own JS loads via `JS.InvokeAsync<IJSObjectReference>("import", ...)` from inside the component (an ES module, e.g. Mermaid's `mermaidDiagram.js`) rather than a `<script src>` tag still needs rule 8's `theme-tokens.js` composed as a plain `<script>` — that module sets `window.CanDoItAll.themeTokens` as a global and the dynamically-imported module reads it at call time, so the only ordering requirement is that the body-assets script tag runs before the component's first render, not before the dynamic import itself. Give the `*BodyAssets` component the same `IncludeThemeTokens` opt-out as `GanttChartBodyAssets`; `MermaidBodyAssets` is the reference implementation.
11. **A component wrapping a third-party library's own internal theme concept** (e.g. mermaid.js's `theme` config, distinct from `--ui-<pkg>-*` chrome tokens) can auto-follow `data-ui-theme` via an `"auto"` sentinel default on the relevant option, resolved by reading the nearest `data-ui-theme` ancestor's attribute directly rather than through `readTokens` (it's a discrete mode name, not a CSS color) — with live updates wired through `watchTheme` (rule 8) routed back into the component's existing render pipeline (a `[JSInvokable]` callback that bumps a render-key field and calls `StateHasChanged`, not a JS-driven re-render) so existing render-queue serialization and lifecycle events stay intact. An explicit non-`"auto"` value always overrides and skips the watch entirely. `MermaidDiagramOptions.Theme` is the reference implementation.
13. **`data-ui-theme` is the canonical theme-boundary attribute**, stamped by `ThemeHost` (`data-ui-theme="light"|"dark"`, replacing the earlier `data-cad-theme`) alongside its `.ui-theme-host` wrapper class (replacing `.cad-theme-host`). Each `[data-ui-theme="light"|"dark"]` block in `Tailwind/theme.css` also sets `--ui-theme: light|dark` — a discriminator custom property that exists specifically to drive Tailwind's `dark:`/`light:` utility variants via CSS style container queries (`Tailwind/input-base.css` and `input-sandbox.css` both redefine `@custom-variant dark`/`@custom-variant light` as `@container style(--ui-theme: <value>) { @slot; }`, mirroring `Tailwind/input-test.css`'s prototype). Style container queries don't need an explicit `container-type` opt-in, so this works against any ancestor `[data-ui-theme]` scope, including a nested `ThemeHost` — a `dark:` utility inside a nested light `ThemeHost` correctly resolves to light without extra plumbing, unlike a plain `[data-ui-theme="dark"] ...` attribute-selector rule (still used for non-utility CSS, e.g. the `--color-chrome-*`/`--tone-*` token blocks in `theme.css`), which still needs the manual "Duplicate required for theme nesting" restatement described in rule 3's `--color-chrome-*` guidance — that restatement is a property-inheritance concern independent of the attribute name and is unaffected by this rule. Prefer `dark:`/`light:` utility classes for new component markup that needs to branch on theme; keep the raw `[data-ui-theme="..."]` selector pattern for `theme.css`'s own custom-property blocks.
14. **A package whose styling lives in plain `wwwroot/css/**` files (not `.razor.css`) and whose `*BodyAssets`/`*HeadAssets` components are generator-emitted** (e.g. CanvasLib's `tools/canvaslib/build-assets.cjs` from `asset-manifest.json`) still follows rules 8–9 the same way a hand-authored package does — the two-layer `--<pkg>-*`/`--ui-<pkg>-*` tokens live in the plain CSS files exactly as they would in a `.razor.css` file, declared on the package's own root wrapper class(es) rather than `:root` (see rule 9's per-host-element resolution requirement — `:root` would resolve `--ui-<pkg>-*` against the document root's `data-ui-theme` instead of a nested `ThemeHost` ancestor's). The one difference is *how* `theme-tokens.js` gets composed: add the `<script>` tag and its `IncludeThemeTokens` parameter inside the generator's body-component builder function (ahead of the package's own runtime scripts, same ordering as a hand-authored `*BodyAssets`), then regenerate with `node tools/<pkg>/build-assets.cjs` — hand-editing the generated `.razor` file directly gets silently overwritten on the next run. **If the package also has a separate `verify-assets.cjs`, check whether it duplicates the body-component-building logic rather than importing it from `build-assets.cjs`** (CanvasLib's does) — if so, any change to `build-assets.cjs`'s output shape must be mirrored there by hand or `verify-assets.cjs` reports the freshly-generated file as stale. For canvas-painted colors (a `<canvas>` 2D context, not CSS), mirror rule 8's `resolveColors(host)`/`watchTheme` pattern in the package's own JS module scope, keyed off the package's root wrapper element (e.g. `.cw-workbench-shell`) as the theming host, with the resolved colors threaded through the existing per-instance state object (e.g. `state.colors`) rather than a module-level global, so multiple instances on one page can resolve independently.

15. **Bake a Tailwind plugin's static output into a generated, `@layer components`-wrapped file instead of registering it live, when its own emitted rules would otherwise go unlayered.** Some Tailwind plugins (e.g. `@tailwindcss/forms`, class strategy) emit their own classes outside any `@layer` block — verify this by building the entry CSS and inspecting the compiled output before assuming otherwise. This repo's `input-base.css` also compiles `@tailwind utilities;` output unlayered in practice (no explicit `@layer utilities { ... }` wrapper appears in the built CSS), so if a shared component-primitive class (e.g. `.ui-input`) also had to go unlayered to win the cascade against such a plugin, any consumer's `Class="..."` utility override on the components using that primitive would silently and permanently lose to it, regardless of source order. Avoid this by: writing a `tools/<area>/generate-<name>.cjs` script that builds a throwaway Tailwind entry (registering the plugin via `@plugin` against a small fixture file listing only the classes actually used), extracts the plugin's compiled rule bodies, wraps them in `@layer components { ... }`, and writes the result to a checked-in `Tailwind/<group>/<name>.generated.css` file headed with a `Do not edit by hand -- regenerate via npm run generate:<name>` comment; expose it as an `npm run generate:<name>` script (root `package.json`, alongside `canvaslib:build-assets` etc.); import the generated file *before* any hand-authored file whose own `@layer components` rules must win over the plugin's defaults (same layer, later import wins the tie). `tools/forms/generate-form-plugin-css.cjs` (`npm run generate:forms`, producing `Tailwind/forms/form-plugin.generated.css`, consumed by `Tailwind/forms/form-controls.css`'s `.ui-input`/`.ui-select`/`.ui-checkbox`) is the reference implementation.

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
- Keep sibling repositories (e.g. `CanDoItAll`, `CanDoItAll.FileTools`, `CanDoItAll.SharedInfo`) read-only unless the user explicitly requests a multi-repo change.
- Preserve repository-specific changes unrelated to the active task — don't revert or clean up unrelated in-progress work you find while editing a file.

### Package versioning

All packages share one version, set via `CanDoItAllPackageBaseVersion` in `Directory.Build.props`. `Build-NuGets.ps1` prints the effective version; use `-Version` to override for one invocation (local/validation builds) without editing the committed value, which should only change for real releases.
