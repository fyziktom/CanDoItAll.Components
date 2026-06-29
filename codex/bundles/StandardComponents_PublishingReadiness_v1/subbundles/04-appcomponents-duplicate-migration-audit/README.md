# SB04 AppComponents Duplicate Migration Audit

## Status

- Status: `Completed`

## Objective

Reduce old basic components in the main CanDoItAll AppComponents project while preserving useful behavior and app-specific complex surfaces.

## Covered Inputs

- RAW06: Audit duplicate AppComponents basic components.

## Prerequisites

- SB03 base/helper decisions complete.
- Inventory duplicate matrix reviewed.

## Exact Source References

- C:\repositories\CanDoItAll\src\CanDoItAll.AppComponents\Components
- C:\repositories\CanDoItAll\src\CanDoItAll.AppComponents\Primitives
- repo://src/CanDoItAll.Components.BaseLib/Components
- repo://src/CanDoItAll.Components.Charts/Components

## Deliverables

- Duplicate-by-duplicate migration table.
- Behavior improvements ported to Components or recorded as app-specific exceptions.
- Main app consumers migrated or compatibility aliases defined.

## Dependency Impact

- Unlocks publishing transfer because basic primitives must live in Components.
- Mistakes can break the main CanDoItAll app, so consumer proof is required.

## Validation Depth

- Critical Semantic Adequacy Gate.
- Cross-repo source assertions and build proof.
- Behavior comparison tests for old/new copies.
- Critical foundation: before closure, create `proof/SB04/manifest.md` and `proof/SB04/semantic-invariants.md` with Semantic Adequacy Gate evidence, changed-file hashes, transcripts, source assertions, anti-stub audit, and raw-note literal closure.


## Implementation Steps

- Compare old and standard component parameters and behavior.
- Port useful old behavior such as click in-flight guard when appropriate.
- Remove, alias, or mark app-specific each old component.

## Scope Exceptions

- WebGL and Canvas implementation changes are excluded unless this subbundle explicitly updates sandbox separation metadata without touching their component internals.

## Do Not Do

- Do not refactor unrelated WebGL or Canvas source.
- Do not delete compatibility or AppComponents code without migration proof and consumer checks.
- Do not close UI work from source inspection only.

## Acceptance Checklist

- Every old basic component is Solved, Partially solved, or intentionally app-specific.
- Main CanDoItAll app builds after migration.
- No standard primitive remains only in AppComponents unless exception is documented.

## Proof Required

- Cross-repo build transcripts.
- Migration matrix updates.
- Source assertions for deleted/aliased/ported components.
- Playwright proof for migrated visual surfaces if routes exist.

## Browser Validation Logging

- Use app and sandbox routes for migrated components when visible.
- Capture desktop and mobile screenshots for any changed component surface.
- Open menus/dialogs/tabs/dropdowns where relevant.

## Progression Gate

- The subbundle validator must pass closure review before downstream dependent subbundles start.
- If proof is weak or a screenshot shows wrapping, clipping, layout, available-space, or interaction defects, keep this subbundle `In progress` and reopen prerequisites as needed.
- Closure proof accepted at `bundle://proof/SB04/manifest.md`.
- Semantic invariants accepted at `bundle://proof/SB04/semantic-invariants.md`.
- Progression result: `Passed`; proceed to SB05 sandbox taxonomy and coverage expansion.

## Suggested Agent Prompt

Execute SB04 as a careful migration audit, not a deletion spree. Compare behavior first, port improvements, then remove or alias old basics with proof.
