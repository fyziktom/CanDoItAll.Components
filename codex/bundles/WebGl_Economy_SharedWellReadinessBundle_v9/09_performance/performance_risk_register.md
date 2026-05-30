# Performance risk register

| Risk | Area | Why it matters | Required mitigation |
| --- | --- | --- | --- |
| Motion dedupe drops staged actions | WebGlRun/WebGlLib | Same actor moves multiple times in one sequence | Stage-aware batching |
| C# vs JS batch drift | WebGlLib | Different results in tests vs browser | JSON parity fixtures |
| Link sync scans all links | WebGlLib JS | Dense scenes become slow | Use object-to-link index |
| Full scene rebuilds | WebGlLib | Run playback should apply patches, not rebuild | Batch patches and incremental updates |
| Event mapper O(events * nodes) | Economy Visualization | Many events and nodes slow mapping | Binding dictionary/index |
| Precomputed frame factories | SimpleAccounts | Not scalable to many scenarios | State transition engine |
| Hardcoded materializer switch | SimpleAccounts | Scenario IDs become code branches | Registry-based materializers/handlers |
| Duplicate alias fields | Economy Abstractions | Hash/validation ambiguity | Normalizer + conflict validator |
| Small-screen optimization drift | WebGL docs/Codex | Wastes time and scope | Large-screen-only audit |
