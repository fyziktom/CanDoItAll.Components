# Raw user request

The user requested a CanDoItAll Workflow Bundle for hardening the generic WebGL/3D engine work currently split across two repositories:

- `CanDoItAll.Components` on branch `webgl-engine`
- `CanDoItAll.Economy` on its current working branch/main line

The bundle must preserve the generic, reusable nature of the engine. `CanDoItAll.Components` must stay a generic visualization/interaction engine that can be used for simple 3D model display, generic scene visualization, and more advanced interaction. `CanDoItAll.Economy` is the first real consumer and should remain generic enough for multiple economic simulations, including larger experimental-economics scenarios inspired by Vernon Smith.

The user explicitly asked to incorporate the architecture observations about:

- keeping `WebGlLib` lightweight;
- putting robust run/simulation concerns in a higher layer;
- hardening performance and patching;
- texture/resource lifecycle safety;
- cross-repo integration;
- browser proof;
- XLSX checklist/reference matrix;
- subbundles with forced refactoring gates before continuing;
- a senior QA inspection before final ZIP creation.
