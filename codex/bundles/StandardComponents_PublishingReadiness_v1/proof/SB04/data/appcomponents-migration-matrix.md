# SB04 AppComponents Migration Matrix

## Kept App-Specific Surfaces

| File | Status | Reason |
|---|---|---|
| `Components/AppShell.razor` | App-specific | Main CanDoItAll shell, navigation, project context, and workbench chrome. |
| `Components/AppShellMode.cs` | App-specific | Shell mode primitive. |
| `Components/AppShellNavigationMode.cs` | App-specific | Shell navigation mode primitive. |
| `Components/AppTabStrip.razor` | App-specific | Main app workspace tab management, recent tabs, overflow tabs, and app route metadata. |
| `Components/TunableComponentBoundary.razor` | App-specific | Tuning coordinator integration for the main app. |
| `Components/TuningBoundaryRequest.cs` | App-specific | Main app tuning request model. |

## Deleted Parked Basic Duplicates

These files were already excluded from compilation by `CanDoItAll.AppComponents.csproj`; SB04 removes the parked source copies so basic components live only in the standard component packages.

| Deleted AppComponents file | Standard owner / replacement | Status |
|---|---|---|
| `Primitives/ComponentPrimitives.cs` | `CanDoItAll.Components.Common` layout primitives; `CanDoItAll.Components.BaseLib` button, typography, feedback, tabs, notification, and attribute helpers | Solved: deleted parked duplicate. |
| `Components/Alert.razor` | `BaseLib/Components/Feedback/Alert.razor` | Solved: deleted parked duplicate. |
| `Components/Body.razor` | `BaseLib/Components/Layout/Body.razor` | Solved: deleted parked duplicate. |
| `Components/Button.razor` | `BaseLib/Components/Buttons/Button.razor` | Solved: deleted parked duplicate; old in-flight click guard ported to BaseLib. |
| `Components/Card.razor` | `BaseLib/Components/Cards/Card.razor` | Solved: deleted parked duplicate. |
| `Components/CategoryAxis.razor` | `BaseLib/Components/DataVisualization/CategoryAxis.razor` | Solved: deleted parked duplicate. |
| `Components/Chart.razor` | `BaseLib/Components/DataVisualization/Chart.razor`; modern package owner is `Components.Charts/Components/CdaChart.razor` | Solved: deleted parked duplicate. |
| `Components/CheckBox.razor` | `BaseLib/Components/Forms/CheckBox.razor` | Solved: deleted parked duplicate. |
| `Components/Column.razor` | `BaseLib/Components/Layout/Column.razor` | Solved: deleted parked duplicate. |
| `Components/ContextMenu.razor` | No active compiled AppComponents surface; overlay/menu work remains in standard packages | Solved: deleted parked duplicate. |
| `Components/DataGrid.razor` | `BaseLib/Components/DataVisualization/DataGrid.razor` | Solved: deleted parked duplicate. |
| `Components/DataGridColumn.razor` | `BaseLib/Components/DataVisualization/DataGridColumn.razor` | Solved: deleted parked duplicate. |
| `Components/Dialog.razor` | `BaseLib/Components/Modals/Dialog.razor` | Solved: deleted parked duplicate. |
| `Components/DropDown.razor` | `BaseLib/Components/Forms/DropDown.razor` | Solved: deleted parked duplicate. |
| `Components/DropDownOption.cs` | `BaseLib/Components/Forms/DropDownOption.cs` | Solved: deleted parked duplicate. |
| `Components/Fieldset.razor` | `BaseLib/Components/Forms/Fieldset.razor` | Solved: deleted parked duplicate. |
| `Components/FormField.razor` | `BaseLib/Components/Forms/FormField.razor` | Solved: deleted parked duplicate. |
| `Components/GridLines.razor` | `BaseLib/Components/DataVisualization/GridLines.razor` | Solved: deleted parked duplicate. |
| `Components/Header.razor` | `BaseLib/Components/Typography/Header.razor` | Solved: deleted parked duplicate. |
| `Components/Icon.razor` | `BaseLib/Components/Identity/Icon.razor` | Solved: deleted parked duplicate. |
| `Components/Layout.razor` | `BaseLib/Components/Layout/Layout.razor` | Solved: deleted parked duplicate. |
| `Components/LineSeries.razor` | `BaseLib/Components/DataVisualization/LineSeries.razor` | Solved: deleted parked duplicate. |
| `Components/Notification.razor` | `BaseLib/Components/Feedback/Notification.razor` and `NotificationService` | Solved: deleted parked duplicate. |
| `Components/Numeric.razor` | `BaseLib/Components/Forms/Numeric.razor` | Solved: deleted parked duplicate. |
| `Components/Password.razor` | `BaseLib/Components/Forms/Password.razor` | Solved: deleted parked duplicate. |
| `Components/ProgressBar.razor` | `BaseLib/Components/DataVisualization/ProgressBar.razor` | Solved: deleted parked duplicate. |
| `Components/Row.razor` | `BaseLib/Components/Layout/Row.razor` | Solved: deleted parked duplicate. |
| `Components/Sidebar.razor` | `BaseLib/Components/Layout/Sidebar.razor` | Solved: deleted parked duplicate. |
| `Components/Slider.razor` | `BaseLib/Components/Forms/Slider.razor` | Solved: deleted parked duplicate. |
| `Components/Stack.razor` | `BaseLib/Components/Layout/Stack.razor` | Solved: deleted parked duplicate. |
| `Components/Steps.razor` | `BaseLib/Components/Navigation/Steps.razor` | Solved: deleted parked duplicate. |
| `Components/StepsItem.razor` | `BaseLib/Components/Navigation/StepsItem.razor` | Solved: deleted parked duplicate. |
| `Components/Switch.razor` | `BaseLib/Components/Forms/Switch.razor` | Solved: deleted parked duplicate. |
| `Components/Tabs.razor` | `BaseLib/Components/Navigation/Tabs.razor` | Solved: deleted parked duplicate. |
| `Components/TabsItem.razor` | `BaseLib/Components/Navigation/TabsItem.razor` | Solved: deleted parked duplicate. |
| `Components/TextArea.razor` | `BaseLib/Components/Forms/TextArea.razor` | Solved: deleted parked duplicate. |
| `Components/TextBlock.razor` | `BaseLib/Components/Typography/TextBlock.razor` | Solved: deleted parked duplicate. |
| `Components/TextBox.razor` | `BaseLib/Components/Forms/TextBox.razor` | Solved: deleted parked duplicate. |
| `Components/Tooltip.razor` | `BaseLib/Components/Feedback/Tooltip.razor` and `TooltipTarget.razor` | Solved: deleted parked duplicate. |
| `Components/ValueAxis.razor` | `BaseLib/Components/DataVisualization/ValueAxis.razor` | Solved: deleted parked duplicate. |

## Behavior Ported

| Behavior | Old source | New source | Proof |
|---|---|---|---|
| Prevent concurrent button click callbacks while an async click is in flight | Deleted `AppComponents/Components/Button.razor` | `BaseLib/Components/Buttons/Button.razor` | `tests/CanDoItAll.Components.BaseLib.Tests/ButtonBehaviorTests.cs` and `proof/SB04/transcripts/sb04-baselib-button-tests.txt`. |
