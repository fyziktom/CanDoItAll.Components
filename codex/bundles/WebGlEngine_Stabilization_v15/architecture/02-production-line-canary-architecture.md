
# Production-line canary architecture

A future production-line simulator should integrate as a domain package just like Economy:

```text
ProductionLine.Simulation
  -> produces domain frames/events/jobs/station states
ProductionLine.Visualization
  -> maps frames into domain visual frames/actions
ProductionLine.WebGlBridge
  -> owns ProductionLineWebGlRunDomainMappingDriver
  -> maps domain action kinds into WebGlRunActionKinds
CanDoItAll.Components.WebGlRunLib
  -> consumes only generic WebGlRunDocument
CanDoItAll.Components.WebGlLib
  -> renders generic scene/commands
```

The canary in this bundle should be small and generic: a test/sample run document with stations, buffers, conveyors and WIP tokens. It must not create production-line simulation infrastructure in Components.
