# SB06 - Economy bridge dependency strategy

Goal:
- Keep repo boundaries clean while supporting local development.

Tasks:
1. Keep bridge in Economy repo.
2. Components must never reference Economy.
3. Bridge may reference Components.WebGlRunLib.
4. Replace hardcoded sibling path with a conditional property if feasible:
   - local ProjectReference when `CanDoItAllComponentsRoot` exists,
   - package reference strategy documented for CI/release.
5. Update boundary audit accordingly.

Acceptance:
- Boundary audit passes.
- CI path is documented and not dependent on a developer-specific relative checkout.

## Status

Completed.

## Prerequisites

SB05 bridge projection path and current Economy project layout.

## Validation Depth

Inspect bridge project references, document conditional local/package reference strategy, and run the Economy boundary audit.

## Progression Gate

SB15 may proceed only after dependency proof shows the bridge is the only Economy layer referencing Components WebGlRunLib and no developer-specific checkout path is the sole strategy.
