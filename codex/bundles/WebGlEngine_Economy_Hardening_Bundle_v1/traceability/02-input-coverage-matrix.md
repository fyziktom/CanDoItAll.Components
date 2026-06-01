# Input Coverage Matrix

| Raw input / observation | Normalized requirements | Owning subbundle | Closure proof |
| --- | --- | --- | --- |
| Keep Components engine generic and usable beyond Economy | REQ-001, REQ-010, REQ-014 | SB07, SB08, SB12 | Boundary audit, dependency scan, minimal WebGlLib sample |
| Economy is first consumer but must remain generic for varied simulations | REQ-012, REQ-013 | SB10, SB11 | Strict bridge tests, scenario provider inventory, large scenario proof |
| Future production-line simulator must reuse same 3D engine | REQ-001, REQ-010, REQ-011 | SB08, SB09 | Generic non-economy run fixture and forbidden-domain scan |
| Potential performance issues in large simulations | REQ-005, REQ-015 | SB04, SB13 | Stress diagnostics and browser proof |
| Texture/resource lifecycle concern | REQ-006, REQ-007 | SB05, SB13 | Multi-instance GLB dispose proof |
| Need ultra-light WebGlLib and higher robust engine layer | REQ-001, REQ-010 | SB07, SB08 | Layering ADR and package dependency proof |
| Need phases with forced refactor while executing | REQ-015 | All subbundles, SB07/SB09/SB12 | Refactor gate entries in manifests |
| Need XLSX references and checklist | REQ-015 | Bundle artifact | Companion workbook |
