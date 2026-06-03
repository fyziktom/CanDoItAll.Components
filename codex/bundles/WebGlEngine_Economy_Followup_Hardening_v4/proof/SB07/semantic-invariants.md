# SB07 semantic invariants

- The implementation must preserve Components genericity.
- The implementation must not introduce Economy semantics into WebGlLib or WebGlRunLib.
- Runtime proof must demonstrate behavior, not only compilation.
- Required proof artifacts must be non-empty.

## Completion assertions

- Async-first methods are added to `IEconomySimulationSandboxSessionService` and implemented in `EconomySimulationSandboxSessionService`.
- Exported sessions carry `ScenarioSourceKind`, `ScenarioPackManifestSchemaVersion`, `SnapshotReferences`, and `PortablePackMetadata`.
- Legacy path fields remain present for backward compatibility and path-based hosts.
- Import prefers catalog scenario source when `ScenarioId` and a catalog are available, then falls back to legacy path fields.
- Sync-over-async scans for `.GetAwaiter().GetResult()` and `.Wait()` in the session service have no matches.
