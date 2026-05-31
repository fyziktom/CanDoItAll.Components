# SB14 - Bridge Adapter Design and Skeleton Only

## Goal

Prepare the first bridge project without wiring a final demo.

## Proposed project

```text
CanDoItAll.Economy.Simulation.WebGlBridge
```

## References

Allowed:

```text
CanDoItAll.Economy.Simulation.Visualization
CanDoItAll.Economy.Simulation.Abstractions
CanDoItAll.Components.WebGlRunLib
```

Not allowed:

```text
CanDoItAll.Components.WebGlLib internal JS/runtime concepts
CanDoItAll.Economy.Simulation.SimpleAccounts
CanDoItAll.Economy.Simulation.Ledger
```

## Proposed interfaces

```csharp
public interface IEconomyWebGlRunProjector
{
    WebGlRunDocument Project(EconomyVisualRunInput input, EconomyWebGlProjectionOptions options);
}

public interface IEconomyVisualActionWebGlMapper
{
    WebGlRunAction Map(EconomyVisualAction action, EconomyWebGlMappingContext context);
}
```

## This wave

- Add design document and maybe compile-only contracts if safe.
- Do not build final UI/demo.
- Do not make Components depend on Economy.
