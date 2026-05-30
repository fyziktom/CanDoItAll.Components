# Performance risk register

| Area | Risk | Suggested proof |
| --- | --- | --- |
| Components command batching | Stage-insensitive coalescing can drop ordered motions | staged sequence fixture with repeated object motions |
| Components JS/C# parity | Two batch normalizers drift | shared JSON fixture tested by C# and Node |
| Components link updates | moving many objects updates many links repeatedly | 100 actors / 500 links motion benchmark |
| Components symbol overlays | status symbol animation keeps render loop alive | render idle proof after no active motion/symbol |
| Components asset cache | GLB templates leak or disposed too early | create/dispose repeated scene proof |
| Economy validation | O(n²) reference checks on large input packs | 1000 actors/resources/events validation benchmark |
| Economy event expansion | behavior/rule expansion can explode events | expansion count threshold and diagnostics |
| Economy transition engine | per-step full frame rebuild can be expensive | delta-first transition benchmark |
| Economy visual mapper | binding lookup is list-based | dictionary binding map benchmark |
| Ledger adapter | frame/delta recomputes full projections repeatedly | snapshot diff cache proof |
