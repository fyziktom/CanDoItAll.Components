# SB05 Source Assertions

- Economy WebGL projection preserves source input pack hash, simulation frame id, visual action id, and source event id field on every stage.
- Strict bridge mapping rejects unresolved subjects/targets by default and allows diagnostic fallback only when explicitly requested.
- Wait-only visual actions are preserved as explicit wait-marker stages instead of being silently dropped.

