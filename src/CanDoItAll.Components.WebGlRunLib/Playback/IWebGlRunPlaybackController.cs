namespace CanDoItAll.Components.WebGlRunLib;

public interface IWebGlRunPlaybackController
{
    WebGlRunPlaybackState State { get; }

    ValueTask<WebGlRunFrame?> ApplyAsync(WebGlRunPlaybackCommand command, CancellationToken cancellationToken = default);

    ValueTask<WebGlRunPlaybackResult> ApplyDetailedAsync(WebGlRunPlaybackCommand command, CancellationToken cancellationToken = default);

    WebGlRunRuntimeSnapshot ExportRuntimeSnapshot();
}
