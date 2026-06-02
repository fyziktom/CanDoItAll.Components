# Proof Manifest

Subbundle: `SB10`
Status: `Completed`
Prepared by: Codex
Completed at UTC: `2026-06-02T04:53:22Z`

## Changed File Hashes

| Repo | Path | Before SHA-256 | After SHA-256 | Why changed |
| --- | --- | --- | --- | --- |
| CanDoItAll.Economy | `src/CanDoItAll.Economy.Simulation.WebGlBridge/CanDoItAll.Economy.Simulation.WebGlBridge.csproj` | `cc7983cfb0ef52ce5717cd7ff4903dce363dabc668742d0b533f118c54a1463c` | `e109245cf4e9a3d3d4ae5c9c154e0f108b97069b5a94fbeffac60053a59691bc` | Added conditional WebGlLib package reference for package-consumption mode because bridge code directly consumes WebGlLib scene contracts. |
| CanDoItAll.Economy | `src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlActionStageProjector.cs` | `8a409311084242b5794dfee8eb54c0d59f456877f5d1c397b809c84ac4c70743` | `60315336b5fc9d9d7305bf01d433b349810424023b7cfc2e5ff5c797ed3b3f1d` | Stamps visual-action, event, simulation-frame and input-pack provenance onto generated motions and scene patches. |
| CanDoItAll.Economy | `src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunValidator.cs` | `7cbe88a85e33d33f30f7476ad0487b7d6b5433f98ec4357578cb25f8218ed782` | `854b54dc231b2b097f185371d023ee7b50a214f4bbfd95387b68ab8ee342ada2` | Enforces command-level source provenance with structured diagnostics. |
| CanDoItAll.Economy | `tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.cs` | `f560b07d616e50d57a7780392588d97677df7f1dc2b7dbfa1df56ce7af1927ef` | `eb248c8ae2db59ffd4ffb99dc578e37d4f70e7c17331b1515f6a95009fd24b93` | Added failing-first positive/negative command provenance coverage. |
| CanDoItAll.Economy | `tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeTests.cs` | `d34ace2603060514d4ba7458a3e291aebb1b538684a198c84df8062941da752b` | `f624aae7223b80e0eef6565b02fd55e0051fcb1cf97599b510d9256a7efc4c2c` | Updated dependency audit to allow WebGlLib only as a conditional package-mode reference. |

Hash transcript: `proof/SB10/transcripts/sb10-file-hashes.txt`.

## Commands

| Command | Working directory | Transcript path | Result |
| --- | --- | --- | --- |
| `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter 'ProjectorStampsSourceProvenanceOnEveryGeneratedCommand|ValidatorRejectsEveryStrictExecutionGapWithStructuredDiagnostics'` | `C:\repositories\CanDoItAll.Economy` | `proof/SB10/transcripts/failing-first-command-provenance.txt` | Failed first: motion command lacked `source.simulationFrameId`, and validator accepted a motion missing `source.inputPackHash`. |
| Same focused command | `C:\repositories\CanDoItAll.Economy` | `proof/SB10/transcripts/passing-command-provenance-tests.txt` | Passed: 2 tests. |
| `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter EconomyWebGlBridge` | `C:\repositories\CanDoItAll.Economy` | `proof/SB10/transcripts/passing-economy-webglbridge-tests.txt` | Passed: 23 tests. Existing `ncalc`/DI prune warnings preserved. |
| `dotnet build src\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj` | `C:\repositories\CanDoItAll.Economy` | `proof/SB10/transcripts/passing-webglbridge-project-reference-build.txt` | Passed: project-reference mode, 0 warnings/errors. |
| `dotnet pack ... /p:PackageVersion=0.1.0-sb10` for Components package chain | `C:\repositories\CanDoItAll.Components` | `proof/SB10/transcripts/passing-pack-sb10-package-reference-feed.txt` | Passed: created proof feed packages for Common, BaseLib, OverlayLib, WebGlLib, and WebGlRunLib. |
| `dotnet build ... /p:UseComponentsWebGlRunLibPackage=true /p:ComponentsWebGlRunLibPackageVersion=0.1.0-sb10 /p:ComponentsWebGlLibPackageVersion=0.1.0-sb10 /p:RestoreAdditionalProjectSources=proof\SB10\package-feed` | `C:\repositories\CanDoItAll.Economy` | `proof/SB10/transcripts/passing-webglbridge-package-reference-build.txt` | Passed: package-reference mode, 0 warnings/errors. |
| Source assertion scan | `C:\repositories\CanDoItAll.Economy` | `proof/SB10/transcripts/sb10-source-assertions.txt` | Passed. |
| Anti-stub and forbidden bridge dependency scan | `C:\repositories\CanDoItAll.Economy` | `proof/SB10/transcripts/sb10-anti-stub-and-boundary-scan.txt` | Passed. |
| Components domain/package leak scan | `C:\repositories\CanDoItAll.Components` | `proof/SB10/transcripts/components-domain-leak-scan.txt` | Passed. |
| `git diff --check` | `C:\repositories\CanDoItAll.Economy` | `proof/SB10/transcripts/economy-git-diff-check.txt` | Passed with line-ending warnings only. |
| `git diff --check` | `C:\repositories\CanDoItAll.Components` | `proof/SB10/transcripts/components-git-diff-check.txt` | Passed with existing line-ending warnings only. |

Additional diagnostic transcripts retained:

- `proof/SB10/transcripts/failed-package-reference-build-source-arg.txt`: shell source-argument attempt parsed the NuGet URL as a local path.
- `proof/SB10/transcripts/failing-package-reference-missing-webgllib.txt`: package mode failed before adding the conditional WebGlLib package reference.
- `proof/SB10/transcripts/failing-package-reference-stale-webgllib-cache.txt`: package mode failed when restoring a stale cached `0.1.0` WebGlLib package; proof switched to unique `0.1.0-sb10`.
- `proof/SB10/transcripts/failing-economy-webglbridge-dependency-audit.txt`: dependency audit failed until it distinguished project references from conditional package references.

## Source Assertions

| Assertion | Source path | Line/range or search term | Verified by |
| --- | --- | --- | --- |
| Projector stamps command provenance before creating run stages. | `EconomyWebGlActionStageProjector.cs` | `StampCommandProvenance` | `proof/SB10/transcripts/sb10-source-assertions.txt` |
| Motions and patches receive `source.inputPackHash`. | `EconomyWebGlActionStageProjector.cs` | `source.inputPackHash` | `proof/SB10/transcripts/sb10-source-assertions.txt` |
| Validator checks command-level source metadata. | `EconomyWebGlRunValidator.cs` | `AddCommandSourceDiagnostics` | `proof/SB10/transcripts/sb10-source-assertions.txt` |
| Missing command event id has a structured code. | `EconomyWebGlRunValidator.cs` | `missing-command-source-event-id` | `proof/SB10/transcripts/sb10-source-assertions.txt` |
| Package mode declares WebGlLib directly. | `CanDoItAll.Economy.Simulation.WebGlBridge.csproj` | `CanDoItAll.Components.WebGlLib` | `proof/SB10/transcripts/sb10-source-assertions.txt` |
| Positive command provenance test exists. | `EconomyWebGlBridgeStrictMappingTests.cs` | `ProjectorStampsSourceProvenanceOnEveryGeneratedCommand` | `proof/SB10/transcripts/sb10-source-assertions.txt` |
| Dependency audit guards conditional package mode. | `EconomyWebGlBridgeTests.cs` | `UseComponentsWebGlRunLibPackage` | `proof/SB10/transcripts/sb10-source-assertions.txt` |

## Semantic Adequacy Gate

| Item | Required evidence | Status | Artifact |
| --- | --- | --- | --- |
| Shallow-pass trap named | A stage-only provenance implementation could pass weak tests while generated motions/patches remain anonymous. | Passed | `proof/SB10/semantic-invariants.md` |
| Adversarial negative proof | Removing motion `source.inputPackHash` and adding a patch without `source.eventId` fails for command-specific codes. | Passed | `proof/SB10/transcripts/failing-first-command-provenance.txt`, `proof/SB10/transcripts/passing-command-provenance-tests.txt` |
| Semantic positive proof | A valid strict action produces a WebGlRunDocument whose stage and generated motion carry action/event/frame/input-pack provenance. | Passed | `proof/SB10/transcripts/passing-command-provenance-tests.txt` |
| Anti-stub audit | No TODO/stub/placeholder markers in touched source/tests; no forbidden bridge dependencies. | Passed | `proof/SB10/transcripts/sb10-anti-stub-and-boundary-scan.txt` |
| Raw-note closure | Economy bridge strict mapping and package-mode proof closed for SB10 scope. | Passed | `reviews/01-execution-report.md`, `traceability/01-requirement-traceability.md` |
| Downstream smoke | Economy bridge test slice and project/package reference builds pass on final code. | Passed | `proof/SB10/transcripts/passing-economy-webglbridge-tests.txt`, `proof/SB10/transcripts/passing-webglbridge-package-reference-build.txt` |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Command source metadata (`source.visualActionId`, `source.eventId`, `source.simulationFrameId`, `source.inputPackHash`) | `EconomyWebGlActionStageProjector` | `EconomyWebGlRunValidator`, downstream playback/debug tooling | Stamped when compiled WebGlLib batch stages are copied into WebGlRun frame stages. | Missing motion input-pack hash fails with `missing-command-source-input-pack-hash`. |
| Command source diagnostics | `EconomyWebGlRunValidator` | Scenario validation consumers | Emitted for each generated motion and frame patch in a stage. | Patch missing event id fails with `missing-command-source-event-id`. |
| Package-mode bridge references | `CanDoItAll.Economy.Simulation.WebGlBridge.csproj` | NuGet restore/build | Conditional only when `UseComponentsWebGlRunLibPackage=true`. | Package mode failed before direct WebGlLib package reference and passed after the conditional reference. |

## Browser Validation Analytics

| Route | Viewport | Actions | Screenshot/log artifacts | Result |
| --- | --- | --- | --- | --- |
| N/A | N/A | SB10 changed command-level projection, validation, tests, and package metadata. No Economy browser host route was changed for this subbundle. | Command-level projection proof: `proof/SB10/transcripts/passing-economy-webglbridge-tests.txt`; browser-host gap recorded here. | Passed / browser proof not applicable for SB10. |

## Refactor Gate Result

- Touched files reviewed: `CanDoItAll.Economy.Simulation.WebGlBridge.csproj`, `EconomyWebGlActionStageProjector.cs`, `EconomyWebGlRunValidator.cs`, `EconomyWebGlBridgeStrictMappingTests.cs`, `EconomyWebGlBridgeTests.cs`.
- Duplicates removed: command source requirements are centralized in `AddSourceMetadataDiagnostics`; stamping uses a single `StampCommandProvenance` helper.
- Layering checked: Economy bridge still depends only on visual abstractions plus Components WebGlLib/WebGlRunLib; no Economy references were added to Components.
- Fixture-specific code removed: none introduced.
- Docs/tests updated: strict tests, dependency audit, manifest, semantic invariants, traceability, and execution report updated.
- Remaining refactor risk: package proof used a local `0.1.0-sb10` feed to avoid stale NuGet cache; SB12 should continue broader package/public-feed integration.
