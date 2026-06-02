# Normalized requirements

| Id | Requirement |
| --- | --- |
| R01 | Browser apply and runner paths must be fail-closed for all known frame/apply errors. |
| R02 | Multi-frame playback must have explicit semantics; no public helper may silently apply only the last frame of a multi-frame playback result. |
| R03 | Economy UI must use deterministic replay semantics for Step/First/Last/Seek and must not apply a delta frame as if it were absolute. |
| R04 | Generic validation and runtime application must use the same stage ordering policy. |
| R05 | Scenario catalogs and session services must support pathless scenario sources. |
| R06 | Scenario packs must have manifests, deterministic hashes, and path traversal/size validation. |
| R07 | Session export/import must be portable and hash-verified without relying on machine-local absolute paths. |
| R08 | Session persistence must offer async-first APIs and avoid sync-over-async blocking. |
| R09 | Provenance metadata must be typed or allowlisted while keeping domain semantics outside Components. |
| R10 | WebGlSceneView external import must update or invalidate component-side scene lifecycle keys. |
| R11 | Large-scene/replay performance budgets must be measured and enforced with repeatable proof. |
| R12 | Proof artifacts must be non-empty, assertion-backed, and tied to changed-source hashes. |
