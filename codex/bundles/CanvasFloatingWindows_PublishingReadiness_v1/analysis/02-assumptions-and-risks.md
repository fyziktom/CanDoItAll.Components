# Assumptions And Risks

## Assumptions

- The new pure repositories will receive polished CanvasLib and OverlayLib output later; this bundle prepares the source before transfer.
- CanvasLib remains the canonical shared canvas implementation for active consumers.
- OverlayLib remains the generic floating-window package and CanvasLib should wrap or consume it rather than forking generic window behavior.
- Existing sandbox routes are allowed to become stronger proof harnesses as long as they do not replace production behavior with fake fixtures.
- WebGL will receive its own separate publishing pass and should not be edited here.

## Critical Path Risks

- SB01, SB02, SB03, and SB04 are critical foundations. If inventory, window ownership, state contracts, or asset/runtime boundaries are wrong, every later Playwright and publishing proof can be misleading.
- Canvas runtime JavaScript refactors can pass compilation while breaking interaction dispatch, state replay, or asset load order.
- Floating-window CSS and JS changes can look correct in the overlay sandbox but fail inside the Canvas workbench safe-top/container model.
- Adding package/API approval late can reveal public-surface drift after behavior work is already done.
- A visual matrix that only checks page render and body text would miss the requested true validation for drag, resize, open overlays, keyboard routing, and floating-window layering.

## Validation Risks

- Canvas interactions rely on browser behavior, pointer events, layout measurements, and JS interop; unit tests alone are not enough.
- Playwright proof may need a headed or maximized browser for reliable visual review of stage composition and floating windows.
- The Canvas workbench has both retained DOM/SVG rendering and a benchmark page with a narrow true-canvas prototype; proof must avoid confusing benchmark output with shipped feature parity.
- Component MCP was unavailable during preparation. Execution should retry it, but local source inspection and sandbox proof must remain sufficient if it stays unavailable.
- Some CSS uses custom Canvas tokens and generated static assets rather than Tailwind. Refactor should improve clarity without forcing a risky Tailwind migration.

## Reopen Triggers

- Reopen SB01 if an implementation phase discovers missing Canvas, Overlay, sandbox, asset, package, or test surfaces in the inventory.
- Reopen SB02 if Canvas floating-window behavior diverges from OverlayLib behavior or duplicates generic geometry/lifecycle logic.
- Reopen SB03 if state roundtrip, serialization, selection, window geometry, calendar contracts, or layout model tests reveal ambiguous ownership.
- Reopen SB04 if asset verification, script load order, node syntax checks, or source assertions fail after runtime edits.
- Reopen SB05 or SB07 if screenshots show text overflow, clipping, bad layering, broken drag/resize/minimize, inaccessible controls, or dead available space.
- Reopen SB08 if any route/scenario in the visual matrix cannot be exercised through production sandbox paths.
- Reopen SB09 if package/API docs drift, README versions, static assets, public API approvals, or NuGet package contents do not match the intended open-source boundary.
