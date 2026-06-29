# SB02 Tailwind Component Styling Foundation Hardening

## Status

- Status: `Completed`

## Objective

Refactor the shared Tailwind input layer before component-by-component fixes so styling rules are consistent and maintainable.

## Covered Inputs

- RAW05: Use Tailwind for component styling and inspect custom CSS/hacks.
- RAW08: Identify phases with general foundations first.

## Prerequisites

- SB01 inventory accepted.
- Tailwind/CSS severity rows reviewed.

## Exact Source References

- repo://Tailwind/input.css
- repo://Tailwind/forms/fields.css
- repo://Tailwind/controls/buttons.css
- repo://Tailwind/navigation/tabs.css
- repo://Tailwind/foundation/theme.css

## Deliverables

- Tailwind policy codified in source/docs.
- Simple layout declarations converted to Tailwind @apply where appropriate.
- Token/state CSS retained only with documented rationale.

## Dependency Impact

- Unlocks every visual hardening subbundle.
- Weak Tailwind proof requires reopening before SB06-SB09 continue.

## Validation Depth

- Critical Semantic Adequacy Gate.
- Build CSS output and compare changed file hashes.
- Desktop and narrow screenshots for representative input/button/tabs surfaces.
- Critical foundation: before closure, create `proof/SB02/manifest.md` and `proof/SB02/semantic-invariants.md` with Semantic Adequacy Gate evidence, changed-file hashes, transcripts, source assertions, anti-stub audit, and raw-note literal closure.


## Implementation Steps

- Classify raw CSS as token/state/browser-required or refactorable utility composition.
- Refactor in small groups and rebuild output.css.
- Capture before/after screenshots for affected surfaces.

## Scope Exceptions

- WebGL and Canvas implementation changes are excluded unless this subbundle explicitly updates sandbox separation metadata without touching their component internals.

## Do Not Do

- Do not refactor unrelated WebGL or Canvas source.
- Do not delete compatibility or AppComponents code without migration proof and consumer checks.
- Do not close UI work from source inspection only.

## Acceptance Checklist

- No broad visual regression in Inputs, Actions, Navigation.
- Raw CSS that remains has a reason.
- Tailwind build output is updated intentionally.

## Proof Required

- Failing-first or red-team visual assertion for a known refactor target.
- Passing Tailwind build transcript.
- Playwright screenshots for affected routes.
- Anti-stub audit for TODO/NotImplemented in styling scripts.

## Browser Validation Logging

- Routes: /groups/inputs, /groups/actions, /groups/navigation/tabs.
- Viewports: maximized desktop, 1366x900, 390x844.
- Open dropdown/tabs states where affected and record clipping/wrapping answers.

## Progression Gate

- Passed. Closure proof is recorded in `bundle://proof/SB02/manifest.md` and `bundle://proof/SB02/semantic-invariants.md`.
- Tailwind build, clean .NET build, semantic verifier, source assertions, anti-stub audit, strict Playwright visual proof, and MCP screenshots are complete.
- Baseline mobile actions/tabs overflow issues were repaired before closure.

## Suggested Agent Prompt

Execute SB02 by hardening Tailwind foundations first. Keep changes small, rebuild CSS, and capture real screenshots before allowing downstream visual work.
