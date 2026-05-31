# SB02 proof manifest

## Scope

Components ordered action stage contracts.

## Changed files

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Actions/WebGlRunAction.cs`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Actions/WebGlRunStageContracts.cs`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Documents/WebGlRunFrame.cs`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/WebGlRunActionCompiler.cs`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Playback/WebGlRunFrameApplyResult.cs`
- `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunActionCompilerTests.cs`

## Proof

- Transcript: `bundle://proof/SB02/transcripts/components-validation.txt`
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- Semantic invariants: `bundle://proof/SB02/semantic-invariants.md`

## Failing-first / semantic proof

The added compiler test proves a same-frame staged action preserves `StageGroupId` and `CoalescingScope`, and existing same-actor move/admin/return tests prove stage boundaries prevent cross-stage duplicate-motion collapse.

## Production Behavior Artifact Matrix

| Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- |
| `WebGlRunAction.StageGroupId` / `CoalescingScope` | `WebGlRunActionCompiler` and `WebGlRunFrameApplyResult` | Action plan -> run frame stage -> command batch stage | Duplicate same-object motions across stages remain preserved in WebGlRunLib/WebGlLib tests. |
