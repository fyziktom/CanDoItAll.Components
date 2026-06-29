# {{SUBBUNDLE_TITLE}}

## Status

- `Ready`

## Objective

- State the single outcome this subbundle must produce.

## Covered Inputs

- List raw notes, requirements, and findings owned by this subbundle.

## Prerequisites

- List predecessor subbundles and proof artifacts required before editing.

## Exact Source References

- Use portable `repo://` and `bundle://` paths that exist.

## Deliverables

- List source, test, docs, proof, and report outputs.

## Dependency Impact

- State which later subbundles become untrustworthy if this phase is wrong.

## Validation Depth

- State whether this is a critical foundation, critical UI foundation, package closure, or final closure.

## Implementation Steps

1. Verify prerequisites.
2. Make the smallest scoped changes.
3. Capture proof and update the execution report.

## Scope Exceptions

- State explicit exceptions.

## Do Not Do

- Include scope boundaries.

## Acceptance Checklist

- Add observable done conditions.

## Proof Required

- List commands, transcripts, screenshots, manifests, and semantic-invariant requirements.

## Browser Validation Logging

- State routes, viewports, actions, screenshots, assertions, and visual review questions, or `N/A` with a reason.

## Progression Gate

- State exactly what must pass before downstream subbundles may continue.

## Suggested Agent Prompt

```text
Implement this subbundle only.
Preserve all functionality, verify prerequisites, avoid WebGL work, capture artifact-backed proof, update bundle status/report rows, and stop if the progression gate cannot honestly pass.
```
