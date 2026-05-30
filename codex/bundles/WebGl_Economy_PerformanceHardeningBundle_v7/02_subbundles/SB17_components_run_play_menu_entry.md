# SB17 - Components Run Play menu entry

## User-requested addition

The `RunPlayback` page exists, but the sandbox menu did not expose it as a first-class route. Add the missing menu entry as part of the Components WebGL sandbox work.

## Required changes

1. Add a `Run Play` navigation link to the WebGL sandbox menu.
2. Add a matching overview/home card so users can discover the page from the first screen.
3. Keep the existing `/run-playback` route and page implementation unchanged unless validation exposes a defect.
4. Validate at large-screen desktop viewports only.

## Validation

- `Run Play` appears in the left navigation.
- The overview page links to `/run-playback`.
- Browser proof for `/run-playback` confirms the WebGL scene renders at a desktop viewport.
