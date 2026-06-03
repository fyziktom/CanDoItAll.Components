# SB09 semantic invariants

- The implementation must preserve Components genericity.
- The implementation must not introduce Economy semantics into WebGlLib or WebGlRunLib.
- Runtime proof must demonstrate behavior, not only compilation.
- Required proof artifacts must be non-empty.

## Production Behavior Artifact Matrix

| Behavior | Producer | Consumer | Lifecycle | Negative or parity proof |
| --- | --- | --- | --- | --- |
| Shared playback stage order | `WebGlRunStageOrderingPolicy` | Components runtime paths and Economy bridge paths | Stages are ordered by `StartsAtSeconds`, effective `StageIndex`, effective `OrderIndex`, and `StageId`; negative indexes sort last | `transcripts/source-assertions.txt` and `transcripts/passing-tests.txt` |
| Dynamic object lifecycle validation | Components runner validation and Economy bridge validation | WebGlRun runner and strict Economy bridge validation | Later playback-ordered stages may target objects created by earlier stages; same-stage motion to a newly added object is invalid | `transcripts/failing-first.txt` and `transcripts/passing-tests.txt` |

## Completion assertions

- Components WebGlRunLib exposes a generic public stage ordering policy with no Economy dependency.
- FromFrame, runner validation, runner diagnostics/state, playback identity, playback controller state, Economy validation, and Economy snapshot runtime attachment use the same stage ordering policy.
- Reversed raw input order no longer causes false dynamic-object validation failures when playback order adds the object before motion.
- Same-stage add-and-motion remains invalid in both Components runner behavior and Economy strict validation.
