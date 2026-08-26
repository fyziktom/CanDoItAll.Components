# Changelog

All notable changes to this repository's packages are recorded here, per the changelog rule in `CLAUDE.md`.

## [Unreleased] — 2.0.0

### BaseLib

#### Internal

- Changed: Tailwind source files under `Tailwind/` now mirror
  `src/CanDoItAll.Components.BaseLib/Components/<Group>/<Component>.razor`, in lowercase-kebab (e.g.
  `buttons/copy-button.css` for `Buttons/CopyButton.razor`). No emitted class names changed.
- Changed: Tailwind selectors with no current component owner moved into per-group `compatibility/` subfolders (e.g.
  `cards/compatibility/sheet-card.css`) instead of being deleted. Check there before assuming a class is dead.
- Added: `wwwroot/js/theme/theme-tokens.js`, a shared JS module (`window.CanDoItAll.themeTokens`) for reading
  `--cad-*`/component-scoped CSS custom properties from a specific host element and reacting to `data-cad-theme`
  changes on that element's nearest themed ancestor (`readTokens(host, tokenMap)`, `watchTheme(host, onChange)`).
  Other packages that already depend on BaseLib (Gantt, CanvasLib, WebGlLib, OverlayLib) can reference it via
  `_content/CanDoItAll.Components.BaseLib/js/theme/theme-tokens.js`, following the same `<script src>` composition
  pattern already used for `CanvasRuntimeBodyAssets`. See the new "Shared JS theme-token module" development rule in
  `CLAUDE.md`.
- Removed: all 12 `Tailwind/**/compatibility/*.css` files and their `@import`s from `Tailwind/input-base.css` —
  `compatibility/theme.css`, `forms/compatibility/fields.css`, `forms/compatibility/sheet-field.css`,
  `layout/compatibility/stacks.css`, `layout/compatibility/radzen-layout.css`, `layout/compatibility/stats.css`,
  `cards/compatibility/cards.css`, `cards/compatibility/sheet-grid.css`, `cards/compatibility/sheet-card.css`,
  `cards/compatibility/sheet-section.css`, `navigation/compatibility/tabs.css`, `typography/compatibility/text.css`.
  Verified against a repo-wide grep: none of these selectors had any `.razor`/`.cs` call site. **Breaking
  (compatibility-policy deviation):** `SheetField`, `SheetGrid`, `SheetCard`, and `SheetSection`
  (`Components/{Forms,Cards}/Compatibility/`) still apply their `cad-`/`zy-` classes via `BuildAttributes(...)`, but
  those classes now have no CSS backing — this jumps ahead of the SB12 consumer-migration gate in
  `docs/standard-components-compatibility-policy.md` at explicit maintainer request. A consumer still instantiating
  these shims will see unstyled markup; migrate to `Card`/`SectionCard`/`Grid`/`FormField` per that doc's
  replacement column.
- Changed: `Tailwind/navigation/tabs.css` — `.cad-tabs--variant-modal-compact` renamed to
  `.cad-tabs--density-compact`. Removed `.cad-tabs--variant-workspace-secondary`,
  `.cad-tabs--variant-workspace-tertiary`, and `.cad-tabs--variant-workstation`, along with the
  `--cad-workstation-*` tokens they referenced in `Tailwind/theme.css`.
- Added: `--cus-*` raw-color palette and semantic `--color-*` mapping `@theme` blocks to `Tailwind/theme.css`,
  ported verbatim from `input-test.css`'s prototype (CLAUDE.md rule 3) — a `/*CUSTOM COLORS, Should not be used
  directly*/` block (`--cus-lavender-blue-*`, `--cus-pervenche-*`, `--cus-iris-*`, `--cus-photon-barrier-*`, and 6
  more full OKLCH ramps) and a `/*SEMANTIC COLORS */` block (`--color-info-*`, `--color-success-*`,
  `--color-warn-*`, `--color-danger-*`, `--color-primary-*`, `--color-secondary-*`, `--color-tertiary-*`, plus the
  existing `--color-chrome-*` mapping, now folded into this block). Landing step only — nothing in `theme.css`
  consumes these yet, so no existing token's resolved value changed; wiring `--tone-*` to them is a follow-up
  change.
- Changed: `Tailwind/theme.css`'s `--tone-secondary-*`/`-success-*`/`-info-*`/`-warning-*`/`-danger-*` tokens
  (both `-soft-*` and `-solid-*` variants, light and dark blocks) now derive from the semantic `--color-secondary-*`/
  `-success-*`/`-info-*`/`-warn-*`/`-danger-*` tokens instead of raw Tailwind color families (`fuchsia`/`emerald`/
  `green`/`sky`/`amber`/`rose`/`red`/`pink`, plus one raw `#0f62d6` hex) — the follow-up mentioned above. Most steps
  are visually unchanged (`success`/`danger` already mapped to `emerald`/`rose`), but `--tone-info-*` shifts from a
  `sky`-based blue to the `--cus-pervenche-*`-based `--color-info-*`, and `--tone-warning-*` shifts from `amber` to
  the `yellow`-based `--color-warn-*` — both intentional per the semantic mapping, not regressions.
- Changed: the theme-host structural reset (box-sizing, form-element font inheritance, media max-width) and base
  component style (`background`/`color` from `--tone-page-bg`/`--tone-text`) moved from `Tailwind/theme.css` into
  new `Tailwind/layout/theme-host.css`, per the Tailwind file-layout convention (CLAUDE.md rule 6) — `theme.css` now
  holds only the shared `--color-chrome-*`/token blocks. Purely a file move at the time; the class itself
  (`.cad-theme-host` → `.ui-theme-host`) was renamed in the same release, see the Public interface entry below.
- Added a generated-asset convention (new "Bake a Tailwind plugin's static output into a generated,
  `@layer components` file" development rule in `CLAUDE.md`): `tools/forms/generate-form-plugin-css.cjs`
  (`npm run generate:forms`) bakes `@tailwindcss/forms`' compiled class-strategy output
  (`.form-input`/`.form-select`/`.form-checkbox`) into the checked-in, `@layer components`-wrapped
  `Tailwind/forms/form-plugin.generated.css`, instead of registering the plugin live via `@plugin`. Registering it
  live would emit those rules unlayered (verified by building `Tailwind/input-test.css`'s prototype and inspecting
  the compiled CSS), which would permanently defeat any consumer `Class="..."` utility override on the components
  that use them, regardless of source order. Regenerate via `npm run generate:forms` after editing
  `tools/forms/fixture.html` or bumping the `@tailwindcss/forms` version.
- Added `Tailwind/forms/form-controls.css` (`.ui-input`/`.ui-select`/`.ui-checkbox`, `@layer components`), ported
  from `Tailwind/input-test.css`'s prototype (CLAUDE.md rule 3) and extended with `min-w-0 max-w-full` (the
  prototype only has `w-full`; the flex containers these inputs sit in — `FormField`, `SecretField`,
  `PrefixedField` — rely on `min-w-0` to avoid overflow on narrow viewports) and `.ui-input--textarea`/`--mono`/
  `--toolbar` (folded in from the now-removed `Tailwind/forms/text-area.css`).
- Removed `Tailwind/forms/text-area.css` (its `.cda-input`/`.cda-input--textarea`/`--mono`/`--toolbar` rules moved
  into `form-controls.css` as `.ui-input`/`.ui-input--textarea`/`--mono`/`--toolbar`) and its `input-base.css`
  import.
- Moved `Tailwind/forms/prefixed-field.css`'s input-descendant override into `form-controls.css`, retargeted from
  `.cda-input`/`.rz-input`/`.rz-numeric`/`.rz-select` onto `.ui-input`/`.ui-select`.
- Fixed: `form-controls.css`'s `.ui-input--outlined`/`--filled` and `.ui-checkbox--outlined`/`--filled` (and the
  `[disabled]` background rule) now set `background-color` instead of the `background` shorthand. The shorthand
  was clearing `background-image` on any element combining these with a `@tailwindcss/forms` plugin class —
  `DropDown`'s `form-select ui-input ui-input--outlined` lost its dropdown-arrow icon, and `CheckBox`'s
  `form-checkbox ui-checkbox ui-checkbox--outlined`/`--filled` would have lost its checked-state checkmark icon
  the same way.
- Added `Tailwind/forms/switch.css`, `slider.css`, `fieldset.css`, `settings-switch-row.css`,
  `settings-switch-label.css`, `file-upload.css` — new `--tone-*`-token-based `.ui-switch`/`.ui-slider`/
  `.ui-fieldset`/`.ui-settings-row`/`.ui-settings-label`/`.ui-file-upload__*` classes, replacing hardcoded
  `chrome-*`/`indigo-*`/raw-hex-gradient utility strings that were previously written directly in each
  component's `.razor` markup rather than a Tailwind file (a pre-existing gap in the file-layout convention this
  change now closes for these six components).
- Removed the now-dead `Class="cda-input"` passthrough from five Sandbox Forms example components
  (`samples/CanDoItAll.Components.Sandbox/Components/Examples/Forms/{FieldSlots,FormRowVariants,
  FormSectionShowcase,ScoringFieldset,BudgetReference}.razor`) — the class no longer resolves to anything now that
  `text-area.css` is gone, and each component's own default look already matches it.

#### Public interface

- **Breaking:** `ThemeHost`'s theme-boundary attribute renamed `data-cad-theme` → `data-ui-theme`, and its wrapper
  class renamed `.cad-theme-host` → `.ui-theme-host`, per the new "`data-ui-theme` theming attribute" development
  rule in `CLAUDE.md`. Update any consumer CSS/JS/tests that key off `[data-cad-theme]`/`.cad-theme-host` to the new
  names — values (`"light"`/`"dark"`) are unchanged. Each `[data-ui-theme="light"|"dark"]` block in
  `Tailwind/theme.css` now also sets a `--ui-theme` discriminator custom property, and
  `Tailwind/input-base.css`/`input-sandbox.css` redefine Tailwind's `dark:`/add a `light:` variant to key off it via
  a CSS style container query (`@container style(--ui-theme: dark)`), so `dark:`/`light:` utility classes now
  correctly nest under a `ThemeHost` boundary instead of only following `prefers-color-scheme`. `theme-tokens.js`'s
  `readTokens`/`watchTheme` and Mermaid's `"auto"` theme resolution now read `data-ui-theme`.
- **Breaking:** `ThemeHost` no longer renders `data-cad-theme-key` on its wrapper `<div>`. It was an exact, unused
  duplicate of the (now-renamed) theme-boundary attribute, read nowhere in this repo. No replacement needed.
- **Breaking:** `Tabs.Variant` (`TabsVariant`) removed, per the new "Standardized appearance properties" development
  rule in `CLAUDE.md`. Replaced by `Tabs.Density` (`TabsDensity`: `Normal` default, `Compact`):
  - `TabsVariant.ModalCompact` → `TabsDensity.Compact`
  - `TabsVariant.WorkspacePrimary` → default (`TabsDensity.Normal`)
  - `TabsVariant.WorkspaceSecondary`, `TabsVariant.WorkspaceTertiary`, `TabsVariant.Workstation` → removed, no
    replacement; apply app-specific shell theming via `Class`/`Style` on `Tabs` instead.
- **Breaking:** Removed all 21 remaining `Components/*/Compatibility/*.razor` shims from
  `CanDoItAll.Components.BaseLib`, ahead of the SB12 consumer-migration gate in
  `docs/standard-components-compatibility-policy.md` at explicit maintainer request (no remaining call sites in this
  repo). Replace removed types with their listed successor:
  - `ProfileTagChip`, `ProfileTagChipRow` → `Chip`, `ChipRow`
  - `BuilderStatBox`, `BuilderStatStrip` → `Card`/`SummaryTile`, `SummaryTiles`/`Grid`+`SummaryTile`
  - `SheetCard`, `SheetSection` → `Card` or `SectionCard`
  - `SheetCardHeading` → `TextBlock` or `SectionCard` title/description
  - `SheetCardTop` → `Toolbar`, `Stack`, or explicit header layout
  - `SheetGrid` → `Grid`
  - `SheetNote` → `Alert`, `EmptyState`, or `TextBlock`
  - `DebugToggle`, `ProfileToggle` → `Switch` or `CheckBox`
  - `ProfileField`, `SheetField` → `FormField`
  - `TagTextEdit` → `TagEditor`
  - `CreatorAvatar` → `Avatar`
  - `ZyWorkspaceModal` → `Dialog`
  - `DashboardActions`, `PageHeaderActions` → `Toolbar`/`Button`/`ButtonGroup`
  - `ImmersiveRibbonTabs` → `RibbonTabs`
  - `PageHeaderCopy` → `PageHeader`, `TextBlock`, or page title/description slots
- **Breaking:** `Tailwind/theme.css` custom property prefixes standardized onto the target CSS namespacing
  convention (CLAUDE.md rule 3). Suffixes are unchanged, so update via prefix find-and-replace on any consumer
  CSS/JS that reads these tokens directly:
  - `--cad-color-*` → `--tone-*` (e.g. `--cad-color-surface` → `--tone-surface`)
  - `--cad-tone-*` → `--tone-*` (e.g. `--cad-tone-primary-soft-bg` → `--tone-primary-soft-bg`)
  - `--cad-chip-{0-9}-*` → `--tone-{0-9}-*` (e.g. `--cad-chip-0-bg` → `--tone-0-bg`)
  - `--cad-nav-*` → `--ui-nav-*` (e.g. `--cad-nav-bg` → `--ui-nav-bg`; `Tailwind/navigation/side-menu.css`'s
    `--cad-side-menu-*` alias block now points at `--ui-nav-*` internally, its own names unchanged)
  - No values changed — this is a naming-only migration. `Tailwind/input-test.css`'s separate, unrelated `--tone-*`
    prototype block (a different, disjoint palette, not built into production output) was intentionally left
    untouched.
- **Breaking:** `TextBox`, `Password`, `Numeric`, `DropDown`, `CheckBox`, `SecretField`'s inner input, and
  `TextArea` now render `@tailwindcss/forms`-plugin classes (`form-input`/`form-select`/`form-checkbox`) plus
  `.ui-input`/`.ui-checkbox`/`--outlined`/`--filled` (see `form-controls.css`, above) instead of the old
  untokenized `.rz-input`/`.rz-select`/`.rz-numeric`/`.rz-checkbox` strings — unifying their look with each other
  and with the rest of the design system. A consumer's `Class="..."` override on one of these six components still
  wins (verified against the compiled CSS: Tailwind's utility output stays unlayered in this repo's build, which
  unconditionally beats any `@layer components` content regardless of source order). `TextArea` moves from
  `.cda-input`/`.cda-input--textarea` to the same `.ui-input` family, which also changes its corner radius to
  match every other input (was `rounded-xl` via `--cad-radius-control`, now `rounded-md`).
- Added `InputLook.Filled`, mapping to `.ui-input--filled`/`.ui-checkbox--filled` (a lighter/tinted background),
  reachable from `TextBox`, `Password`, `Numeric`, `DropDown`, `CheckBox`, `SecretField`, and `TextArea`. The
  existing `InputLook.Default` now explicitly renders `.ui-input--outlined`/`.ui-checkbox--outlined` (today's
  `bg-white`-equivalent look, unchanged); `InputLook.Plain` is unchanged (renders no class).
- Added `--tone-*`-token-based looks for `Switch`, `Slider`, `Fieldset`, `FileUpload`, `SettingsSwitchRow`, and
  `SettingsSwitchLabel` (new `.ui-switch`/`.ui-slider`/`.ui-fieldset`/`.ui-file-upload__*`/`.ui-settings-row`/
  `.ui-settings-label` classes), replacing their previous hardcoded `chrome-*`/`indigo-*`/raw-hex-gradient
  rendering. No parameter changes; only the rendered class names and resulting colors changed. `FileUpload`'s drop
  zone loses its bespoke gradient/shadow art in favor of flat `--tone-*` fills for the idle/drag-active/disabled
  states — a deliberate simplification, not a partial port.

#### Internal (chrome color scale)

- Added: `--color-chrome-*` neutral color scale to `Tailwind/theme.css`, reachable from both `input-base.css` and
  `input-sandbox.css`. It inverts at each `data-cad-theme` boundary (step 50 <-> 950, ... 500 fixed) — see the new
  "Chrome is structural" guidance in `CLAUDE.md`.
- Changed: `Tailwind/theme.css` — structural `--cad-color-*` and the `primary`/`light`/`base`/`dark` `--cad-tone-*`
  tokens now derive from `--color-chrome-*` instead of raw `--color-slate-*` literals. Values that must stay fixed
  across themes (shadows, `--cad-color-scrim`, the permanently-dark `--cad-nav-*` rail) now reference
  `--color-neutral-*` directly instead of `--color-slate-*`, since chrome would incorrectly invert them. No visible
  change in either theme — this only replaces the token source.
- Changed: all direct `slate-*` Tailwind utility classes in `CanDoItAll.Components.BaseLib` and
  `CanDoItAll.Components.Sandbox` `.razor` markup renamed to the equivalent `chrome-*` utility, so they now respond
  to the `data-cad-theme` boundary instead of staying visually fixed. Two call sites were deliberately left on
  `slate-*`: `ThemeHostComparison.razor` (its whole point is to show the light and dark `ThemeHost` scopes side by
  side) and the canvas host `<canvas>` element in `BenchmarkPrototypeResults.razor` (an intentionally always-dark
  preview surface, not application chrome).
- Removed: the `.sandbox-theme-host[data-cad-theme="light"]` override block in `Tailwind/sandbox/shell.css` that
  redeclared `--cad-color-*` with raw hex/`rgba()` literals for a since-abandoned Sandbox-only modernization pass —
  this violated the centralized-color rule and diverged Sandbox's light theme from the BaseLib scheme it's meant to
  preview. `Tailwind/input-sandbox.css` now imports `theme.css` directly so the Sandbox build always shares
  BaseLib's theme tokens and the `--color-chrome-*` scale.
- Added: 22 `--ui-gantt-*` custom properties to `Tailwind/theme.css` — the public override surface for
  `CanDoItAll.Components.Gantt`'s own `--gantt-*` tokens, per the new "`--<pkg>-*` / `--ui-<pkg>-*` two-layer
  tokens" development rule in `CLAUDE.md`. 6 structural tokens (`surface`, `surface-muted`, `grid`, `border`,
  `text`, `text-muted`) get a `data-cad-theme` dark override; the remaining 16 "paint" tokens (accent, critical,
  handle, connector, progress, effort, and the 9 canvas-literal tokens added above) are declared once, matching how
  `--cad-tone-*` solid colors already behave. Same values as Gantt's existing literals — no visible change. This
  establishes the pattern for CanvasLib/WebGlLib to follow later (see `CHANGELOG.md` → Gantt → Internal and the
  color-unification plan's follow-on table).
- Added: 13 more `--ui-gantt-*` custom properties, completing the extraction of every remaining raw color literal
  out of `GanttChart.razor.css` — `header-bg`/`header-border` (task-table header row),
  `title-editor-bg`/`title-editor-text` (inline task-title editor),
  `shadow-color`/`table-shadow-color`/`assignment-tooltip-shadow-color` (the three box-shadows, previously baked
  directly into `box-shadow` declarations), and `assignment-indicator-text`/`-bg`/`-border` plus
  `assignment-tooltip-text`/`-bg`/`-border` (the purple DOM assignment badge and its hover tooltip — distinct from
  the canvas-drawn assignment badge tokenized earlier). 5 of the 13 are structural (get a dark override); the other
  8 are fixed-value like the existing paint-token family. Same values as before — no visible change, including the
  pre-existing minor discrepancy where `title-editor-text`'s dark value (`#f8fafc`) doesn't quite match
  `--gantt-text`'s dark value (`#f1f5f9`), preserved rather than "fixed" silently.
- Added: 13 `--ui-mermaid-*` custom properties to `Tailwind/theme.css` — the public override surface for
  `CanDoItAll.Components.Mermaid`'s own `--mermaid-*` tokens, following the same two-layer pattern as
  `--ui-gantt-*`. All 13 are structural chrome (title/description/toolbar/viewport/error surface), so all get a
  `data-cad-theme` dark override, unlike Gantt's paint-token subset. Same values as Mermaid's existing light
  literals — no visible change to light mode; dark mode is new (Mermaid previously had no dark palette at all).
- Added: 8 `--ui-zoom-pan-*` custom properties to `Tailwind/theme.css` — the public override surface for
  `ZoomPanFrame`'s own `--cda-zoom-pan-*` tokens, following the same two-layer pattern as
  `--ui-gantt-*`/`--ui-mermaid-*`. `ZoomPanFrame.razor.css`'s 8 color reads now chain through `var(--cda-zoom-pan-X,
  var(--ui-zoom-pan-X, <original literal>))`, previously falling straight to a bare hex literal with no `theme.css`
  declaration at all (a rule 9 gap). 6 of the 8 derive from `--color-chrome-*` and self-invert automatically; the 2
  pure-white surface tokens (`tool-background`, `background`) get an explicit `data-cad-theme` dark override to
  `--color-chrome-50`. The focus-ring token (`--ui-zoom-pan-focus`) reuses the existing
  `--cad-tone-primary-soft-border` token instead of a standalone raw blue literal. Same values as before in light
  mode; dark-mode chrome is new (`ZoomPanFrame` previously had no dark palette at all — its
  background/border/text/focus-ring stayed fixed regardless of `data-cad-theme`).

### Gantt

#### Public interface

- Added: `GanttChartBodyAssets.IncludeThemeTokens` (`bool`, default `true`). Set to `false` to omit the
  `theme-tokens.js` `<script>` tag, mirroring the existing `GanttChartHeadAssets.IncludeBaseLibStyles` opt-out. Not
  required for normal use — only relevant if a consumer composes Gantt's scripts manually rather than via
  `<GanttChartBodyAssets />` and wants to omit BaseLib's theme module deliberately.

#### Internal

- Changed: `wwwroot/js/gantt-chart.js` now reads `--gantt-*` colors through BaseLib's shared `theme-tokens.js`
  (`readTokens`) instead of an inline `getComputedStyle` closure, and subscribes to theme changes (`watchTheme`) for
  the lifetime of each chart instance, disconnecting on dispose. No public API change; behavior change: a Gantt
  chart repaints its colors live when its `data-cad-theme` ancestor flips, instead of staying stale until the
  component remounts. `GanttChartBodyAssets.razor` now also emits `<script
  src="_content/CanDoItAll.Components.BaseLib/js/theme/theme-tokens.js">` ahead of `gantt-chart.js`.
- **Visual behavior change:** `GanttChart.razor.css`'s dark palette (`--gantt-*` dark values) was previously keyed
  only on `@media (prefers-color-scheme: dark)` — the OS preference — and never responded to the app's own
  `data-cad-theme` attribute at all. It's now keyed on `[data-cad-theme="dark"] .cda-gantt` instead, matching the
  attribute-only convention the rest of the app's theming already uses (`--color-chrome-*`, CLAUDE.md rule 3), and
  the OS-preference path was removed. A `CanDoItAll` app rendering `GanttChart` inside a dark `ThemeHost` on a
  light-OS machine will now correctly get a dark chart (previously light); a light `ThemeHost` on a dark-OS machine
  will now correctly get a light chart (previously dark). No parameter/API change, but check any screenshot
  baselines that include a themed Gantt chart. **Note:** a Gantt chart with no `data-cad-theme` ancestor at all
  (standalone, no `ThemeHost`) is now light-only — there is no OS-preference fallback for that case any more.
- Fixed: `resolveColors()` no longer throws when BaseLib's `theme-tokens.js` isn't loaded (e.g.
  `IncludeThemeTokens="false"`, or scripts composed manually without `<GanttChartBodyAssets />`). It falls back to
  the same inline `getComputedStyle` read Gantt used before the shared module existed, and skips live theme-change
  reactivity in that case instead of crashing — this was a regression introduced when the shared module landed (see
  the entry above), now brought in line with the graceful-degrade contract in CLAUDE.md rule 8.
- Added: 9 new `--gantt-*` custom properties for canvas colors that were previously hardcoded string literals in
  `gantt-chart.js` (resize-handle outline, default task-bar border, task-title text, assignment-badge fill/glyph,
  dependency-port stroke, and the hover-popover background/title/subtitle) — `--gantt-handle-outline`,
  `--gantt-task-border-default`, `--gantt-task-title-text`, `--gantt-assignment-badge-bg`,
  `--gantt-assignment-glyph-text`, `--gantt-dependency-port-stroke`, `--gantt-popover-bg`,
  `--gantt-popover-title-text`, `--gantt-popover-subtitle-text`. Same values as before (no visible change); like the
  existing `--gantt-accent`/`--gantt-critical`/etc. family, these are painted atop colored bars/badges rather than
  page chrome, so they intentionally have no separate dark-mode value.
- Changed: all 22 `--gantt-*` custom properties in `GanttChart.razor.css` now read `var(--ui-gantt-*, <original
  literal>)` instead of declaring their literal value directly, and the `[data-cad-theme="dark"] .cda-gantt` block
  no longer redeclares the 6 structural ones (`--ui-gantt-*` in `Tailwind/theme.css` already resolves per theme on
  its own now — see the BaseLib entry above). Same rendered values, no visible change — this makes
  `Tailwind/theme.css` the single file a designer edits to restyle Gantt, instead of `GanttChart.razor.css`.
  **Note:** a consumer who both sets `IncludeBaseLibStyles="false"` *and* keeps a `data-cad-theme` ancestor now gets
  Gantt's un-themed literal fallback for all 22 tokens regardless of the attribute's value, since the override
  surface (`--ui-gantt-*`) requires `theme.css` to be loaded — this is a documented tradeoff (CLAUDE.md rule 9), not
  a bug.
- Changed: added 13 more `--gantt-*` custom properties (see the matching BaseLib `--ui-gantt-*` entry above) and
  converted every remaining hardcoded `background`/`color`/`border`/`box-shadow` value in `GanttChart.razor.css` to
  reference one, including the two box-shadows that used to be literal `rgba(...)` values baked directly into the
  `box-shadow` shorthand. The entire `[data-cad-theme="dark"] .cda-gantt__table-row--header` and
  `[data-cad-theme="dark"] .cda-gantt__title-editor` override rules, and the `box-shadow` line in
  `[data-cad-theme="dark"] .cda-gantt`, were removed — theme.css's own per-theme `--ui-gantt-*` values make them
  redundant. `GanttChart.razor.css` now has zero raw color literals outside `var()` fallback positions.
- Changed: `GanttChart.razor.css`'s doc comments updated from `data-cad-theme` to `data-ui-theme`, following the
  BaseLib rename (see BaseLib → Public interface). No functional change — Gantt's own dark-mode CSS already resolves
  through `theme.css`'s `--ui-gantt-*` tokens rather than a `[data-cad-theme="dark"]` selector of its own.

### Mermaid

#### Public interface

- Added: `MermaidBodyAssets` (new component), composing BaseLib's `theme-tokens.js` `<script>` tag ahead of
  Mermaid's own JS, with an `IncludeThemeTokens` (`bool`, default `true`) opt-out — mirrors
  `GanttChartBodyAssets`/`GanttChartBodyAssets.IncludeThemeTokens`. Mermaid's own script (`mermaidDiagram.js`) is
  always loaded via a dynamic ES-module `import()` from the component itself, not through this component, so
  `MermaidBodyAssets` only needs to emit the shared theme-tokens script tag.
- Added: `MermaidHeadAssets.IncludeBaseLibStyles` (`bool`, default `true`), mirroring
  `GanttChartHeadAssets.IncludeBaseLibStyles`. Set to `false` to omit `MermaidHeadAssets`'s link to BaseLib's
  scoped-CSS bundle when a consumer already loads it elsewhere.
- **Breaking:** `MermaidDiagramOptions.Theme` default changed from `"default"` to `"auto"`. `"auto"` (or leaving
  `Theme` unset) makes the rendered diagram follow the nearest `data-cad-theme` ancestor — mermaid's own `"dark"`
  theme when it resolves dark, `"default"` otherwise — and re-render live when that attribute flips. An explicit
  theme name (`"default"`, `"dark"`, `"forest"`, `"neutral"`, `"base"`, ...) always overrides and disables the live
  watch. Visually identical to before unless the host page is themed dark, where a diagram now correctly renders in
  mermaid's dark theme instead of staying light.

#### Internal

- Changed: 13 raw color literals in `MermaidDiagram.razor.css` (title/description/toolbar/viewport/state/error
  chrome) extracted to `--mermaid-*` custom properties, each reading `var(--ui-mermaid-*, <original literal>)` — see
  the matching BaseLib `--ui-mermaid-*` entry above. Same values as before in light mode; dark-mode chrome is new
  (previously unthemed regardless of `data-cad-theme`).
- Changed: `wwwroot/js/mermaidDiagram.js` resolves an `"auto"`/unset `Options.Theme` by reading the `data-cad-theme`
  attribute off the diagram's nearest themed ancestor directly (a discrete mode name, not a CSS color, so this
  doesn't go through `theme-tokens.js`'s `readTokens`), and subscribes to `window.CanDoItAll.themeTokens.watchTheme`
  for the component's lifetime when present, degrading to a static (no live-flip) resolution when it isn't loaded —
  same graceful-degrade contract as Gantt (CLAUDE.md rule 8). A theme flip routes back through the existing Blazor
  render pipeline (`HandleThemeChangedAsync` → `StateHasChanged` → the existing render-key/render-queue path) rather
  than re-rendering from JS directly, so `Rendered`/`Error` events and `ZoomPanFrame` reset still fire consistently.
- `samples/CanDoItAll.Components.Sandbox/Components/App.razor` now includes `<MermaidBodyAssets
  IncludeThemeTokens="false" />` (theme-tokens.js is already loaded once via the existing `<GanttChartBodyAssets />`
  in the same page). `samples/CanDoItAll.Components.SandboxWasm/wwwroot/index.html` now loads `theme-tokens.js` via
  a manual `<script>` tag — this file composes scripts by hand rather than through Razor components, and previously
  omitted `theme-tokens.js` entirely, which also silently disabled Gantt's live theme-flip under WASM; that's fixed
  as a side effect.
- Changed: `wwwroot/js/mermaidDiagram.js`'s `"auto"` theme resolution (`resolveTheme`) now reads `data-ui-theme`
  instead of `data-cad-theme`, following the BaseLib rename (see BaseLib → Public interface). Same behavior — only
  the attribute name changed.

### Charts

#### Public interface

- Added a `ProjectReference` from `CanDoItAll.Components.Charts.csproj` to
  `CanDoItAll.Components.BaseLib.csproj` (previously a standalone leaf package with no dependency on BaseLib at
  all). This is what makes BaseLib's shared `theme-tokens.js` module reachable from Charts, per CLAUDE.md rule 8's
  eligibility ("any package that already depends on BaseLib").
- Added: `ChartsBodyAssets` (new component), composing BaseLib's `theme-tokens.js` `<script>` tag ahead of the
  package's own new `charts-theme.js`, with an `IncludeThemeTokens` (`bool`, default `true`) opt-out — mirrors
  `GanttChartBodyAssets`/`MermaidBodyAssets`. Consumers that render `<CdaChart>` should add `<ChartsBodyAssets />`
  to their page to get theme-aware series/grid/legend/stroke colors and live theme-flip support; without it,
  `<CdaChart>` keeps behaving exactly as before (CLAUDE.md rule 8 degrade path).
- Added: `CdaChartPalette.StrokeDefault` (`const string`, `"#64748b"`) — the series stroke fallback used when
  neither a series nor point supplies its own color, previously an inline literal in `CdaChart.razor`.

#### Internal

- Changed: `CdaChart.razor`'s shell/empty-state/heading markup now uses the theme-aware `--color-chrome-*` Tailwind
  utilities (`border-chrome-*`, `bg-chrome-*`, `text-chrome-*`) instead of raw `slate-*` classes, so the card
  border, background, and text now invert correctly under a dark `data-ui-theme` scope. Visually unchanged in
  light mode.
- Added `Tailwind/input-base.css`'s `@source` list now includes `../src/CanDoItAll.Components.Charts` (it was
  previously missing, so none of Charts' own Tailwind utility classes — including the `slate-*`/`chrome-*` ones
  above — were actually being compiled into BaseLib's `output.css`).
- Added 17 new `--ui-charts-*` tokens to `Tailwind/theme.css` (the public override surface for Charts' palette,
  grid-stripe, legend-text, and stroke-default colors): `--ui-charts-series-1` through `-8` (from the former
  `CdaChartPalette.Energetic`/`Default`), `--ui-charts-series-calm-1` through `-6` (from the former
  `CdaChartPalette.Calm`), `--ui-charts-grid-stripe`, `--ui-charts-legend-text`, and `--ui-charts-stroke-default`.
  `--ui-charts-grid-stripe`/`-legend-text` paint atop the shell's own inverting `--color-chrome-*` surface/text, so
  they get a dark-mode override; the series palette and stroke default paint atop chart lines/fills and are
  declared once, same as `--ui-gantt-*`'s paint tokens. Same values as before in light mode — no visual change
  without `ChartsBodyAssets`.
- Added `wwwroot/js/charts-theme.js`: exposes `window.CanDoItAll.charts.resolveTokens(host)` (reads the
  `--ui-charts-*` tokens above via BaseLib's `theme-tokens.js`, degrading to an inline `getComputedStyle` read with
  the same literal fallbacks when that module isn't loaded) and `watchTheme`/`unwatchTheme(host, dotNetReference)`
  (subscribes to theme flips and invokes the component's `HandleChartThemeChangedAsync` JS-invokable method).
- Changed: `CdaApexChartOptionsFactory.Build` takes an additional optional `ChartThemeColors? themeColors`
  parameter (new internal record: `Palette`, `GridStripe`, `LegendText`, `StrokeDefault`, `IsDark`). When
  supplied, its values replace the previously hardcoded palette/grid-stripe/legend-text/stroke-default literals;
  `null` keeps the exact previous hardcoded behavior (the degrade path for no `ChartsBodyAssets`/JS unavailable).
- Fixed: the ApexCharts hover tooltip and x-axis crosshair label were unreadable under a dark `data-ui-theme`
  scope (light background, near-white text) — ApexCharts styles both from a discrete `Tooltip.Theme`
  (`Mode.Light`/`Mode.Dark`) option rather than CSS custom properties, so it wasn't covered by the color-token
  work above. `charts-theme.js`'s `resolveTokens` now also returns the resolved `isDark` flag (read from the
  nearest `data-ui-theme` ancestor's attribute directly, mirroring CLAUDE.md rule 11's "auto" pattern, since it's
  a discrete mode rather than a color), and `CdaApexChartOptionsFactory.BuildTooltip` sets `Tooltip.Theme`
  accordingly when `themeColors` is supplied.
- Changed: `CdaChart.razor` now resolves `ChartThemeColors` via JS interop on first render and on every
  `data-ui-theme` flip (watched for the component's lifetime), rebuilding `_apexOptions` and pushing the change
  into the live ApexCharts instance via the existing `UpdateOptionsAsync` call. The component now implements
  `IAsyncDisposable` instead of `IDisposable` to unwind the JS theme watcher and the `DotNetObjectReference`
  alongside the existing `ApexChart.Dispose()` call.
- `samples/CanDoItAll.Components.Sandbox/Components/App.razor` now includes `<ChartsBodyAssets
  IncludeThemeTokens="false" />` (theme-tokens.js is already loaded once via the existing `<GanttChartBodyAssets
  />` on the same page). `samples/CanDoItAll.Components.SandboxWasm/wwwroot/index.html` now also loads
  `charts-theme.js` via a manual `<script>` tag, next to the existing hand-loaded `theme-tokens.js`.

### CanvasLib

#### Public interface

- **Visual behavior change:** `CanvasThemeTokenPack`'s default property values (`BorderSoft`, `ShadowSoft`,
  `ShadowStrong`, `BackgroundStart`/`Mid`/`End`, `AccentStart`/`End`, `DarkCard`, `DarkCardBorder`, `Panel`,
  `MutedText`, `ForegroundText`) now resolve through `var(--ui-canvas-*, <original literal>)` (or
  `var(--color-neutral-900, ...)` for `DarkCard`) instead of returning the raw literal directly, per the
  "`--<pkg>-*` / `--ui-<pkg>-*` two-layer tokens" development rule in `CLAUDE.md`.
  `CanvasWorkbench`/`CanvasCalendar` apply this pack's `ToInlineStyle()` unconditionally as an inline `style`
  attribute on their root element, which previously made the pack's hardcoded hex values impossible to override or
  theme — that's what this change fixes. Combined with the CSS changes below, `CanvasWorkbench`'s page-background
  gradient, toolbar, and action-button chrome now correctly darken under a `data-cad-theme="dark"` `ThemeHost` —
  verified in the Sandbox — which CanvasLib has never done before. Node cards and their palette accents
  (violet/success/info/warning/danger) intentionally keep the same vivid colors in both themes, matching how Gantt's
  accent/critical/progress tokens already behave.
  `ToCssVariables()`/`ToInlineStyle()`/`BuildPreviewSwatches()`/`BuildMetrics()` are unchanged in shape — they just
  now return strings containing `var()` expressions instead of bare literals.
- Added: `CanvasLibBodyAssets.IncludeThemeTokens` (`bool`, default `true`). Set to `false` to omit the
  `theme-tokens.js` `<script>` tag, mirroring
  `GanttChartBodyAssets.IncludeThemeTokens`/`MermaidBodyAssets.IncludeThemeTokens`. Not required for normal use —
  only relevant if a consumer composes CanvasLib's scripts manually, or already loads `theme-tokens.js` once via
  another package's `*BodyAssets` on the same page and wants to avoid the duplicate (harmless but redundant)
  `<script>` tag.

#### Internal

- Added: ~140 `--ui-canvas-*` custom properties to `Tailwind/theme.css` — the public override surface for
  `CanDoItAll.Components.CanvasLib`'s own `--cw-*` tokens, following the same two-layer pattern as
  `--ui-gantt-*`/`--ui-mermaid-*`/`--ui-zoom-pan-*`. Structural chrome (toolbar/action/panel surfaces, page
  background, borders, text) gets a `data-cad-theme` dark override; the much larger "tone" family
  (accent/info/mint/warn/danger/sky/etc., used by node-palette surfaces, marker/chip/priority variants, and
  decorative preview widgets in the Settings panel) is declared once, matching how `--cad-tone-*`/Gantt's accent
  family already behave — a colorful workflow diagram or badge shouldn't have its category color muddied by theme
  inversion. This completes the CanvasLib pass of the follow-on work flagged in the BaseLib `--ui-gantt-*` entry
  above, covering `CanvasThemeTokenPack.cs` and all six `workbench/**/*.css` files.
- Changed: all six `workbench/**/*.css` files' `--cw-*` declaration blocks (`shell/01-layout-and-shell.css`,
  `chrome/02-toolbar-and-windows.css`) moved from `:root` to `.cw-editor-shell, .cw-workbench-shell,
  .cdi-canvas-calendar-shell` — the package's own root wrapper classes — since declaring on `:root` would resolve
  `--ui-canvas-*` against the document root's `data-cad-theme` rather than a nested `ThemeHost` ancestor's, silently
  breaking theme-scoping for a consumer that nests a themed CanvasLib instance inside another theme scope (CLAUDE.md
  rule 8's per-host-element resolution requirement, mirrored here in CSS form). The shell file's `:root` block
  previously duplicated `CanvasThemeTokenPack.cs`'s literal defaults verbatim (a rule-2 duplicate-source-of-truth)
  and is now reconciled. Every remaining raw color literal across `panels/03-help-settings-and-preview.css`,
  `scene/04-scene-and-nodes.css`, `overlays/05-overlays-and-composer.css`, and
  `responsive/06-motion-and-responsive.css` (roughly 700 occurrences, mostly alpha variants of a small palette) was
  converted to either `color-mix(in srgb, var(--color-chrome-*|--ui-canvas-*) N%, transparent)` (for the
  neutral/slate scale and the new tone tokens) or a direct `var(--ui-canvas-*, <literal>)` reference — no raw color
  literals remain in any workbench CSS file outside `var()` fallback positions. Same rendered values as before in
  light mode; dark-mode chrome is new.
- Fixed: a stale pre-compressed `.gz` static-web-asset sidecar for `workbench/chrome/02-toolbar-and-windows.css`,
  dated 2026-08-10, was still being served to gzip-accepting clients (i.e. every real browser) after this session's
  edits — `curl` without `Accept-Encoding: gzip` got the fresh file while the actual app got the two-week-stale one
  with none of the new tokens, making the whole file's custom properties resolve empty. A full clean (`rm -rf obj
  bin`) + rebuild of the Sandbox project regenerated the compression manifest and fixed it. Worth checking for on
  any future CanvasLib CSS change that doesn't seem to take effect in a running Sandbox instance — verify via the
  emitted `Content-Length` in `obj/**/staticwebassets.build.endpoints.json` rather than assuming a `dotnet
  build`/`dotnet test` run always refreshes it.
- Added: `canvasColorTokenMap`/`resolveCanvasColors(host)` in `wwwroot/js/runtime/workbench/01-foundation.js`,
  following the same `readTokens`-or-inline-`getComputedStyle`-fallback degrade contract as `gantt-chart.js`
  (CLAUDE.md rule 8). Covers the 10 named accent colors
  (`violet`/`mint`/`sky`/`amber`/`rose`/`success`/`warning`/`danger`/`info`/`neutral`) that
  `resolveNodeAccentColor()` (`06a-canvas-scene-and-hit-testing.js`) previously returned as hardcoded hex literals —
  these now resolve through the matching `--ui-canvas-*`/`--color-chrome-600` tokens added above, with the same
  literal as the inline-fallback default. `resolveNodeAccentColor` takes a new second `colors` parameter (the
  resolved token map); its one call site (`renderNodes` in `07b-runtime-rendering.js`) passes `state.colors`.
  `hydrateState` (`05-viewport-and-events.js`) now populates `state.colors` at creation, and
  `root.canvasWorkbench.create()` (`07-runtime-entry.js`) wires `watchTheme(state.shell || host, ...)` to re-resolve
  `state.colors` and re-render on every `data-cad-theme` flip, disconnected in `disposeWorkbenchStateCore`. Verified
  live in the Sandbox: toggling the theme updates `state.colors` and repaints without a remount. **Now converted,
  see the entry below:** `resolveCanvasNodePaletteStyle`'s ~130 precomputed literal shades and the rest of
  `06-canvas-renderers.js`'s tone-keyed literals.
- Added: `theme-tokens.js` `<script>` (gated by the new `IncludeThemeTokens` parameter above) to
  `tools/canvaslib/build-assets.cjs`'s generated `CanvasLibBodyAssets.razor`, ahead of the package's own runtime
  scripts — same ordering as `GanttChartBodyAssets`. `tools/canvaslib/verify-assets.cjs` duplicates (rather than
  imports) `build-assets.cjs`'s body-component-building logic, so it needed the identical change to stop reporting
  the freshly generated file as stale — see the new CLAUDE.md rule 12 note about this trap for any future generator
  change. `samples/CanDoItAll.Components.Sandbox/Components/App.razor`'s `<GanttChartBodyAssets />` now also sets
  `IncludeThemeTokens="false"`, since `<CanvasLibBodyAssets />` (declared first on that page) already loads it once.
- Added: `samples/CanDoItAll.Components.Sandbox/Components/Pages/TestCanvas.razor` (`/test-canvas`), a proof page
  for the theme-token work above — a `CanvasWorkbench` with one node per named accent tone (`PaletteKey` set,
  `AccentColor` left empty so `resolveNodeAccentColor()` falls through to the token-resolved path instead of
  returning a literal verbatim), toggled via `?dark=true` the same way `/test` already works. Verified live: the
  shell/toolbar/canvas-background chrome now darkens under `data-cad-theme="dark"` while each node keeps its
  distinct accent color. Linked from the Sandbox sidebar next to the existing `/test` link. Later extended in the
  same session (see the tokenization entry below) to also exercise `resolveCanvasNodePaletteStyle`'s palette blocks,
  group frames, ports, link tones, progress-badge states, markers, an annotation badge, a media preview, a
  preview-only outline, and a decision node.
- Added: `tools/screenshots/screenshots.config.test-canvas.json` (`npm run screenshots:test-canvas`), a screenshot
  config scoped to only `/test-canvas` (light + dark), used to prove the palette/renderer-literal tokenization below
  is visually a no-op. Uses `tools/screenshots/`'s new `key` config field (`"test-canvas"`; default `"screenshots"`
  elsewhere) so its branch history doesn't collide with the main `screenshots_*` history in the same storage repo —
  `key` now drives the branch-name prefix, which branches count as candidates for baseline auto-detection/pruning,
  and the generated report's title (`tools/screenshots/README.md` documents it).
- Completed the tokenization flagged as follow-on work in the `canvasColorTokenMap`/`resolveCanvasColors` entry
  above: every remaining raw color literal in `resolveCanvasNodePaletteStyle`
  (`06a-canvas-scene-and-hit-testing.js`, 9 palette/read-only blocks × 15 literal slots — 14 keys, plus
  `surfaceStroke`'s selected/unselected split) and in `06-canvas-renderers.js` (`drawCanvasFrame`,
  `resolveCanvasPortVisualStyle`, `resolveCanvasLinkStyle`, `resolveCanvasLinkLabelStyle`,
  `drawCanvasProgressBadge`, `resolveMarkerToneAccentColor`, the annotation-badge default pill,
  `drawNodeMediaPreview`, `drawCanvasDecisionCues`, `renderCanvasDecisionNode`, `renderCanvasAdvancedNode`'s
  side-bar strip, `renderCanvasStandardNode`'s preview-only outline) is now tokenized, following the same
  `--ui-canvas-*`/`canvasColorTokenMap`/`state.colors` pattern as the accent-color pass — 246 new `--ui-canvas-*`
  custom properties added to `Tailwind/theme.css` (declared once, no `data-cad-theme` dark override, matching the
  existing tone/paint-token family) and 246 matching `canvasColorTokenMap` entries in `01-foundation.js`. Pure 1:1
  literal extraction — no `color-mix()`-derived computation, per the prior session's explicit rejection of that
  approach for `resolveCanvasNodePaletteStyle` (a computed formula doesn't numerically match the fixed literals).
  - Signature changes: `resolveCanvasNodePaletteStyle` gains a 4th `colors` parameter (all 5 call sites updated in
    the same edit). `resolveCanvasPortVisualStyle`, `drawCanvasPortPill`, `resolveCanvasLinkConnectionStyle`,
    `resolveCanvasLinkLabelStyle`, `drawCanvasLinkLabel`, and `renderCanvasLinkLabels` each gain a new `colors`
    parameter, threaded down from `state.colors` at their respective call chains (`07b-runtime-rendering.js`'s
    `renderCanvasLinkLabels` call, and `renderLinks`'s 4 `resolveCanvasLinkConnectionStyle`/`drawCanvasLink` call
    sites in `06-canvas-renderers.js`). `resolveCanvasLinkStyle` and `resolveMarkerToneAccentColor` also gain a
    `colors` source (via `options.colors` and a new 3rd parameter respectively). `drawCanvasFrame`,
    `drawCanvasProgressBadge`, `drawCanvasAnnotationBadges`, `drawNodeMediaPreview`, `drawCanvasDecisionCues`,
    `renderCanvasDecisionNode`, `renderCanvasAdvancedNode`, and `renderCanvasStandardNode` needed no signature
    change — they already receive `state` and now read `state.colors` directly.
  - Verified with `npm run screenshots:test-canvas`: extended `TestCanvas.razor` (see above) to paint every literal
    being converted, captured a baseline, then re-captured after the JS/CSS changes — 0 changed pages, proving the
    extraction is value-preserving. Followed by a mandatory positive control (a 0-diff alone doesn't prove the
    tokens are read, since an unthreaded `colors` param would also produce 0 diff): temporarily overrode
    `--ui-canvas-node-success-surface-fill`, `--ui-canvas-link-danger-stroke`, and `--ui-canvas-frame-accent-fill`
    to `#ff00ff` in `theme.css`, re-ran the screenshot diff and confirmed both pages changed, then reverted and
    confirmed back to 0 diff.
  - **Not exercised by the screenshot proof, verified by code review only:** `resolveCanvasLinkStyle`'s
    `hovered`/`preview` branches are interaction-driven (drag-to-connect, delete-mode hover) and not reachable from
    static `Surface` data.
  - **Still out of scope, not touched in this pass:** `07b-runtime-rendering.js`'s delete-mode hover overlay has its
    own 2 untouched literals (`rgba(254, 226, 226, 0.12)` / `rgba(220, 38, 38, 0.92)`).
- Changed: `wwwroot/css/workbench/shell/01-layout-and-shell.css`'s doc comment updated from `data-cad-theme` to
  `data-ui-theme`, following the BaseLib rename (see BaseLib → Public interface). No functional change — CanvasLib's
  dark-mode CSS already resolves through `theme.css`'s `--ui-canvas-*` tokens rather than a
  `[data-cad-theme="dark"]` selector of its own.
