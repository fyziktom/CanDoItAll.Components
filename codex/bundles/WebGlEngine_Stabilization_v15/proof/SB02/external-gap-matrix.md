# SB02 External Gap Matrix

| External lesson | Generic Components requirement | v15 outcome |
| --- | --- | --- |
| Three.js keeps scene/render primitives generic. | Keep `WebGlLib` as scene/runtime only, with no run or domain semantics. | Domain hard gates passed in `proof/SB18/transcripts/rc-validation-transcript.txt`. |
| PlayCanvas separates asset registry metadata from loaded runtime resources. | Prove state-local asset cache, template/instance ownership, pending disposal, and repeated disposal counters. | `npm run webgllib:test-resource-ownership` passed in the RC transcript. |
| regl treats commands as complete resource operations. | Preserve command batch normalization, lifecycle, idle result metadata, and failure behavior. | Command parity, motion queue, stage runner, and command-batch idle policy audits passed. |
| Babylon.js emphasizes tooling and diagnostics, not only render features. | Freeze diagnostics/proof schema and browser observer output for consumer proof. | Browser observer proof JSON and screenshot captured under `proof/SB16/browser`. |
| Mature engines use versioned APIs. | Freeze C# public APIs, JS facade surface, action kinds, package content, and driver manifest schema. | WebGlLib/WebGlRunLib approval tests passed in the RC transcript. |

No external API was copied. Each finding was converted into a generic stabilization gate.
