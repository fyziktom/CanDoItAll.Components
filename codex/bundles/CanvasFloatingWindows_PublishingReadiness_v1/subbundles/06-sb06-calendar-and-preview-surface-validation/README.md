# SB06 Calendar And Preview Surface Validation

## Status

- `Ready`

## Objective

- Prove CanvasLib calendar and preview surfaces as first-class publishing surfaces through contract review, browser actions, screenshots, and focused repairs.

## Covered Inputs

- RAW03: True validation of Canvas.
- RAW05: Preserve all functionality.
- RAW06: Improve maintainability, clarity, and documentation for open source.
- R04, R09, R11, R12.

## Prerequisites

- SB03 contract tests passed.
- SB04 asset/runtime foundation passed.
- SB05 workbench route proof passed or documented a non-calendar blocker.
- Sandbox `/groups/canvas` route is runnable.

## Exact Source References

- repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CanvasCalendar.razor
- repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CalendarTimeGridRenderer.razor
- repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CalendarSelectionPanel.razor
- repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CalendarMiniMonthNavigator.razor
- repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CalendarExportMenu.razor
- repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CalendarEventEditorModal.razor
- repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CalendarCrudBridge.razor
- repo://src/CanDoItAll.Components.CanvasLib/Canvas/Calendar/CanvasCalendarContracts.cs
- repo://src/CanDoItAll.Components.CanvasLib/Canvas/Calendar
- repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/canvasCalendarInterop.js
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor
- repo://src/CanDoItAll.Components.Sandbox/Components/Canvas

## Deliverables

- Calendar and preview browser verifier or Playwright proof script.
- Screenshots and action traces for calendar, preview cards, and edge states.
- Contract and source assertions for selection, editor, CRUD, playlist/search, export, range, and callback behavior.
- Minimal repairs needed to preserve current calendar and preview behavior.
- Documentation notes for any public calendar or preview usage patterns discovered during validation.

## Dependency Impact

- SB08 matrix depends on stable Canvas route scenarios and clear calendar/preview test hooks.
- SB09 package/docs proof depends on the calendar public contract being understood and documented.
- SB10 final closure depends on proof that CanvasLib validation was not limited to the workbench shell.

## Validation Depth

- Critical UI foundation.
- Browser action proof, contract/source assertions, screenshots, and semantic proof.
- Artifact-backed proof manifest required.

## Implementation Steps

1. Start the sandbox and open `/groups/canvas`.
2. Locate the calendar and preview scenarios; add focused sandbox controls only when production behavior cannot otherwise be exercised.
3. Exercise calendar selection, date range navigation, time grid rendering, mini month navigation, event editor, CRUD callbacks, playlist/search/link/clone/unlink behavior where exposed, and export.
4. Exercise preview components for happy, dense, empty, disabled/loading, and long-text states.
5. Verify timezone/date handling with deterministic fixtures or source assertions.
6. Repair defects in the smallest scoped way and preserve public contracts.
7. Capture screenshots, console logs, DOM assertions, source assertion transcripts, and visual review notes.
8. Update execution report and create SB06 proof artifacts.

## Scope Exceptions

- Non-calendar workbench interactions are owned by SB05.
- Floating-window lifecycle proof is owned by SB07.
- Full route matrix proof is owned by SB08.

## Do Not Do

- Do not redesign the calendar surface or change public callback semantics without failing-first proof.
- Do not treat a route render as sufficient calendar validation.
- Do not edit WebGL files.

## Acceptance Checklist

- Calendar renders meaningful content and expected empty/loading states.
- Selection and date navigation update visible state and callbacks.
- Event editor and CRUD bridge behavior is either browser-proven or source-asserted with tests.
- Playlist/search/export paths are validated where exposed.
- Preview cards do not clip text or overlap controls across required viewports.
- Console errors are zero or explicitly classified with blockers.

## Proof Required

- Playwright/MCP action transcript.
- Screenshots for each required viewport and state.
- DOM assertion JSON or transcript.
- Console log transcript.
- Source assertion transcript.
- `bundle://proof/SB06/manifest.md`
- `bundle://proof/SB06/semantic-invariants.md`
- Anti-stub audit transcript.

## Browser Validation Logging

- Route: `/groups/canvas` with calendar and preview scenarios.
- Viewports: maximized desktop, 1366x900, and 390x844. Add 1024x768 if layout changes.
- Actions: calendar navigation, date/time selection, event editor open/close/save/cancel where available, export menu, playlist/search actions where exposed, preview component state toggles.
- Required evidence paths: `bundle://proof/SB06/screenshots/...`, `bundle://proof/SB06/browser-actions.txt`, `bundle://proof/SB06/console-log.txt`.
- Review questions: Does calendar text fit? Are editor overlays readable? Are preview cards useful on desktop and mobile? Are disabled/loading/empty states visually distinct?

## Progression Gate

- SB08 may proceed only after calendar and preview proof has no unresolved critical defects.
- Reopen SB03 if failures point to ambiguous calendar contracts or state semantics.

## Suggested Agent Prompt

```text
Execute SB06 only. Use real browser actions and source assertions to validate Canvas calendar and preview surfaces, repair only scoped defects, capture screenshots and transcripts, and update proof/report artifacts before closing.
```
