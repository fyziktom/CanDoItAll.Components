# SB05 — Economy WebGL bridge projector refactor

## Goal

Keep bridge projection maintainable before adding more scenarios.

## Required refactor

Split `EconomyWebGlInitialSceneProjector` responsibilities into focused helpers:

- layer projector
- node/object projector
- link projector
- symbol projector
- visual state catalog projector
- bridge metadata/provenance helper
- fallback/diagnostic helper

Split `EconomyWebGlActionStageProjector` if it grows further:

- frame action selector
- action-to-WebGlRunAction mapper
- plan compiler adapter
- stage metadata/provenance builder
- diagnostics collector

## Required behavior

Projection must still create:

- initial scene objects;
- links;
- layers;
- node-object map;
- visual state catalog;
- staged scene patches/motions;
- source metadata for input pack, simulation frame, visual action, event id.

## Closure proof

- tests pass before and after refactor;
- diff shows no behavior regression;
- file size audit passes.
