# Proof Manifest

Subbundle: `SB12`
Status: `Completed`
Prepared by: Codex
Completed at UTC: `2026-06-02T05:52:41Z`

## Changed File Hashes

| Repo | Path | Before SHA-256 | After SHA-256 | Why changed |
| --- | --- | --- | --- | --- |
| CanDoItAll.Components | `README.md` | `0f70d1aedc01d0c70e1e138b66bcb9f966778f360f1aa894dac0cce362bcd609` | `aedd6df9940676a7f9c2c669a6129a729baf11967b2d2516310e785483875404` | Documented the WebGlLib/WebGlRunLib package outputs and the stale `0.1.0` cache/feed warning for package proofs. |
| CanDoItAll.Components | `docs/webgl/run-layer-boundary.md` | `d87e2feea3cfe2a1296f7780136632fc587f5431daa737ea62d0b51fad92c695` | `af168b5032e16cf1323f79209c5af286009cfc2f4a739d631eeb09d2cb996f5b` | Added project-reference and package-consumption commands for Economy bridge integration. |
| CanDoItAll.Economy | `README.md` | `29ed8463fbd7f0e2257216c56da725d35b81fb15d18749b0b21a5c38ac43597f` | `bd81637f56491f786886241c1dc231e0c7eed7e96a94f14b77ca86c320f7893c` | Added WebGL Components integration instructions for local project-reference and package mode. |
| CanDoItAll.Components | `codex/bundles/WebGlEngine_Economy_Hardening_Bundle_v1/proof/SB12/package-proof.NuGet.config` | `new-file` | `3de5d43a5810cd83bd77b1af5a30b62f8c5cb26132de03424e96a080b4d3747c` | Proof-only NuGet.config that clears stale private feeds and restores from fresh Components pack output plus nuget.org. |

Hash transcript: `proof/SB12/transcripts/sb12-file-hashes.txt`.

## Commands

| Command | Working directory | Transcript path | Result |
| --- | --- | --- | --- |
| `dotnet pack CanDoItAll.Components.slnx --configuration Release --output artifacts/packages` | `C:\repositories\CanDoItAll.Components` | `proof/SB12/transcripts/components-pack-release.txt` | Passed; emitted `CanDoItAll.Components.WebGlLib.0.1.0.nupkg` and `CanDoItAll.Components.WebGlRunLib.0.1.0.nupkg` plus the shared package set. |
| `dotnet build src\CanDoItAll.Economy.Simulation.WebGlBridge\CanDoItAll.Economy.Simulation.WebGlBridge.csproj --configuration Release /p:ComponentsRepoRoot=C:\repositories\CanDoItAll.Components` | `C:\repositories\CanDoItAll.Economy` | `proof/SB12/transcripts/economy-webglbridge-project-reference-build.txt` | Passed in local project-reference mode; existing nullable warnings in Economy abstractions preserved. |
| Package-mode build with `NUGET_PACKAGES=proof\SB12\nuget-cache-proof-config`, `dotnet restore --configfile proof\SB12\package-proof.NuGet.config /p:UseComponentsWebGlRunLibPackage=true ...`, then `dotnet build --no-restore ...` | `C:\repositories\CanDoItAll.Economy` | `proof/SB12/transcripts/economy-webglbridge-package-reference-build.txt` | Passed; assets graph shows WebGlLib/WebGlRunLib compile/runtime package entries from the proof feed. |
| Package-mode attempt with existing source order | `C:\repositories\CanDoItAll.Economy` | `proof/SB12/transcripts/failing-package-reference-stale-feed-order.txt` | Failed first: existing `0.1.0` feed/cache restored stale WebGlLib assets and bridge compile types were missing. |
| Package-mode attempts with inline source arguments | `C:\repositories\CanDoItAll.Economy` | `proof/SB12/transcripts/failed-package-reference-restoresources-arg.txt`, `proof/SB12/transcripts/failed-package-reference-source-args.txt` | Failed: MSBuild/dotnet source argument parsing treated the nuget.org URL as a local path. Fixed by proof NuGet.config. |
| Invalid `ComponentsRepoRoot` local-reference build | `C:\repositories\CanDoItAll.Economy` | `proof/SB12/transcripts/failing-invalid-componentsrepo-root.txt` | Failed for the intended validation-target error and documented the package-mode alternative. |
| `npm run webgllib:audit-boundary` | `C:\repositories\CanDoItAll.Components` | `proof/SB12/transcripts/components-webgllib-boundary-audit.txt` | Passed. |
| `npm run webglrunlib:audit-boundary` | `C:\repositories\CanDoItAll.Components` | `proof/SB12/transcripts/components-webglrunlib-boundary-audit.txt` | Passed. |
| `dotnet test tests\CanDoItAll.Economy.Tests\CanDoItAll.Economy.Tests.csproj --filter BridgeProjectReferencesOnlyAllowedProjects` | `C:\repositories\CanDoItAll.Economy` | `proof/SB12/transcripts/economy-bridge-dependency-audit-test.txt` | Passed; existing restore warnings preserved. |
| Source assertion and package graph scans | Both repos | `proof/SB12/transcripts/sb12-source-assertions.txt`, `proof/SB12/transcripts/sb12-package-restore-graph.txt` | Passed. |
| Anti-stub and boundary scan | Both repos | `proof/SB12/transcripts/sb12-anti-stub-and-boundary-scan.txt`, `proof/SB12/transcripts/components-domain-leak-scan.txt` | Passed with intentional forbidden-term validator/test matches noted. |
| `python scripts\validate_bundle.py --stage execution --profile initiative` | `C:\repositories\CanDoItAll.Components\codex\bundles\WebGlEngine_Economy_Hardening_Bundle_v1` | `proof/SB12/transcripts/bundle-validate-execution.txt` | Passed: `Bundle validation passed for stage=execution, profile=initiative, subbundles=14`. |

## Source Assertions

| Assertion | Source path | Line/range or search term | Verified by |
| --- | --- | --- | --- |
| Components versioning is centralized and pack command exists. | `Directory.Build.props`, `package.json`, `README.md` | `PackageVersion`, `dotnet pack`, `artifacts\packages` | `proof/SB12/transcripts/sb12-source-assertions.txt` |
| WebGlRunLib remains layered over WebGlLib. | `src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj` | `ProjectReference Include="..\CanDoItAll.Components.WebGlLib` | `proof/SB12/transcripts/sb12-source-assertions.txt` |
| Solution includes WebGlLib, WebGlRunLib, sandbox, and tests. | `CanDoItAll.Components.slnx` | `CanDoItAll.Components.WebGlLib`, `CanDoItAll.Components.WebGlRunLib` | `proof/SB12/transcripts/sb12-source-assertions.txt` |
| Economy bridge supports local project-reference mode. | `CanDoItAll.Economy.Simulation.WebGlBridge.csproj` | `ComponentsRepoRoot`, conditional `ProjectReference` | `proof/SB12/transcripts/sb12-source-assertions.txt` |
| Economy bridge supports package-consumption mode for both required packages. | `CanDoItAll.Economy.Simulation.WebGlBridge.csproj` | `UseComponentsWebGlRunLibPackage`, `CanDoItAll.Components.WebGlLib`, `CanDoItAll.Components.WebGlRunLib` | `proof/SB12/transcripts/sb12-source-assertions.txt` |
| Fresh proof feed is isolated from stale private packages. | `proof/SB12/package-proof.NuGet.config` | `<clear />`, `ComponentsProofFeed`, `nuget.org` | `proof/SB12/transcripts/sb12-source-assertions.txt`, `proof/SB12/transcripts/sb12-package-restore-graph.txt` |
| Docs explain ultra-light WebGlLib, generic WebGlRunLib, and Economy bridge consumption paths. | `README.md`, `docs/webgl/run-layer-boundary.md`, `C:\repositories\CanDoItAll.Economy\README.md` | `Package And Project Integration`, `WebGL Components Integration` | `proof/SB12/transcripts/sb12-source-assertions.txt` |

## Semantic Adequacy Gate

| Item | Required evidence | Status | Artifact |
| --- | --- | --- | --- |
| Shallow-pass trap named | A package proof could pass using project references, stale global cache, or an old private `0.1.0` package rather than the fresh Components output. | Passed | `proof/SB12/semantic-invariants.md` |
| Adversarial negative proof | Bad `ComponentsRepoRoot` fails with explicit validation target; stale/source-order package restore fails before proof config pins the feed. | Passed | `proof/SB12/transcripts/failing-invalid-componentsrepo-root.txt`, `proof/SB12/transcripts/failing-package-reference-stale-feed-order.txt` |
| Semantic positive proof | Components release pack, Economy project-reference bridge build, and Economy package-mode bridge build all pass. | Passed | `proof/SB12/transcripts/components-pack-release.txt`, `proof/SB12/transcripts/economy-webglbridge-project-reference-build.txt`, `proof/SB12/transcripts/economy-webglbridge-package-reference-build.txt` |
| Anti-stub audit | No first-party stubs in touched docs/source; boundary audits pass; intentional forbidden-term validator/test matches are documented. | Passed | `proof/SB12/transcripts/sb12-anti-stub-and-boundary-scan.txt`, `proof/SB12/transcripts/components-webgllib-boundary-audit.txt`, `proof/SB12/transcripts/components-webglrunlib-boundary-audit.txt` |
| Raw-note closure | Cross-repo package/project integration closed for SB12; final browser/perf proof remains SB13. | Passed | `reviews/01-execution-report.md`, `traceability/01-requirement-traceability.md` |
| Downstream smoke | Bridge dependency audit test passes after docs/proof changes. | Passed | `proof/SB12/transcripts/economy-bridge-dependency-audit-test.txt` |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Components WebGL packages | `dotnet pack CanDoItAll.Components.slnx --configuration Release --output artifacts/packages` | Economy bridge/package-mode consumers | Emitted from Components release pack output, versioned by `Directory.Build.props`. | Stale external `0.1.0` package feed failed package-mode bridge build. |
| `package-proof.NuGet.config` | SB12 proof | Package-mode restore proof | Clears inherited feeds and points to fresh `artifacts\packages` plus nuget.org. | Inline restore source arguments failed; proof config fixed deterministic restore. |
| Economy bridge reference knobs | `CanDoItAll.Economy.Simulation.WebGlBridge.csproj` | Local developers, package consumers, CI | `ComponentsRepoRoot` for local project mode; `UseComponentsWebGlRunLibPackage=true` plus package versions for package mode. | Invalid `ComponentsRepoRoot` fails with explicit validation target. |

## Browser Validation Analytics

| Route | Viewport | Actions | Screenshot/log artifacts | Result |
| --- | --- | --- | --- | --- |
| N/A | N/A | SB12 touched docs/proof and package/reference validation only; no browser-visible runtime behavior changed. | Build/package proof transcripts listed above. | Passed / browser proof not applicable for SB12. |

## Refactor Gate Result

- Touched files reviewed: `README.md`, `docs/webgl/run-layer-boundary.md`, `C:\repositories\CanDoItAll.Economy\README.md`, `proof/SB12/package-proof.NuGet.config`.
- Duplicates removed: integration commands are documented once per owning context: Components package output, shared run-layer boundary, and Economy bridge consumption.
- Layering checked: WebGlLib remains independently packable; WebGlRunLib depends on WebGlLib; Economy consumes both and Components does not consume Economy.
- Fixture-specific code removed: none introduced.
- Docs/tests updated: Components README, run-layer boundary doc, Economy README, dependency audit test transcript, proof manifest, semantic invariants, traceability and execution report.
- Remaining refactor risk: default `0.1.0` package version can still collide with older private-feed packages; SB12 documents and proves the isolated config/cache mitigation.
