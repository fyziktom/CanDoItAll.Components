# Shared Resource Probe

Do not implement the final UI demo in this bundle. Use this as a real headless readiness probe.

Generic expected behavior:

1. Actors have locations.
2. Resource source has a location and finite/limited stock.
3. Actors create travel/use/transfer/admin/rule events.
4. Simulation backend materializes frames and deltas.
5. Visualization maps frames to nodes, links, symbols and actions.
6. WebGL bridge projects stages and motions.
7. Snapshot analysis can explain the current state.

The probe passes only if the generated artifacts prove the full pipeline.
