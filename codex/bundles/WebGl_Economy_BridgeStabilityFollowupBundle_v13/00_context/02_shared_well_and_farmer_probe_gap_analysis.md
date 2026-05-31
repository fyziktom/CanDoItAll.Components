# Shared-well and farmer-land probe gap analysis

Use these scenarios only as probes. Do not hardcode them into generic libraries.

## Shared-well probe

To represent the simple shared-well scenario generically, we need:

1. Explicit input pack:
   - scenario definition
   - placement
   - parameters
   - institutional rules
   - run plan
   - visual mapping
   - expected invariants

2. Simulation:
   - actor resource requirements
   - resource stores
   - distance and travel cost
   - carry capacity
   - inventory build
   - transfer/trade/resale
   - tax/fee/admin burden
   - rule violation detection
   - relationship/trust/conflict update
   - deterministic frame/delta output

3. Visualization:
   - node/object mapping
   - initial scene projection
   - action mapping
   - staged command batches
   - motion queue
   - pose and symbol mapping
   - traceability back to event/frame/experiment hashes

Current status: much of the model exists, but the bridge does not yet generate a usable WebGL run with scene objects and actionable command batches.

## Farmer-land probe

To avoid overfitting to the well scenario, the same pipeline must also support:

- finite spatial resource: land parcels
- actors competing for resource expansion
- external buyer / market pressure
- anti-concentration rules
- metrics such as top-owner share or HHI
- invariant failure if concentration exceeds cap
- visual flow for ownership transfer, rule check, warning/admin state

If the pipeline requires `water`, `well`, `household`, or `shared-well` to work, it is not generic enough.
