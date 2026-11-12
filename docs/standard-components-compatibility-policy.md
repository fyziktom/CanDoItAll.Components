# Standard Components Compatibility Policy

This document owned the compatibility shims that existed in `CanDoItAll.Components.BaseLib` during publishing preparation.

Do not remove, rename, or change the public parameters of a compatibility shim during SB10 without a documented removal gate. Shims exist to keep old app/sandbox call sites compiling while the pure publishing repositories are prepared. Removal is normally allowed only after SB12 proves downstream consumer migration, AppComponents parity, package/API approval updates, and a documented replacement path — see the closure note below for the one occasion that gate was waived by explicit maintainer request.

New component work should use a shim's replacement column directly. Compatibility shims are not the preferred generic component base for new projects.

## Closure note (2026-08-25)

All 21 BaseLib component shims previously tracked in this document were removed on 2026-08-25, ahead of the SB12 consumer-migration gate, at explicit maintainer request. There were no remaining call sites in this repo. This mirrors the earlier removal of these same shims' Tailwind CSS (see `CHANGELOG.md`, `[Unreleased]` → BaseLib → Internal).

Consumers still instantiating a removed shim should migrate to its replacement:

| Removed shim | Preferred replacement |
|---|---|
| `ProfileTagChip` | `Chip` |
| `ProfileTagChipRow` | `ChipRow` |
| `BuilderStatBox` | `Card`, `SummaryTile`, or local layout markup depending on consumer shape |
| `BuilderStatStrip` | `SummaryTiles` or `Grid` with `SummaryTile` |
| `SheetCard` | `Card` or `SectionCard` |
| `SheetCardHeading` | `TextBlock`, `SectionCard` title/description, or page header primitives |
| `SheetCardTop` | `Toolbar`, `Stack`, or explicit header layout |
| `SheetGrid` | `Grid` |
| `SheetNote` | `Alert`, `EmptyState`, or `TextBlock` depending on message importance |
| `SheetSection` | `SectionCard` or `Card` |
| `DebugToggle` | `Switch` or `CheckBox` inside a `FormField` |
| `ProfileField` | `FormField` |
| `ProfileToggle` | `Switch` or `CheckBox` |
| `SheetField` | `FormField` |
| `TagTextEdit` | `TagEditor` |
| `CreatorAvatar` | `Avatar` |
| `ZyWorkspaceModal` | `Dialog` |
| `DashboardActions` | `Toolbar`, `Button`, or `ButtonGroup` |
| `ImmersiveRibbonTabs` | `RibbonTabs` |
| `PageHeaderActions` | `Toolbar` or page header action slots |
| `PageHeaderCopy` | `PageHeader`, `TextBlock`, or page title/description slots |

## Publishing Rule

- This policy still applies to any *future* compatibility shim added to `CanDoItAll.Components.BaseLib` — it currently tracks none.
- Any removal of a future shim must include an AppComponents/consumer migration table, compile proof for affected consumers, and updated public API approval snapshots, unless waived by explicit maintainer request as documented in a closure note here.
- Any new compatibility shim must be added to this document, the SB01 inventory data, and the SB10 approval snapshots in the same change.
