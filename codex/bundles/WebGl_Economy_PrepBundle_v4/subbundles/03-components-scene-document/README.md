# SB03 Components Scene Document Hashing And Validation

## Status

- Status: Completed

## Objective

- Split scene content/document hashes and harden scene document validation/options.

## Covered Inputs

- `bundle://02_subbundles/SB03_components_scene_document_hashing.md`
- Components review risk R4.

## Prerequisites

- SB02 command-result contract remains stable.

## Exact Source References

- `bundle://02_subbundles/SB03_components_scene_document_hashing.md`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocument.cs`
- `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Scene/WebGlSceneDocumentSerializer.cs`

## Deliverables

- Updated scene document serializer/options/validation and unit tests.

## Dependency Impact

- Provides stable generic content snapshots for future run playback.

## Validation Depth

- Unit tests prove UI-only state does not change content hash and content changes do.

## Implementation Steps

- Add hash fields/options, validation cases, and serializer tests.

## Do Not Do

- Do not include simulation or domain state in WebGlLib.

## Acceptance Checklist

- Duplicate ids, dangling links, missing assets, invalid vectors, and reserved metadata keys are validated.
- Content and document hashes behave as specified.

## Proof Required

- Test transcripts and changed-file hashes.

## Browser Validation Logging

- No browser proof required for serializer-only behavior.

## Progression Gate

- Proceed to SB04 when serializer tests pass.

## Suggested Agent Prompt

- Harden scene document hashing/validation while preserving generic WebGlLib boundaries.

