# Performance risk register

| Risk | Location | Why it matters | Mitigation |
| --- | --- | --- | --- |
| Store lookup scans per event | Simple transition engine | O(events * stores) growth | Build indexed state maps by actor/resource/location |
| JS/C# normalizer drift | Components batch normalizers | Production/browser behavior may diverge | Golden fixture parity tests |
| Multiple active motions for same object | WebGL motion runtime | Ordered action sequence can break | Per-object motion queue or run-level serialization |
| Stage wait ignored | JS command batch | Visual timeline inaccurate | WebGlRunLib stage executor owns timing |
| Input pack hash shallow validation | Economy experiment inputs | Results may not be reproducible | Hash recomputation and schema validation |
| Domain-specific generic params | Economy abstractions | Engine overfits shared-well | Resource-scoped generic parameter model |
| Event switch grows indefinitely | Simple transition engine | Hard to maintain and extend | Event handler registry |
| Large JSON hash overhead | Deterministic hash | Slow tests/runs for large experiments | Cache canonical hashes, benchmark |
