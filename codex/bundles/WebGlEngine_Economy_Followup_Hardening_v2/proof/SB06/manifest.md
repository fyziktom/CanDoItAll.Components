# SB06 proof manifest

Status: Completed 2026-06-02.

## Changed file hashes

| SHA-256 | Path |
| --- | --- |
| `636adea46734542c06cf9e1c988275dca188121ce63aa21c2da9171faa270cfd` | `docs/webgl/run-layer-boundary.md` |
| `f65e042e7df2d8ec6109494b9561954cb6979cfcfb42cef2ca373f76f297f41e` | `src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunDocumentValidator.cs` |
| `d7ff5f56027a1204076d0e07a4ac54cff2e03a3651df2d8ff759447e947dc843` | `src/CanDoItAll.Components.WebGlRunLib/Actions/WebGlRunActionPlanValidator.cs` |
| `36e3db7c6e75f5eb95aebe1b8868d774f304596a9c9670133140cf4770202a67` | `tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunValidatorTests.cs` |
| `7b32b6e38269e5d344f1614668c859ef10285c3f1d48ddb09485a24670eb9cec` | `../CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlRunProjector.cs` |
| `16678d760af6b121d77678d787b29e0ec61afe0845c438c37342b23a19fd39df` | `../CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlInitialSceneProjector.cs` |
| `89077bfc32e9676fc5f88f31754e15435d05151bafc9722d3989606b92cdb706` | `../CanDoItAll.Economy/src/CanDoItAll.Economy.Simulation.WebGlBridge/EconomyWebGlBridgeContracts.cs` |
| `638c3ecbdb4483863e9414d797a8e7b9839e3b23bb1e40582f2809a790407663` | `../CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeTests.cs` |

## Command transcripts

| Command | Transcript | Exit | Result |
| --- | --- | --- | --- |
| Focused Components validator tests before implementation | `bundle://proof/SB06/transcripts/failing-first-provenance-boundary-tests.txt` | 1 for `dotnet test` | Failing-first: source provenance metadata was rejected and domain-specific stage id was accepted. |
| Focused Economy bridge failing-first attempt | `bundle://proof/SB06/transcripts/failing-first-economy-provenance-bridge-tests.txt` | Timeout | Restore/build exceeded the timeout before the semantic assertion ran; not used as the primary failing-first gate. |
| Focused Components validator tests after implementation | `bundle://proof/SB06/transcripts/passing-components-provenance-boundary-tests.txt` | 0 | 9 validator tests passed, including source provenance, stage id rejection, action kind rejection, and strict source parameters. |
| Focused Economy provenance bridge test | `bundle://proof/SB06/transcripts/passing-economy-provenance-bridge-tests.txt` | 0 | Economy projected run preserves `source.*` provenance and passes `WebGlRunDocumentValidator`. |
| `dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj` | `bundle://proof/SB06/transcripts/components-webglrunlib-tests.txt` | 0 | 41 WebGlRunLib tests passed. |
| `dotnet build CanDoItAll.Components.slnx` | `bundle://proof/SB06/transcripts/components-build.txt` | 0 | Components solution build passed with 0 warnings and 0 errors. |
| `npm run webglrunlib:audit-boundary` | `bundle://proof/SB06/transcripts/components-webglrunlib-boundary-audit.txt` | 0 | WebGlRunLib boundary audit passed: WebGlLib-only reference and no forbidden domain terms in first-party source. |
| `npm run webgllib:audit-boundary` | `bundle://proof/SB06/transcripts/components-webgllib-boundary-audit.txt` | 0 | WebGlLib boundary audit passed. |
| Economy bridge-family tests | `bundle://proof/SB06/transcripts/economy-webglbridge-tests.txt` | 0 | 24 Economy WebGL bridge tests passed with local Components project references. |
| Components `git diff --check` | `bundle://proof/SB06/transcripts/components-git-diff-check.txt` | 0 | No whitespace errors; only existing LF-to-CRLF warnings. |
| Economy `git diff --check` | `bundle://proof/SB06/transcripts/economy-git-diff-check.txt` | 0 | No whitespace errors; only existing LF-to-CRLF warnings. |
| `python scripts/validate_bundle.py --stage prepared --profile initiative` | `bundle://proof/SB06/transcripts/bundle-validator-after-sb06.txt` | 0 | Bundle validator passed after SB06 proof/doc updates. |
| SB06 proof placeholder scan | `bundle://proof/SB06/transcripts/sb06-proof-placeholder-scan.txt` | 0 | No stale SB06 proof placeholders found. |

## Browser artifacts

N/A. SB06 changes validator and bridge provenance behavior only; no browser-visible runtime or UI behavior changed.

## Source assertions

`bundle://proof/SB06/transcripts/source-policy-assertions.txt` records:

- `WebGlRunDocumentValidator` has an `allowSourceProvenance` gate and recognizes `source.*` keys.
- Stage ids are scanned for domain terms.
- `WebGlRunActionPlanValidator` keeps action parameters strict with `allowSourceProvenance: false`.
- Components tests cover positive `source.*` metadata and negative domain stage ids.
- Economy bridge stores bridge provenance under `source.bridge`.
- Economy bridge test validates projected output through the generic validator.
- Plain `bridge` metadata is no longer used in Economy WebGlBridge source.

## Anti-stub audit

`bundle://proof/SB06/transcripts/changed-file-placeholder-scan.txt` passed with no `TODO`, `stub`, `placeholder`, or `NotImplementedException` markers in SB06 changed production/test/doc files. `bundle://proof/SB06/transcripts/sb06-proof-placeholder-scan.txt` passed with no stale SB06 proof placeholders.

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| `source.*` provenance metadata gate | `WebGlRunDocumentValidator.ValidateDomainTerms` | Run document and action plan validators | Skips domain-term scanning only for provenance metadata keys; values are stored but not interpreted | Failing-first source provenance tests failed before the gate. |
| Stage id domain-term validation | `WebGlRunDocumentValidator.ValidateFrames` | Run documents and playback callers | Rejects obvious domain terms in generic stage ids before playback | Failing-first stage-id test passed invalid input before the new check. |
| Strict action parameters | `WebGlRunActionPlanValidator` | Action plan validation callers | Action parameters remain generic contract data even if a key starts with `source.*` | `Action_plan_validator_rejects_domain_specific_source_parameters` proves parameters are not widened with metadata provenance. |
| Economy bridge provenance | Economy WebGlBridge projectors/mappers | Economy validators plus generic WebGlRun validator | Bridge, scenario, event, and hash provenance is emitted under `source.*`; plain metadata remains domain-neutral | Focused Economy test fails if `source.bridge` is absent or generic validation rejects the projected run. |

## Gate decision

Pass. SB06 defines and enforces the provenance boundary, preserves source provenance through the Economy bridge, proves negative and positive generic validator behavior, and keeps Components boundary audits green.
