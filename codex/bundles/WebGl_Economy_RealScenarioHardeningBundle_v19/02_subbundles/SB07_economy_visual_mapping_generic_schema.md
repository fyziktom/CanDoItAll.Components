# SB07 - Economy visual mapping generic schema

Codex must ensure visual mapping remains renderer-neutral in abstractions.

Renderer-neutral:

- visual category
- action kind
- pose key
- symbol category
- anchor alias
- relationship category

WebGL-specific:

- concrete WebGL asset id
- WebGL fallback object id
- WebGL symbol asset id
- WebGL anchor key if it is runtime-specific

If WebGL-specific fields remain in abstraction for now, mark them as bridge-bound and add a follow-up note to split them later.
