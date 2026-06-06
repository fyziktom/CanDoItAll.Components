# Target Components stabilization architecture

## Layering

```text
CanDoItAll.Components.WebGlLib
  Generic scene, assets, WebGlSceneView, JS runtime, patches, motions, command batches, runtime idle, diagnostics.

CanDoItAll.Components.WebGlRunLib
  Generic run documents, action plans, timeline, frame/stage contracts, browser apply adapter, domain-driver interface.

Domain packages
  Economy, future production-line simulator, or other consumers.
  Own all domain semantics and mapping drivers.
```

## Freeze target

After this bundle, Components should be considered release-candidate for generic WebGL/Run usage. Future changes should be categorized:

- bugfix
- performance
- documentation
- approval-baseline update
- explicitly approved generic API change
- domain-specific change rejected from Components and moved to a driver/consumer

## Public API freeze assets

Required baselines:

- `WebGlLib` public C# API
- `WebGlRunLib` public C# API
- `WebGlRunActionKinds.All`
- `IWebGlRunDomainMappingDriver` manifest schema
- JS `window.CanDoItAll.webglScene` method/result manifest
- package content for WebGlLib
- package content for WebGlRunLib
- sample package-mode proof
- domain-boundary audit configuration
