# Performance risk register

| Risk | Area | Why it matters | Mitigation |
|---|---|---|---|
| Stage queues grow unbounded | WebGL runtime | Long scenarios may accumulate journal/stage data | bounded journal + diagnostics |
| Run document too large | Bridge | Browser smoke may become slow | measure JSON size and stage counts |
| Snapshot export too heavy | Economy | Many snapshots can slow tests | optional snapshot interval and file store |
| Warning noise hides regressions | Economy build | Existing package warnings can hide new issues | warning budget |
| Fixture mappings incomplete | Bridge | Browser smoke uses fallback instead of real assets | strict mapping profile |
| Generated artifacts dirty repo | Tests | Tests may leave generated files | use temp paths or explicit artifact mode |
