# WebGL large-screen-only policy

The WebGL visualization surface is a desktop/large-screen tool.

Codex must not spend time on:

- mobile responsiveness;
- phone/tablet layouts;
- small or medium viewport screenshots;
- touch/mobile UX optimizations;
- mobile performance budgets.

Allowed work:

- desktop viewport validation at 1440x900 or larger;
- clear unsupported-size warning for too-small viewports;
- no layout polishing for small screens.
