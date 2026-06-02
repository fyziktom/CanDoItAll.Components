# SB06 semantic invariants

Status: Completed 2026-06-02.

| Invariant ID | Expected behavior | Shallow-pass trap | Negative proof | Positive proof | Source files | Downstream dependency |
| --- | --- | --- | --- | --- | --- | --- |
| SB06-INV-001 | `source.*` metadata is provenance and may carry upstream domain identifiers without generic interpretation. | Allowing all metadata or contract fields with domain terms instead of limiting the exception to provenance metadata. | `failing-first-provenance-boundary-tests.txt` shows source metadata was rejected before the policy existed. | `passing-components-provenance-boundary-tests.txt`, `passing-economy-provenance-bridge-tests.txt`. | `WebGlRunDocumentValidator.cs`, `WebGlRunActionPlanValidator.cs`, `docs/webgl/run-layer-boundary.md` | SB07 object-reference policy and SB10 public docs. |
| SB06-INV-002 | Generic contract fields reject obvious domain semantic leakage, including action kinds, stage ids, non-source metadata, and action parameters. | Testing only non-source metadata while domain-specific action or stage identifiers still pass. | Failing-first stage-id test accepted `stage.market.transfer` before implementation; existing action-kind negative test remains active. | Focused and full WebGlRunLib tests, including strict source-parameter negative test. | `WebGlRunDocumentValidator.cs`, `WebGlRunActionPlanValidator.cs`, `WebGlRunValidatorTests.cs` | SB07/SB11 playback safety. |
| SB06-INV-003 | Economy bridge output preserves bridge/scenario/event/hash provenance under `source.*` and can pass the generic WebGlRun validator. | Validating only with the Economy-specific validator and missing generic boundary leakage. | Timed Economy failing-first attempt recorded the intended pre-fix check but timed out during build; Components failing-first covers the generic validator contract. | `passing-economy-provenance-bridge-tests.txt` and `economy-webglbridge-tests.txt`. | Economy WebGlBridge projector/mapper files and `EconomyWebGlBridgeTests.cs` | SB10 docs/public surface and SB11 UI route proof. |
| SB06-INV-004 | Components packages remain generic and do not reference Economy assemblies or domain projects. | Passing tests by putting domain rules into Components production source. | Boundary audits would fail on forbidden source terms or references; source assertions verify no plain `bridge` metadata remains in Economy bridge output. | `components-webglrunlib-boundary-audit.txt`, `components-webgllib-boundary-audit.txt`, `source-policy-assertions.txt`. | WebGlRunLib/WebGlLib production source and audit scripts | SB09 package proof and SB12 red-team closure. |

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative test |
| --- | --- | --- | --- | --- |
| Provenance metadata gate | `WebGlRunDocumentValidator.ValidateDomainTerms` | Document/action plan validators | Applies to metadata dictionaries; `source.*` entries bypass domain-term value scans | Failing-first source provenance tests failed until the gate was added. |
| Stage id scan | `ValidateFrames` | Run document validation | Rejects domain terms in stage identifiers before playback | Failing-first stage-id test expected invalid but old validator returned valid. |
| Strict parameters | `WebGlRunActionPlanValidator` | Action plans | Parameters stay generic even when a key is named like provenance | Source-parameter negative test rejects `source.eventKind=market-clearing`. |
| `source.bridge` | Economy WebGlBridge projectors/mappers | Economy and generic validators | Bridge provenance is emitted as source metadata, not plain run metadata | Economy test asserts `source.bridge` exists and plain `bridge` is absent. |

## Raw Requirement Closure

| Requirement | Closure |
| --- | --- |
| R07 | Solved for SB06. Generic WebGlRun validation now distinguishes provenance metadata from semantic leakage: `source.*` metadata is accepted, stage/action ids and non-source contract fields remain strict, and Economy bridge output validates through the generic stack. |
| R14 | Maintained locally. Components boundary audits pass after the policy change and no Economy/domain project references were introduced. |
