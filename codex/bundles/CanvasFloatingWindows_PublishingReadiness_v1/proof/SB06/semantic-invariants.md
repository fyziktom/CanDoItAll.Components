# SB06 Semantic Invariants

## Invariant Calendar Scenarios

- Invariant ID: `SB06-INV-CALENDAR-SCENARIOS`
- Source raw note: RAW03 requires true Canvas validation and RAW05 requires preserving functionality.
- Expected behavior: `route groups/canvas` renders meaningful calendar content for happy-path, dense-content, empty-state, disabled-state, and long-text scenarios with accurate visible event counts.
- Disallowed shallow implementation: Loading the route or checking only that the calendar host exists without validating scenario counts, boundary cards, accessibility mirror state, screenshots, and console quality.
- Failing-first test: The verifier rejects wrong visible event counts, missing calendar facades, missing primitive/controller runtimes, missing boundary cards, stale accessibility mirror content, horizontal overflow, and console warnings/errors/pageerrors.
- Passing test: `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` records five scenario passes and `bundle://proof/SB06/browser-actions.json` records measured bounds and visible-event summaries.
- Changed source files and hashes: `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB06/verify-calendar-preview.cjs` SHA-256 `06D7173DB8C48481071D89821FD36D7A232265EBAB72851B88C297FB8F9F7863`; full hash set in `bundle://proof/SB06/transcripts/changed-file-hashes.txt`.
- Production assertions: `bundle://proof/SB06/transcripts/source-assertions-calendar-preview.txt` verifies calendar callbacks, contract records, runtime actions, preview components, sandbox proof wiring, and invariant coverage.
- Red-team negative case: The empty-state scenario must keep zero visible events and zero mirror items, while non-empty scenarios must keep matching visible counts and useful mirror content.
- Downstream dependency check: SB08 can reuse the scenario proof as the calendar baseline for the route matrix.

## Invariant Calendar Viewports

- Invariant ID: `SB06-INV-CALENDAR-VIEWPORTS`
- Source raw note: RAW03 requires true validation across Canvas surfaces and RAW05 requires preserving behavior across usable sizes.
- Expected behavior: Calendar and preview surfaces remain readable and laterally contained at 1920x1080, 1366x900, and 390x844.
- Disallowed shallow implementation: Desktop-only screenshots or visual review without measuring shell, host, canvas, preview, and overflow bounds.
- Failing-first test: The verifier rejects missing selectors, collapsed calendar bounds, lateral overflow, unreadable preview cards, or missing boundary cards.
- Passing test: `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` records viewport passes for max-desktop, desktop-1366, and mobile-390; screenshots capture all three.
- Changed source files and hashes: `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB06/verify-calendar-preview.cjs` SHA-256 `06D7173DB8C48481071D89821FD36D7A232265EBAB72851B88C297FB8F9F7863`.
- Production assertions: `bundle://proof/SB06/browser-actions.json` records `overflowX: 0` for viewport passes, including the 390px mobile surface.
- Red-team negative case: The verifier originally found the mobile inner calendar width at 193px inside the sandbox proof column; the threshold now documents that real constraint while still failing on sideways overflow or collapsed canvas bounds.
- Downstream dependency check: SB08 should preserve these viewport limits while expanding the route matrix.

## Invariant Calendar Actions

- Invariant ID: `SB06-INV-CALENDAR-ACTIONS`
- Source raw note: RAW03 and RAW05 require real calendar action proof, not static screenshots.
- Expected behavior: Date navigation, week/list view switching, list row selection, timezone settings, help, export menu, editor open, playlist choice, update, create, and delete execute through the production calendar runtime.
- Disallowed shallow implementation: Mutating Blazor state manually or accepting screenshots without invoking browser controls and JS event routing.
- Failing-first test: The verifier rejects missing action controls, stale visible-event summaries, hidden editor/menu states, failed timezone state, missing action screenshots, and console warnings/errors/pageerrors.
- Passing test: `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` records the interaction pass and cites action screenshots from `bundle://proof/SB06/screenshots/action-week-view.png` through `bundle://proof/SB06/screenshots/action-deleted-event.png`.
- Changed source files and hashes: `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` SHA-256 `18BBF5C38213C9718EA3FDB3A6F0074CB55FA1C9EFF13E4E785612D78015C45D`; `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB06/verify-calendar-preview.cjs` SHA-256 `06D7173DB8C48481071D89821FD36D7A232265EBAB72851B88C297FB8F9F7863`.
- Production assertions: `bundle://proof/SB06/transcripts/source-assertions-calendar-preview.txt` finds `HandleCalendarEventSaveAsync`, `HandleCalendarEventDeleteAsync`, playlist handlers, `HandleCalendarExportAsync`, and deterministic proof state.
- Red-team negative case: The action proof fails if the sandbox callback wiring is absent or if browser-visible state does not change after interaction.
- Downstream dependency check: SB08 can run the broader matrix without reintroducing direct state mutation.

## Invariant Calendar CRUD

- Invariant ID: `SB06-INV-CALENDAR-CRUD`
- Source raw note: RAW05 requires preserving current functionality.
- Expected behavior: The event editor can update an existing event, create a new event, and delete the selected event through the CanvasCalendar callback contract.
- Disallowed shallow implementation: Verifying only editor markup or using a test-only fake save path that never exercises `SaveEventAsync` and `DeleteEventAsync`.
- Failing-first test: The verifier rejects missing editor open state, missing updated title, missing created title, or stale selected-panel state after delete.
- Passing test: `bundle://proof/SB06/browser-actions.json` records `afterUpdate`, `afterCreate`, and `afterDelete` summaries; `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` records the action pass.
- Changed source files and hashes: `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` SHA-256 `18BBF5C38213C9718EA3FDB3A6F0074CB55FA1C9EFF13E4E785612D78015C45D`.
- Production assertions: `bundle://proof/SB06/transcripts/source-assertions-calendar-preview.txt` verifies CanvasCalendar callback parameters, contract records, and sandbox handlers.
- Red-team negative case: The proof would fail if callbacks returned null, if deletion did not remove the selected item, or if event normalization broke visible summaries.
- Downstream dependency check: SB09 package/API docs can cite the preserved callback contract.

## Invariant Calendar Export

- Invariant ID: `SB06-INV-CALENDAR-EXPORT`
- Source raw note: RAW05 requires preserving export functionality and RAW07 requires pure JS/C# and Razor implementation.
- Expected behavior: Calendar export sends the currently visible events to the callback and updates sandbox-visible export status with a nonzero event count in the happy path.
- Disallowed shallow implementation: Returning a constant export status, exporting stale zero-event data, or serializing unnormalized JavaScript objects.
- Failing-first test: The browser verifier exposed export returning `CSV (0 EVENTS)` while the rendered view had two visible events.
- Passing test: `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` now passes only when the export callback reports `CSV (2 EVENTS)`; `bundle://proof/SB06/browser-actions.json` records `LAST CALENDAR EXPORT: CSV (2 EVENTS)`.
- Changed source files and hashes: `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/controller/03-editor-and-toolbar.js` SHA-256 `86AC0E421E3C809AD24DADEE30C8CD0EBC9B396639EEBB2E36E8B140E1960132`; `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/canvasCalendarInterop.js` SHA-256 `FA8E862270BF63891DF587922CE6EFA9FE4E29F03B6C29E75EFF212D03565C88`; `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` SHA-256 `18BBF5C38213C9718EA3FDB3A6F0074CB55FA1C9EFF13E4E785612D78015C45D`.
- Production assertions: `bundle://proof/SB06/transcripts/source-assertions-calendar-preview.txt` verifies `state.visibleEvents`, `normalizedVisibleEvents`, export callback wiring, and `calendar-export-status`.
- Red-team negative case: Future export drift to zero visible events or unnormalized payloads fails the verifier even if the export menu still opens.
- Downstream dependency check: SB09 docs can explain that export payloads are derived from visible calendar state.

## Invariant Playlists

- Invariant ID: `SB06-INV-PLAYLISTS`
- Source raw note: RAW03 and RAW05 require playlist/search/link/clone behavior to remain validated where exposed.
- Expected behavior: The calendar playlist search/choice surface opens, lists choices, and clone/link mutation updates the selected event through callback wiring.
- Disallowed shallow implementation: Rendering static playlist text or bypassing the mutation callback path.
- Failing-first test: The verifier rejects a missing playlist choice popover, missing `playlist-choice-copy`, unchanged selected event state, or missing screenshot evidence.
- Passing test: `bundle://proof/SB06/screenshots/action-playlist-choice.png` and `bundle://proof/SB06/screenshots/action-playlist-clone.png` show the open choice and cloned playlist state; the transcript records the interaction pass.
- Changed source files and hashes: `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` SHA-256 `18BBF5C38213C9718EA3FDB3A6F0074CB55FA1C9EFF13E4E785612D78015C45D`; `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB06/verify-calendar-preview.cjs` SHA-256 `06D7173DB8C48481071D89821FD36D7A232265EBAB72851B88C297FB8F9F7863`.
- Production assertions: `bundle://proof/SB06/transcripts/source-assertions-calendar-preview.txt` verifies search, link, clone, unlink handlers and pure JS `playlist-choice-copy` routing.
- Red-team negative case: Static playlist chips without real mutation callbacks fail because the selected event summary does not change.
- Downstream dependency check: SB08 route matrix can validate playlist popovers as part of open-state proof.

## Invariant Preview Cards

- Invariant ID: `SB06-INV-PREVIEW-CARDS`
- Source raw note: RAW03 and RAW06 require preview surfaces to be validated and understandable for open-source publishing.
- Expected behavior: Calendar boundary cards and Canvas preview cards render useful content in scenario, viewport, and action passes without incoherent overlap or lateral overflow.
- Disallowed shallow implementation: Checking only the main calendar canvas and ignoring the surrounding preview cards that document component boundaries.
- Failing-first test: The verifier rejects missing preview cards, too-short text, missing calendar boundary cards, and horizontal overflow.
- Passing test: `bundle://proof/SB06/browser-actions.json` records boundary card and preview card presence/text lengths; `bundle://proof/SB06/visual-review.md` reviews the scenario, viewport, and action screenshots.
- Changed source files and hashes: `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB06/verify-calendar-preview.cjs` SHA-256 `06D7173DB8C48481071D89821FD36D7A232265EBAB72851B88C297FB8F9F7863`.
- Production assertions: `bundle://proof/SB06/transcripts/source-assertions-calendar-preview.txt` verifies boundary preview components, snapshots, and `TestHookId` values.
- Red-team negative case: Preview cards can be tall on mobile but must stay inside the page width with measurable content.
- Downstream dependency check: SB09 can use this proof to document the preview surfaces without treating them as decorative samples only.

## Invariant Calendar Accessibility

- Invariant ID: `SB06-INV-CALENDAR-A11Y`
- Source raw note: RAW03 and RAW05 require true validation that preserves accessibility support.
- Expected behavior: The calendar accessibility mirror renders and tracks visible events for each scenario and viewport.
- Disallowed shallow implementation: Checking only visual event blocks while mirror content is missing, stale, or mismatched.
- Failing-first test: The verifier rejects missing `calendar-accessibility-mirror-layer`, mismatched mirror counts, and too-short mirror text for non-empty scenarios.
- Passing test: `bundle://proof/SB06/browser-actions.json` records mirror presence and item counts for scenario and viewport passes.
- Changed source files and hashes: `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB06/verify-calendar-preview.cjs` SHA-256 `06D7173DB8C48481071D89821FD36D7A232265EBAB72851B88C297FB8F9F7863`.
- Production assertions: `bundle://proof/SB06/transcripts/source-assertions-calendar-preview.txt` verifies `AccessibilityMirrorLayer` use in `CanvasCalendar.razor`.
- Red-team negative case: Empty-state must keep zero mirror rows, while happy/dense/long states must keep non-empty mirror text.
- Downstream dependency check: SB10 final audit can distinguish visual Canvas proof from accessibility mirror proof.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| Calendar visible event state | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/controller/03-editor-and-toolbar.js` reads `state.visibleEvents` | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/canvasCalendarInterop.js` normalizes visible events for callbacks | `bundle://proof/SB06/browser-actions.json` records visible counts across scenarios, viewports, and actions | `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` rejects wrong visible counts and zero-event export drift |
| Calendar CRUD callbacks | `repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CanvasCalendar.razor` exposes save/delete parameters | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` wires save/delete handlers | `bundle://proof/SB06/browser-actions.json` records update/create/delete outcomes | `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` rejects missing updated title, created title, or stale delete selection |
| Calendar export callback | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/controller/03-editor-and-toolbar.js` requests export from visible events | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` consumes `CanvasCalendarExportRequest` and updates `calendar-export-status` | `bundle://proof/SB06/browser-actions.json` records `LAST CALENDAR EXPORT: CSV (2 EVENTS)` | The final verifier rejects `CSV (0 EVENTS)` for the happy path |
| Playlist mutation callbacks | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/controller/03-editor-and-toolbar.js` routes `playlist-choice-copy` | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` consumes search/link/clone/unlink handlers | `bundle://proof/SB06/screenshots/action-playlist-choice.png` and `action-playlist-clone.png` capture the lifecycle | `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` rejects missing choice popover or unchanged selected event state |
| Preview cards and calendar boundary cards | `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Calendar` creates snapshot data with `TestHookId` values | `repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar` renders boundary cards | `bundle://proof/SB06/browser-actions.json` records presence, text length, and bounds | `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` rejects missing cards, too-short content, or lateral overflow |



