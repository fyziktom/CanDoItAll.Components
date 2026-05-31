# Performance risk register

| Area | Risk | Mitigation |
|---|---|---|
| Event normalization | Repeated normalization per frame | Cache normalized event stream by scenario hash |
| Store lookup | O(n) store scans | Maintain actor/resource and location/resource indexes |
| Visual mapping | Repeated mapping per frame/action | Prebuild mapping dictionaries |
| Bridge projection | Many actions create many stages | Batch by frame while preserving stage barriers |
| JS runtime | Too many interop calls | Use staged command batches and compact journal |
| Motion runtime | Many queued motions | Per-object queues and bounded diagnostics |
| Snapshot export | Huge JSON snapshots | Descriptor index, optional visual state, compression later |
| Snapshot diff | Expensive full comparisons | Hash-first comparison and focused sections |
