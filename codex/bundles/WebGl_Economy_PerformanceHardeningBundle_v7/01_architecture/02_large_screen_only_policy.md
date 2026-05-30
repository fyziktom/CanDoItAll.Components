# Large-screen only WebGL policy

The WebGL surfaces are desktop/large-screen only.

## Required rule

Codex must not spend implementation time optimizing the WebGL renderer, sandbox, WebGlRunLib demos, or WebGL CSS for small or medium screens.

## Allowed

- Large desktop viewport proof: 1440x900 or larger.
- Optional ultrawide proof: 1920x1080 or larger.
- Basic overflow handling to avoid a broken page on smaller windows.
- Clear warning/banner if viewport is smaller than supported minimum.

## Forbidden in this wave

- Mobile-first layout work.
- Small-screen breakpoint optimization.
- Tablet layout optimization.
- Touch UX tuning beyond what already exists in the generic pointer handlers.
- Collapsing WebGL panels for phone/tablet.
- Spending time on small/medium screenshot proofs.

## Suggested implementation guard

Add a visible but simple unsupported-size warning in `WebGlSandbox` only:

```text
This WebGL sandbox is optimized for desktop/large-screen use.
```

Do not add layout complexity to support that warning.
