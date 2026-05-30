# Large-screen-only policy

WebGL surfaces are desktop/large-screen tools.

Codex must not spend time on:
- mobile layout;
- tablet layout;
- small-screen responsiveness;
- medium-screen breakpoint tuning;
- phone screenshots;
- mobile gesture optimizations.

Allowed:
- a simple unsupported-size warning below a minimum viewport;
- desktop proof viewports of 1440x900, 1600x900, 1920x1080, or larger;
- keyboard/mouse interactions for desktop use.

Validation must fail if new WebGL tasks ask for mobile/tablet/small-screen optimization.
