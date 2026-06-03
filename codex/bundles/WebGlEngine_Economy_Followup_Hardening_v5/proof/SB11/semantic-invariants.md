# Semantic invariants SB11

Status: completed

## Invariants

- Invariant 1: Components docs must explain host-side playback lifecycle without adding domain-specific scenario or Economy semantics to generic Components guidance.
- Invariant 2: Pause troubleshooting must describe both host cancellation and browser runtime stop verification.
- Invariant 3: Host integration guidance must name the public APIs used for runner playback, browser apply, and runtime stop.
- Invariant 4: Economy docs must own scenario-pack manifest and deterministic replay guidance.
- Invariant 5: Package-mode guidance must require fresh package feeds, isolated caches, explicit package-mode properties, and no stale-feed fallback.
- Invariant 6: Proof docs must remain artifact-backed and non-placeholder.

## Proof Mapping

| Invariant | Evidence |
| --- | --- |
| Invariant 1 | `bundle://proof/SB11/transcripts/components-domain-neutral-host-doc-scan.txt` proves the new Components host guide has no forbidden domain terms. |
| Invariant 2 | `bundle://proof/SB11/transcripts/source-assertion-docs-coverage-scan.txt` finds the Pause checklist, `StopRuntimeActivityAsync`, `queuedCommandStageCount`, `runtimeStopCount`, and screenshot-only proof warning. |
| Invariant 3 | `repo://components/docs/webgl/playback-hosting-and-troubleshooting.md` names `WebGlSceneViewBrowserRuntime`, `WebGlRunBrowserApplyAdapter`, `WebGlRunDocumentRunner`, `ApplyPlaybackAsync`, and `StopRuntimeActivityAsync`. |
| Invariant 4 | `bundle://proof/SB11/transcripts/source-assertion-docs-coverage-scan.txt` finds `scenario.manifest.json`, `packHash`, `fileHashes`, `IEconomySimulationScenarioCatalog`, `full` replay, and `incremental` replay in Economy docs. |
| Invariant 5 | `repo://components/docs/webgl/playback-hosting-and-troubleshooting.md`, `repo://components/README.md`, and `repo://economy/README.md` document fresh packages, isolated `NUGET_PACKAGES`, explicit package-mode properties, and stale-feed failure. |
| Invariant 6 | `bundle://proof/SB11/transcripts/anti-stub-docs-scan.txt` and `bundle://proof/SB11/transcripts/docs-link-check.txt` prove changed docs have no placeholder markers and all linked docs exist. |

## Production Behavior Artifact Matrix

No production behavior artifacts were added by SB11. Documentation now describes existing production and proof contracts.
