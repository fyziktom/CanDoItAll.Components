# Economy WebGL snapshot bridge shape

```csharp
namespace CanDoItAll.Economy.Simulation.WebGlBridge;

public sealed class EconomyWebGlSnapshotAttachmentBuilder
{
    public SimulationSnapshotVisualState Attach(
        EconomyVisualFrame visualFrame,
        WebGlRunDocument runDocument,
        WebGlRunFrame webGlFrame,
        EconomyWebGlMappingContext context,
        IReadOnlyDictionary<string, string> runtimeDiagnostics)
    {
        return new()
        {
            VisualFrameId = visualFrame.FrameId,
            WebGlRunFrameIndex = webGlFrame.Index,
            NodeObjectIds = new Dictionary<string, string>(context.NodeObjectIds, StringComparer.Ordinal),
            ActiveStageIds = [],
            PendingStageIds = [.. webGlFrame.Stages.Select(stage => stage.StageId)],
            RuntimeDiagnostics = new Dictionary<string, string>(runtimeDiagnostics, StringComparer.Ordinal)
        };
    }
}
```
