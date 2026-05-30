# SB19 — Performance bottleneck proofs

## Components proof
Desktop-only proof:
- 100+ actors;
- 200+ actions;
- command batch application;
- no small/medium/mobile proof.

Metrics to capture:
- batch duration;
- command count;
- coalesced patch count;
- dropped duplicate motion count;
- rendered frames;
- active motion count;
- asset cache hits/misses;
- link geometry rebuild count.

## Economy proof
Simulation proof:
- 50+ actors;
- 1 shared resource;
- 10+ steps;
- deterministic event expansion;
- no WebGL references.

Metrics to capture:
- event compile duration;
- frame materialization duration;
- visual action mapping duration;
- event/action counts.
