# Semantic Invariants - SB06

## Status

Completed.

## Invariants

- Mixed direct and staged WebGlRun frames are blocked before runtime mutation.
- Browser reset failure cannot fall through to batch application.

## Adversarial Negative Proof

WebGlRun tests cover mixed-frame API use, reset without initial scene, import failure, and pre-existing frame errors.

## Semantic Positive Proof

WebGlRunLib release tests pass and boundary audit remains green.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Frame apply result | WebGlRunLib | Browser adapter | Playback | Error result has inert empty command batch |
