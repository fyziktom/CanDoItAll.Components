# SB09 proof manifest

Status: completed

## Semantic assertion

Components WebGlRunLib now exposes a shared `WebGlRunStageOrderingPolicy` and uses it for FromFrame command batches, runtime validation, runner state/diagnostics, deterministic timeline identity, and playback controller state. Economy bridge validation and snapshot runtime attachment consume the same policy. Dynamic objects added by an earlier playback-ordered stage are valid even when input order is reversed, while same-stage add-and-motion remains invalid.

## Required proof artifacts

- `transcripts/failing-first.txt`
- `transcripts/passing-tests.txt`
- `transcripts/source-assertions.txt`
- `transcripts/boundary-audit.txt`
- `changed-file-hashes.md`
- browser artifacts if browser behavior is claimed

## Results

- Failing-first proof: `transcripts/failing-first.txt` records the pre-fix Components runner failure when input order put dynamic-object motion before object creation.
- Passing tests: `transcripts/passing-tests.txt` records 13/13 focused Components runner/controller tests and 18/18 focused Economy strict mapping/snapshot tests passing.
- Source assertions: `transcripts/source-assertions.txt` proves the relevant Components and Economy code paths use `WebGlRunStageOrderingPolicy`, with only structural document validation retaining raw per-stage iteration.
- Boundary audit: `transcripts/boundary-audit.txt` proves Components WebGlRunLib source has no Economy reference and the new policy is generic.
- Validator audits: `transcripts/validator-audits.txt` records the prepared-stage bundle validator and proof-integrity audit passing after SB09 proof was added.
- Changed hashes: `changed-file-hashes.md` records hashes for SB09 production, test, and documentation files.

## Production Behavior Artifact Matrix

| Behavior | Producer | Consumer | Lifecycle | Negative or parity proof |
| --- | --- | --- | --- | --- |
| Shared playback stage order | `WebGlRunStageOrderingPolicy` in Components WebGlRunLib | FromFrame, runtime validator, runner diagnostics/state, playback clock/controller, Economy validator, Economy snapshot builder | A frame's raw stages are ordered by `StartsAtSeconds`, effective `StageIndex`, effective `OrderIndex`, and `StageId`; all runtime/bridge paths consume the ordered sequence | `transcripts/passing-tests.txt` cites `Controller_and_frame_apply_result_use_shared_stage_ordering_policy`; `transcripts/source-assertions.txt` cites all consumer paths |
| Dynamic object lifecycle validation | `WebGlRunFrameExecutionValidator` and `EconomyWebGlRunValidator` | `WebGlRunDocumentRunner`, Economy strict mapping validation | Known object ids start from the initial scene, then object additions/removals from earlier playback-ordered stages are applied before later stage commands are checked | `transcripts/failing-first.txt` records the pre-fix reversed-input failure; `transcripts/passing-tests.txt` cites reversed-input acceptance and same-stage rejection |

## Refactor Gate

- Changed Components files: added `WebGlRunStageOrderingPolicy`; updated `WebGlRunFrameApplyResult`, `WebGlRunFrameExecutionValidator`, `WebGlRunExecutionResultDiagnostics`, `WebGlRunDocumentRunner`, `WebGlRunPlaybackClock`, `WebGlRunPlaybackController`, Components runner/controller tests, and `docs/webgl/run-layer-boundary.md`.
- Changed Economy files: updated `EconomyWebGlRunValidator`, `EconomyWebGlSnapshotVisualStateBuilder`, strict mapping tests, and snapshot visual-state tests.
- Public API changed: yes, additive only. New public type `WebGlRunStageOrderingPolicy` exposes `OrderStages(WebGlRunFrame)` and `OrderStages(IEnumerable<WebGlRunActionStage>)`. Existing callers do not need migration; consumers with their own playback ordering should migrate to this policy for parity.
- Test/build/audit commands: see `transcripts/failing-first.txt`, `transcripts/passing-tests.txt`, `transcripts/source-assertions.txt`, `transcripts/boundary-audit.txt`, and `transcripts/validator-audits.txt`.
- Proof artifact paths: this manifest, `semantic-invariants.md`, `changed-file-hashes.md`, and all files under `transcripts/`.
- Open risks: no known SB09 runtime gap. WebGlRunDocumentValidator still iterates raw stages for structural validation only, which is intentionally order-independent.

## Completion rules

This manifest cannot be marked completed unless all required proof files are non-empty and cite the command, result, and semantic assertion.
