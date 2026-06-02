# Semantic Invariants - SB07

## Status

Completed.

## Invariants

- Scene document import preserves runtime options.
- Browser reset/import uses the document-level runtime options, not only scene model state.

## Adversarial Negative Proof

Tests assert reset import no longer drops continuous render mode or runtime key.

## Semantic Positive Proof

Runtime options import source scan and WebGlRunLib tests pass.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| `WebGlSceneDocument.RuntimeOptions` | WebGlLib/WebGlRunLib | Browser scene runtime | Import/reset | Stripped runtime options regression covered by test |
