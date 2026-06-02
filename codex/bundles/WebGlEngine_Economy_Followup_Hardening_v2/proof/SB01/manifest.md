# SB01 proof manifest

Status: Completed.

Owned requirements: R13, R14 baseline support.

Raw notes: `bundle://inputs/raw-user-request.md`.

Semantic invariant contract: `bundle://proof/SB01/semantic-invariants.md`.

## Changed file hashes

| File | Before SHA-256 | After SHA-256 | Evidence |
| --- | --- | --- | --- |
| `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.cs` | `eb248c8ae2db59ffd4ffb99dc578e37d4f70e7c17331b1515f6a95009fd24b93` | `792bfba166cc439825a54f69d7a4d30cf4e4de0bef37ee40cdbc3349aef63060` | `bundle://proof/SB01/transcripts/changed-file-before-after-hashes.txt` |
| `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.Helpers.cs` | `absent` | `b0c02b56a963a7c2fb560376aaea2b0be391ab2f4fed3eb3533a28220a5123e2` | `bundle://proof/SB01/transcripts/changed-file-before-after-hashes.txt` |

Additional baseline hashes: `bundle://proof/SB01/changed-file-baseline.md`, `bundle://proof/SB01/transcripts/source-baseline-and-hashes.txt`.

## Command transcripts

| Command | Result | Transcript |
| --- | --- | --- |
| `dotnet build CanDoItAll.Components.slnx` | Pass | `bundle://proof/SB01/transcripts/components-dotnet-build.txt` |
| `dotnet test tests/CanDoItAll.Components.WebGlLib.Tests/CanDoItAll.Components.WebGlLib.Tests.csproj --no-build` | Pass | `bundle://proof/SB01/transcripts/components-webgllib-tests.txt` |
| `dotnet test tests/CanDoItAll.Components.WebGlRunLib.Tests/CanDoItAll.Components.WebGlRunLib.Tests.csproj --no-build` | Pass | `bundle://proof/SB01/transcripts/components-webglrunlib-tests.txt` |
| `npm run webgllib:audit-scene-runtime-imports` | Pass | `bundle://proof/SB01/transcripts/components-npm-audit-scene-runtime-imports.txt` |
| `npm run webgllib:audit-boundary` | Pass | `bundle://proof/SB01/transcripts/components-npm-audit-webgllib-boundary.txt` |
| `npm run webglrunlib:audit-boundary` | Pass | `bundle://proof/SB01/transcripts/components-npm-audit-webglrunlib-boundary.txt` |
| `dotnet build CanDoItAll.Economy.slnx` | Pass with existing warnings | `bundle://proof/SB01/transcripts/economy-dotnet-build.txt` |
| `dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj --no-build` | Pass before SB01 test split | `bundle://proof/SB01/transcripts/economy-tests.txt` |
| `powershell -NoProfile -ExecutionPolicy Bypass -File scripts/audit-simulation-boundaries.ps1` | Fail before SB01 test split | `bundle://proof/SB01/transcripts/economy-audit-simulation-boundaries.txt` |
| `powershell -NoProfile -ExecutionPolicy Bypass -File scripts/audit-simulation-boundaries.ps1` | Pass after SB01 test split | `bundle://proof/SB01/transcripts/economy-audit-simulation-boundaries-after-test-split.txt` |
| `dotnet test tests/CanDoItAll.Economy.Tests/CanDoItAll.Economy.Tests.csproj` | Pass after SB01 test split | `bundle://proof/SB01/transcripts/economy-tests-after-test-split.txt` |

## Browser artifacts

N/A. SB01 did not change browser-visible runtime or UI.

## Source assertions

| Assertion | Evidence |
| --- | --- |
| Exact source references exist and are hashed. | `bundle://proof/SB01/transcripts/source-baseline-and-hashes.txt` |
| Economy sandbox still has runtime fixture path dependency for SB02. | `bundle://proof/SB01/transcripts/source-assertions-current-risks.txt` |
| WebGlRun direct+staged command risk remains for SB03. | `bundle://proof/SB01/transcripts/source-assertions-current-risks.txt` |
| Runtime option reset policy still requires SB04. | `bundle://proof/SB01/transcripts/source-assertions-current-risks.txt` |
| Dynamic object reference policy still requires SB07. | `bundle://proof/SB01/transcripts/source-assertions-current-risks.txt` |

## Anti-stub audit

| Scope | Result | Evidence |
| --- | --- | --- |
| Components WebGlLib/WebGlRunLib first-party paths | No TODO, NotImplemented, fixture, proof-only, placeholder-success, or stub matches. | `bundle://proof/SB01/transcripts/components-first-party-anti-stub-scan-v2.txt` |
| Components WebGlLib including vendor | One vendor GLTFLoader TODO, excluded from first-party production audit. | `bundle://proof/SB01/transcripts/components-anti-stub-scan.txt` |
| Economy simulation/sandbox production paths | Runtime `Fixtures/ExperimentInputs` hits remain in sandbox page; SB02 owns removal. No TODO/NotImplemented hits in this scan. | `bundle://proof/SB01/transcripts/economy-anti-stub-fixture-scan.txt` |

## Production Behavior Artifact Matrix

SB01 introduced no new production signal, state, record, or event.

| Artifact / signal | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Boundary-audit test layout | `repo://CanDoItAll.Economy/tests/CanDoItAll.Economy.Tests/EconomyWebGlBridgeStrictMappingTests.cs` and helper partial | `repo://CanDoItAll.Economy/scripts/audit-simulation-boundaries.ps1` | Audit runs during proof and fails if simulation/bridge test files exceed the line gate | Failing-first transcript `bundle://proof/SB01/transcripts/economy-audit-simulation-boundaries.txt`; passing transcript `bundle://proof/SB01/transcripts/economy-audit-simulation-boundaries-after-test-split.txt` |

## Gate decision

Pass. SB01 produced current-state inventory, proof-hygiene audit, key source hashes, baseline command transcripts, failing-first and passing proof for the discovered boundary-audit failure, and anti-stub/source assertions. Downstream subbundles remain responsible for the risks mapped in `bundle://proof/SB01/current-state-inventory.md`.
