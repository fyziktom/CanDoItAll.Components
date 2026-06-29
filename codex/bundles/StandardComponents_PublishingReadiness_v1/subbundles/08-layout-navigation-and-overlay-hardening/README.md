# SB08 Layout Navigation And Overlay Hardening

## Status

- Status: `Completed`

## Objective

Harden structural components that control available space, navigation, overlays, dialogs, and layout composition.

## Covered Inputs

- RAW10: Real Playwright screenshots one by one, including interactive states.

## Prerequisites

- Checkpoint B passed.
- SB05 route/test hooks for overlays available.

## Exact Source References

- repo://src/CanDoItAll.Components.BaseLib/Components/Layout
- repo://src/CanDoItAll.Components.BaseLib/Components/Navigation
- repo://src/CanDoItAll.Components.BaseLib/Components/Modals
- repo://src/CanDoItAll.Components.OverlayLib
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Layout.razor
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Navigation.razor
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Overlays.razor

## Deliverables

- Layout/navigation/overlay fixes.
- Open-state visual proof.
- Keyboard/focus checks where feasible.

## Dependency Impact

- Depends on foundations and feeds later app migration.
- Overlay clipping defects can reopen Tailwind or sandbox foundations.

## Validation Depth

- Deep UI proof for responsive layout and open overlays.
- Dependent smoke through migrated AppComponents surfaces if SB04 touched them.

## Implementation Steps

- Validate Stack, Grid, Row, Column, Split, PageScaffold, PageHeader, Sidebar, WorkspaceSplit, Tabs, SecondaryTabs, Steps, ContextMenu, Toolbar, TreeView, Dialog, DialogHost, StickyActionFooter, OverlayWindow.
- Open every overlay/menu/dialog state and inspect layering/clipping.
- Fix width/height stretch and dead-space defects.

## Scope Exceptions

- WebGL and Canvas implementation changes are excluded unless this subbundle explicitly updates sandbox separation metadata without touching their component internals.

## Do Not Do

- Do not refactor unrelated WebGL or Canvas source.
- Do not delete compatibility or AppComponents code without migration proof and consumer checks.
- Do not close UI work from source inspection only.

## Acceptance Checklist

- Desktop uses available space intentionally.
- Mobile first viewport remains oriented.
- Overlays are readable, unclipped, layered correctly, and dismissible.

## Proof Required

- Build/test transcript.
- Playwright screenshot matrix for open states.
- Keyboard/focus assertions where practical.

## Browser Validation Logging

- Routes: /groups/layout, /groups/layout/composition, /groups/navigation, /groups/navigation/tabs, /groups/overlays.
- Actions: open dialogs, popovers, context menus, tooltips, tab overflow, tree expansion.
- Viewports: maximized desktop, 1366x900, 390x844.

## Progression Gate

- The subbundle validator must pass closure review before downstream dependent subbundles start.
- If proof is weak or a screenshot shows wrapping, clipping, layout, available-space, or interaction defects, keep this subbundle `In progress` and reopen prerequisites as needed.

## Suggested Agent Prompt

Execute SB08 as the most visual standard-component pass: open the overlays and inspect the actual screenshots before claiming success.
