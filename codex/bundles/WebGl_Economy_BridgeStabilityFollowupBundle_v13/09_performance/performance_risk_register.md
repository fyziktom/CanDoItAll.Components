# Performance risk register

| Risk | Why it matters | Mitigation |
|---|---|---|
| Duplicating global actions per frame | Timeline grows O(frames * global actions) | Partition actions by step/time |
| Metadata-only stages | Run document looks valid but does nothing | Require stages to contain command batches/actions |
| Per-action interop | Browser overhead and frame stalls | Batch commands per frame/stage |
| Stage wait ignored | Ordered behavior visually wrong | Stage executor with barriers |
| Competing per-object motions | Actor jitters or teleports | Per-object motion queue |
| Full scene rebuild | Large scenes slow | Patch-based updates |
| Unbounded proof snapshots | Large JSON payload | Compact result mode |
| Unindexed bridge mapping | O(N^2) node/action resolution | Node-object index |
| Cross-repo project ref fragility | CI/build breaks | package/local dual-mode strategy |
