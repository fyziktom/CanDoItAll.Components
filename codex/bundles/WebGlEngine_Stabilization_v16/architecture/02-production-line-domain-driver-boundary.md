# Production-line domain-driver boundary

## Components-owned generic surface

Components may expose:

- `WebGlSceneObject`
- `WebGlSceneLink`
- `WebGlStatusSymbol`
- `WebGlSceneLayer`
- `WebGlScenePatch`
- `WebGlObjectMotionCommand`
- `WebGlSceneCommandBatch`
- `WebGlRunDocument`
- `WebGlRunFrame`
- `WebGlRunAction`
- `WebGlRunActionKinds.DirectedFlowVisual`
- `IWebGlRunDomainMappingDriver`

## Production-line driver-owned surface

A future domain driver may expose:

- `ProductionLineRunDomainMappingDriver`
- station/machine/work-order/WIP action names
- station-to-object mapping
- WIP-to-token mapping
- queue-to-layer mapping
- alarm-to-symbol mapping
- throughput overlays
- domain trace-map artifact

This must live outside generic Components source.

## Canary expectation

The canary should be a small generic sample in Components that proves the engine supports the shape of a production-line visualization without production-line semantics in the engine. Use neutral ids like `node.input`, `node.processor-a`, `node.output`, `token.a`, `route.main`, and `status.warning`.
