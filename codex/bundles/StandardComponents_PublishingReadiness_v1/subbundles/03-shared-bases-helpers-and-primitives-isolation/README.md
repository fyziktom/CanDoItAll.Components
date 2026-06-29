# SB03 Shared Bases Helpers And Primitives Isolation

## Status

- Status: `Completed`

## Objective

Separate reusable helpers, bases, enum primitives, and services so generic components are easier to maintain and AppComponents no longer carries basic definitions.

## Covered Inputs

- RAW02: Detailed study of actual implementation and identify all refactoring/hardening.
- RAW08: Identify phases with general foundations first.

## Prerequisites

- SB01 inventory accepted.
- SB02 style policy known if class output changes.

## Exact Source References

- repo://src/CanDoItAll.Components.Common/CssClassBuilder.cs
- repo://src/CanDoItAll.Components.Common/LayoutPrimitives.cs
- repo://src/CanDoItAll.Components.BaseLib/StyledComponentBase.cs
- repo://src/CanDoItAll.Components.BaseLib/Components/Forms/FormPrimitives.cs
- repo://src/CanDoItAll.Components.BaseLib/Components/Buttons/ButtonPrimitives.cs

## Deliverables

- Clear Common vs BaseLib ownership.
- Consistent attribute/class/style merge behavior.
- Contract tests for helper behavior.

## Dependency Impact

- Unlocks duplicate migration and every component group.
- If base behavior changes later screenshots must re-check all affected groups.

## Validation Depth

- Critical Semantic Adequacy Gate.
- Unit/contract tests for helpers.
- Source assertions for every migrated primitive.
- Critical foundation: before closure, create `proof/SB03/manifest.md` and `proof/SB03/semantic-invariants.md` with Semantic Adequacy Gate evidence, changed-file hashes, transcripts, source assertions, anti-stub audit, and raw-note literal closure.


## Implementation Steps

- Audit duplicated primitive enums/services.
- Move non-Razor helpers to Common only when dependency direction stays clean.
- Standardize component base inheritance where appropriate.

## Scope Exceptions

- WebGL and Canvas implementation changes are excluded unless this subbundle explicitly updates sandbox separation metadata without touching their component internals.

## Do Not Do

- Do not refactor unrelated WebGL or Canvas source.
- Do not delete compatibility or AppComponents code without migration proof and consumer checks.
- Do not close UI work from source inspection only.

## Acceptance Checklist

- No duplicate primitive source remains without an exception.
- Components preserve class/style/additional attribute behavior.
- Build and tests pass.

## Proof Required

- Failing-first helper/primitive tests where behavior changes.
- Passing dotnet test transcript.
- Changed-file hashes and source assertion manifest.

## Browser Validation Logging

- N/A unless component markup/base behavior changes. If it does, smoke /groups/inputs and /groups/layout at desktop and mobile.

## Progression Gate

- The subbundle validator must pass closure review before downstream dependent subbundles start.
- If proof is weak or a screenshot shows wrapping, clipping, layout, available-space, or interaction defects, keep this subbundle `In progress` and reopen prerequisites as needed.
- Closure proof accepted at `bundle://proof/SB03/manifest.md`.
- Semantic invariants accepted at `bundle://proof/SB03/semantic-invariants.md`.
- Progression result: `Passed`; proceed to SB04/SB05 foundation checkpoints.

## Suggested Agent Prompt

Execute SB03 by isolating shared bases and primitives conservatively, proving attribute/class/style semantics before any downstream migration.
