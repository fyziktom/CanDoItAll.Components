# TODO

Extracted from the sibling app repo's UI/UX refactor notes where this component library is used.

## Findings summary

- **`ListDetailShell` is the single highest-leverage fix.** It's used in 14 files / 30
  references app-side, but it's a plain two-column grid with a hardcoded rounded/shadowed
  card look, no collapse, no resize, wrong responsive behavior (stacks panes instead of
  switching list-or-detail), and no keyboard selection semantics.
- **The design-token layer is real and mostly good** (`--cad-tone-*`, `--cad-color-*`,
  `--cad-space-*`, `--cad-radius-*`, `--cad-shadow-*` in `foundation/theme.css`), but
  `ListDetailShell` itself bypasses it with hardcoded `slate`/`white` classes.
- **Dark mode is fully built and unused.** The dark palette, `ThemeHost`, and a working
  toggle all exist (proven out in the Sandbox), but nothing in the app turns it on — and
  it can't work anywhere `ListDetailShell` is used until that hardcoding is fixed.
- **Typography has ~30 ways to render a string and no token layer.** 17 `TextStyle`
  members plus 13 standalone typography components, zero `--cad-font-*`/`--cad-text-*`
  tokens, with known duplicates (`Subtitle1`/`H5`, `Note`/`Muted`).
- **The library is more complete than it looks from outside**, but per-component API
  documentation is essentially absent, and the Sandbox catalogue — the library's best
  asset — is invisible unless you clone the repo and run it.
- **Two smaller primitives contribute to app-side "everything is loud" feel**: `CompactStat`
  has no muted-when-zero mode, and `PageHeader.Description` has no collapsed state.
- **`SecondaryTabs` (key-based) and `Tabs` (index-based) render differently**, which is
  part of why tab styling is inconsistent across app pages.

## TODO

### Documentation / catalogue
- [ ] Write per-component API documentation: prop tables, usage guidance, do/don't
  (`docs/architecture/` currently has exactly one file).
- [ ] Write a short "which component when" decision guide (e.g. `SectionHead` vs.
  `SectionHeading` vs. `Header`; `SecondaryTabs` vs. `Tabs`).
- [ ] Publish the Sandbox catalogue as a GitHub Page. Recommended approach: add a WASM
  host project alongside the existing Blazor Server sandbox, sharing the same
  catalogue pages and component registry, and static-publish that. There's already a
  `ci.yml` to hang a Pages job off of. (Prerendering to static HTML or a screenshot
  gallery are cheaper fallback options but lose interactivity.)

### `ListDetailShell` (highest leverage)
- [ ] Add a flat/hairline variant — no default `rounded-[1.75rem]`, no default
      `shadow-sm shadow-slate-200/60`; radius and shadow should be opt-in, not baked into
      `ResolveHeaderClass()`/`ResolveContentClass()`.
- [ ] Replace hardcoded `px-4 py-3.5` / `px-4 py-4` padding with `--cad-space-*` tokens.
- [ ] Support collapsing the list pane to give the detail pane full width.
- [ ] Add a real resizable/draggable divider. Today `Split.razor` is a plain flex row and
      `WorkspaceSplit.razor` is a bare `<div>` — the only resize behavior anywhere in the
      library is `AllowResize` on floating `OverlayWindow`.
- [ ] Fix the responsive behavior: the existing `46rem` container query in
      `list-detail.css` stacks the panes vertically, which is wrong for this shape. Replace
      with a list-*or*-detail switch plus a back affordance.
- [ ] Add selection semantics: keyboard/arrow-key navigation, roving focus, `aria-selected`.
- [ ] Remove the hardcoded `slate`/`white` classes so the component is token-driven (also
      the blocking piece for dark mode wherever this component is used).

### Typography consolidation
- [ ] Collapse the `TextStyle` enum (17 members) and the 13 standalone typography
      components toward a target of roughly 5 sizes × 3 weights.
- [ ] Introduce `--cad-font-*`/`--cad-text-*` tokens to back `typography/text.css` (there
      are currently zero).
- [ ] Resolve known duplicate styles: `Subtitle1`/`H5` render identically, as do
      `Note`/`Muted`.
- [ ] Keep old enum members as aliases (mark `[Obsolete]` with a warning) so the app's 472
      `TextBlock` call sites can migrate incrementally rather than all at once.

### Other primitive gaps
- [ ] `CompactStat`: add a muted-when-zero mode (today a `0` count still renders in full
      tone color).
- [ ] `PageHeader.Description`: support a collapsed/expandable state for long hint text.
- [ ] Reconcile `SecondaryTabs` (key-based) vs. `Tabs` (index-based) — two primitives doing
      the same job with different visual output. Either consolidate or document clearly
      which to use when.

### Dark mode readiness
- [ ] No new build needed — palette, `ThemeHost`, and the Sandbox toggle
      (`SandboxThemeState.cs`) already work. The only blocker is the `ListDetailShell`
      hardcoding above; once that's fixed, verify no other library component hardcodes raw
      palette colors instead of tokens.

### Policy / process
- [ ] Add a CI lint rule that rejects raw palette Tailwind utilities (e.g. `slate-*`,
      `sky-*`) outside the token layer, to enforce the existing Tailwind policy rather than
      relying on review discipline.
- [ ] Reconcile this repo's Visual Gate (`docs/standard-components-tailwind-policy.md`,
      which requires narrow-mobile screenshot proof for every styling change) with the app
      repo's `ui-support-scope.md`, which currently scopes the app to large-desktop only.
      The two documents currently disagree.
- [ ] Confirm whether output-CSS regeneration in this repo has build enforcement (the app
      repo's generated `output.css` is committed with none, which is a silent-drift risk
      worth checking here too).

### Open questions (joint with app repo)
- How aggressive to be on the typography collapse, and who absorbs the migration cost
  across the app's 472 `TextBlock` call sites (some mappings are mechanical, an estimated
  15–20% need a human judgment call).
- Concrete density reference numbers to adopt as token targets — roughly a 4px base grid,
  32px list rows, 13–14px body text, taken from the Linear-style target referenced in the
  source doc.
- Whether per-component API documentation is worth writing at all, versus the published
  Sandbox catalogue plus a short decision guide being sufficient.

# BaseLib components missing from the Sandbox TOC

## Context

You noticed `Avatar.razor` (`Components/Identity/Avatar.razor`) isn't listed anywhere in the Sandbox's table of contents at `https://localhost:55173/`, and asked for a full audit of every `CanDoItAll.Components.BaseLib` component against what the Sandbox actually documents, so you know the full scope of the documentation gap.

**Method:**
- Enumerated all `.razor` components in `src/CanDoItAll.Components.BaseLib/Components/` → **165 components** across 18 folders (Badges, Buttons, Cards, DataDisplay, DataVisualization, Feedback, Forms, Identity, Layout, Lists, Modals, Navigation, Storage, Typography — several with `Compatibility` subfolders).
- Enumerated everything documented in the Sandbox via its single source of truth, `samples/CanDoItAll.Components.Sandbox/SandboxCatalogRegistry.cs` (`PageSections`, read by both `MainLayout.razor`'s sidebar and `Home.razor`'s full TOC) → **47 unique BaseLib components** documented (the registry also lists non-BaseLib components from Charts/Mermaid/QRCode/OverlayLib/CanvasLib, which are out of scope here).
- Diffed by component name.

**Result: ~118 of 165 BaseLib components (72%) have no Sandbox entry**, confirming `Avatar` is not an isolated case — it's part of a much larger documentation backlog concentrated in Cards, Navigation, Typography, and the various `Compatibility` subfolders.

## Missing components, by folder

### ✅ Identity (4 of 6 missing) — includes Avatar
- **Avatar** ← the one you spotted
- CreatorLine
- CreatorSocialLink
- CreatorAvatar *(Compatibility)*

Documented: Icon, RoboAvatar

### ✅ Badges (8 of 11 missing)
BadgesGroup, ChipRow, CompactStatStrip, Pill, PillList, StatusBadge, ProfileTagChip *(Compat)*, ProfileTagChipRow *(Compat)*

Documented: Badge, Chip, CompactStat

### ✅ Cards (27 of 29 missing)
ActionCard, ActionReviewPanel, AuthCard, CardActions, CardButton, CardGrid, CardStatsWithNumber, HeroCard, MetricCard, PanelCard, ParitySectionCard, PriceBar, PriceRow, SectionCard, StatBox, StatsCardRow, StatsGrid, SummaryTile, SurfaceCard, BuilderStatBox *(Compat)*, BuilderStatStrip *(Compat)*, SheetCard *(Compat)*, SheetCardHeading *(Compat)*, SheetCardTop *(Compat)*, SheetGrid *(Compat)*, SheetNote *(Compat)*, SheetSection *(Compat)*

Documented: Card, SummaryTiles

### ✅ DataDisplay (3 of 3 missing — entire folder undocumented)
DiffViewer, Timeline, TimelineStepper

### ✅ DataVisualization (7 of 8 missing)
CategoryAxis, Chart, DataGridColumn, GridLines, LineSeries, ProgressBar, ValueAxis

Documented: DataGrid

### ✅ Feedback (4 of 10 missing)
EmptyState, LoadingState, Notification, Tooltip

Documented: Alert, Callout, HelpPopover, StatusCheckList, TooltipTarget, VerificationList

### ✅ Forms (11 of 27 missing)
FormField, FormRow, FormSection, FormStack, InlineActions, SettingsSwitchLabel, DebugToggle *(Compat)*, ProfileField *(Compat)*, ProfileToggle *(Compat)*, SheetField *(Compat)*, TagTextEdit *(Compat)*

Documented: CheckBox, DropDown, Editable, EntityPicker, Fieldset, FileUpload, Numeric, Password, PrefixedField, SecretField, SettingsSwitchRow, Slider, Switch, TagEditor, TextArea, TextBox

### ✅ Layout (9–10 of 16 missing)
Body, Cluster, Layout, PageShell, Sidebar, Split, ThemeHost, WorkspacePanel, WorkspaceSplit

*Column* is ambiguous: the Layout group page has a combined anchor `row-column`, but no dedicated registry section for Column on its own — effectively under-documented.

Documented: Grid, PageScaffold, Row, Stack, StickyActionFooter, ZoomPanFrame

### ✅ Lists (6 of 8 missing)
FactTable, ListGroup, ListItem, ListPanelHeader, MetaList, PlainList

Documented: ListDetailShell, SelectionListItem

### ✅ Modals (6 of 7 missing)
DangerActionDialog, DialogHost, DialogScaffold, InspectorDialogLayout, PickerDialogShell, ZyWorkspaceModal *(Compat)*

Documented: Dialog

### ✅ Navigation (18 of 24 missing)
ContextMenu, FilterBar, LegalToc, LegalTocNav, PageHeaderActionButton, RibbonTabs, SecondaryTabs, SideMenuItem, StepsItem, TabsItem, ToolbarActions, ToolbarFields, ToolbarRow, TreeViewNodeRow, DashboardActions *(Compat)*, ImmersiveRibbonTabs *(Compat)*, PageHeaderActions *(Compat)*, PageHeaderCopy *(Compat)*

Documented: PageHeader, SideMenu, Steps, Tabs, Toolbar, TreeView

### ✅ Storage (2 of 2 missing — entire folder undocumented)
StorageBadgeStrip, StorageSummaryCard

### ✅ Typography (11 of 12 missing)
CopyableMonoValue, Divider, Eyebrow, FooterText, HashDisplay, Header, MonoText, MutedInline, SectionHead, SectionHeading, SmallText

Documented: TextBlock

### ✅ Buttons (0 missing — fully documented)
Button, CopyButton

## Notes on scope
- "Compatibility" subfolder components (legacy/shim wrappers) are almost entirely undocumented across every folder that has one — worth deciding as a group whether they're meant to be Sandbox-visible at all, or intentionally excluded as deprecated.
- Three entire folders (DataDisplay, Storage, and effectively most of Cards) have next to no Sandbox presence.
- This plan only delivers the audit list above — no code changes. If you want, a natural next step would be to scope adding Sandbox demo sections for a subset (e.g., start with Identity/Avatar, or tackle a whole folder like DataDisplay or Storage).

# DEV NOTES

- [ ] radzen-layout.css?
- [ ] favicon + states
- [ ] full dark theme html level, disable animation during transition
- [ ] btn mouse over, pointer
- [ ] mermaind tooltip absolute element 2px padding global scroll glitch
- [x] ignore output
- [x] local net downgrade
- [x] 1st component header margin top fix
- [x] usage counts
- [x] map component usage across products
- [x] nav filter
- [x] reorg toc to readme
- [x] pr notes
- [x] homo avatars
- [x] link to source
