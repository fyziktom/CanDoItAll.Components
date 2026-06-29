# SB07 Actions Badges And Feedback Visual Hardening

## Status

- Status: `Completed`

## Objective

Harden actions, badges, and feedback surfaces for icon/text rhythm, long labels, loading, empty, status, notification, and tooltip behavior.

## Covered Inputs

- RAW10: Real Playwright screenshots one by one, including interactive states.

## Prerequisites

- Checkpoint B passed.
- SB02 button/feedback styling stable.

## Exact Source References

- repo://src/CanDoItAll.Components.BaseLib/Components/Buttons
- repo://src/CanDoItAll.Components.BaseLib/Components/Badges
- repo://src/CanDoItAll.Components.BaseLib/Components/Feedback
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Actions.razor
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Feedback.razor

## Deliverables

- Button/badge/feedback source fixes.
- Behavior tests for services/callbacks.
- Visual proof for states and open overlays.

## Dependency Impact

- Depends on SB02-SB05.
- Can reopen SB02 if button/alert shared classes fail visual proof.

## Validation Depth

- UI proof with interactive states.
- Service behavior tests for notifications/tooltips when changed.

## Implementation Steps

- Validate Button, CopyButton, Badge, Chip, StatusBadge, Alert, Callout, EmptyState, LoadingState, Notification, Tooltip, HelpPopover, verification/status lists.
- Fix wrapping, icon fallback, disabled/loading states.
- Capture open tooltip/popover/toast states.

## Scope Exceptions

- WebGL and Canvas implementation changes are excluded unless this subbundle explicitly updates sandbox separation metadata without touching their component internals.

## Do Not Do

- Do not refactor unrelated WebGL or Canvas source.
- Do not delete compatibility or AppComponents code without migration proof and consumer checks.
- Do not close UI work from source inspection only.

## Acceptance Checklist

- Icon-only buttons remain accessible.
- Long labels wrap without disrupting layout.
- Feedback states are visually distinct and not clipped.
- HelpPopover click-open, tooltip hover, toast open/clear, copy state, and disabled-state scenarios are covered by browser proof.

## Proof Required

- Build/test transcript: `bundle://proof/SB07/transcripts/sb07-sandbox-build.txt`, `bundle://proof/SB07/transcripts/sb07-baselib-tests.txt`.
- Tailwind rebuild transcript: `bundle://proof/SB07/transcripts/sb07-tailwind-build.txt`.
- Browser verifier: `bundle://proof/SB07/transcripts/sb07-playwright-verifier.txt`, `bundle://proof/SB07/data/sb07-actions-feedback-validation.json`.
- Playwright MCP screenshots for actions and feedback states: `bundle://proof/SB07/screenshots/mcp/sb07-actions-desktop-full.png`, `bundle://proof/SB07/screenshots/mcp/sb07-actions-mobile-long-full.png`, `bundle://proof/SB07/screenshots/mcp/sb07-feedback-desktop-toast-help-v3.png`, `bundle://proof/SB07/screenshots/mcp/sb07-feedback-mobile-long-toast-help-v2.png`.
- Visual repair trail: `bundle://proof/SB07/data/sb07-visual-repair-observations.json`.
- Proof manifest and semantic invariants: `bundle://proof/SB07/manifest.md`, `bundle://proof/SB07/semantic-invariants.md`.

## Browser Validation Logging

- Routes: /groups/actions, /groups/feedback, /groups/overlays where tooltip/help is shared.
- Actions: hover/click tooltip, show toast, copy button state, loading/disabled examples.
- Viewports: desktop and mobile.

## Progression Gate

- The subbundle validator must pass closure review before downstream dependent subbundles start.
- If proof is weak or a screenshot shows wrapping, clipping, layout, available-space, or interaction defects, keep this subbundle `In progress` and reopen prerequisites as needed.

## Suggested Agent Prompt

Execute SB07 with screenshot-first discipline for every stateful action/feedback component.
