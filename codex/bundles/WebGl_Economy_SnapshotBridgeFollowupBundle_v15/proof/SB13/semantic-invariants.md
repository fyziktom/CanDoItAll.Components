# Semantic invariants SB13

Status: Completed.

## Shared-Resource Generic Probe

Invariant ID: SB13-shared-resource-generic-probe

The shared-resource readiness case is valid only as a generic capability probe. Production simulation, visualization, bridge, sandbox, snapshot, and analysis code must not encode the fixture terms, while the fixture-driven tests prove the generic system can model shared location access, unequal distance, inventory change, trade, fee/admin burden, rule violation, issues, relationship stress, visual action sequencing, and snapshot analysis.

## Shallow-Pass Trap

A shallow pass would merely load the fixture and count frames. The strengthened tests assert the actual semantics: actor distances, inventory growth, buy/sell events, admin/fee events, violation events, issue and relationship mutation, ordered sequence steps, commandful WebGL stages, and analysis findings with source paths.

## Adversarial Negative Proof

The first strengthened run failed because metadata-only WebGL stages were present. The fix skips commandless batch stages and the final probe asserts no projected stage is empty. `generic-production-forbidden-term-scan.txt` also proves the fixture terms are absent from generic production code.

## Semantic Positive Proof

`SharedWellInputPack_CompilesRunsAndMapsReadinessFlowWithoutDemoUi` proves shared-resource simulation and visual action sequencing. `SharedResourceSnapshotAnalysisProbe_ExportsReimportsAndAnswersVisualStateQuestion` proves hash-verified snapshot export/import and analysis findings for admin burden, active issues, resource concentration, relationship stress, pending events, visual-stage pressure, and invariant summary.

## Anti-Stub Audit

`bundle://proof/SB13/source-assertions/anti-stub-scan.txt` records no TODO, NotImplementedException, NotSupportedException, stub, or placeholder markers in the changed bridge/test files.

## Completed Validator Tokens

Shallow-pass trap: SB13 rejects merely loading the shared-resource fixture or counting frames.
Adversarial negative proof: the first strengthened run exposed commandless WebGL stages, which are now filtered and verified absent.
Semantic positive proof: shared-resource simulation, visual action sequencing, snapshot analysis, bridge tests, and full Economy tests prove the generic shared-resource path.
Anti-stub audit: SB13 anti-stub source assertion confirms proof is not placeholder code.
