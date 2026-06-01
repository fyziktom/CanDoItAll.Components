# WebGlRunLib Boundary Target

## Purpose

`WebGlRunLib` is the generic layer above `WebGlLib` for replayable visual runs. It exists because large simulations need more than a single scene snapshot, but this logic must not pollute the light render substrate.

## Allowed concepts

- Run document
- Timeline
- Frame
- Action
- Action stage
- Object binding
- Anchor
- Visual state
- Pose key
- Symbol key
- Motion command
- Scene patch
- Command batch
- Barrier/wait/event policy
- Playback status
- Run validator

## Forbidden concepts

- Economy account, market, exchange, ledger, price, buyer, seller
- Vernon Smith-specific experiment semantics
- production line, station, work order, machine, product
- persistence provider details
- domain event store details

## Runtime relation to WebGlLib

The preferred dependency direction:

```text
WebGlRunLib -> WebGlLib
Application/Economy -> WebGlRunLib -> WebGlLib
WebGlLib -X-> WebGlRunLib
```

A browser host may combine `WebGlSceneView` and a higher-layer run player. The lower WebGlLib package must remain independently usable.
