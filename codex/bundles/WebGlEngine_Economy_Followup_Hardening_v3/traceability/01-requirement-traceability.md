# Requirement Traceability Matrix

| Requirement | Finding(s) | Source refs | Owning subbundles | Required proof |
|---|---|---|---|---|
| R01 | F01 | v2 proof folders and transcripts | SB01 | Proof artifact inventory with non-empty transcript audit |
| R02 | F02 | Economy page DI, Node registration, BUnit manual registration | SB02 | Service extension tests and non-Node host build |
| R03 | F03 | FileSystem catalog, Node scenario content | SB03 | Manifest/hash tests and published output proof |
| R04 | F04 | Economy page `DefaultScenarioId` only | SB03, SB09 | Browser scenario selection proof |
| R05 | F03 | Session `Load(string path)`, export path fields | SB04 | Pathless load/export/import tests |
| R06 | F05 | Session service `.GetAwaiter().GetResult()` | SB05 | Async API tests and scan proving sync-over-async removed |
| R07 | F06 | `WebGlRunFrameApplyResult.FromFrame` | SB06 | Direct API failing-first and passing tests |
| R08 | F07 | `WebGlRunBrowserApplyAdapter.ApplyAsync` | SB06 | Reset failure does not apply batch |
| R09 | F08 | `WebGlSceneViewBrowserRuntime.ImportSceneAsync` | SB07 | RuntimeOptions import/reset test |
| R10 | F09 | `WebGlRunDocumentValidator` source provenance skip | SB08 | Provenance envelope tests and domain boundary audit |
| R11 | F04/F11 | Economy sandbox UI | SB09 | Large + narrow browser proof with diagnostics |
| R12 | F10 | JS/C# diagnostics and resource cache | SB10 | Large-scene budget stress proof |
| R13 | F11 | README/package mode docs | SB11 | Fresh feed, isolated cache, no stale package proof |
| R14 | F12 | All | SB12 | Final red-team manifest |
