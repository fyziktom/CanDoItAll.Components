# SB03 - WebGlLib/WebGlRunLib package and asset boundary

Ensure the engine package boundary is stable.

Tasks:
- Verify WebGlLib can be consumed alone by a simple viewer.
- Verify WebGlRunLib can be consumed without Economy.
- Ensure sandbox/demo GLB assets are not accidentally packaged as mandatory engine runtime assets.
- Separate demo assets from generic engine package content or document package inclusion explicitly.
- Add package content approval tests.

Required proof:
- WebGlLib-only sample build,
- WebGlRunLib generic sample build,
- NuGet/package file list snapshot,
- asset inclusion/exclusion proof.

