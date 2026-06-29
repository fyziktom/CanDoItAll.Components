# Scope Inventory

## Included Projects

| Project | Role | Publishing Concern | Owning Subbundles |
|---|---|---|---|
| `src/CanDoItAll.Components.CanvasLib` | Shared canvas runtime, workbench, graph, calendar, previews, assets | Large JS/CSS/Razor surface needs tests, docs, generated asset proof, package/API approval, and visual matrix | SB01, SB03-SB10 |
| `src/CanDoItAll.Components.OverlayLib` | Generic floating overlay/window package | Generic window behavior must remain stable and reusable by CanvasLib | SB01, SB02, SB07, SB09, SB10 |
| `src/CanDoItAll.Components.Sandbox` | Proof harness for Canvas and overlays | Needs focused routes, scenarios, test hooks, and visual matrix coverage | SB01, SB05-SB08 |
| `tests/CanDoItAll.Components.BaseLib.Tests` | Existing standard tests and approvals | Overlay tests exist; Canvas-specific approvals need a deliberate home or a new test project | SB02, SB03, SB09 |
| `tools/canvaslib` | Canvas asset manifest, generator, verifier | Generated asset synchronization must be proof-backed | SB04, SB09 |

## Key Routes And Scenarios

| Route | Current Purpose | Required Proof Expansion |
|---|---|---|
| `route groups/canvas` | Workbench, calendar, floating window, boundary previews | Workbench interaction, calendar, CanvasFloatingWindow, dense/empty/disabled/long text scenarios |
| `route groups/canvas/benchmark` | Retained DOM/SVG versus narrow true-canvas materialization evidence | Validate as benchmark-only evidence; do not treat as renderer parity |
| `route groups/overlays` | Dialogs, tooltips, toasts, sticky footer, OverlayWindow | Open-state floating-window proof and generic OverlayLib regression checks |

## Current Test And Approval Coverage

- Existing: `LayoutNavigationOverlayBehaviorTests` covers `OverlayWindowState.Normalize` and `OverlayWindowState.AreEquivalent`.
- Existing: `StandardPublishingApprovalTests` includes OverlayLib in standard public API/package snapshots.
- Missing: dedicated CanvasLib contract tests, CanvasLib public API approval, CanvasLib package content approval, Canvas asset-order approval, and browser-driven Canvas interaction proof.

## Planned Inventory Deliverables During SB01

- `bundle://inventories/canvas-floating-windows-publishing-map.xlsx`
- `bundle://inventories/current-state-data.json`
- `bundle://proof/SB01/transcripts/sb01-inventory-generation.txt`
- `bundle://proof/SB01/manifest.md`
- `bundle://proof/SB01/semantic-invariants.md`

