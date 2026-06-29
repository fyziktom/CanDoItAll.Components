# SB07 Proof Manifest - Actions Badges And Feedback Visual Hardening

Status: `Passed`  
Completed local date: `2026-06-28`

## Owned Requirements

- RAW10: Real Playwright screenshots one by one, including interactive states.
- SB07 acceptance: icon-only buttons remain accessible; long labels wrap without disrupting layout; feedback states are visually distinct and not clipped.

## Semantic Contract

- `bundle://proof/SB07/semantic-invariants.md`

## Production Changes

- Hardened `Button` anchor accessibility so disabled links emit literal `aria-disabled="true"` / `"false"` instead of framework-dependent boolean attributes.
- Hardened action controls with Tailwind wrapping constraints for long labels, icon-button tokens, badges, chips, and copy affordances.
- Added `Badge.Disabled` support so button badges can participate in disabled scenarios like other action controls.
- Added Tailwind-owned `StatusCheckList` styling so status and verification lists are not unstyled markup in feedback proof.
- Converted `HelpPopover` styling from the old isolated `.razor.css` file to the shared Tailwind feedback layer, added deterministic click-pinned open behavior, added a testable panel id, and repaired mobile overflow with a viewport-bound fixed sheet.
- Expanded `/groups/actions` and `/groups/feedback` sandbox coverage for icon-only actions, long labels, badges, chips, status lists, tooltip/help, notifications, copy states, and disabled states.
- Added BaseLib regression tests for literal button ARIA formatting, button-badge disabled API, and notification copy eligibility/value.
- Added `verify-sb07-actions-feedback.mjs` for desktop/mobile overflow, copy, disabled, tooltip, popover, and notification interaction proof.

## Changed-File Hashes

- Hash manifest JSON: `bundle://proof/SB07/data/sb07-file-hashes.json`
- Hash transcript: `bundle://proof/SB07/transcripts/sb07-file-hashes.txt`

## Command Transcripts

- Tailwind rebuild: `bundle://proof/SB07/transcripts/sb07-tailwind-build.txt`
- Sandbox build: `bundle://proof/SB07/transcripts/sb07-sandbox-build.txt`
- BaseLib tests: `bundle://proof/SB07/transcripts/sb07-baselib-tests.txt`
- Browser verifier: `bundle://proof/SB07/transcripts/sb07-playwright-verifier.txt`
- Source assertions: `bundle://proof/SB07/transcripts/sb07-source-assertions.txt`
- Anti-stub audit: `bundle://proof/SB07/transcripts/sb07-anti-stub-audit.txt`
- Git whitespace check: `bundle://proof/SB07/transcripts/sb07-git-diff-check.txt`
- Git whitespace stderr capture: `bundle://proof/SB07/transcripts/sb07-git-diff-check-warnings.txt`
- Prepared-stage validator: `bundle://proof/SB07/transcripts/sb07-prepared-validator.txt`
- Closure gate path/status check: `bundle://proof/SB07/transcripts/sb07-closure-gate.txt`

## Browser And Visual Proof

- Actions desktop: `bundle://proof/SB07/screenshots/mcp/sb07-actions-desktop-full.png`
- Actions mobile long text: `bundle://proof/SB07/screenshots/mcp/sb07-actions-mobile-long-full.png`
- Feedback desktop initial toast/help capture: `bundle://proof/SB07/screenshots/mcp/sb07-feedback-desktop-toast-help.png`
- Feedback desktop unstyled-popover failure trail: `bundle://proof/SB07/screenshots/mcp/sb07-feedback-desktop-toast-help-v2.png`
- Feedback desktop repaired toast/help capture: `bundle://proof/SB07/screenshots/mcp/sb07-feedback-desktop-toast-help-v3.png`
- Feedback mobile initial long-text toast capture: `bundle://proof/SB07/screenshots/mcp/sb07-feedback-mobile-long-toast.png`
- Feedback mobile repaired long-text help capture: `bundle://proof/SB07/screenshots/mcp/sb07-feedback-mobile-long-toast-help-v2.png`
- Visual repair observations: `bundle://proof/SB07/data/sb07-visual-repair-observations.json`
- Browser verifier JSON: `bundle://proof/SB07/data/sb07-actions-feedback-validation.json`
- Verifier script: `bundle://scripts/verify-sb07-actions-feedback.mjs`

## Validation Summary

- `npm --prefix Tailwind run build`: passed.
- `dotnet build src/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj --no-restore`: passed, 0 warnings, 0 errors.
- `dotnet test tests/CanDoItAll.Components.BaseLib.Tests/CanDoItAll.Components.BaseLib.Tests.csproj --no-restore`: passed, 20 tests.
- `node codex/bundles/StandardComponents_PublishingReadiness_v1/scripts/verify-sb07-actions-feedback.mjs`: passed, 37 checks, 0 failed, 0 console errors.
- `git diff --check`: passed; line-ending warnings were captured separately.
- `python codex/bundles/StandardComponents_PublishingReadiness_v1/scripts/validate_bundle.py codex/bundles/StandardComponents_PublishingReadiness_v1 --profile initiative --stage prepared --repo-root C:\repositories\CanDoItAll.Components`: passed.
- Playwright MCP screenshots were captured and visually reviewed for actions desktop, actions mobile long text, feedback desktop toast/help open state, and feedback mobile long-text help open state.

## Visual Findings Repaired

- HelpPopover opened but rendered as static transparent text because isolated component CSS was not applied in the sandbox. Repaired by moving its visual styling into `Tailwind/feedback/help-popover.css` and rebuilding `output.css`.
- HelpPopover mobile long-text proof overflowed 42px off the left side of a 390px viewport. Repaired with a mobile viewport-bound fixed sheet and verified by MCP geometry and the SB07 browser verifier.
- StatusCheckList markup had component classes but no Tailwind-owned CSS. Repaired with `Tailwind/feedback/status-check-list.css` and sandbox proof.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `HelpPopoverPinnedOpenState` | `repo://src/CanDoItAll.Components.BaseLib/Components/Feedback/HelpPopover.razor` and `bundle://proof/SB07/transcripts/sb07-source-assertions.txt` prove `ToggleAsync` sets `isPinnedOpen` and opens the panel. | `repo://src/CanDoItAll.Components.BaseLib/Components/Feedback/HelpPopover.razor`, `repo://Tailwind/feedback/help-popover.css`, and screenshots `bundle://proof/SB07/screenshots/mcp/sb07-feedback-desktop-toast-help-v3.png`, `bundle://proof/SB07/screenshots/mcp/sb07-feedback-mobile-long-toast-help-v2.png` prove the state renders a styled panel. | `repo://src/CanDoItAll.Components.BaseLib/Components/Feedback/HelpPopover.razor` and `bundle://proof/SB07/data/sb07-actions-feedback-validation.json` prove Escape closes the panel and mobile keeps the open state viewport-bound. | `bundle://proof/SB07/data/sb07-visual-repair-observations.json` records the unstyled desktop panel and mobile left-overflow failures that a shallow desktop-only implementation would miss. |

## Closure Decision

SB07 is closed. Downstream SB08-SB09 may rely on hardened actions/feedback styling, deterministic HelpPopover behavior, the actions/feedback sandbox proof surfaces, and the reusable SB07 verifier pattern.
