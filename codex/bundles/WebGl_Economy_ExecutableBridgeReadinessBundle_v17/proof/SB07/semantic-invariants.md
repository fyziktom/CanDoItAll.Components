# SB07 Semantic Invariants

| Invariant ID | Shallow-pass trap | Adversarial negative proof | Semantic positive proof | Anti-stub audit |
| --- | --- | --- | --- | --- |
| SB07-INV-01 | A session DTO with no navigation behavior could satisfy names only. | Tests pause, seek to a real later step, snapshot that step, analyze it, and resume. | `SimulationSandboxSessionTests` validates current backend frame, visual frame, run frame, stage ids, snapshot, and analysis. | The test uses real fixture input packs and the production workflow, not hand-built mock frames. |
| SB07-INV-02 | A UI-bound service could hide browser dependencies. | Session tests run under `dotnet test` with no Blazor renderer or browser. | Service resides in `SimulationSandbox` and exposes headless C# methods. | Constructor dependencies are workflow and snapshot analysis services only. |
