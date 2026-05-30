# Large-screen-only policy

The WebGL surfaces are large-screen / desktop only.

## Rules

- Use proof viewport 1440x900 or larger.
- Do not optimize WebGL for small screens, medium screens, phones, tablets, or touch-first UX.
- Do not add mobile screenshot gates.
- Do not redesign the WebGL surface responsively.
- If needed, add only an unsupported-size warning below a minimum width/height.

## Validation guard

Add/keep an audit rule that fails if WebGL docs or Codex bundles introduce small/medium/mobile/tablet optimization tasks unless they are explicitly marked as forbidden/out-of-scope.
