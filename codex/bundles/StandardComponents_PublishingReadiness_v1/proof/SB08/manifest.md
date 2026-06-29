# SB08 Proof Manifest - Layout Navigation And Overlay Hardening

Status: `Passed`  
Completed local date: `2026-06-28`

## Owned Requirements

- RAW10: Real Playwright screenshots one by one, including interactive states.
- SB08 acceptance: desktop uses available space intentionally; mobile first viewport remains oriented; overlays are readable, unclipped, layered correctly, and dismissible.

## Semantic Contract

- `bundle://proof/SB08/semantic-invariants.md`

## Production Changes

- Hardened `ContextMenu` to inherit `StyledComponentBase` and forward attributes, enabling standard proof hooks while keeping the hidden shared host contract.
- Hardened `StickyActionFooter` to inherit `StyledComponentBase`, accept attributes/classes/styles, and cap sticky footer width inside page shells.
- Added explicit OverlayLib usage to the sandbox with `DisableTransitiveProjectReferences=true` and a direct OverlayLib project reference to avoid duplicate static web assets in publishing-style project graphs.
- Expanded `/groups/layout` with proof hooks for PageScaffold, primary grid, ListDetailShell, and StickyActionFooter.
- Repaired `/groups/layout/composition` by replacing unsafe `max-content` tracks with bounded `minmax(0, fr)` tracks and removing forced two-row stretches that left mobile dead space.
- Expanded `/groups/navigation` with Toolbar, TreeView, hidden ContextMenu host, a visible TreeView right-click action menu, and proof hooks for tabs/steps/list-detail.
- Expanded `/groups/overlays` with HelpPopover panel test id, OverlayWindow bounded host frame, OverlayWindow normal/minimized/hidden/show lifecycle state, and sticky footer proof hook.
- Added BaseLib regression tests for ContextMenu/StickyActionFooter wrapper contracts and OverlayWindowState normalization/equivalence.
- Added `verify-sb08-layout-navigation-overlays.mjs` for layout overflow, navigation interaction, dialog/backdrop/result, tooltip/toast, and OverlayWindow lifecycle proof.

## Changed-File Hashes

- Hash manifest JSON: `bundle://proof/SB08/data/sb08-file-hashes.json`
- Hash transcript: `bundle://proof/SB08/transcripts/sb08-file-hashes.txt`

## Command Transcripts

- Sandbox build: `bundle://proof/SB08/transcripts/sb08-sandbox-build.txt`
- BaseLib tests: `bundle://proof/SB08/transcripts/sb08-baselib-tests.txt`
- Browser verifier: `bundle://proof/SB08/transcripts/sb08-playwright-verifier.txt`
- Source assertions: `bundle://proof/SB08/transcripts/sb08-source-assertions.txt`
- Anti-stub audit: `bundle://proof/SB08/transcripts/sb08-anti-stub-audit.txt`
- Git whitespace check: `bundle://proof/SB08/transcripts/sb08-git-diff-check.txt`
- Git whitespace stderr capture: `bundle://proof/SB08/transcripts/sb08-git-diff-check-warnings.txt`
- Prepared-stage validator: `bundle://proof/SB08/transcripts/sb08-prepared-validator.txt`

## Browser And Visual Proof

- Layout desktop: `bundle://proof/SB08/screenshots/mcp/sb08-layout-desktop-full.png`
- Layout mobile long text: `bundle://proof/SB08/screenshots/mcp/sb08-layout-mobile-long-full.png`
- Layout composition desktop: `bundle://proof/SB08/screenshots/mcp/sb08-layout-composition-desktop-full.png`
- Layout composition mobile long text: `bundle://proof/SB08/screenshots/mcp/sb08-layout-composition-mobile-long-full.png`
- Navigation desktop context menu: `bundle://proof/SB08/screenshots/mcp/sb08-navigation-desktop-context-menu.png`
- Navigation tabs mobile long text: `bundle://proof/SB08/screenshots/mcp/sb08-navigation-tabs-mobile-long-full.png`
- Overlays desktop backdrop dialog: `bundle://proof/SB08/screenshots/mcp/sb08-overlays-desktop-backdrop-dialog.png`
- Overlays desktop HelpPopover: `bundle://proof/SB08/screenshots/mcp/sb08-overlays-desktop-help-popover.png`
- Overlays desktop toast/tooltip/window: `bundle://proof/SB08/screenshots/mcp/sb08-overlays-desktop-toast-tooltip-window.png`
- Overlays mobile long-text window: `bundle://proof/SB08/screenshots/mcp/sb08-overlays-mobile-long-window-full.png`
- Screenshot contact sheet: `bundle://proof/SB08/screenshots/mcp/sb08-mcp-contact-sheet.png`
- Visual repair observations: `bundle://proof/SB08/data/sb08-visual-repair-observations.json`
- Browser verifier JSON: `bundle://proof/SB08/data/sb08-layout-navigation-overlays-validation.json`
- Verifier script: `bundle://scripts/verify-sb08-layout-navigation-overlays.mjs`

## Validation Summary

- `dotnet build src/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj --no-restore --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test tests/CanDoItAll.Components.BaseLib.Tests/CanDoItAll.Components.BaseLib.Tests.csproj --no-restore --nologo`: passed, 24 tests.
- `node codex/bundles/StandardComponents_PublishingReadiness_v1/scripts/verify-sb08-layout-navigation-overlays.mjs`: passed, 67 checks, 0 failed, 0 console errors.
- `git diff --check`: passed; line-ending warnings were captured separately.
- `python codex/bundles/StandardComponents_PublishingReadiness_v1/scripts/validate_bundle.py codex/bundles/StandardComponents_PublishingReadiness_v1 --profile initiative --stage prepared --repo-root C:\repositories\CanDoItAll.Components`: passed.
- Playwright MCP screenshots were captured and visually reviewed for layout, layout composition, navigation context menu, tabs lab mobile long-text, dialog, HelpPopover, tooltip/toast, and OverlayWindow mobile/desktop states.

## Visual Findings Repaired

- Direct OverlayLib sandbox proof initially produced duplicate static-web-asset keys because CanvasLib already referenced OverlayLib transitively. Repaired by disabling implicit transitive project references in the sandbox and keeping explicit project references.
- Layout composition used `max-content` two-track templates that produced internal horizontal overflow under verifier measurement. Repaired with bounded `minmax(0,1fr) minmax(0,0.85fr)` tracks.
- Layout composition Row/Column mobile screenshots showed large empty stretched blue areas from forced two-row tracks. Repaired by removing forced `Rows="2"` so rows size to content.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `OverlayWindowSandboxState` | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Overlays.razor` creates and normalizes `OverlayWindowState`. | `repo://src/CanDoItAll.Components.OverlayLib/Components/Core/OverlayWindow.razor` consumes the state; screenshots prove desktop/mobile rendering. | `bundle://proof/SB08/data/sb08-layout-navigation-overlays-validation.json` proves normal, minimized, hidden, and show-again lifecycle states stay inside the host frame. | `bundle://proof/SB08/data/sb08-visual-repair-observations.json` records the duplicate asset graph failure rejected by the repaired sandbox setup. |

## Closure Decision

SB08 is closed. Downstream SB09 may rely on hardened layout/navigation/overlay sandbox proof, explicit OverlayLib sandbox dependency handling, reusable overlay lifecycle verifier patterns, and repaired layout-composition responsive examples.
