# SB08 - Economy: canonical event normalizer

## Problem
Events contain duplicate fields:
`Kind`/`EventKind`, `ActorId`/`ActorIds`, `Quantity`/`Magnitude`, `Duration`/`Timing.Duration`.

## Tasks
- Add `SimulationEventNormalizer`.
- Add `SimulationEventValidation`.
- Detect conflicts if aliases disagree.
- Normalize event kind aliases to a canonical event kind.
- Normalize timing into `Timing.StepIndex`, `Offset`, `Duration`.
- Normalize participants from actor/resource/source/target fields.

## Tests
- Dotted and legacy hyphen event names map to canonical kinds.
- Conflicting `ActorId` and `ActorIds[0]` fails validation.
- Duration alias conflict is detected.
