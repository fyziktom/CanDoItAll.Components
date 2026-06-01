# SB11 — Performance and scalability gates

## Goal

Prevent hidden bottlenecks before UI integration.

## Required probes

Measure/validate at least:

- 100 actors, 20 resources, 500 events, 1000 visual actions;
- bridge projection time;
- snapshot build time;
- snapshot JSON size;
- visual action normalization time;
- WebGlRunDocument size;
- stage count and motion count;
- max per-object motion queue length.

## Risks to watch

- repeated LINQ scans per frame/action;
- building all snapshots eagerly;
- large snapshot JSON;
- bridge projecting all frames every time;
- visual action list duplication;
- stage logs growing without bounds;
- large GLB assets causing runtime load bottlenecks.

## Output

Produce a small JSON performance proof artifact with thresholds and actual values.
