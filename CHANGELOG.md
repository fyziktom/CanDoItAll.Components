# Changelog

All notable changes to this repository's packages are recorded here, per the changelog rule in `CLAUDE.md`.

## [Unreleased] — 2.0.0

### BaseLib

#### Internal

- Changed: Tailwind source files under `Tailwind/` now mirror `src/CanDoItAll.Components.BaseLib/Components/<Group>/<Component>.razor`, in lowercase-kebab (e.g. `buttons/copy-button.css` for `Buttons/CopyButton.razor`). No emitted class names changed.
- Changed: Tailwind selectors with no current component owner moved into per-group `compatibility/` subfolders (e.g. `cards/compatibility/sheet-card.css`) instead of being deleted. Check there before assuming a class is dead.
- Removed: all 12 `Tailwind/**/compatibility/*.css` files and their `@import`s from `Tailwind/input-base.css` — `compatibility/theme.css`, `forms/compatibility/fields.css`, `forms/compatibility/sheet-field.css`, `layout/compatibility/stacks.css`, `layout/compatibility/radzen-layout.css`, `layout/compatibility/stats.css`, `cards/compatibility/cards.css`, `cards/compatibility/sheet-grid.css`, `cards/compatibility/sheet-card.css`, `cards/compatibility/sheet-section.css`, `navigation/compatibility/tabs.css`, `typography/compatibility/text.css`. Verified against a repo-wide grep: none of these selectors had any `.razor`/`.cs` call site. **Breaking (compatibility-policy deviation):** `SheetField`, `SheetGrid`, `SheetCard`, and `SheetSection` (`Components/{Forms,Cards}/Compatibility/`) still apply their `cad-`/`zy-` classes via `BuildAttributes(...)`, but those classes now have no CSS backing — this jumps ahead of the SB12 consumer-migration gate in `docs/standard-components-compatibility-policy.md` at explicit maintainer request. A consumer still instantiating these shims will see unstyled markup; migrate to `Card`/`SectionCard`/`Grid`/`FormField` per that doc's replacement column.
- Changed: `Tailwind/navigation/tabs.css` — `.cad-tabs--variant-modal-compact` renamed to `.cad-tabs--density-compact`. Removed `.cad-tabs--variant-workspace-secondary`, `.cad-tabs--variant-workspace-tertiary`, and `.cad-tabs--variant-workstation`, along with the `--cad-workstation-*` tokens they referenced in `Tailwind/theme.css`.

#### Public interface

- **Breaking:** `Tabs.Variant` (`TabsVariant`) removed, per the new "Standardized appearance properties" development rule in `CLAUDE.md`. Replaced by `Tabs.Density` (`TabsDensity`: `Normal` default, `Compact`):
  - `TabsVariant.ModalCompact` → `TabsDensity.Compact`
  - `TabsVariant.WorkspacePrimary` → default (`TabsDensity.Normal`)
  - `TabsVariant.WorkspaceSecondary`, `TabsVariant.WorkspaceTertiary`, `TabsVariant.Workstation` → removed, no replacement; apply app-specific shell theming via `Class`/`Style` on `Tabs` instead.
- **Breaking:** Removed all 21 remaining `Components/*/Compatibility/*.razor` shims from `CanDoItAll.Components.BaseLib`, ahead of the SB12 consumer-migration gate in `docs/standard-components-compatibility-policy.md` at explicit maintainer request (no remaining call sites in this repo). Replace removed types with their listed successor:
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
