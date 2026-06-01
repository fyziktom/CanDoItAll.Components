# Gap analysis using generic probes

This bundle still uses two example probes, but only as probes. The implementation must remain generic.

## Probe A: shared resource community

Example interpretation: people live at different distances from a shared resource. Some actors are closer, can collect more cheaply, may build inventory and resell to farther actors. Rules introduce draw limits, fees/taxes, admin work, and enforcement.

Generic concepts required:

- finite or replenishing resource store;
- actors with resource requirements;
- placement/topology/distance;
- storage/carry capacity;
- resource transfer/trade;
- administrative overhead;
- rule evaluation;
- issue/event generation;
- relationship/trust/conflict changes;
- visual actions: move to target, perform at target, change pose, show symbol, return to anchor;
- snapshot analysis: current stores, active issues, admin burden, relationship stress, pending events.

Do not hardcode `water`, `well`, `near-household`, or `far-household` in generic libraries.

## Probe B: finite land / constrained resource growth

Example interpretation: actors compete over a finite spatial resource. External demand may make expansion profitable, but institution rules should prevent extreme concentration.

Generic concepts required:

- finite spatial resource;
- ownership/holding stores;
- market demand;
- transfer of ownership-like quantities;
- concentration metric;
- anti-concentration rule;
- fees/taxes/admin overhead;
- inequality/fairness invariant;
- visual actions: move/trade/ownership transfer, rule warning symbol, admin pose, market/institution node.

Do not hardcode `farmer`, `land`, `parcel`, or `oligarchy` in generic libraries.

## Remaining missing generic capabilities

1. Executable run session that can apply a `WebGlRunDocument` to a `WebGlSceneView`.
2. Stage barrier proof against real motion completion, not only time.
3. Snapshot builder/analyzer as production services, not test-local code.
4. Visual mapping schema split between renderer-neutral and WebGL-specific bridge mapping.
5. Full bridge diagnostics for unresolved nodes, assets, anchors, symbols, poses, links, and stages.
6. File-backed snapshot store and analysis export for later UI use.
7. Headless E2E proof from input pack to WebGL run document to simulated execution state.
