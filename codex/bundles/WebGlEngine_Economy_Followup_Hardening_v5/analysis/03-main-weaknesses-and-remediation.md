# Main current weaknesses and remediation strategy

## Playback lifecycle is split across layers

C# playback state, WebGlRun runner state, browser command-stage queue, and JS motion queues are separate. The current design treats cancellation primarily as a C# concern. For real simulations, cancellation must be a cross-layer operation.

Remediation: define a public WebGlLib stop API, map it through `WebGlSceneView`, use it from WebGlRun browser adapters and sandbox UIs, and test it with browser assertions.

## Deterministic replay is correct but can be expensive

Economy UI currently rebuilds replay from the initial scene up to current frame for every apply. This is safe for seek/backward, but expensive for forward play.

Remediation: use incremental apply for contiguous forward stepping and full replay only for seek/back/reset or when the runtime state is unknown.

## Scenario packs are better but still need tamper-proofing

Manifests validate required files and experiment content hash, while pack hash is computed by catalog. The manifest should explicitly bind file hashes and pack hash so the scenario pack is portable and tamper-evident.

## Proof quality is still a risk

Committed proof folders contain many blank transcript files. The next bundle must add a validator that rejects empty proof artifacts and browser proof without JSON assertions.
