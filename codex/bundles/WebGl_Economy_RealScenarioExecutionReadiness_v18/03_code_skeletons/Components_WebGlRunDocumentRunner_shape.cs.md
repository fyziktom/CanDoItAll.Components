# Components WebGlRunDocument Runner Shape

```csharp
namespace CanDoItAll.Components.WebGlRunLib;

public interface IWebGlRunDocumentRunner
{
    WebGlRunExecutionResult Load(WebGlRunDocument document);
    WebGlRunExecutionResult Seek(long frameIndex);
    WebGlRunExecutionResult ApplyCurrentFrame();
    WebGlRunExecutionResult StepForward();
    WebGlRunExecutionState State { get; }
}

public sealed class WebGlRunExecutionState
{
    public WebGlRunDocument Document { get; set; } = new();
    public long CurrentFrameIndex { get; set; }
    public List<string> ActiveStageIds { get; set; } = [];
    public List<string> PendingStageIds { get; set; } = [];
    public Dictionary<string, string> Diagnostics { get; set; } = [];
}

public sealed class WebGlRunExecutionResult
{
    public bool Succeeded => Errors.Count == 0;
    public List<string> AppliedStageIds { get; set; } = [];
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
    public Dictionary<string, string> Diagnostics { get; set; } = [];
}
```
