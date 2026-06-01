# Performance Risk Register

| Risk | Area | Impact | Mitigation |
|---|---|---|---|
| Too many stage actions per frame | Bridge/WebGLRun | Slow interop and runtime queue pressure | Coalesce only within safe stages; add stage count metrics |
| Queued motion explosion | WebGL runtime | Long animation queues, memory pressure | Cap queue length per object, expose diagnostics |
| Large snapshot JSON | Economy snapshots | Slow export/import | Add compact summary export and file-store streaming later |
| Visual mapping lookup repeated per action | Bridge | O(n*m) mapping cost | Pre-index mapping by category/action/event |
| Snapshot analysis repeated over full frames | Economy analysis | Slow pause-analysis | Add indexed frame/snapshot views |
| Many links/relationships | WebGL scene | Rendering and patch cost | Link LOD or category filtering later |
| GLB asset load cost | WebGL scene | startup delay | Preload profile and fallback primitives |
| Test files too large | Maintenance | fragile reviews | Split tests by domain: bridge, snapshot, sandbox, performance |
