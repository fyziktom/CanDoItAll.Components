# SB07 Semantic Invariants

## Invariant SB07-ACTIONS-WRAP-DISABLE-COPY

- Raw note owned: RAW10 requires real Playwright proof one by one, including interactive states.
- Expected behavior: actions, copy buttons, icon buttons, button badges, and chips remain readable and contained on desktop and mobile long-text routes; disabled scenarios disable all interactive action affordances; copy buttons copy the real source value and report copied state.
- Disallowed shallow implementation: add sandbox examples or test ids without making the underlying action/badge/copy controls wrap, disable, and interact correctly.
- Failing-first or adversarial negative proof: the SB07 browser verifier checks long-text mobile overflow, disabled action state, and real clipboard copy; a markup-only or fixture-only implementation fails these interaction checks.
- Passing proof: `bundle://proof/SB07/transcripts/sb07-playwright-verifier.txt` and `bundle://proof/SB07/data/sb07-actions-feedback-validation.json` show 37 passed checks, including copy, disabled, and no-overflow cases.
- Source proof: `repo://src/CanDoItAll.Components.BaseLib/Components/Buttons/Button.razor`, `repo://Tailwind/controls/buttons.css`, `repo://src/CanDoItAll.Components.BaseLib/Components/Badges/Badge.razor`, `repo://Tailwind/controls/badges.css`, `repo://src/CanDoItAll.Components.BaseLib/Components/Badges/Chip.razor`, `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Actions.razor`.
- Test proof: `bundle://proof/SB07/transcripts/sb07-baselib-tests.txt`.
- Browser proof: `bundle://proof/SB07/screenshots/mcp/sb07-actions-desktop-full.png`, `bundle://proof/SB07/screenshots/mcp/sb07-actions-mobile-long-full.png`.
- Anti-stub proof: `bundle://proof/SB07/transcripts/sb07-anti-stub-audit.txt`.
- Downstream dependency check: SB08-SB09 may reuse the Button/Badge/Chip wrapping and disabled-state conventions.

## Invariant SB07-FEEDBACK-HELP-POPOVER-VIEWPORT

- Raw note owned: RAW10 requires open-state screenshot proof for overlays and interactive controls.
- Expected behavior: HelpPopover click opens a styled, readable panel; the panel stays open long enough for a user to read it; Escape/scrim lifecycle closes it; desktop and mobile open states stay inside the viewport without clipping.
- Disallowed shallow implementation: rely on isolated `.razor.css`, prove only the trigger, or only pass a desktop screenshot while mobile panels overflow.
- Failing-first proof: `bundle://proof/SB07/data/sb07-visual-repair-observations.json` records the unstyled desktop panel (`SB07-NEG-HELP-POPOVER-UNSTYLED`) and mobile left-overflow (`SB07-NEG-HELP-POPOVER-MOBILE-OVERFLOW`) found by Playwright MCP.
- Passing proof: `bundle://proof/SB07/screenshots/mcp/sb07-feedback-desktop-toast-help-v3.png`, `bundle://proof/SB07/screenshots/mcp/sb07-feedback-mobile-long-toast-help-v2.png`, `bundle://proof/SB07/data/sb07-actions-feedback-validation.json`.
- Source proof: `repo://src/CanDoItAll.Components.BaseLib/Components/Feedback/HelpPopover.razor`, `repo://Tailwind/feedback/help-popover.css`, `repo://Tailwind/input.css`, `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Feedback.razor`.
- Test proof: `bundle://proof/SB07/transcripts/sb07-playwright-verifier.txt` checks desktop tooltip/popover containment, Escape lifecycle, and mobile fixed-sheet containment.
- Anti-stub proof: `bundle://proof/SB07/transcripts/sb07-anti-stub-audit.txt`.
- Downstream dependency check: SB08 overlay work must preserve the shared HelpPopover open-state behavior and viewport containment.

## Invariant SB07-FEEDBACK-STATUS-NOTIFICATION

- Raw note owned: RAW10 requires visual proof of feedback states, toasts, and open-state overlays.
- Expected behavior: feedback badges, callouts, status lists, verification lists, notifications, and tooltips are visually distinct, wrap long text, and stay contained on desktop/mobile routes.
- Disallowed shallow implementation: leave status-list classes unstyled, prove only static alert markup, or skip notification/tooltip action proof.
- Failing-first or adversarial negative proof: `bundle://proof/SB07/data/sb07-visual-repair-observations.json` records the overlay failures; the SB07 verifier also rejects feedback pages with horizontal overflow, missing tooltip, missing persistent notification, or non-clearing notification state.
- Passing proof: `bundle://proof/SB07/transcripts/sb07-playwright-verifier.txt`, `bundle://proof/SB07/data/sb07-actions-feedback-validation.json`, `bundle://proof/SB07/screenshots/mcp/sb07-feedback-desktop-toast-help-v3.png`, `bundle://proof/SB07/screenshots/mcp/sb07-feedback-mobile-long-toast-help-v2.png`.
- Source proof: `repo://Tailwind/feedback/status-check-list.css`, `repo://src/CanDoItAll.Components.BaseLib/Components/Feedback/Notification.razor`, `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Feedback.razor`.
- Test proof: `bundle://proof/SB07/transcripts/sb07-baselib-tests.txt` proves warning notification copy eligibility/value and informational notification exclusion.
- Anti-stub proof: `bundle://proof/SB07/transcripts/sb07-anti-stub-audit.txt`.
- Downstream dependency check: SB08 overlay and navigation/layout proof can depend on feedback-hosted tooltip/notification surfaces being stable.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `HelpPopoverPinnedOpenState` | `repo://src/CanDoItAll.Components.BaseLib/Components/Feedback/HelpPopover.razor` and `bundle://proof/SB07/transcripts/sb07-source-assertions.txt` prove `ToggleAsync` produces the pinned open state. | `repo://src/CanDoItAll.Components.BaseLib/Components/Feedback/HelpPopover.razor`, `repo://Tailwind/feedback/help-popover.css`, and screenshots `bundle://proof/SB07/screenshots/mcp/sb07-feedback-desktop-toast-help-v3.png`, `bundle://proof/SB07/screenshots/mcp/sb07-feedback-mobile-long-toast-help-v2.png` prove the state is consumed by rendered panel markup and shared Tailwind styling. | `repo://src/CanDoItAll.Components.BaseLib/Components/Feedback/HelpPopover.razor` and `bundle://proof/SB07/data/sb07-actions-feedback-validation.json` prove Escape closes the panel and mobile open state remains viewport-bound. | `bundle://proof/SB07/data/sb07-visual-repair-observations.json` records the desktop unstyled-panel and mobile overflow failures that the repaired implementation rejects. |

## Semantic Gate Decision

Pass. SB07 includes source proof, tests, Playwright MCP screenshots, a 37-check browser verifier, negative visual repair observations, anti-stub audit output, and portable hash proof.
