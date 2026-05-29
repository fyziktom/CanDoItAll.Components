# WebGL hardening follow-up execution plan

Hard rule from user: large-screen validation only. Do not spend bundle time tuning small screens.

## Phases

1. Inventory and regression guard.
2. Asset variants and external model alternatives.
3. Runtime refactor into maintainable modules.
4. Drag/move and object command completion.
5. Scene patch, export, and import.
6. Motion interpolation primitive.
7. Render loop and diagnostics hardening.
8. Runtime error handling.
9. Tests, browser proof, and docs.

All phases completed by implementation in `WebGlLib`, `WebGlSandbox`, tests, and `artifacts/webgl-scene-hardening`.
