# SB09 Proof Manifest - Data Display Charts And Diagram Hardening

Status: `Passed`  
Completed local date: `2026-06-29`

## Owned Requirements

- RAW10: Real Playwright screenshots one by one, including interactive states.
- SB09 acceptance: dense data remains scannable, charts and diagrams render nonblank, and errors/empty states are informative.

## Semantic Contract

- `bundle://proof/SB09/semantic-invariants.md`

## Production Changes

- Hardened `CdaChart` so ready and empty states preserve unmatched attributes, emit explicit wrapper state markers, keep chart shells bounded, render heading copy once, and avoid duplicated empty-state text.
- Hardened `SelectionListItem` and `SummaryTile` dense-label behavior so long labels wrap instead of truncating or forcing horizontal overflow.
- Hardened sandbox key/value summary CSS with wrapping and mobile full-width value behavior.
- Expanded `/groups/data-display` with dense, empty, summary strip, card grid, and selection row proof hooks.
- Expanded `/groups/charts` with an empty chart proof hook and verifier coverage for dense/nonblank and empty/no-SVG states.
- Hardened `MermaidDiagram` lifecycle so Blazor rerenders trigger a missing-SVG check instead of silently clearing JS-owned diagrams.
- Hardened `mermaidDiagram.js` with SVG restoration guards, parser-message cleanup, source-aware column clamping, and removal of Mermaid fallback error artifacts.
- Dedented Mermaid raw-string sources through `MermaidSourceNormalizer` and added regression tests for relative indentation preservation.
- Repaired Mermaid gallery samples, including the Sankey sample that produced NaN paths when a CSV header row was parsed as data.
- Added `verify-sb09-data-display-charts-mermaid.mjs` for data-display overflow, chart nonblank/empty states, Mermaid gallery SVGs, click/zoom/pan interaction, empty/error states, and console-error proof.

## Changed-File Hashes

- Hash manifest JSON: `bundle://proof/SB09/data/sb09-file-hashes.json`
- Hash transcript: `bundle://proof/SB09/transcripts/sb09-file-hashes.txt`

## Command Transcripts

- Sandbox build: `bundle://proof/SB09/transcripts/sb09-sandbox-build.txt`
- BaseLib tests: `bundle://proof/SB09/transcripts/sb09-baselib-tests.txt`
- Browser verifier: `bundle://proof/SB09/transcripts/sb09-playwright-verifier.txt`
- Source assertions: `bundle://proof/SB09/transcripts/sb09-source-assertions.txt`
- Anti-stub audit: `bundle://proof/SB09/transcripts/sb09-anti-stub-audit.txt`
- Git whitespace check: `bundle://proof/SB09/transcripts/sb09-git-diff-check.txt`
- Prepared-stage validator: `bundle://proof/SB09/transcripts/sb09-prepared-validator.txt`

## Browser And Visual Proof

- Data display dense desktop: `bundle://proof/SB09/screenshots/mcp/sb09-data-display-dense-desktop-full.png`
- Data display long-text mobile: `bundle://proof/SB09/screenshots/mcp/sb09-data-display-long-mobile-full.png`
- Data display empty mobile: `bundle://proof/SB09/screenshots/mcp/sb09-data-display-empty-mobile-full.png`
- Charts dense desktop: `bundle://proof/SB09/screenshots/mcp/sb09-charts-dense-desktop-full.png`
- Charts dense mobile: `bundle://proof/SB09/screenshots/mcp/sb09-charts-dense-mobile-full.png`
- Charts empty desktop: `bundle://proof/SB09/screenshots/mcp/sb09-charts-empty-desktop-full.png`
- Mermaid desktop click/zoom: `bundle://proof/SB09/screenshots/mcp/sb09-mermaid-desktop-click-zoom-full.png`
- Mermaid gallery desktop: `bundle://proof/SB09/screenshots/mcp/sb09-mermaid-gallery-desktop.png`
- Mermaid mobile: `bundle://proof/SB09/screenshots/mcp/sb09-mermaid-mobile-full.png`
- Mermaid empty/error: `bundle://proof/SB09/screenshots/mcp/sb09-mermaid-empty-error-full.png`
- Visual repair observations: `bundle://proof/SB09/data/sb09-visual-repair-observations.json`
- Browser verifier JSON: `bundle://proof/SB09/data/sb09-data-display-charts-mermaid-validation.json`
- Verifier script: `bundle://scripts/verify-sb09-data-display-charts-mermaid.mjs`

## Validation Summary

- `dotnet build src/CanDoItAll.Components.Sandbox/CanDoItAll.Components.Sandbox.csproj --no-restore --nologo`: passed, 0 warnings, 0 errors.
- `dotnet test tests/CanDoItAll.Components.BaseLib.Tests/CanDoItAll.Components.BaseLib.Tests.csproj --no-restore --nologo`: passed, 26 tests.
- `node codex/bundles/StandardComponents_PublishingReadiness_v1/scripts/verify-sb09-data-display-charts-mermaid.mjs`: passed, 57 checks, 0 failed, 0 console errors.
- `git diff --check`: passed; LF-to-CRLF warnings were captured in the transcript.
- `python codex/bundles/StandardComponents_PublishingReadiness_v1/scripts/validate_bundle.py codex/bundles/StandardComponents_PublishingReadiness_v1 --profile initiative --stage prepared --repo-root C:\repositories\CanDoItAll.Components`: passed.
- Playwright MCP screenshots were captured and visually reviewed for data-display dense/mobile/empty states, chart dense/mobile/empty states, Mermaid click/zoom/gallery/mobile, and Mermaid empty/error diagnostics.

## Visual Findings Repaired

- Chart empty-state copy repeated the same no-data explanation. Repaired by using the shared heading once and keeping only a concise no-data body.
- Selection list titles and summary/meta helper labels needed wrapping hardening for dense generated labels. Repaired with title/body break-word rules and key/value row wrapping.
- Mermaid diagrams could disappear after Blazor parent rerenders because the SVG is JS-owned. Repaired by checking for missing SVGs and restoring cached markup when necessary.
- Mermaid Sankey gallery source included a CSV header row that produced invalid NaN SVG paths. Repaired by removing the header and proving every gallery sample has a nonblank SVG.
- Mermaid parser failures leaked Mermaid's built-in fallback error SVG at the bottom of the page. Repaired by removing render artifacts on success/failure and proving no leaked fallback SVG remains.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `MermaidDiagram` JS-owned SVG lifecycle | `repo://src/CanDoItAll.Components.Mermaid/wwwroot/js/mermaidDiagram.js` installs, guards, and restores SVG markup. | `repo://src/CanDoItAll.Components.Mermaid/Components/MermaidDiagram.razor` calls `hasRenderedSvg` and rerenders only when the wrapper really needs it. | `bundle://proof/SB09/data/sb09-data-display-charts-mermaid-validation.json` proves all Mermaid gallery diagrams are nonblank and click/zoom/pan still work. | `bundle://proof/SB09/data/sb09-visual-repair-observations.json` records the JS-owned SVG clearing and fallback error-SVG leak found during validation. |
| `CdaChart` empty/ready state contract | `repo://src/CanDoItAll.Components.Charts/Components/CdaChart.razor` emits state markers and forwards unmatched attributes in both branches. | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Charts.razor` consumes the wrapper through dense and empty chart examples. | `bundle://proof/SB09/data/sb09-data-display-charts-mermaid-validation.json` proves ready charts have nonblank Apex SVGs and empty charts preserve empty-state semantics. | `bundle://proof/SB09/data/sb09-visual-repair-observations.json` records the duplicate empty copy rejected during visual review. |

## Closure Decision

SB09 is closed. Downstream SB10 may rely on hardened dense data-display wrapping, `CdaChart` ready/empty wrapper semantics, Mermaid source normalization, SVG lifecycle restoration, parser diagnostics, and the SB09 verifier pattern for nonblank visual assertions.
