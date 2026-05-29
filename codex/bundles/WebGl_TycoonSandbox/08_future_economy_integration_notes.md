# 08 - Future economy integration notes

This bundle intentionally does not implement economy visualization.

The future economy repo should consume `CanDoItAll.Components.WebGlLib` and add a separate layer like:

```text
CanDoItAll.Economy.Visualization
CanDoItAll.Economy.WebGlSandbox
```

Future economy-specific mapper:

```csharp
public interface IEconomySceneMapper
{
    WebGlSceneModel Map(EconomySimulationSnapshot snapshot, EconomySceneMappingOptions options);
}
```

Future economy symbol policies can map domain concepts to generic symbols:

```text
domain value -> SemanticKind + SymbolAssetId + Intensity + Color + EffectKey
```

Example future mapping, not for this bundle:

```text
DebtPressure -> "pressure" symbol
TrustLevel -> "support" symbol
ConflictLevel -> "warning" symbol
CollapseRisk -> "alert" symbol
```

The generic layer should not know these domain meanings.
