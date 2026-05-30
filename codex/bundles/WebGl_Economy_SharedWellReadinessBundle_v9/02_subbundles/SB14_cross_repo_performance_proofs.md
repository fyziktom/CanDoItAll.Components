# SB14 - Cross-repo performance proofs

## Components proof
Large-screen only:
- 100 actors, 100 motions;
- 25 actors, 4 ordered staged motions;
- 300 links, move one actor and assert indexed link updates;
- no mobile/tablet/small-screen proof.

## Economy proof
- 50 actors, 10 resource sources, 200 scheduled events;
- compile event stream;
- materialize frames/deltas;
- map visual actions;
- verify deterministic hash stability.

## Output
Write performance summaries to artifacts with:
- operation counts;
- elapsed milliseconds;
- warnings;
- bottleneck notes.
