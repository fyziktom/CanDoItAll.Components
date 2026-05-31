# Semantic invariants SB14

Status: Completed.

## Finite-Resource Generic Probe

Invariant ID: SB14-finite-resource-generic-probe

The finite-resource readiness case is valid only as a generic capability probe. Production generic simulation, visualization, bridge, and sandbox code must not encode farmer/land/parcel/oligarchy vocabulary. The fixture may contain those terms, but the executable proof must consume it through generic contracts: scenario loading, materialization, metric evaluation, invariant evaluation, snapshot building, and snapshot diffing.

## Required Semantics

The probe must prove that a finite resource has capacity, actors can accumulate the resource, external demand is represented, concentration metrics are computed, anti-concentration policy is expressed as a rule/invariant, fee or enforcement evidence exists, and a before/after policy checkpoint snapshot diff includes resource stores and metrics.

## Semantic Positive Proof

`FiniteResourceMarketProbe_UsesGenericMetricsRulesFeesAndSnapshotDiffs` proves the full semantic set:

- finite capacity is declared and the final resource total remains within capacity
- actor expansion changes resource ownership
- external market demand is represented by actor, cash store, and market-targeted scheduled event
- HHI and top-owner-share metrics exceed the concentration threshold
- the anti-concentration rule is present and the invariant fails after concentration
- the compiled event stream emits the generic tax/fee event
- production-built snapshots produce resource store, metric, invariant, applied-event, and pending-event diffs

## Shallow-Pass Trap

A shallow pass would only load the farmer/land fixture or count frames. SB14 rejects that by asserting concrete state changes, computed metrics, failed invariants, event stream fee evidence, and snapshot diff paths.

## Genericity Proof

`bundle://proof/SB14/source-assertions/generic-production-forbidden-term-scan.txt` records that no `farmer`, `land`, `parcel`, or `oligarchy` terms appear in generic simulation abstractions, visualization, WebGL bridge, or sandbox production code.

## Anti-Stub Audit

`bundle://proof/SB14/source-assertions/anti-stub-scan.txt` records no TODO, FIXME, stub, or NotImplementedException markers in the SB14 touched test and generic simulation production scan set.

## Completed Validator Tokens

Shallow-pass trap: SB14 rejects merely loading the finite-resource fixture or counting frames.
Adversarial negative proof: the probe asserts actual capacity, ownership, demand, concentration, invariant, fee event, and snapshot diff paths to prevent fake green proof.
Semantic positive proof: finite-resource focused test and full Economy run prove generic fixture path through production loaders, evaluators, snapshot builder, and diff logic.
Anti-stub audit: SB14 anti-stub source assertion confirms the proof does not rely on placeholder code.
