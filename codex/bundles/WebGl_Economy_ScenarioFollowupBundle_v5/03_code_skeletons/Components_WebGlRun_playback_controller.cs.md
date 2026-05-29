# Code skeleton: WebGlRunLib playback controller

```csharp
namespace CanDoItAll.Components.WebGlRunLib;

public sealed class WebGlRunPlaybackController(
    IWebGlRunFrameSource frameSource,
    IWebGlRunTimelineValidator timelineValidator)
    : IWebGlRunPlaybackController
{
    public WebGlRunPlaybackState State { get; private set; } = new();

    public async ValueTask<WebGlRunFrame?> ApplyAsync(
        WebGlRunPlaybackCommand command,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        switch (command.Kind)
        {
            case WebGlRunPlaybackCommandKinds.Play:
                State.IsPlaying = true;
                break;
            case WebGlRunPlaybackCommandKinds.Pause:
                State.IsPlaying = false;
                break;
            case WebGlRunPlaybackCommandKinds.Seek:
                if (command.TargetFrameIndex.HasValue)
                {
                    State.CurrentFrameIndex = command.TargetFrameIndex.Value;
                }
                break;
            case WebGlRunPlaybackCommandKinds.Next:
                State.CurrentFrameIndex += 1;
                break;
        }

        return await frameSource
            .GetFrameAsync(State.RunId, State.CurrentFrameIndex, cancellationToken)
            .ConfigureAwait(false);
    }
}
```
