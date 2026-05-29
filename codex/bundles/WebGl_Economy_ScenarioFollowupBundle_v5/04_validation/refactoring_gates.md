# Required refactoring gates

## Gate A: after Components runtime fixes

Must pass before `WebGlRunLib` action compiler work:
- `npm run webgllib:audit-scene-runtime`
- no runtime JS hard-threshold violations;
- asset cache disposal proof;
- scene index patch proof;
- command result consistency proof.

## Gate B: after WebGlRunLib action compiler

Must pass before Economy scenario loading work:
- WebGlRunLib tests;
- sandbox run playback uses WebGlRunLib controller/service;
- no economy references in reusable Components projects.

## Gate C: after Economy file split

Must pass before scenario definitions:
- all old tests still pass;
- no new Simulation.* forbidden references;
- line-count report shows all new simulation files under thresholds.

## Gate D: after scenario/event/visual-action contracts

Must pass before closure:
- shared-well definition roundtrip;
- entrepreneur definition roundtrip;
- event stream hash tests;
- visual action mapper tests;
- no bridge/WebGL dependency in Economy.
