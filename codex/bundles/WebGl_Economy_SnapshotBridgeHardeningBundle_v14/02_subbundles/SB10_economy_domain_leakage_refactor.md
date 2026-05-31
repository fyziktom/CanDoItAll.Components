# SB10 - Economy domain leakage refactor

Goal:
- Remove example terms from generic production code.

Tasks:
1. Run boundary audit.
2. Search generic projects for:
   - water, well, farmer, land, parcel, oligarchy, shared-well, near-household, far-household.
3. Example terms may exist only in:
   - scenario factories,
   - fixture JSON,
   - tests/probe names,
   - documentation/examples.
4. Generic models must use resource-scoped concepts:
   - resource requirements,
   - resource limits,
   - capacity,
   - ownership,
   - transfer,
   - rule,
   - relationship,
   - issue.

Acceptance:
- No domain-specific terms in generic abstractions/bridge/policies except allowlisted examples/tests.
