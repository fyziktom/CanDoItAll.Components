# SB08 — Scene Document and Run Boundary Preparation

## Goal

Add generic scene save/load contracts while keeping run persistence and simulation semantics outside `WebGlLib`.

## Implementation tasks

Add:

```text
WebGlSceneDocument
WebGlSceneDocumentSerializer
WebGlSceneDocumentValidationResult
```

Suggested fields:

```text
SchemaVersion
DocumentId
Scene
RuntimeOptions
SavedAtUtc
Source
ContentHash
Metadata
```

The serializer should:

- use deterministic JSON options;
- validate schema version;
- preserve scene metadata;
- avoid domain-specific fields;
- avoid storage provider logic.

Document the future run layer boundary in:

```text
docs/webgl/run-layer-boundary.md
```

Future run layer examples may be described but not implemented:

```text
WebGlRunSnapshot
WebGlRunFrame
WebGlRunClock
WebGlRunCommand
IWebGlRunSceneMapper<TSnapshot>
IWebGlRunPersistenceProvider
```

## Acceptance criteria

- Scene document serialization round-trip tests pass.
- `WebGlLib` has no economy/process/game references.
- Docs clearly state what belongs to a future run layer.

