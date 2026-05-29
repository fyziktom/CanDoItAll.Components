# SB04 - Generic symbol system

## Status

Completed. Proof: `repo://src/CanDoItAll.Components.WebGlLib/WebGl/Symbols/WebGlStatusSymbol.cs` and `bundle://proof/SB08/browser/webgl-tycoon-village-final-desktop.png`.

## Goal

Add generic "objects above objects" support for tycoon-like visualization.

## Required files

```text
src/CanDoItAll.Components.WebGlLib/WebGl/Symbols/WebGlStatusSymbol.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Symbols/WebGlSymbolAnchor.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Symbols/WebGlSymbolEffect.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Symbols/WebGlSymbolIntensityPolicy.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Symbols/WebGlSymbolPalette.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Symbols/IWebGlSymbolPolicy.cs
src/CanDoItAll.Components.WebGlLib/WebGl/Symbols/DefaultWebGlSymbolPolicy.cs
```

## Symbol requirements

Each scene object may have zero or more status symbols.

A symbol supports:

- logical id
- asset id
- semantic kind
- intensity from 0 to 1
- color
- scale
- height offset
- billboard-to-camera flag
- effect key
- tooltip
- sort order
- visibility
- metadata

## Required generic effects

Support these effect keys:

```text
none
pulse
blink
float
spin
glow
shake
scale-by-intensity
```

Implement only lightweight MVP behavior. Do not overbuild particles unless it is easy and isolated.

## Visual placement

Symbols should appear above the target object, using:

```text
object position + object size height + symbol height offset
```

For multiple symbols, use a small horizontal/arc offset.

## Acceptance criteria

- At least three objects in the sandbox have symbols above them.
- Symbols use different colors/intensities.
- A symbol can billboard to camera.
- A symbol can be included in the deterministic proof snapshot.
