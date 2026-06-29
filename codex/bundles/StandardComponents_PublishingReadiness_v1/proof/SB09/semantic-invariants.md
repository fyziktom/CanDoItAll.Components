# SB09 Semantic Invariants

## Invariant SB09-DATA-DENSE-WRAP

- Raw note owned: RAW10 requires visual proof that dense component content remains readable, and the architect notes call out wrapping and available-space defects.
- Expected behavior: selection rows, summary tiles, key/value summaries, and data-display card grids must wrap long labels without truncating essential text or creating horizontal overflow.
- Disallowed shallow implementation: add proof hooks while leaving `SelectionListItem` titles truncated, summary helper labels fixed to one line, or sandbox key/value rows unable to wrap.
- Passing proof: `bundle://proof/SB09/transcripts/sb09-playwright-verifier.txt` and `bundle://proof/SB09/data/sb09-data-display-charts-mermaid-validation.json` prove desktop dense, mobile long-text, and mobile empty-state data-display routes without overflow.
- Browser proof: `bundle://proof/SB09/screenshots/mcp/sb09-data-display-dense-desktop-full.png`, `bundle://proof/SB09/screenshots/mcp/sb09-data-display-long-mobile-full.png`, `bundle://proof/SB09/screenshots/mcp/sb09-data-display-empty-mobile-full.png`.
- Source proof: `repo://src/CanDoItAll.Components.BaseLib/Components/Lists/SelectionListItem.razor`, `repo://src/CanDoItAll.Components.BaseLib/Components/Lists/SelectionListItem.razor.css`, `repo://src/CanDoItAll.Components.BaseLib/Components/Cards/SummaryTile.razor`, `repo://src/CanDoItAll.Components.Sandbox/wwwroot/sandbox.css`.
- Anti-stub proof: `bundle://proof/SB09/transcripts/sb09-anti-stub-audit.txt`.

## Invariant SB09-CHART-WRAPPER-BOUNDARY

- Raw note owned: RAW10 requires real chart screenshots and nonblank rendering proof, and the publishing notes require generic basic components to be reusable outside the app sandbox.
- Expected behavior: `CdaChart` forwards unmatched attributes in both ready and empty states, preserves wrapper state markers, keeps headings readable, renders nonblank Apex SVGs with data, and shows an informative no-data state without duplicated copy.
- Disallowed shallow implementation: prove only the sandbox markup, omit `AdditionalAttributes` from empty charts, hide a blank chart as success, or leave repeated empty-state descriptions.
- Passing proof: `bundle://proof/SB09/data/sb09-data-display-charts-mermaid-validation.json` proves dense desktop/mobile Apex SVG rendering and empty chart state without an Apex SVG.
- Browser proof: `bundle://proof/SB09/screenshots/mcp/sb09-charts-dense-desktop-full.png`, `bundle://proof/SB09/screenshots/mcp/sb09-charts-dense-mobile-full.png`, `bundle://proof/SB09/screenshots/mcp/sb09-charts-empty-desktop-full.png`.
- Source proof: `repo://src/CanDoItAll.Components.Charts/Components/CdaChart.razor`, `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Charts.razor`.
- Anti-stub proof: `bundle://proof/SB09/transcripts/sb09-anti-stub-audit.txt`.

## Invariant SB09-MERMAID-LIFECYCLE

- Raw note owned: RAW10 requires action screenshots for dropdown-like/open/interactive states, and diagram wrappers must be visually and functionally correct, not merely source-plausible.
- Expected behavior: `MermaidDiagram` renders nonblank SVGs for every bundled graph sample, preserves JS-owned SVGs across Blazor rerenders, supports click, zoom, and pan interactions, handles empty source calmly, and reports parse errors with wrapper-owned structured diagnostics without leaking Mermaid fallback error SVGs.
- Disallowed shallow implementation: render only a single flowchart, skip the gallery, skip click/zoom/pan proof, accept NaN SVG paths, rely on raw Mermaid parser output, or allow Mermaid's fallback error SVG to escape the component boundary.
- Failing-first proof: `bundle://proof/SB09/data/sb09-visual-repair-observations.json` records the SVG clearing, Sankey header, raw diagnostic, and fallback error-SVG issues found during browser/screenshot validation.
- Passing proof: `bundle://proof/SB09/transcripts/sb09-playwright-verifier.txt` and `bundle://proof/SB09/data/sb09-data-display-charts-mermaid-validation.json` prove click events, zoom/pan viewBox changes, gallery nonblank SVGs, empty state, structured errors, and zero leaked fallback SVGs.
- Browser proof: `bundle://proof/SB09/screenshots/mcp/sb09-mermaid-desktop-click-zoom-full.png`, `bundle://proof/SB09/screenshots/mcp/sb09-mermaid-gallery-desktop.png`, `bundle://proof/SB09/screenshots/mcp/sb09-mermaid-mobile-full.png`, `bundle://proof/SB09/screenshots/mcp/sb09-mermaid-empty-error-full.png`.
- Source proof: `repo://src/CanDoItAll.Components.Mermaid/Components/MermaidDiagram.razor`, `repo://src/CanDoItAll.Components.Mermaid/MermaidSourceNormalizer.cs`, `repo://src/CanDoItAll.Components.Mermaid/wwwroot/js/mermaidDiagram.js`, `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Mermaid.razor`.
- Test proof: `bundle://proof/SB09/transcripts/sb09-baselib-tests.txt` includes the Mermaid source normalizer dedent tests.
- Anti-stub proof: `bundle://proof/SB09/transcripts/sb09-anti-stub-audit.txt`.

## Production Behavior Artifact Matrix

| Artifact | Producer proof | Consumer proof | Lifecycle proof | Negative proof |
|---|---|---|---|---|
| `MermaidRenderError` structured diagnostics | `repo://src/CanDoItAll.Components.Mermaid/wwwroot/js/mermaidDiagram.js` normalizes raw Mermaid errors into friendly message, line, column, excerpt, token, and expected tokens. | `repo://src/CanDoItAll.Components.Mermaid/Components/MermaidDiagram.razor` consumes the normalized error and renders the wrapper-owned alert. | `bundle://proof/SB09/data/sb09-data-display-charts-mermaid-validation.json` proves `Line 3, column 14`, excerpt, token, expected tokens, and no leaked Mermaid fallback SVG. | `bundle://proof/SB09/data/sb09-visual-repair-observations.json` records the raw parser block and fallback SVG leak rejected by the repaired wrapper. |
| `CdaChart` state attributes | `repo://src/CanDoItAll.Components.Charts/Components/CdaChart.razor` emits ready/empty state markers and forwards unmatched attributes. | `repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Charts.razor` consumes `CdaChart` through dense and empty examples with proof ids. | `bundle://proof/SB09/data/sb09-data-display-charts-mermaid-validation.json` proves nonblank Apex SVGs for ready charts and no Apex SVG for empty charts. | `bundle://proof/SB09/data/sb09-visual-repair-observations.json` records duplicated empty-state copy rejected during visual review. |

## Semantic Gate Decision

Pass. SB09 includes source proof, tests, 57 browser checks, Playwright MCP screenshots for dense/empty/interactive states, visual repair observations, anti-stub audit output, and portable hash proof.
