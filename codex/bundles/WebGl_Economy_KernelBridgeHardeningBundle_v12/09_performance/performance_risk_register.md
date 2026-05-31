# Performance Risk Register

| Area | Risk | Current status | Follow-up |
|---|---|---|---|
| WebGL staged batches | Stage wait not executed asynchronously | Stage metadata exists, execution immediate | Add stage runner |
| WebGL motions | Same-object append can fight existing motion | Active Map of motions | Add per-object queue |
| WebGL interop | Batch result too large | Compaction exists but needs limits | Add caps and diagnostics |
| WebGL link updates | Many links rebuild geometry per object update | Indexed links partially exist | Add stress proof |
| Economy transition | Large state clone per frame | Clones all stores/relationships | Add streaming/delta mode |
| Economy hashing | Canonical JSON for large frames | Deterministic but CPU-heavy | Add hash benchmark |
| Economy input pack | Placeholder hashes in fixtures | Strict mode incomplete in fixtures | Add real strict fixtures |
| Economy policies | Broad policy file grows | Many policies in one file | Split by concern |
| Bridge | Missing bridge adapter | Not implemented | Add bridge design/contracts |
