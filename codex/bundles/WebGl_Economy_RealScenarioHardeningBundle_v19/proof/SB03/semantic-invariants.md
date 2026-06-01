# SB03 Semantic Invariants

## Invariant ID: SB03.run-document-controller.timeline-runtime-snapshot

Raw note: The generic controller must seek frames, apply frame stages, export runtime snapshot, pause/resume, step forward/back, report current stage/action ids, and stay Economy-free.

Expected behavior: Public generic interfaces expose runtime snapshot export and forward/backward timeline navigation, and tests prove these paths on a realistic staged document.

Disallowed shallow implementation: Only adding constants or DTO fields while leaving the reusable runner unable to step backward or callers unable to access snapshot export through the interface.

Failing-first proof: `bundle://proof/SB03/transcripts/prechange-interface-gap.txt`

Passing proof: `bundle://proof/SB03/transcripts/webglrunlib-tests.txt`

Changed source files and hashes: `bundle://proof/SB03/transcripts/changed-file-hashes.txt`

Production assertions: `bundle://proof/SB03/transcripts/source-assertions.txt`

Red-team negative case: The prechange source scan proves the public runner interface lacked `StepBackwardAsync` and the playback-controller interface lacked `ExportRuntimeSnapshot`.

Downstream dependency check: SB08/SB09 can attach or consume runtime state through generic WebGlRunLib surfaces instead of importing Economy concepts into Components.

Shallow-pass trap: A test that only constructs a `WebGlRunDocument` would not prove timeline navigation or snapshot export.

Adversarial negative proof: The runner backward-step test proves replay and scene reset to frame 0 with traceable source-stage diagnostics.

Semantic positive proof: The controller test proves pause/resume, seek, step forward/back, command-batch stage ids, action ids, provenance, and runtime snapshot export.

Anti-stub audit: `bundle://proof/SB03/transcripts/anti-stub-audit.txt`
