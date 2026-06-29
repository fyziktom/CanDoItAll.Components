# SB06 Forms And Inputs Behavior Visual Hardening

## Status

- Status: `Completed`

## Objective

Harden every standard form/input component for behavior, accessibility, wrapping, disabled/loading/long-text states, and available width.

## Covered Inputs

- RAW10: Real Playwright screenshots one by one, including interactive states.

## Prerequisites

- Checkpoint B passed.
- SB05 routes available.

## Exact Source References

- repo://src/CanDoItAll.Components.BaseLib/Components/Forms
- repo://Tailwind/forms
- repo://src/CanDoItAll.Components.Sandbox/Components/Pages/Inputs.razor

## Deliverables

- Source fixes for forms/inputs.
- Focused tests where behavior changes.
- Playwright screenshots for each input component.

## Dependency Impact

- Depends on SB02-SB05.
- Findings can reopen Tailwind or sandbox foundations.

## Validation Depth

- Group-level UI proof plus behavior tests.
- Open-state proof for dropdown/select/file upload interactions where possible.

## Implementation Steps

- Validate TextBox, TextArea, Numeric, DropDown, CheckBox, Switch, Slider, Password, SecretField, Editable, FormField, FormSection, FileUpload, TagEditor.
- Fix width/stretch/wrapping/accessibility defects.
- Record each component result in browser analytics.

## Scope Exceptions

- WebGL and Canvas implementation changes are excluded unless this subbundle explicitly updates sandbox separation metadata without touching their component internals.

## Do Not Do

- Do not refactor unrelated WebGL or Canvas source.
- Do not delete compatibility or AppComponents code without migration proof and consumer checks.
- Do not close UI work from source inspection only.

## Acceptance Checklist

- No text overflow or clipped controls in standard scenarios.
- Labels and accessible names are coherent.
- Interactive changes raise expected callbacks.

## Proof Required

- dotnet build/test transcript.
- Playwright screenshots per component and state.
- Source assertions for accessibility/parameter behavior.

## Browser Validation Logging

- Route: /groups/inputs and focused input routes.
- Actions: open dropdowns, type long text, toggle switches/checkboxes, drag/drop or picker where feasible.
- Viewports: maximized desktop, 1366x900, 390x844.

## Progression Gate

- Closure review passed with `bundle://proof/SB06/manifest.md`.
- Downstream dependent subbundles may start from the hardened inputs foundation.

## Closure Evidence

- Build/test: `bundle://proof/SB06/transcripts/sb06-sandbox-build.txt`, `bundle://proof/SB06/transcripts/sb06-baselib-tests.txt`
- Browser proof: `bundle://proof/SB06/transcripts/sb06-playwright-verifier.txt`, `bundle://proof/SB06/data/sb06-input-validation.json`
- MCP screenshots: `bundle://proof/SB06/screenshots/mcp/sb06-inputs-desktop-full-v3.png`, `bundle://proof/SB06/screenshots/mcp/sb06-inputs-mobile-long-text-full-v3.png`, `bundle://proof/SB06/screenshots/mcp/sb06-inputs-tag-suggestions-v3.png`, `bundle://proof/SB06/screenshots/mcp/sb06-inputs-mobile-disabled-full.png`

## Suggested Agent Prompt

Execute SB06 one component at a time and do not infer styling success without screenshots of each relevant state.
