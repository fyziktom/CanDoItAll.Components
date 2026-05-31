# Current review summary

## Components review

The Components repo now has `CanDoItAll.Components.WebGlRunLib` in the solution and a dedicated test project. The WebGL scene command batch model now has batch ordering modes, stage objects, batching policies, stage metrics, and C#/JS normalization paths. This is the right direction.

However, the current runtime still has a key semantic gap: stage normalization and stage objects exist, but `applyCommandBatch` applies root commands and then stage commands immediately. `waitSeconds` is normalized but not honored by the JS batch executor. For actions such as `go-to-well -> use-water -> admin-writing -> return-home`, the temporal order needs to be controlled by `WebGlRunLib`, not by an immediate WebGL batch.

The motion runtime also still replaces existing motions for the same object unless queue mode is append. Even with append, multiple motions for one object are active together rather than a true per-object queue. That means ordered duplicate motions can still interfere unless WebGlRunLib serializes them or WebGlLib grows real motion queues.

## Economy review

The Economy repo now has experiment input packs, placement/parameter documents, deterministic placement generation, scenario normalization, event normalization, event kind registry, a simple transition engine, visual action normalization, and experiment input fixtures.

The major issue is that the generic abstraction layer is not fully generic yet. `SimulationActorParameter.DailyWaterNeed` and `SimulationRuleParameter.MaxDailyDraw` are water/well-specific and should not live in generic simulation abstractions. These should become resource-scoped parameters or semantic quantity requirements.

The simple transition engine is useful but currently remains a limited switch over known event kinds. It also uses repeated linear scans for store resolution and does not yet interpret event effects, rule parameters, expected invariants, or experiment input packs as a unified deterministic run source.
