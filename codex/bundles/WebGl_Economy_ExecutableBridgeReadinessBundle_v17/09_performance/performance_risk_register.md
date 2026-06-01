# Performance risk register

| Risk | Area | Why it matters | Required mitigation |
|---|---|---|---|
| Eager projection of all frames/actions | Economy bridge | Large runs may create huge WebGlRunDocuments | Add lazy/windowed projection option |
| Eager snapshot of every frame | Economy sandbox | Snapshot JSON can explode | Add snapshot policy: selected/current/checkpoint only |
| Repeated LINQ over visual frames/actions | Economy | O(n²) in large runs | Add indexes by step/action/node |
| Stage journal growth | WebGL JS | Long runs can leak memory | Keep bounded journals and expose limits |
| Motion queue starvation | WebGL JS | Stages may never progress if object motion stuck | Add timeout/error policy |
| GLB asset bloat | WebGL runtime | Desktop only still can stall | Asset diagnostics + quality profiles |
| Bridge fallback hides mapping errors | Economy bridge | Demo can look valid while wrong | Strict validation options |
| Snapshot diff misses visual state | Economy snapshot | Debugging visual vs data drift impossible | Diff visual state and relationships |
