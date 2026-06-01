# SB10 — Performance And Scalability Probe

## Goal

Find bottlenecks before building UI.

## Required measurements

- experiment load time
- scenario materialization time
- visual frame mapping time
- WebGL run document projection time
- snapshot build/export time
- number of frames, actions, stages, motions, patches, objects, links, snapshots
- peak stage count per frame
- peak motion count per frame
- snapshot JSON size
- WebGL run document JSON size

## Probe sizes

Use at least:

- small: existing fixtures
- medium: synthetic generic scenario, e.g. 50 actors, 100 stores, 200 events
- large-ish desktop planning probe: 200 actors, 500 stores, 1,000 events

Do not optimize for mobile/small screens.
