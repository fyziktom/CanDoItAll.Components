# SB08 Semantic Invariants

## Invariant SB08-LAYOUT-AVAILABLE-SPACE

- Raw note owned: RAW10 requires real Playwright proof for layout behavior, and the architect notes call out components that wrap badly or fail to use available width/height.
- Expected behavior: PageScaffold, ListDetailShell, StickyActionFooter, Grid/Row/Column layout-composition examples, and mobile long-text layout scenarios remain contained, use available space intentionally, and avoid dead stretched areas.
- Disallowed shallow implementation: add test ids or screenshots while leaving max-content tracks, forced empty rows, or hidden horizontal overflow in layout demos.
- Failing-first proof: `bundle://proof/SB08/data/sb08-visual-repair-observations.json` records the max-content overflow and forced empty row stretch found during the SB08 verifier/MCP screenshot loop.
- Passing proof: `bundle://proof/SB08/transcripts/sb08-playwright-verifier.txt` and `bundle://proof/SB08/data/sb08-layout-navigation-overlays-validation.json` show no page/component horizontal overflow for `/groups/layout` and `/groups/layout/composition` on desktop and mobile long-text viewports.
- Browser proof: `bundle://proof/SB08/screenshots/mcp/sb08-layout-desktop-full.png`, `bundle://proof/SB08/screenshots/mcp/sb08-layout-mobile-long-full.png`, `bundle://proof/SB08/screenshots/mcp/sb08-layout-composition-desktop-full.png`, `bundle://proof/SB08/screenshots/mcp/sb08-layout-composition-mobile-long-full.png`.
- Source proof: `repo://src/CanDoItAll.Components.BaseLib/Components/Layout/StickyActionFooter.razor`, `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Layout.razor`, `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/LayoutComposition.razor`.
- Anti-stub proof: `bundle://proof/SB08/transcripts/sb08-anti-stub-audit.txt`.

## Invariant SB08-NAVIGATION-INTERACTION

- Raw note owned: RAW10 requires open-state screenshots and real action proof, including menus/dropdowns/tree-like controls where layout defects appear only after interaction.
- Expected behavior: Tabs, tabs lab, Steps, Toolbar, TreeView, ContextMenu host, and the visible TreeView right-click menu support selection/filtering/expansion/context-menu interactions and stay viewport-contained.
- Disallowed shallow implementation: prove only static navigation markup, omit Toolbar/TreeView/ContextMenu from the sandbox, or fake a context menu without a TreeView event.
- Passing proof: the SB08 verifier clicks tabs, advances Steps, filters the Toolbar, expands TreeView, opens the TreeView context menu by right click, verifies the hidden ContextMenu host, and reports 67 checks with 0 console errors.
- Browser proof: `bundle://proof/SB08/screenshots/mcp/sb08-navigation-desktop-context-menu.png`, `bundle://proof/SB08/screenshots/mcp/sb08-navigation-tabs-mobile-long-full.png`.
- Source proof: `repo://src/CanDoItAll.Components.BaseLib/Components/Navigation/ContextMenu.razor`, `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Navigation.razor`, `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/NavigationTabs.razor`.
- Test proof: `bundle://proof/SB08/transcripts/sb08-baselib-tests.txt` includes the wrapper contract test for ContextMenu.
- Anti-stub proof: `bundle://proof/SB08/transcripts/sb08-anti-stub-audit.txt`.

## Invariant SB08-OVERLAY-LAYERING

- Raw note owned: RAW10 requires real screenshots for overlay/menu/dialog open states and visual inspection for clipping/layering.
- Expected behavior: DialogService/DialogHost, HelpPopover, TooltipService, NotificationService, StickyActionFooter, and OverlayWindow render through shared hosts; open states remain readable, dismissible, layered correctly, and bounded in desktop/mobile viewports.
- Disallowed shallow implementation: mount overlay components without opening them, rely on CanvasLib's transitive OverlayLib dependency accidentally, skip mobile OverlayWindow proof, or skip backdrop/tooltip/toast lifecycle checks.
- Failing-first proof: `bundle://proof/SB08/data/sb08-visual-repair-observations.json` records the duplicate static-web-assets failure that appeared when OverlayLib was referenced directly without hardening the sandbox dependency graph.
- Passing proof: the SB08 verifier opens compact/backdrop/result dialogs, verifies backdrop lock, hovers TooltipService, opens NotificationService toast, verifies OverlayWindow normal/minimized/hidden/show states inside its host frame, and reports 67 checks with 0 console errors.
- Browser proof: `bundle://proof/SB08/screenshots/mcp/sb08-overlays-desktop-backdrop-dialog.png`, `bundle://proof/SB08/screenshots/mcp/sb08-overlays-desktop-help-popover.png`, `bundle://proof/SB08/screenshots/mcp/sb08-overlays-desktop-toast-tooltip-window.png`, `bundle://proof/SB08/screenshots/mcp/sb08-overlays-mobile-long-window-full.png`.
- Source proof: `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Overlays.razor`, `repo://src/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj`, `repo://src/CanDoItAll.Components.Sandbox/_Imports.razor`.
- Test proof: `bundle://proof/SB08/transcripts/sb08-baselib-tests.txt` includes OverlayWindowState normalization/equivalence tests and StickyActionFooter wrapper contract proof.
- Anti-stub proof: `bundle://proof/SB08/transcripts/sb08-anti-stub-audit.txt`.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `OverlayWindowSandboxState` | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Overlays.razor` creates and normalizes `OverlayWindowState` for the sandbox example. | `repo://src/CanDoItAll.Components.OverlayLib/Components/Core/OverlayWindow.razor` consumes the state; screenshots `bundle://proof/SB08/screenshots/mcp/sb08-overlays-desktop-toast-tooltip-window.png` and `bundle://proof/SB08/screenshots/mcp/sb08-overlays-mobile-long-window-full.png` prove the rendered window. | `bundle://proof/SB08/data/sb08-layout-navigation-overlays-validation.json` proves normal, minimized, hidden, and show-again lifecycle states remain inside the host frame. | `bundle://proof/SB08/data/sb08-visual-repair-observations.json` records the static-web-assets duplication failure that would make direct OverlayLib sandbox proof unstable in publishing repositories. |

## Semantic Gate Decision

Pass. SB08 includes source proof, tests, 67 browser checks, Playwright MCP screenshots for open states, visual repair observations, anti-stub audit output, and portable hash proof.
