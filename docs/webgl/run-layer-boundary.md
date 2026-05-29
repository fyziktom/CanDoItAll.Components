# WebGL Run Layer Boundary

`CanDoItAll.Components.WebGlLib` owns generic scene rendering contracts only. It can describe a scene layout, assets, variants, import diagnostics, interaction state, command results, proof snapshots, and a serializable `WebGlSceneDocument`.

The future run layer should own simulation concerns such as clocks, replay, persistence providers, domain events, pathfinding, collision, and scenario lifecycle. Candidate future contracts include `WebGlRunSnapshot`, `WebGlRunFrame`, `WebGlRunClock`, `WebGlRunCommand`, `IWebGlRunSceneMapper<TSnapshot>`, and `IWebGlRunPersistenceProvider`.

Scene documents are intentionally storage-neutral. They may preserve a generic scene, runtime options, source, metadata, and content hash, but they must not include economy, process, game-rule, replay-log, or persistence-provider semantics.
