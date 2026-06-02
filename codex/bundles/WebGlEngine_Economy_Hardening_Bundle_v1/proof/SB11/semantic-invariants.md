# SB11 Semantic Invariants

## Invariants

1. Generic scenario scale proof must pass through the same visual-frame and WebGlRun bridge contracts used by smaller real scenarios.
2. Deterministic replay proof must ignore volatile save timestamps and compare stable run structure, stage sources, motion targets, patch ids, frame ordering, object ids, and link ids.
3. Unknown scenario-specific actions must fail under strict mapping and may only become wait/diagnostic behavior through explicit permissive options.
4. Scenario examples may be named at fixture/factory boundaries, but bridge projection must not switch on concrete scenario names.
5. Economy proof must not introduce Economy references into Components WebGlLib or WebGlRunLib.

## Shallow-Pass Trap

A weak implementation could compile and count visual actions but never project them to WebGlRun stages, never validate the run, and never prove deterministic replay. Another weak implementation could make unknown actions wait silently, which would hide incomplete Vernon-Smith-style mappings. SB11 guards both by projecting the large scenario into a strict WebGlRunDocument, validating it, comparing deterministic replay fingerprints, and rerunning the unsupported-action negative proof.

## Positive Proof

`proof/SB11/transcripts/passing-large-generic-webglrun-proof.txt` and `proof/SB11/artifacts/large-generic-webglrun-proof.json` prove that the generic large shared-resource definition produces 15,000 visual actions, 15,000 WebGlRun stages, 10,000 motions, valid strict diagnostics, and a matching replay fingerprint.

## Negative Proof

`proof/SB11/transcripts/passing-unsupported-action-negative-proof.txt` proves unsupported action kinds are rejected by default and only become permissive diagnostics when the test explicitly requests permissive behavior.

## Browser Host Gap

`proof/SB11/transcripts/passing-real-scenario-readiness-probe.txt` proves the real scenario fixtures are headless-ready, while `proof/SB11/transcripts/browser-host-gap-scan.txt` records the explicit missing generated-browser-route actions. SB11 therefore closes command-level projection readiness without claiming browser playback.
