# SB02 - Components: move playback orchestration into WebGlRunLib

Problem: generic playback orchestration currently exists in sandbox code. Sandbox should only demo the library.

Implement in `CanDoItAll.Components.WebGlRunLib`:

- `WebGlRunPlaybackController`
- `WebGlRunPlaybackOptions`
- `WebGlRunPlaybackClock`
- `WebGlRunFrameApplyResult`
- `IWebGlRunFrameApplier`
- `IWebGlRunFrameStore` or in-memory frame source helper

Requirements:

- support play/pause/step/seek/reset;
- support deterministic frame stepping;
- do not reference Razor or JS runtime directly in core contracts;
- expose a method that returns a batch of scene commands/actions to be applied by UI host;
- update sandbox `RunPlayback` to use the controller instead of owning the while-loop.

Performance:

- avoid `Task.Delay` loop inside a Razor page;
- allow host to drive playback via timer or UI loop;
- avoid calling JS interop once per action when a batch can be applied.
