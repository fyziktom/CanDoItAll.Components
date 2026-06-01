# SB03 Semantic Invariants

## Invariant ID: SB03-motion-queue-semantics

Raw note owned: queued object motion must be deterministic, sequential, cancellable, and analyzable; clear-all must report affected objects from active and queued motions.

Expected behavior: append motions run sequentially per object with recalculated start transforms, independent objects run in parallel, queued cancellation preserves active motion, clear object cancels active and queued motions, clear-all reports all affected active/queued objects, and deterministic mode generates stable ids.

Shallow-pass trap: a test that only checks active motion completion would miss queued-only affected objects, queued cancellation, and deterministic id stability.

Adversarial negative proof: `bundle://proof/SB03/transcripts/source-assertions-and-hashes.txt` records the HEAD baseline where clear-all only reported active motion objects; `bundle://proof/SB03/transcripts/motion-queue-audit.txt` proves the current clear-all reports active and queued object ids.

Semantic positive proof: `bundle://proof/SB03/transcripts/motion-queue-audit.txt` exercises ordered A-to-B-to-C motion, parallel object motion, cancellation, clear operations, deterministic ids, and diagnostics through the runtime audit module.

Anti-stub audit: `bundle://proof/SB03/transcripts/anti-stub-audit.txt` reports no TODO, NotImplemented, template-only, or fixture-specific production markers in the changed motion runtime surface.

Changed source files and hashes: `bundle://proof/SB03/transcripts/source-assertions-and-hashes.txt`.

Production assertions: `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/14-webgl-scene-motion.js` and `repo://src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/29-webgl-scene-motion-queues.js`.

Red-team negative case: clear-all with one active actor and one queued object fails the old active-only implementation and passes the new audit.

Downstream dependency check: SB04 and SB10 may rely on deterministic queued motion state because SB03 proves queue diagnostics and cancellation behavior.
