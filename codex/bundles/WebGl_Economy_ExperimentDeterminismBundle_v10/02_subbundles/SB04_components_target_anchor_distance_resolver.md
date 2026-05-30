# SB04 — Components: target/anchor/distance resolver

## Problem

Visual actions need generic resolution of `object A moves to anchor/use point of object B`, but action logic must not know about wells/farms/markets.

## Tasks

1. Add `WebGlRunTargetResolver`.
2. Support target resolution by:
   - object id
   - anchor key
   - explicit position
   - relative offset
   - fallback anchor policy
3. Expose distance and travel metrics:
   - source position
   - target position
   - distance units
   - estimated duration from speed
4. Add tests:
   - target anchor resolution
   - missing target produces diagnostic not exception
   - home/work/use/admin anchors work generically

## Done criteria

- Shared-well-style target resolution works without economy vocabulary.
