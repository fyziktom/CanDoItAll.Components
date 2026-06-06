# Production-line canary analysis

## Purpose

The production-line canary is a non-Economy pressure test. It must remain in Components as a generic sample/proof only, not as a domain simulator.

## Future domain simulator assumptions

A manufacturing/production-line simulator will likely own:

- process model
- machine/station definitions
- buffers/queues
- orders/jobs/WIP domain state
- throughput and cycle-time simulation
- fault/maintenance model
- operator actions
- scheduling and routing policies

Components must not own these concepts.

## What Components should provide

Components should provide generic visual primitives sufficient for the domain driver to map onto:

- node-like objects
- lane/path/link visuals
- directed-flow visuals
- status symbols
- overlays
- labels
- selectable/draggable objects
- command batches
- staged motions
- wait/barrier policies
- proof snapshot
- diagnostics
- runtime idle/stop
- package-mode samples

## Canary vocabulary rules

Allowed in generic source code:

- node
- object
- route
- zone
- lane
- buffer as generic visual buffer only if not domain-specific; prefer `queue-zone`
- flow
- token
- status
- alarm as generic visual severity? prefer `warning-symbol` or `status-symbol`

Forbidden in generic source code outside test fixture names/docs:

- production line
- station
- machine
- work order
- WIP
- operator
- conveyor
- takt
- downtime
- maintenance
- throughput

These terms may appear in a test-only domain-driver canary fixture with explicit boundary allowlist.

## Blind spots revealed

1. Need generic token/flow visualization support without resource/economy semantics.
2. Need high-count repeated-object proof.
3. Need overlay/status-symbol pooling.
4. Need path-following motion proof or explicit decision not to implement pathfinding in Components.
5. Need event-callback policy for small UI controls without turning engine into simulator.
6. Need domain driver manifest to record mapping semantics externally.
