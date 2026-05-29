# SB06 — Components WebGlRunLib foundation

## Goal

Prepare a generic visual run/playback layer without economy semantics.

## Project

Create:

`src/CanDoItAll.Components.WebGlRunLib/CanDoItAll.Components.WebGlRunLib.csproj`

References:
- `CanDoItAll.Components.WebGlLib`
- no Economy references.

## Contracts

Add generic contracts:

- `WebGlRunDocument`
- `WebGlRunManifest`
- `WebGlRunTimeline`
- `WebGlRunFrame`
- `WebGlRunFramePatch`
- `WebGlRunPlaybackState`
- `WebGlRunPlaybackCommand`
- `IWebGlRunFrameSource`
- `IWebGlRunPlaybackController`

## Rules

The project must not contain:
- economy;
- ledger;
- account;
- tax;
- business unit;
- well;
- entrepreneur;
- process;
- agent framework.

Use terms:
- run;
- frame;
- timeline;
- scene;
- object;
- event;
- patch;
- playback.

## Validation

- package/project builds;
- dependency scan proves no Economy references.
