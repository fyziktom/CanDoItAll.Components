# Requirement traceability

| Requirement | Subbundles | Proof |
| --- | --- | --- |
| R01 Browser/runner fail-closed | SB02, SB03, SB12 | Runner and adapter tests, fake runtime proof |
| R02 Multi-frame playback explicit | SB03, SB04, SB12 | Multi-frame apply tests and Economy UI browser proof |
| R03 Economy deterministic replay | SB04, SB11, SB12 | Seek/step/last browser proof with applied frame indexes |
| R04 Stage ordering parity | SB09, SB12 | Shared ordering policy tests |
| R05 Pathless scenario source | SB05, SB06, SB12 | Source API tests and runtime UI proof |
| R06 Scenario pack manifest/security | SB06, SB12 | Traversal and manifest validation tests |
| R07 Portable export/import | SB07, SB12 | Cross-directory import/export proof |
| R08 Async session persistence | SB07, SB12 | No sync-over-async scan and async tests |
| R09 Provenance policy | SB08, SB12 | Generic and Economy validator tests |
| R10 External import lifecycle | SB10, SB12 | WebGlSceneView lifecycle test |
| R11 Performance budgets | SB11, SB12 | Large scene diagnostics |
| R12 Proof quality | SB01, SB12 | Proof integrity validator |
