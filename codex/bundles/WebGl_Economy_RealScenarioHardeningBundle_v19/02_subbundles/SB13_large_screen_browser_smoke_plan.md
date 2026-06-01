# SB13 - Large-screen browser smoke plan

Prepare, but do not overbuild, a browser smoke path:

- load one `WebGlRunDocument`,
- render initial scene,
- play one frame's stage sequence,
- wait for object motion barrier,
- export runtime diagnostics,
- compare expected completed stage ids.

Large-screen only: 1440x900 or larger.
No mobile/tablet/small-screen optimization.
