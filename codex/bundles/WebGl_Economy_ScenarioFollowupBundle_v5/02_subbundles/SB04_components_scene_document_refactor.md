# SB04 - Components scene document serializer refactor

Repository: `CanDoItAll.Components`

## Problem

`WebGlSceneDocumentSerializer.cs` mixes serialization, normalization, validation, hashing, metadata policy, sorting, and vector validation.

## Target files

Split into:

```text
WebGlSceneDocumentSerializer.cs
WebGlSceneDocumentNormalizer.cs
WebGlSceneDocumentValidator.cs
WebGlSceneDocumentHasher.cs
WebGlSceneDocumentMetadataPolicy.cs
WebGlSceneDocumentSortExtensions.cs
```

## Required behavior preservation

- `Serialize` still produces deterministic hashes.
- UI state can be excluded from scene content hash.
- Runtime/playback/domain metadata remains forbidden in generic scene documents.
- Duplicate object/link IDs are detected.
- Dangling links are detected.
- Missing asset references produce warnings unless configured as errors.

## Tests

Expand `WebGlSceneDocumentSerializerTests` to cover:
- volatile UI state does not alter content hash;
- object order does not alter content hash;
- duplicate IDs fail validation;
- dangling links fail validation;
- forbidden metadata fails validation.
