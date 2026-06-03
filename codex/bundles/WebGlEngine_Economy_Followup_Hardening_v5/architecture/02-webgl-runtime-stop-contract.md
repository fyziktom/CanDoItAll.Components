# WebGlLib runtime stop contract

The WebGlLib runtime stop operation must:

1. cancel command-stage runner queue and active barrier,
2. clear active and queued motions,
3. update diagnostics,
4. schedule one render to settle state,
5. be idempotent,
6. return a detailed command result.

It must not dispose the scene or reset camera/layout. It is a pause/cancel runtime activity operation, not a full scene teardown.
