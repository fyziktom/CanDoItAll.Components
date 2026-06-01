# SB03 Semantic Invariants

## Invariant ID

SB03-stage-barrier-sequencing

## Shallow-pass trap

The runtime could expose journal entries while allowing queued stages to execute before motion, render-idle, or event barriers release.

## Adversarial negative proof

`assertUnknownPolicyDiagnostics` in `bundle://proof/SB03/transcripts/stage-runner-audit.txt` proves an unknown barrier policy is visible as `unknown-policy:<policy>` and journaled instead of silently passing.

## Semantic positive proof

`assertWaitForSameObjectMotionSequence` proves a two-stage same-object motion sequence waits until the first actor motion clears before the second stage applies.

## Anti-stub audit

`bundle://proof/SB03/transcripts/anti-stub-audit.txt` confirms no placeholder code in the stage barrier runtime or audit changes.

## Raw-note literal closure

- wait-for-active-motions: covered by audit.
- wait-for-object-motions: covered by audit, including same-object sequence.
- wait-for-render-idle: covered by audit.
- wait-for-event/manual-step: covered by audit.
- unknown barrier policy: explicit warning no-op is implemented and tested.
