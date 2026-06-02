# Requirement Traceability Matrix

| Requirement | Finding(s) | Source refs | Owning subbundles | Closure proof |
|---|---|---|---|---|
| R01 | F01 | v2 proof folders and transcripts | SB01 | `proof/SB01/manifest.md`, `proof/SB01/transcripts/current-state-audit.txt` |
| R02 | F02 | Economy page DI, Node registration, BUnit manual registration | SB02 | `proof/SB02/manifest.md`, `proof/SB02/transcripts/service-registration-and-catalog-source-scan.txt` |
| R03 | F03 | FileSystem catalog, Node scenario content | SB03 | `proof/SB03/manifest.md`, `proof/SB03/transcripts/economy-sandbox-focused-tests-release-after-restore.txt` |
| R04 | F04 | Economy page `DefaultScenarioId` only | SB03, SB09 | `proof/SB09/browser/economy-sandbox-browser-diagnostics.json` |
| R05 | F03 | Session `Load(string path)`, export path fields | SB04 | `proof/SB04/manifest.md`, `proof/SB04/transcripts/pathless-session-api-source-scan.txt` |
| R06 | F05 | Session service `.GetAwaiter().GetResult()` | SB05 | `proof/SB05/transcripts/economy-sandbox-sync-over-async-scan.txt` |
| R07 | F06 | `WebGlRunFrameApplyResult.FromFrame` | SB06 | `proof/SB06/transcripts/components-webglrunlib-tests-release.txt` |
| R08 | F07 | `WebGlRunBrowserApplyAdapter.ApplyAsync` | SB06 | `proof/SB06/transcripts/components-webglrunlib-tests-release.txt` |
| R09 | F08 | `WebGlSceneViewBrowserRuntime.ImportSceneAsync` | SB07 | `proof/SB07/transcripts/runtime-options-import-source-scan.txt` |
| R10 | F09 | `WebGlRunDocumentValidator` source provenance skip | SB08 | `proof/SB08/transcripts/components-webglrunlib-boundary-audit.txt` |
| R11 | F04/F11 | Economy sandbox UI | SB09 | `proof/SB09/browser/economy-sandbox-browser-proof.png`, `proof/SB09/browser/economy-sandbox-console.log` |
| R12 | F10 | JS/C# diagnostics and resource cache | SB10 | `proof/SB10/transcripts/components-webgllib-scene-runtime-audit.txt`, `proof/SB10/transcripts/components-webgllib-resource-ownership.txt` |
| R13 | F11 | README/package mode docs | SB11 | `proof/SB11/manifest.md`, `proof/SB11/transcripts/expected-fail-webgllib-only-stale-feed-restore.txt`, fresh-feed build transcripts |
| R14 | F12 | All | SB12 | `proof/SB12/manifest.md`, `proof/SB12/transcripts/components-solution-build-release.txt`, `proof/SB12/transcripts/economy-solution-build-release.txt` |
