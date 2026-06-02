# SB07 semantic invariants

Status: Completed 2026-06-02.

| Invariant ID | Expected behavior | Shallow-pass trap | Negative proof | Positive proof | Source files | Downstream dependency |
| --- | --- | --- | --- | --- | --- | --- |
| SB07-INV-001 | Economy bridge validation supports dynamic objects created by earlier patches, stages, or frames. | Checking add-object patches but still validating all later references only against the initial scene. | `failing-first-economy-dynamic-object-validator-tests.txt` rejected add-object-then-motion against the initial scene. | `passing-economy-dynamic-object-validator-tests.txt`, `economy-webglbridge-tests.txt`. | `EconomyWebGlRunValidator.cs`, strict mapping tests | SB10 public docs and future dynamic scenarios. |
| SB07-INV-002 | Same-stage motion to an object created in that same stage remains invalid. | Allowing intra-stage dependencies that may be reordered or batched by the command pipeline. | Same-stage negative test asserts `unresolved-motion-object` for `object.dynamic`. | Focused Economy dynamic-object tests. | `EconomyWebGlRunValidator.cs`, `EconomyWebGlBridgeStrictMappingTests.cs` | SB11 browser playback proof. |
| SB07-INV-003 | Generic WebGlRun runtime validation checks direct frame-level motions before applying a frame. | Fixing Economy validation while direct-only generic run frames can still apply unresolved motions. | `failing-first-components-direct-frame-motion-validation.txt` applied an unresolved direct frame motion before the fix. | `passing-components-direct-frame-motion-validation.txt`, full WebGlRunLib tests. | `WebGlRunFrameExecutionValidator.cs`, `WebGlRunDocumentRunnerTests.cs` | SB09 package proof and SB11 playback route proof. |
| SB07-INV-004 | Current Economy runtime scenarios do not require dynamic object creation, but future scenarios have a documented extension path. | Claiming dynamic support is necessary without auditing shipped examples, or leaving future behavior implicit. | Scenario scan transcript found no add/remove object payloads in scenario inputs/fixtures. | `scenario-inventory.md` and boundary documentation. | Runtime scenario inputs, fixtures, `docs/webgl/run-layer-boundary.md` | SB10 docs/public surface. |

## Production Behavior Artifact Matrix

| Artifact / signal | Producer | Consumer | Lifecycle | Negative test |
| --- | --- | --- | --- | --- |
| `knownObjectIds` | Economy validator | Bridge validation | Evolves through frame/stage order using patch add/remove object effects | Failing-first Economy test rejected later motion before this state existed. |
| `ApplyPatchObjectIds` | Economy validator | Later validation checks | Adds objects after a patch validates; removes objects after removal patches | Same-stage motion test proves additions are not visible to motions already checked in the same stage. |
| Direct frame motion validation | Generic frame execution validator | `WebGlRunDocumentRunner` | Validates frame-level motions before direct frame patches are applied | Components failing-first direct-motion test. |
| Scenario inventory | SB07 proof | Later docs/proof subbundles | Records current static examples and extension path for dynamic object creation | Scenario scan transcript. |

## Raw Requirement Closure

| Requirement | Closure |
| --- | --- |
| R08 | Solved for SB07. Dynamic object references are supported after earlier patch/stage/frame creation, same-stage motion dependencies are rejected, and current Economy scenario inventory shows shipped examples do not need dynamic objects today. |
| R14 | Maintained locally. Components WebGlRunLib/WebGlLib boundary audits pass after the generic direct-frame validator change. |
