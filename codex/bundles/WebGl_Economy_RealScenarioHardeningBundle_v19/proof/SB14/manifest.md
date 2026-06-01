# SB14 Proof Manifest

Status: Completed.

## Scope

SB14 extends the Economy and WebGL performance probes and records real counts, elapsed times, thresholds, and bounded stage queue/journal evidence.

## Source Changes

- `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyPerformanceProbeTests.cs`
  - Raises the headless scale gate to 100 actors, 500 resources, 500 stores, 1000 events, and 500 visual actions.
  - Adds deterministic final-frame hash timing and threshold assertion.
  - Adds WebGL projection scale counts for 500 scene objects, 1000 links, 1000 object-attached symbols, and 500 staged commands.
- `repo://CanDoItAll.Components/tools/webgllib/audit-sharedwell-performance.cjs`
  - Extends the JavaScript runtime audit to build 500 scene objects, 1000 links, 1000 symbols, and drain 500 command stages.
  - Verifies the command stage journal remains bounded at 200 entries while preserving counters.

## Generated Artifacts

- `repo://CanDoItAll.Economy/artifacts/economy/performance/simulation-performance-proof.json`
- `repo://CanDoItAll.Components/artifacts/webgl-economy-kernel-bridge-hardening-v12/performance/components-performance-proof.json`
- `bundle://proof/SB14/performance-result-summary.json`

## Key Counts And Timings

- Economy headless: 100 actors, 500 resources, 500 stores, 1000 events, 500 visual actions.
- Economy elapsed times: normalization 26.068 ms, materialization 35119.84 ms, deterministic hash 26.009 ms, snapshot build 413.895 ms.
- Deterministic hash: `3c1bbaff788733b622c58a6d08d3a0afca8e7f16c9cc4c377eeb36afe5c0e942`.
- WebGL scale: 500 scene objects, 1000 links, 1000 symbols, 500 staged commands.
- WebGL elapsed time: 413.25 ms for the SB14 data/runtime scale proof.
- Stage bounds: queue drained to 0, queue maximum 500, journal bounded at 200 entries with 1300 dropped historical entries.

## Validation

- `bundle://proof/SB14/transcripts/economy-performance-probe-tests.txt`
- `bundle://proof/SB14/transcripts/webgl-runtime-performance-audit.txt`
- `bundle://proof/SB14/transcripts/performance-result-assertions.txt`
- `bundle://proof/SB14/transcripts/stage-journal-source-assertions.txt`
- `bundle://proof/SB14/transcripts/anti-stub-audit.txt`
- `bundle://proof/SB14/transcripts/changed-file-hashes.json`
- `bundle://proof/SB14/transcripts/prepared-validator-after-sb14.txt`

## Result

SB14 progression gate is satisfied: performance output reports real elapsed times and counts for every required Economy and WebGL scale target, and both the stage queue and journal bounds are asserted.
