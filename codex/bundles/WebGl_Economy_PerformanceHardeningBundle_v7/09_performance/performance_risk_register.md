# Performance risk register

| Area | Risk | Likelihood | Impact | Mitigation |
| --- | --- | --- | --- | --- |
| WebGL link updates | Moving many agents scans all links | High | High | Index link groups by object id |
| Command batching | Sequential movements coalesced incorrectly | Medium | High | Preserve ordered batch mode |
| JS/C# normalizers | Rules drift between runtime and C# tests | Medium | Medium | Golden parity tests |
| Asset cache | Templates not disposed on lifecycle dispose | Medium | High | Call `disposeAssetCache` and verify counters |
| Model diagnostics | Bounds/material traversal repeated | Medium | Medium | Cache diagnostics per template URI |
| Render scheduler | Continuous render caused by static symbols | Medium | High | Count animated symbols only |
| Economy frames | Full frame materialization every step | High | Medium | Event stream + minimal deltas |
| Economy ledger adapter | Delta returns all stores | High | Medium | True diff by store id |
| Economy scenario definitions | Hardcoded seeds block generalization | High | High | JSON scenario definitions and validators |
| Visualization | Static frame map cannot express temporal actions | High | High | EconomyVisualAction stream |
