# SB15 performance source map

## Test proof

- `repo://tests/CanDoItAll.Economy.Tests/EconomyPerformanceProbeTests.cs` contains the strengthened performance probe.
- `bundle://proof/SB15/metrics/simulation-performance-proof.json` records the measured output from the full Economy run.

## Required scale coverage

- 250 actors: `scenario normalization for 250 actors, 1000 events, 500 stores`
- 500 stores: `scenario normalization for 250 actors, 1000 events, 500 stores`
- 1000 scheduled events: `scenario normalization for 250 actors, 1000 events, 500 stores`
- 1000 visual actions: `command batch normalization for 1000 visual actions and staged WebGL commands`
- 500 staged WebGL commands: the same command-batch operation records 1000 staged commands
- 100 snapshots: `100 snapshot export/import round trips`

## Timing coverage

- Simulation materialization: `250 actors, 1000 events, 500 stores materialization`
- Visual mapping: `visual frame mapping over 250 actors and 500 stores`
- Bridge projection: `WebGL bridge projection over mapped large frame`
- Snapshot export/import: `100 snapshot export/import round trips`
- Command batch normalization: `command batch normalization for 1000 visual actions and staged WebGL commands`

## Browser policy

No browser proof was used for SB15, so average/peak browser frame timing is intentionally not recorded. The metrics artifact records `largeScreenOnly: true`, and `webgl-scene-runtime-audit.txt` preserves the static WebGL runtime gate.
