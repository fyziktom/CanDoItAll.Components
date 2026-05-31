# Semantic invariants SB04

Status: Completed

- Invariant ID: `SB04-MOTION-QUEUE-POLICIES`
- Source raw note: RN-004
- Expected behavior: ordered object motion queues preserve physical start/end continuity and explicit queue policies control whether active and queued motions are appended, replaced, cancelled, or rejected.
- Disallowed shallow implementation: always appending or always replacing motions without preserving explicit policy and queue diagnostics.
- Failing-first test: `bundle://proof/SB04/transcripts/failing-first-motion-queue-policy-contract.txt`
- Passing test: `bundle://proof/SB04/transcripts/motion-queue-audit.txt`; `bundle://proof/SB04/transcripts/webgllib-tests.txt`
- Changed source files: `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/14-webgl-scene-motion.js`, `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/29-webgl-scene-motion-queues.js`, `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/02-webgl-scene-core.js`, `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/08-webgl-scene-proof.js`, `repo://tools/webgllib/audit-motion-queue.cjs`
- Production assertions: `bundle://proof/SB04/source-assertions/motion-queue-source-assertions.txt`
- Red-team negative case: `reject-if-active` refuses to mutate existing active motion state, and missing-object motion fails instead of queueing.
- Downstream dependency check: SB05 and Economy bridge proof may rely on deterministic queue policy and queue diagnostics.
- Shallow-pass trap: a two-motion append test would not catch replace/cancel/reject policy mistakes.
- Adversarial negative proof: failing-first scan proves the explicit policy/queued-ID contract did not exist before SB04.
- Semantic positive proof: targeted audit proves A -> B -> C -> home and all queue policies.
- Anti-stub audit: anti-stub scan found no placeholder markers in changed motion queue files.

## Production Behavior Artifact Matrix

| Artifact | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| Queue policy state transition | Motion enqueue runtime | Runtime motion maps and command result metadata | Evaluated for every motion enqueue before active/queued state changes. | `reject-if-active` audit assertion confirms no mutation on rejected enqueue. |
| Queue diagnostics snapshot | Motion queue diagnostics sync | Diagnostics snapshot and proof snapshot | Updated whenever motions are enqueued, cancelled, cleared, promoted, or completed. | A-B-C-home audit asserts queued IDs and recalculated start positions. |
