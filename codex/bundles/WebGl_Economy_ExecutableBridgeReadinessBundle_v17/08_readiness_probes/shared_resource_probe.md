# Shared-resource readiness probe

Do not build final UI demo in this bundle.

Use the shared resource scenario only to verify generic capability:

- actors at different locations;
- resource stores;
- distance/cost;
- collection/use/transfer;
- admin/rule events;
- relationship stress;
- visual move/pose/symbol/return;
- snapshot and analysis.

Pass condition:

- no hardcoded shared-resource names outside fixtures/scenario factories;
- headless pipeline creates frames, visual frames, WebGL run document, snapshots and analysis;
- bridge stages include real commands or explicit wait stages.
