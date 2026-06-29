# SB11 Full Playwright Visual Validation Matrix

## Status

- Status: `Completed`

## Objective

Run the complete browser proof program across every standard component group, route, state, and interactive open state required for publishing readiness.

## Covered Inputs

- RAW10: Real Playwright screenshots one by one, including interactive states.

## Prerequisites

- SB10 passed.
- All standard component groups closed or explicitly blocked.

## Exact Source References

- repo://src/CanDoItAll.Components.Sandbox
- repo://src/CanDoItAll.Components.BaseLib
- repo://src/CanDoItAll.Components.Charts
- repo://src/CanDoItAll.Components.OverlayLib
- repo://src/CanDoItAll.Components.Mermaid

## Deliverables

- Browser analytics fully populated.
- Screenshots under proof/SB11.
- Visual review answers and defect reopen decisions.

## Dependency Impact

- Blocks final closure.
- Any defect reopens the owning SB06-SB10 phase.

## Validation Depth

- Critical Semantic Adequacy Gate.
- Playwright MCP proof with screenshots and assertions.
- Final visual matrix review.
- Critical foundation: before closure, create `proof/SB11/manifest.md` and `proof/SB11/semantic-invariants.md` with Semantic Adequacy Gate evidence, changed-file hashes, transcripts, source assertions, anti-stub audit, and raw-note literal closure.


## Implementation Steps

- Start dev server.
- Use maximized desktop first, then fixed desktop/tablet/mobile widths.
- Navigate every standard route/scenario.
- Open every dropdown/menu/dialog/popover/tooltip/toast/contextual layer.
- Record pass/fail and reopen defects.

## Scope Exceptions

- WebGL and Canvas implementation changes are excluded unless this subbundle explicitly updates sandbox separation metadata without touching their component internals.

## Do Not Do

- Do not refactor unrelated WebGL or Canvas source.
- Do not delete compatibility or AppComponents code without migration proof and consumer checks.
- Do not close UI work from source inspection only.

## Acceptance Checklist

- Every planned standard component has visual evidence or explicit exception.
- No text overflow/clipping/layering defect remains unresolved.
- Execution report browser analytics rows are populated.

## Proof Required

- Playwright transcripts.
- Screenshot files.
- Screenshot review notes.
- Anti-stub visual proof audit.

## Browser Validation Logging

- Routes: all standard sandbox routes and focused routes from SB05.
- Viewports: maximized headed browser, 1366x900, 1024x768, 390x844.
- Actions: all open interactive states listed by component inventory.

## Progression Gate

- The subbundle validator must pass closure review before downstream dependent subbundles start.
- If proof is weak or a screenshot shows wrapping, clipping, layout, available-space, or interaction defects, keep this subbundle `In progress` and reopen prerequisites as needed.

## Suggested Agent Prompt

Execute SB11 as a real visual audit. Screenshots must be reviewed against questions, and any real defect reopens the owning implementation subbundle.
