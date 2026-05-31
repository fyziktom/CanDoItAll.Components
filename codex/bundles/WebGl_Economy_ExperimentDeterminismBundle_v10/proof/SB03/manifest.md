# SB03 proof manifest

## Scope

Components action normalization and alias policy.

## Changed files

- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Actions/WebGlRunActionNormalizer.cs`
- `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Planning/WebGlRunActionPlanner.cs`
- `repo://CanDoItAll.Components/tests/CanDoItAll.Components.WebGlRunLib.Tests/WebGlRunActionPlannerTests.cs`

## Proof

- Transcript: `bundle://proof/SB02/transcripts/components-validation.txt`
- Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
- Semantic invariants: `bundle://proof/SB03/semantic-invariants.md`

## Production Behavior Artifact Matrix

| Producer | Consumer | Lifecycle | Negative proof |
| --- | --- | --- | --- |
| `WebGlRunActionNormalizer` canonical action | `WebGlRunActionPlanner` | aliases -> normalized action -> planner output/errors | Missing subject, missing target, unsupported kind tests reject invalid normalized actions. |
