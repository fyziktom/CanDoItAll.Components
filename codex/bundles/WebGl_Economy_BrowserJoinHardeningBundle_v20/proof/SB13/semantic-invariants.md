# SB13 Semantic Invariants

## Domain Boundary

- Components WebGL/WebGlRun production code must remain Economy-free and example-free.
- Economy simulation abstractions below bridge/sandbox layers must remain Components/WebGL-free.
- Example-specific terms are allowed only in fixtures, explicit negative tests, scenario-specific factories, or legacy audit tooling with follow-up context.

## Language Boundary

- Existing JavaScript remains JavaScript.
- No TypeScript/TSX files are introduced by this bundle.

## Maintainability Boundary

- JS runtime modules over the line-count warning threshold must be listed with split follow-ups.
- Broad tests that exceed review thresholds must have split plans rather than silent growth.
