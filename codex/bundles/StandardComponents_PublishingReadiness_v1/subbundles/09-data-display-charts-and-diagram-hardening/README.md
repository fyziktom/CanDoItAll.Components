# SB09 Data Display Charts And Diagram Hardening

## Status

- Status: `Completed`

## Objective

Harden standard display, visualization, chart, and diagram wrappers for dense labels, empty states, nonblank rendering, and wrapper boundaries.

## Covered Inputs

- RAW10: Real Playwright screenshots one by one, including interactive states.

## Prerequisites

- Checkpoint B passed.
- SB05 focused routes available.

## Exact Source References

- repo://src/CanDoItAll.Components.BaseLib/Components/DataDisplay
- repo://src/CanDoItAll.Components.BaseLib/Components/DataVisualization
- repo://src/CanDoItAll.Components.BaseLib/Components/Lists
- repo://src/CanDoItAll.Components.BaseLib/Components/Cards
- repo://src/CanDoItAll.Components.Charts
- repo://src/CanDoItAll.Components.Mermaid
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/DataDisplay.razor
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Charts.razor
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Mermaid.razor

## Deliverables

- Data-display/chart/diagram fixes.
- Nonblank browser proof.
- Dense and empty-state screenshots.

## Dependency Impact

- Depends on sandbox and styling foundations.
- Chart/Mermaid failures can block final visual matrix.

## Validation Depth

- UI proof plus nonblank DOM/canvas/svg checks for chart and diagram wrappers.
- Behavior tests for source normalizers/options factories if changed.

## Implementation Steps

- Validate cards, lists, summary tiles, timeline, progress/data grid primitives, CdaChart, MermaidDiagram.
- Check long labels and dense metadata.
- Assert rendered charts/diagrams are nonblank and readable.

## Scope Exceptions

- WebGL and Canvas implementation changes are excluded unless this subbundle explicitly updates sandbox separation metadata without touching their component internals.

## Do Not Do

- Do not refactor unrelated WebGL or Canvas source.
- Do not delete compatibility or AppComponents code without migration proof and consumer checks.
- Do not close UI work from source inspection only.

## Acceptance Checklist

- Dense data remains scannable.
- Charts and diagrams render nonblank.
- Errors/empty states are informative.

## Proof Required

- Build/test transcript.
- Playwright screenshots.
- Nonblank chart/diagram assertions.

## Completion Evidence

- Proof manifest: `bundle://proof/SB09/manifest.md`
- Semantic invariants: `bundle://proof/SB09/semantic-invariants.md`
- Browser verifier JSON: `bundle://proof/SB09/data/sb09-data-display-charts-mermaid-validation.json`
- Visual repair observations: `bundle://proof/SB09/data/sb09-visual-repair-observations.json`
- Build transcript: `bundle://proof/SB09/transcripts/sb09-sandbox-build.txt`
- Test transcript: `bundle://proof/SB09/transcripts/sb09-baselib-tests.txt`
- Browser verifier transcript: `bundle://proof/SB09/transcripts/sb09-playwright-verifier.txt`

## Browser Validation Logging

- Routes: /groups/data-display, /groups/charts, /groups/mermaid.
- Actions: Mermaid click/pan/zoom/error where supported, chart dense/empty states.
- Viewports: desktop and mobile.

## Progression Gate

- The subbundle validator must pass closure review before downstream dependent subbundles start.
- If proof is weak or a screenshot shows wrapping, clipping, layout, available-space, or interaction defects, keep this subbundle `In progress` and reopen prerequisites as needed.

## Suggested Agent Prompt

Execute SB09 by proving visualizations actually render and dense display surfaces do not collapse under realistic labels.
