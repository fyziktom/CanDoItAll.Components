# SB06 - Economy bridge projector decomposition

## Goal
Prevent bridge projection files from becoming monolithic.

## Required actions

Split or prepare split for:

- layer projection,
- node/object projection,
- link projection,
- symbol projection,
- visual state catalog projection,
- action mapping,
- action planning/compilation,
- metadata/provenance helpers,
- diagnostics helpers.

## Acceptance criteria

- No bridge production file exceeds 300 lines unless explicitly documented.
- `EconomyWebGlInitialSceneProjector` is reduced or decomposed.
- Bridge remains the only layer with WebGL-specific dependencies.
