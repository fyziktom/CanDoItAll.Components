# Semantic Invariants - SB11

## Status

Completed.

## Invariants

- Package proof uses a unique prerelease version and isolated package caches.
- Package-mode consumers do not accidentally fall back to project references or stale feeds.

## Adversarial Negative Proof

Stale-feed restore fails for `CanDoItAll.Components.WebGlLib` `0.1.0-sb11.20260602.1`.

## Semantic Positive Proof

Fresh-feed package-mode restore/build passes for WebGlLib-only sample, Economy WebGl bridge, and Economy.Components.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| SB11 local package feed | Components pack | Package consumers | Restore/build proof | Stale feed cannot resolve proof version |
