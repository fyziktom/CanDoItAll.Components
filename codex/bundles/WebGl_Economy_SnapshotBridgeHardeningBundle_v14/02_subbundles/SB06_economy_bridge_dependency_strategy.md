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
