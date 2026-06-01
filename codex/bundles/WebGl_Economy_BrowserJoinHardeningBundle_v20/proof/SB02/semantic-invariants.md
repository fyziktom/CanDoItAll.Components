# SB02 Semantic Invariants

## Invariant ID

SB02-browser-apply-adapter

## Shallow-pass trap

An adapter could return stage/patch/motion counts without calling the browser runtime or copying live diagnostics.

## Adversarial negative proof

`Adapter_reports_runtime_failure_in_typed_result_and_snapshot` proves a failed runtime command makes the typed result unsuccessful and carries runtime errors into `WebGlRunRuntimeSnapshot`.

## Semantic positive proof

`Adapter_applies_frame_to_runtime_and_returns_counts_and_snapshot` proves reset import, command batch application, stage/patch/motion counts, barrier policy/blockers, active/queued motion IDs, and command journal tail are all produced from a fake runtime call log and diagnostics.

## Anti-stub audit

`bundle://proof/SB02/transcripts/anti-stub-audit.txt` reports no placeholder implementation paths in the adapter or tests.

## Raw-note literal closure

- Generic browser apply adapter: solved for Components with a fakeable browser runtime interface.
- Components must remain Economy-free: source assertion transcript found no Economy references in touched Components WebGL surfaces.
- No browser UI work in Components: adapter is a generic primitive; UI proof is deferred to Economy SB05/SB11.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative-test citation |
|---|---|---|---|---|
| `WebGlRunBrowserApplyResult` | `WebGlRunBrowserApplyAdapter` | Economy page and smoke artifact writer | Produced after every frame/playback apply | Runtime failure test in `bundle://proof/SB02/transcripts/webglrunlib-browser-adapter-tests.txt`. |
| `WebGlRunRuntimeSnapshot` browser state | `WebGlRunBrowserApplyAdapter.BuildSnapshot` | Economy snapshots and browser smoke artifacts | Captured after runtime command batch application | Fake runtime positive test proves state is copied from diagnostics instead of seeded constants. |
