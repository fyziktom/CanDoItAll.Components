# Shared-well readiness matrix

| Capability | Current status | Risk | Follow-up |
| --- | --- | --- | --- |
| Explicit actor/location/resource/stores | Mostly present | Alias drift | Canonical model cleanup |
| Placement JSON | Present | Not fully integrated with transitions | Apply placement/topology into route cost |
| Parameters JSON | Present | Water-specific fields in generic abstractions | Resource-scoped params |
| Actor needs | Partial | `DailyWaterNeed` is not generic | Requirement model |
| Carry/storage capacity | Partial | Needs resource scope | Capacity model |
| Inventory build | Partial | Event kind exists, policy limited | Generic handler |
| Resale/trade | Partial | Event kind exists, tax/admin not expanded | Policy/rule expansion |
| Rule/tax/admin | Partial | Rule params are not interpreted generically | Institution rule interpreter |
| Ordered visual actions | Partial | Stage wait and motion queues are unsafe | WebGlRun queue/stage executor |
| Invariants | Defined conceptually | No evaluator yet | Expected invariant evaluator |
| Reproducible input pack | Partial | Hash validator shallow | Input pack hash manifest validator |
