# Standard Components Compatibility Policy

This document owns the compatibility shims that remain in `CanDoItAll.Components.BaseLib` during publishing preparation.

Do not remove, rename, or change the public parameters of these shims during SB10. They exist to keep old app/sandbox call sites compiling while the pure publishing repositories are prepared. Removal is allowed only after SB12 proves downstream consumer migration, AppComponents parity, package/API approval updates, and a documented replacement path.

New component work should use the replacement column directly. Compatibility shims are not the preferred generic component base for new projects.

| Shim | Source | Preferred replacement | Removal gate |
|---|---|---|---|
| `ProfileTagChip` | `repo://src/CanDoItAll.Components.BaseLib/Components/Badges/Compatibility/ProfileTagChip.razor` | `Chip` | SB12 consumer migration proof |
| `ProfileTagChipRow` | `repo://src/CanDoItAll.Components.BaseLib/Components/Badges/Compatibility/ProfileTagChipRow.razor` | `ChipRow` | SB12 consumer migration proof |
| `BuilderStatBox` | `repo://src/CanDoItAll.Components.BaseLib/Components/Cards/Compatibility/BuilderStatBox.razor` | `Card`, `SummaryTile`, or local layout markup depending on consumer shape | SB12 consumer migration proof |
| `BuilderStatStrip` | `repo://src/CanDoItAll.Components.BaseLib/Components/Cards/Compatibility/BuilderStatStrip.razor` | `SummaryTiles` or `Grid` with `SummaryTile` | SB12 consumer migration proof |
| `SheetCard` | `repo://src/CanDoItAll.Components.BaseLib/Components/Cards/Compatibility/SheetCard.razor` | `Card` or `SectionCard` | SB12 consumer migration proof |
| `SheetCardHeading` | `repo://src/CanDoItAll.Components.BaseLib/Components/Cards/Compatibility/SheetCardHeading.razor` | `TextBlock`, `SectionCard` title/description, or page header primitives | SB12 consumer migration proof |
| `SheetCardTop` | `repo://src/CanDoItAll.Components.BaseLib/Components/Cards/Compatibility/SheetCardTop.razor` | `Toolbar`, `Stack`, or explicit header layout | SB12 consumer migration proof |
| `SheetGrid` | `repo://src/CanDoItAll.Components.BaseLib/Components/Cards/Compatibility/SheetGrid.razor` | `Grid` | SB12 consumer migration proof |
| `SheetNote` | `repo://src/CanDoItAll.Components.BaseLib/Components/Cards/Compatibility/SheetNote.razor` | `Alert`, `EmptyState`, or `TextBlock` depending on message importance | SB12 consumer migration proof |
| `SheetSection` | `repo://src/CanDoItAll.Components.BaseLib/Components/Cards/Compatibility/SheetSection.razor` | `SectionCard` or `Card` | SB12 consumer migration proof |
| `DebugToggle` | `repo://src/CanDoItAll.Components.BaseLib/Components/Forms/Compatibility/DebugToggle.razor` | `Switch` or `CheckBox` inside a `FormField` | SB12 consumer migration proof |
| `ProfileField` | `repo://src/CanDoItAll.Components.BaseLib/Components/Forms/Compatibility/ProfileField.razor` | `FormField` | SB12 consumer migration proof |
| `ProfileToggle` | `repo://src/CanDoItAll.Components.BaseLib/Components/Forms/Compatibility/ProfileToggle.razor` | `Switch` or `CheckBox` | SB12 consumer migration proof |
| `SheetField` | `repo://src/CanDoItAll.Components.BaseLib/Components/Forms/Compatibility/SheetField.razor` | `FormField` | SB12 consumer migration proof |
| `TagTextEdit` | `repo://src/CanDoItAll.Components.BaseLib/Components/Forms/Compatibility/TagTextEdit.razor` | `TagEditor` | SB12 consumer migration proof |
| `CreatorAvatar` | `repo://src/CanDoItAll.Components.BaseLib/Components/Identity/Compatibility/CreatorAvatar.razor` | `Avatar` | SB12 consumer migration proof |
| `ZyWorkspaceModal` | `repo://src/CanDoItAll.Components.BaseLib/Components/Modals/Compatibility/ZyWorkspaceModal.razor` | `Dialog` | SB12 consumer migration proof |
| `DashboardActions` | `repo://src/CanDoItAll.Components.BaseLib/Components/Navigation/Compatibility/DashboardActions.razor` | `Toolbar`, `Button`, or `ButtonGroup` | SB12 consumer migration proof |
| `ImmersiveRibbonTabs` | `repo://src/CanDoItAll.Components.BaseLib/Components/Navigation/Compatibility/ImmersiveRibbonTabs.razor` | `RibbonTabs` | SB12 consumer migration proof |
| `PageHeaderActions` | `repo://src/CanDoItAll.Components.BaseLib/Components/Navigation/Compatibility/PageHeaderActions.razor` | `Toolbar` or page header action slots | SB12 consumer migration proof |
| `PageHeaderCopy` | `repo://src/CanDoItAll.Components.BaseLib/Components/Navigation/Compatibility/PageHeaderCopy.razor` | `PageHeader`, `TextBlock`, or page title/description slots | SB12 consumer migration proof |

## Publishing Rule

- Compatibility shims are included in `CanDoItAll.Components.BaseLib` packages until SB12 closes.
- Any removal must include an AppComponents/consumer migration table, compile proof for affected consumers, and updated public API approval snapshots.
- Any new compatibility shim must be added to this document, the SB01 inventory data, and the SB10 approval snapshots in the same change.
