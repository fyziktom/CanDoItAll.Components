# Shared-well readiness probe

The shared-well example is not the target implementation in this wave. It is a readiness test for generic infrastructure.

## Desired future behavior

A scenario defines:

- actors: households, well keeper, council, trader;
- locations: homes, shared well, council desk, market point;
- resources: water, water stock, labor, tokens, compliance, trust;
- rules: fair draw limit, maintenance contribution, resale/tax policy, admin overhead;
- behaviors: daily water use, collect water, stockpile, resell, report/tax, enforce rule;
- event templates: resource use, collect, travel, return, trade, admin, tax, rule check.

The simulator should generate events and frames:

```text
definition -> event stream -> state transition frames/deltas -> visual intentions/actions
```

The visual layer should not know about rules. It receives visual intentions:

```text
actor goes to target
actor changes pose
actor shows status symbol
actor transfers resource
actor returns home
```

The WebGL run layer should not know about economy. It receives generic actions:

```text
move-to-object
set-pose
show-symbol
resource-transfer-visual
return-to-anchor
```

## Readiness checklist

The current code is close to a minimal version, but missing:

- canonical definition/event normalization;
- rule/behavior-driven event expansion;
- generic state transition materializer;
- inventory/carry-capacity modeling;
- distance-aware travel/action duration;
- resale/trade/tax/admin rules;
- visual action sequencing with stage boundaries;
- target/anchor resolution for location/place/resource nodes;
- batch execution that preserves multiple motions for one actor.
