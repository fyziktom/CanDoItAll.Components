# Semantic Invariants

Subbundle: `SB10`

## Invariants

| Invariant ID | Requirement | Expected behavior | Disallowed shallow implementation | Negative proof | Positive proof |
| --- | --- | --- | --- | --- | --- |
| SB10-INV-001 | Every generated executable command remains traceable to source economy input. | For every projected stage with generated commands, each motion and scene patch carries `source.visualActionId`, `source.eventId`, `source.simulationFrameId`, and `source.inputPackHash`. | Stamping only the containing stage metadata while motions/patches remain anonymous. | `proof/SB10/transcripts/failing-first-command-provenance.txt`, `proof/SB10/transcripts/passing-command-provenance-tests.txt` | `ProjectorStampsSourceProvenanceOnEveryGeneratedCommand` in `proof/SB10/transcripts/passing-command-provenance-tests.txt` |
| SB10-INV-002 | Strict bridge validation fails command provenance gaps with structured diagnostics. | Missing command source metadata produces command-specific error codes and non-empty severity/code/path fields. | Accepting command metadata gaps because the containing stage has source metadata. | `ValidatorRejectsEveryStrictExecutionGapWithStructuredDiagnostics` removes motion `source.inputPackHash` and adds a patch without `source.eventId`. | `proof/SB10/transcripts/passing-economy-webglbridge-tests.txt` |
| SB10-INV-003 | Strict fallback behavior remains explicit and test-covered. | Unresolved subject/target/pose/symbol/action-kind failures are errors by default and warning/diagnostic paths remain explicitly opted in. | Relaxing strict validation globally to make projection happy paths pass. | Existing strict tests in `EconomyWebGlBridgeStrictMappingTests` and `EconomyWebGlBridgeTests`; full slice passed in `proof/SB10/transcripts/passing-economy-webglbridge-tests.txt`. | `FixtureProbeProjectsWithStrictMappingWithoutFallbacks`, `ValidatorAcceptsPositiveStrictMappingWithoutFallbacks` |
| SB10-INV-004 | Economy bridge remains a consumer, not a Components domain leak. | Components packages contain no Economy package/namespace/project references; bridge package mode references WebGlLib/WebGlRunLib conditionally. | Adding Economy APIs to WebGlLib/WebGlRunLib or relying on hidden project-reference-only transitive behavior. | `proof/SB10/transcripts/components-domain-leak-scan.txt`, `proof/SB10/transcripts/failing-package-reference-missing-webgllib.txt` | `proof/SB10/transcripts/passing-webglbridge-package-reference-build.txt`, `proof/SB10/transcripts/passing-economy-webglbridge-tests.txt` |

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- | --- |
| Command source metadata | `EconomyWebGlActionStageProjector` | Validator, diagnostics consumers, future browser playback/debug proof | Written after WebGlRunLib compiles action stages and before the bridge wraps WebGlLib commands in WebGlRun frame stages. | Removing command metadata fails the strict validator. |
| Command source diagnostics | `EconomyWebGlRunValidator` | Economy validation result consumers | Added per motion and per frame patch during document validation. | Missing patch event id fails with `missing-command-source-event-id`. |
| Conditional package mode | WebGlBridge csproj | NuGet/project consumers | Default remains project-reference mode; package mode opts into WebGlRunLib and WebGlLib package versions. | Package build failed before the direct WebGlLib package reference and passed after it. |

## Reopen Triggers

- A later projection emits motions or patches without all four source keys.
- A fallback path becomes valid by default without explicit diagnostic options.
- Package-mode builds fail without local project references.
- Components WebGlLib/WebGlRunLib gains an Economy package/namespace dependency.
