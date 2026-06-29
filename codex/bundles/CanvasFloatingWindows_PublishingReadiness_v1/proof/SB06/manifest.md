# SB06 Proof Manifest

Subbundle: `SB06 Calendar And Preview Surface Validation`

Status: `Completed`

Owned raw notes and requirements:

- RAW03, RAW05, RAW06, and RAW07.
- R04, R09, R11, R12, and R14.

Semantic invariant contract:

- `bundle://proof/SB06/semantic-invariants.md`

Changed-file hashes:

- `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` current SHA-256 `18BBF5C38213C9718EA3FDB3A6F0074CB55FA1C9EFF13E4E785612D78015C45D`
- `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/controller/03-editor-and-toolbar.js` current SHA-256 `86AC0E421E3C809AD24DADEE30C8CD0EBC9B396639EEBB2E36E8B140E1960132`
- `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/canvasCalendarInterop.js` current SHA-256 `FA8E862270BF63891DF587922CE6EFA9FE4E29F03B6C29E75EFF212D03565C88`
- `repo://codex/bundles/CanvasFloatingWindows_PublishingReadiness_v1/proof/SB06/verify-calendar-preview.cjs` current SHA-256 `06D7173DB8C48481071D89821FD36D7A232265EBAB72851B88C297FB8F9F7863`
- Full current changed/proof hash transcript: `bundle://proof/SB06/transcripts/changed-file-hashes.txt`

Command transcripts:

- Sandbox build: `bundle://proof/SB06/transcripts/dotnet-build-sandbox.txt`
- Browser calendar/preview verifier: `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt`
- Node syntax check: `bundle://proof/SB06/transcripts/node-check-calendar-js.txt`
- Source assertions: `bundle://proof/SB06/transcripts/source-assertions-calendar-preview.txt`
- Anti-stub audit: `bundle://proof/SB06/transcripts/anti-stub-audit.txt`
- Semantic adequacy transcript: `bundle://proof/SB06/transcripts/semantic-adequacy.txt`

Failing-first / adversarial negative proof:

- Failing-first: N/A process proof exemption; SB06 relied on browser-verifier assertions and callback state checks rather than preserving a separate failing command transcript.
- The SB06 browser verifier exposed export returning zero events while the rendered happy-path calendar had two visible events. `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/controller/03-editor-and-toolbar.js` now exports from `state.visibleEvents`, and `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/canvasCalendarInterop.js` normalizes visible events before serialization.
- `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` now fails if the export callback does not report a nonzero happy-path event count, if CRUD mutations do not change browser-visible state, if playlist choice/clone does not mutate selection state, if preview cards are missing, if the calendar accessibility mirror is stale, or if console warnings/errors/pageerrors appear.

Passing / semantic positive proof:

- Passing browser transcript: `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt`
- Browser action record: `bundle://proof/SB06/browser-actions.json`
- Semantic positive proof transcript: `bundle://proof/SB06/transcripts/semantic-adequacy.txt`

Browser proof:

- Scenario screenshots: `bundle://proof/SB06/screenshots/scenario-happy-path-calendar-preview.png`, `bundle://proof/SB06/screenshots/scenario-dense-content-calendar-preview.png`, `bundle://proof/SB06/screenshots/scenario-empty-state-calendar-preview.png`, `bundle://proof/SB06/screenshots/scenario-disabled-state-calendar-preview.png`, and `bundle://proof/SB06/screenshots/scenario-long-text-calendar-preview.png`
- Viewport screenshots: `bundle://proof/SB06/screenshots/viewport-max-desktop-calendar-preview.png`, `bundle://proof/SB06/screenshots/viewport-desktop-1366-calendar-preview.png`, and `bundle://proof/SB06/screenshots/viewport-mobile-390-calendar-preview.png`
- Interaction screenshots: `bundle://proof/SB06/screenshots/action-week-view.png`, `bundle://proof/SB06/screenshots/action-list-view.png`, `bundle://proof/SB06/screenshots/action-settings-timezone.png`, `bundle://proof/SB06/screenshots/action-help.png`, `bundle://proof/SB06/screenshots/action-export-menu.png`, `bundle://proof/SB06/screenshots/action-playlist-choice.png`, `bundle://proof/SB06/screenshots/action-playlist-clone.png`, `bundle://proof/SB06/screenshots/action-updated-event.png`, `bundle://proof/SB06/screenshots/action-created-event.png`, and `bundle://proof/SB06/screenshots/action-deleted-event.png`
- Console log: `bundle://proof/SB06/console-log.txt`
- Visual review: `bundle://proof/SB06/visual-review.md`

Source-level assertions:

- `bundle://proof/SB06/transcripts/source-assertions-calendar-preview.txt` verifies calendar callbacks, contract records, pure JS runtime actions, calendar boundary previews, sandbox callback wiring, and SB06 verifier invariants.
- `bundle://proof/SB06/transcripts/node-check-calendar-js.txt` verifies the changed pure JS files and verifier parse successfully.

Anti-stub audit:

- `bundle://proof/SB06/transcripts/anti-stub-audit.txt` states no TODO, FIXME, HACK, NotImplementedException, stubbed, fakeImplementation, or placeholderImplementation matches in SB06 scoped production/proof files.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| Calendar visible event state | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/controller/03-editor-and-toolbar.js` exports from `state.visibleEvents` | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/canvasCalendarInterop.js` normalizes visible events before callback serialization | `bundle://proof/SB06/browser-actions.json` records visible counts in scenarios, viewports, and actions | `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` rejects visible-count drift and zero-event export |
| Calendar CRUD callbacks | `repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar/CanvasCalendar.razor` exposes save/delete callbacks | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` wires update/create/delete handlers | `bundle://proof/SB06/screenshots/action-updated-event.png`, `action-created-event.png`, and `action-deleted-event.png` capture lifecycle states | `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` rejects missing updated/created/deleted state transitions |
| Calendar export callback | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/controller/03-editor-and-toolbar.js` requests export from current visible events | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` consumes `CanvasCalendarExportRequest` and updates `calendar-export-status` | `bundle://proof/SB06/browser-actions.json` records `LAST CALENDAR EXPORT: CSV (2 EVENTS)` | Verifier rejects `CSV (0 EVENTS)` for the happy-path export |
| Playlist mutation callbacks | `repo://src/CanDoItAll.Components.CanvasLib/wwwroot/js/calendar/controller/03-editor-and-toolbar.js` routes playlist choice actions | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Canvas.razor` wires search/link/clone/unlink handlers | `bundle://proof/SB06/screenshots/action-playlist-choice.png` and `action-playlist-clone.png` capture choice and clone states | `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` rejects missing choice popover or unchanged selected state |
| Preview cards and boundary cards | `repo://src/CanDoItAll.Components.CanvasLib/Canvas/Calendar` creates snapshot models with `TestHookId` values | `repo://src/CanDoItAll.Components.CanvasLib/Components/Calendar` renders boundary cards, and sandbox Canvas previews render public surface cards | `bundle://proof/SB06/browser-actions.json` records presence, text length, and measured bounds | `bundle://proof/SB06/transcripts/playwright-calendar-preview.txt` rejects missing cards, too-short content, or lateral overflow |

