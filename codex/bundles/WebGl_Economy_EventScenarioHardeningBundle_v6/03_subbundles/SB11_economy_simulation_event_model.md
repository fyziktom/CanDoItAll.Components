# SB11 - Economy: generic simulation event model

Add backend-neutral events to express what happens without deciding how it is rendered.

Event kinds:

- `resource-use`
- `resource-transfer`
- `actor-move-intention`
- `actor-work`
- `actor-admin`
- `store-changed`
- `relationship-changed`
- `issue-raised`
- `issue-resolved`
- `rule-applied`

Core fields:

- `EventId`
- `Kind`
- `StepIndex`
- `OccurredAtUtc`
- `ActorId`
- `TargetActorId`
- `PlaceId`
- `ResourceId`
- `Quantity`
- `DurationSeconds`
- `Metadata`

For the shared well:

- actor uses resource at place/well;
- actor returns to home place;
- admin work event for keeper/council;
- transfer/sale event if nearby actor resells water.

Do not put visual action or WebGL fields here. Use semantic ids and metadata only.
