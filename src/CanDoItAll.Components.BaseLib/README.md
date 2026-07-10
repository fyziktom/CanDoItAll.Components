# CanDoItAll.Components.BaseLib

Package version: `0.1.0`.

## Purpose

BaseLib is the everyday UI toolkit in CanDoItAll.Components: ready-made, Tailwind-styled Razor components for product pages, forms, navigation, feedback, layout, dialogs, and application chrome. Use it to establish a consistent Blazor UI quickly while keeping your page state and business behavior in the application.

## Get started

Add the namespace, register BaseLib services once, and include the shared stylesheet in the host document:

```csharp
// Program.cs
builder.Services.AddCanDoItAllBaseLib();
```

```razor
@* App.razor <head> *@
<link rel="stylesheet" href="_content/CanDoItAll.Components.BaseLib/css/output.css" />
```

```razor
@using CanDoItAll.Components.BaseLib

<SectionCard Title="Release review" Description="A composed surface using shared spacing and Tailwind styling.">
    <Stack GapScale="LayoutGap.Medium">
        <StatusBadge Text="Ready" Tone="success" />
        <Button Text="Open review" />
    </Stack>
</SectionCard>
```

Use BaseLib for ordinary, document-style product UI. Choose [OverlayLib](../CanDoItAll.Components.OverlayLib/README.md) for bounded floating tools, [CanvasLib](../CanDoItAll.Components.CanvasLib/README.md) for stateful workbenches, and the focused packages for charts, Mermaid, QR, or WebGL.

## What is included

The catalog is grouped by the way developers assemble pages: visual status and actions; cards and metric panels; forms and data display; layout and navigation; dialog, tooltip, and notification feedback; and supporting typography. Compatibility components remain listed so existing consumers can migrate intentionally.

## Component Catalog

BaseLib currently exposes 163 Razor components. Links point to the component source file.

### Badges, Chips, And Status

- [Badge](Components/Badges/Badge.razor)
- [BadgesGroup](Components/Badges/BadgesGroup.razor)
- [Chip](Components/Badges/Chip.razor)
- [ChipRow](Components/Badges/ChipRow.razor)
- [CompactStat](Components/Badges/CompactStat.razor)
- [CompactStatStrip](Components/Badges/CompactStatStrip.razor)
- [Pill](Components/Badges/Pill.razor)
- [PillList](Components/Badges/PillList.razor)
- [ProfileTagChip](Components/Badges/Compatibility/ProfileTagChip.razor)
- [ProfileTagChipRow](Components/Badges/Compatibility/ProfileTagChipRow.razor)
- [StatusBadge](Components/Badges/StatusBadge.razor)

### Buttons And Commands

- [Button](Components/Buttons/Button.razor)
- [CopyButton](Components/Buttons/CopyButton.razor)

### Cards, Panels, And Metrics

- [ActionCard](Components/Cards/ActionCard.razor)
- [ActionReviewPanel](Components/Cards/ActionReviewPanel.razor)
- [AuthCard](Components/Cards/AuthCard.razor)
- [BuilderStatBox](Components/Cards/Compatibility/BuilderStatBox.razor)
- [BuilderStatStrip](Components/Cards/Compatibility/BuilderStatStrip.razor)
- [Card](Components/Cards/Card.razor)
- [CardActions](Components/Cards/CardActions.razor)
- [CardButton](Components/Cards/CardButton.razor)
- [CardGrid](Components/Cards/CardGrid.razor)
- [CardStatsWithNumber](Components/Cards/CardStatsWithNumber.razor)
- [HeroCard](Components/Cards/HeroCard.razor)
- [MetricCard](Components/Cards/MetricCard.razor)
- [PanelCard](Components/Cards/PanelCard.razor)
- [ParitySectionCard](Components/Cards/ParitySectionCard.razor)
- [PriceBar](Components/Cards/PriceBar.razor)
- [PriceRow](Components/Cards/PriceRow.razor)
- [SectionCard](Components/Cards/SectionCard.razor)
- [SheetCard](Components/Cards/Compatibility/SheetCard.razor)
- [SheetCardHeading](Components/Cards/Compatibility/SheetCardHeading.razor)
- [SheetCardTop](Components/Cards/Compatibility/SheetCardTop.razor)
- [SheetGrid](Components/Cards/Compatibility/SheetGrid.razor)
- [SheetNote](Components/Cards/Compatibility/SheetNote.razor)
- [SheetSection](Components/Cards/Compatibility/SheetSection.razor)
- [StatBox](Components/Cards/StatBox.razor)
- [StatsCardRow](Components/Cards/StatsCardRow.razor)
- [StatsGrid](Components/Cards/StatsGrid.razor)
- [SummaryTile](Components/Cards/SummaryTile.razor)
- [SummaryTiles](Components/Cards/SummaryTiles.razor)
- [SurfaceCard](Components/Cards/SurfaceCard.razor)

### Data Display And Timelines

- [DiffViewer](Components/DataDisplay/DiffViewer.razor)
- [Timeline](Components/DataDisplay/Timeline.razor)
- [TimelineStepper](Components/DataDisplay/TimelineStepper.razor)

### Data Visualization

- [CategoryAxis](Components/DataVisualization/CategoryAxis.razor)
- [Chart](Components/DataVisualization/Chart.razor)
- [DataGrid](Components/DataVisualization/DataGrid.razor)
- [DataGridColumn](Components/DataVisualization/DataGridColumn.razor)
- [GridLines](Components/DataVisualization/GridLines.razor)
- [LineSeries](Components/DataVisualization/LineSeries.razor)
- [ProgressBar](Components/DataVisualization/ProgressBar.razor)
- [ValueAxis](Components/DataVisualization/ValueAxis.razor)

### Feedback, Help, And Overlays

- [Alert](Components/Feedback/Alert.razor)
- [Callout](Components/Feedback/Callout.razor)
- [EmptyState](Components/Feedback/EmptyState.razor)
- [HelpPopover](Components/Feedback/HelpPopover.razor)
- [LoadingState](Components/Feedback/LoadingState.razor)
- [Notification](Components/Feedback/Notification.razor)
- [StatusCheckList](Components/Feedback/StatusCheckList.razor)
- [Tooltip](Components/Feedback/Tooltip.razor)
- [TooltipTarget](Components/Feedback/TooltipTarget.razor)
- [VerificationList](Components/Feedback/VerificationList.razor)

### Forms And Inputs

- [CheckBox](Components/Forms/CheckBox.razor)
- [DebugToggle](Components/Forms/Compatibility/DebugToggle.razor)
- [DropDown](Components/Forms/DropDown.razor)
- [Editable](Components/Forms/Editable.razor)
- [EntityPicker](Components/Forms/EntityPicker.razor)
- [Fieldset](Components/Forms/Fieldset.razor)
- [FileUpload](Components/Forms/FileUpload.razor)
- [FormField](Components/Forms/FormField.razor)
- [FormRow](Components/Forms/FormRow.razor)
- [FormSection](Components/Forms/FormSection.razor)
- [FormStack](Components/Forms/FormStack.razor)
- [InlineActions](Components/Forms/InlineActions.razor)
- [Numeric](Components/Forms/Numeric.razor)
- [Password](Components/Forms/Password.razor)
- [PrefixedField](Components/Forms/PrefixedField.razor)
- [ProfileField](Components/Forms/Compatibility/ProfileField.razor)
- [ProfileToggle](Components/Forms/Compatibility/ProfileToggle.razor)
- [SecretField](Components/Forms/SecretField.razor)
- [SettingsSwitchLabel](Components/Forms/SettingsSwitchLabel.razor)
- [SettingsSwitchRow](Components/Forms/SettingsSwitchRow.razor)
- [SheetField](Components/Forms/Compatibility/SheetField.razor)
- [Slider](Components/Forms/Slider.razor)
- [Switch](Components/Forms/Switch.razor)
- [TagEditor](Components/Forms/TagEditor.razor)
- [TagTextEdit](Components/Forms/Compatibility/TagTextEdit.razor)
- [TextArea](Components/Forms/TextArea.razor)
- [TextBox](Components/Forms/TextBox.razor)

### Identity And Icons

- [Avatar](Components/Identity/Avatar.razor)
- [CreatorAvatar](Components/Identity/Compatibility/CreatorAvatar.razor)
- [CreatorLine](Components/Identity/CreatorLine.razor)
- [CreatorSocialLink](Components/Identity/CreatorSocialLink.razor)
- [Icon](Components/Identity/Icon.razor)

### Layout And Shells

- [Body](Components/Layout/Body.razor)
- [Cluster](Components/Layout/Cluster.razor)
- [Column](Components/Layout/Column.razor)
- [Grid](Components/Layout/Grid.razor)
- [Layout](Components/Layout/Layout.razor)
- [PageScaffold](Components/Layout/PageScaffold.razor)
- [PageShell](Components/Layout/PageShell.razor)
- [Row](Components/Layout/Row.razor)
- [Sidebar](Components/Layout/Sidebar.razor)
- [Split](Components/Layout/Split.razor)
- [Stack](Components/Layout/Stack.razor)
- [StickyActionFooter](Components/Layout/StickyActionFooter.razor)
- [ThemeHost](Components/Layout/ThemeHost.razor)
- [WorkspacePanel](Components/Layout/WorkspacePanel.razor)
- [WorkspaceSplit](Components/Layout/WorkspaceSplit.razor)

### Lists And Selection

- [FactTable](Components/Lists/FactTable.razor)
- [ListDetailShell](Components/Lists/ListDetailShell.razor)
- [ListGroup](Components/Lists/ListGroup.razor)
- [ListItem](Components/Lists/ListItem.razor)
- [ListPanelHeader](Components/Lists/ListPanelHeader.razor)
- [MetaList](Components/Lists/MetaList.razor)
- [PlainList](Components/Lists/PlainList.razor)
- [SelectionListItem](Components/Lists/SelectionListItem.razor)

### Modals And Dialogs

- [DangerActionDialog](Components/Modals/DangerActionDialog.razor)
- [Dialog](Components/Modals/Dialog.razor)
- [DialogHost](Components/Modals/DialogHost.razor)
- [DialogScaffold](Components/Modals/DialogScaffold.razor)
- [InspectorDialogLayout](Components/Modals/InspectorDialogLayout.razor)
- [PickerDialogShell](Components/Modals/PickerDialogShell.razor)
- [ZyWorkspaceModal](Components/Modals/Compatibility/ZyWorkspaceModal.razor)

### Navigation, Tabs, And Toolbars

- [ContextMenu](Components/Navigation/ContextMenu.razor)
- [DashboardActions](Components/Navigation/Compatibility/DashboardActions.razor)
- [FilterBar](Components/Navigation/FilterBar.razor)
- [ImmersiveRibbonTabs](Components/Navigation/Compatibility/ImmersiveRibbonTabs.razor)
- [LegalToc](Components/Navigation/LegalToc.razor)
- [LegalTocNav](Components/Navigation/LegalTocNav.razor)
- [PageHeader](Components/Navigation/PageHeader.razor)
- [PageHeaderActionButton](Components/Navigation/PageHeaderActionButton.razor)
- [PageHeaderActions](Components/Navigation/Compatibility/PageHeaderActions.razor)
- [PageHeaderCopy](Components/Navigation/Compatibility/PageHeaderCopy.razor)
- [RibbonTabs](Components/Navigation/RibbonTabs.razor)
- [SecondaryTabs](Components/Navigation/SecondaryTabs.razor)
- [SideMenu](Components/Navigation/SideMenu.razor)
- [SideMenuItem](Components/Navigation/SideMenuItem.razor)
- [Steps](Components/Navigation/Steps.razor)
- [StepsItem](Components/Navigation/StepsItem.razor)
- [Tabs](Components/Navigation/Tabs.razor)
- [TabsItem](Components/Navigation/TabsItem.razor)
- [Toolbar](Components/Navigation/Toolbar.razor)
- [ToolbarActions](Components/Navigation/ToolbarActions.razor)
- [ToolbarFields](Components/Navigation/ToolbarFields.razor)
- [ToolbarRow](Components/Navigation/ToolbarRow.razor)
- [TreeView](Components/Navigation/TreeView.razor)
- [TreeViewNodeRow](Components/Navigation/TreeViewNodeRow.razor)

### Storage

- [StorageBadgeStrip](Components/Storage/StorageBadgeStrip.razor)
- [StorageSummaryCard](Components/Storage/StorageSummaryCard.razor)

### Typography

- [CopyableMonoValue](Components/Typography/CopyableMonoValue.razor)
- [Divider](Components/Typography/Divider.razor)
- [Eyebrow](Components/Typography/Eyebrow.razor)
- [FooterText](Components/Typography/FooterText.razor)
- [HashDisplay](Components/Typography/HashDisplay.razor)
- [Header](Components/Typography/Header.razor)
- [MonoText](Components/Typography/MonoText.razor)
- [MutedInline](Components/Typography/MutedInline.razor)
- [SectionHead](Components/Typography/SectionHead.razor)
- [SectionHeading](Components/Typography/SectionHeading.razor)
- [SmallText](Components/Typography/SmallText.razor)
- [TextBlock](Components/Typography/TextBlock.razor)

## Project Type

- SDK: `Microsoft.NET.Sdk.Razor`
- Target framework(s): `net10.0`
- Validation command:

```powershell
dotnet build src/CanDoItAll.Components.BaseLib/CanDoItAll.Components.BaseLib.csproj
```

## References

Project references:

- `../CanDoItAll.Components.Common/CanDoItAll.Components.Common.csproj`

Framework references:

- None

Direct package references:

- `Microsoft.AspNetCore.Components.Web (10.0.4)`

## Architecture Notes

Keep shared UI reusable and typed. Use BaseLib for ordinary product UI, CanvasLib for graph/canvas surfaces, OverlayLib for floating windows, WebGlLib for WebGL concepts, and sandbox projects only for demos or proof.

## Side Menu

`SideMenu` accepts typed items through `Items`, `MoreItems`, and `BottomItems`, and it also supports declarative `SideMenuItem` tags in the matching Razor regions. Both sources are composed into the same selection and overflow model:

```razor
<SideMenu MenuId="workspace"
          Title="Workspace"
          Items="@PinnedItems"
          @bind-Expanded="menuExpanded"
          ItemSelected="HandleSelection">
    <MenuItems>
        <SideMenuItem Id="timeline" Text="Timeline" Icon="timeline" />
        <SideMenuItem Id="audit"
                      Text="Audit archive"
                      Icon="inventory"
                      OverflowBehavior="SideMenuOverflowBehavior.AlwaysInMore" />
    </MenuItems>
    <MoreContent>
        @* Optional custom content below the typed More items. *@
    </MoreContent>
    <BottomMenuItems>
        <SideMenuItem Id="settings" Text="Settings" Icon="settings">
            <PanelContent>@* Optional BaseLib subcard or menu. *@</PanelContent>
        </SideMenuItem>
    </BottomMenuItems>
</SideMenu>
```

Use the scoped `SideMenuService` when a tab or another application feature needs to drive the menu without a component reference. `SetItems` temporarily replaces the declared primary items, `ResetItems` restores them, and external selection follows the same callback pipeline as pointer selection:

```csharp
@inject SideMenuService SideMenus
@implements IDisposable

@code {
    private IDisposable? subscription;

    protected override void OnInitialized()
        => subscription = SideMenus.Subscribe(
            "workspace",
            selection => HandleSelectionAsync(selection));

    private void ShowProjectMenu(IReadOnlyList<ISideMenuItem> items)
        => SideMenus.SetItems("workspace", items);

    private Task<bool> SelectTimelineAsync()
        => SideMenus.SelectAsync("workspace", "timeline");

    public void Dispose() => subscription?.Dispose();
}
```

`SetExpanded` and `ToggleExpanded` provide external state control. The component also exposes two-way `Expanded` binding and, by default, remembers each `MenuId` under `localStorage`; set `PersistExpandedState="false"` when persistence belongs to application settings instead. Desktop item capacity is measured from the real available height, while the small breakpoint changes the rail into a top menu that opens downward.

## Overlay Services

BaseLib provides scoped services for app-level overlays:

- `DialogService` opens service-driven dialogs through a mounted `<DialogHost />`.
- `TooltipService` opens pointer-positioned tooltips through a mounted `<Tooltip />`.
- `NotificationService` owns toast messages rendered by a mounted `<Notification />`.

Register the services once:

```csharp
builder.Services.AddCanDoItAllBaseLib();
```

Mount the hosts once in the interactive layout:

```razor
<DialogHost />
<Tooltip />
<Notification />
```

`DialogService.OpenAsync(...)` returns the object supplied to `DialogReference.CloseAsync(result)`. Existing direct `<Dialog IsOpen="...">` usage remains supported for controlled component flows.

Notifications can be positioned per message with `NotificationMessage.Position` or the `Notify(..., position: ...)` overload. Supported positions cover top, center, and bottom stacks on the left, center, and right edges, with `TopRight` as the default.

Tooltips can be positioned with `TooltipOptions.Position` or `TooltipTarget Position`. The enum supports the standard `Top`, `Bottom`, `Left`, and `Right` placements plus corner and edge alignments such as `TopLeft`, `BottomRight`, `LeftTop`, and `RightBottom`.

### Overlay Positioning Guidance

Notification placement should protect the current workflow first. Use `TopRight` for ordinary desktop toasts, `BottomCenter` when top navigation or mobile reach would make top stacks awkward, and side positions such as `BottomLeft`, `CenterLeft`, or `CenterRight` when the message belongs near a side rail, list pane, or action region. Reserve `TopCenter` for global high-importance messages that still do not require a decision. If the user must choose, confirm, or resolve something before continuing, use `DialogService` or an inline `Alert` instead of a notification.

Notification alerts intentionally use only the compact X close control so the message width stays available for useful summary and detail text. Keep copy short, set `Duration` deliberately, and use per-message `Position` when one notification needs to appear somewhere other than the host default.

Tooltip placement should keep the bubble visible and away from the next likely action. Use `Top` or `Right` when there is room, `Bottom` for triggers near the top edge, `Top` for triggers near lower toolbars or footers, and `Left` or `Right` for dense inline controls. Use corner or edge placements such as `TopLeft`, `BottomRight`, `LeftTop`, or `RightBottom` near viewport, card, toolbar, or panel corners so the tooltip does not cover neighboring controls.

For agent-driven changes, query the Components MCP metadata for `Notification`, `Tooltip`, or `TooltipTarget` before choosing non-default positions, then validate unusual placements in the sandbox with Playwright at the viewport sizes used by the target page.

## Related Docs

- [Repository overview](../../README.md)
- [Canvas workspace guide](../../docs/canvas/README.md)
