# SB04 proof manifest

Status: completed

## Semantic assertion

`EconomySimulationSandboxPage` no longer applies only `Session.CurrentRunFrame` or tracks browser reset state with a mutable local boolean. Every UI apply path builds an explicit `WebGlRunPlaybackResult` from the run document timeline, marks it `RequiresSceneReset = true`, adds ordered frames from initial frame through the target frame, and applies that result with `WebGlRunBrowserApplyAdapter.ApplyPlaybackAsync`.

## Required proof artifacts

- `transcripts/failing-first.txt`
- `transcripts/passing-component-test.txt`
- `transcripts/component-class-tests.txt`
- `transcripts/source-assertions.txt`
- `transcripts/boundary-audit.txt`
- `transcripts/validator-audits.txt`
- `changed-file-hashes.md`
- `browser/runtime-diagnostics.json`
- `browser/console-review.json`
- `browser/browser-proof-summary.md`

## Results

- Failing-first component test: `transcripts/failing-first.txt` failed on the old implementation because Last applied only frame `2` instead of replaying `0,1,2`.
- Passing focused component test: `transcripts/passing-component-test.txt` passed after the refactor.
- Passing component class: `transcripts/component-class-tests.txt` passed 2/2 tests.
- Node route browser proof: `browser/runtime-diagnostics.json` records Step `0,1`, Last `0,1,2`, First `0`, Apply frame `0`, and Snapshot state with no runtime errors.
- Console review: `browser/console-review.json` contains no captured warning/error/log entries.
- Source assertions: `transcripts/source-assertions.txt` proves `ApplyPlaybackAsync`, replay diagnostics, and absence of `BrowserSceneWasReset`.

## Completion rules

This manifest cannot be marked completed unless all required proof files are non-empty and cite the command, result, and semantic assertion.
