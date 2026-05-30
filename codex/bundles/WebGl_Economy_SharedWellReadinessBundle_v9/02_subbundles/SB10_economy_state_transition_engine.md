# SB10 - Economy: simple state transition engine

## Problem
Frames are still largely precomputed by scenario-specific factories.

## Tasks
- Add simple state transition engine:
  - initial stores;
  - event stream;
  - resource flow application;
  - inventory updates;
  - issue generation hooks;
  - relationship updates;
  - frame/delta emission.
- Keep it in SimpleAccounts.
- Keep Abstractions as DTO/contracts only.

## Shared-well readiness
Engine should handle:
- water use reducing well water and/or actor stock;
- actor inventory increasing when water is collected;
- trade moving water/tokens between actors;
- tax/admin event moving tokens/compliance;
- rule violation creating issue/symbol-ready event.
