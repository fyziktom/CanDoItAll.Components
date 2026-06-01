# Snapshot and pause analysis model

The desired user flow is:

```text
Run simulation
  -> visual state looks suspicious
  -> pause at current step/stage
  -> create snapshot
  -> export snapshot JSON
  -> analyze current state
  -> inspect stores, events, metrics, relationships, issues, visual stages
  -> optionally resume from the same run state
```

## Snapshot must remain data-first

A snapshot is not a screenshot and not primarily a WebGL runtime dump. It is a deterministic data artifact:

- scenario/run identity
- step index and simulated timestamp
- current frame
- last delta
- applied events
- pending events
- resource stores
- relationships
- issues
- metrics
- invariants
- provenance hashes
- optional visual state attachment

## Visual state attachment

The visual state attachment should be enough to answer:

- which visual frame was displayed;
- which WebGL run frame/stage was active;
- which actions/stages were active/pending;
- how visual nodes map to WebGL objects;
- whether there were bridge/runtime diagnostics.

It should not embed full renderer internals unless intentionally exported by a runtime snapshot API.

## Missing hardening

- split `DataHash`, `VisualStateHash`, and `FullSnapshotHash`;
- add snapshot analyzer services;
- add file-backed snapshot store;
- add snapshot diff for relationships and visual state;
- add compression/versioning guidance;
- add restore/read-only resume boundary.
