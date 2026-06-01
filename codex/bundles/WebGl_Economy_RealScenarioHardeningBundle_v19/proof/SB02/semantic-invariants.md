# SB02 Semantic Invariants

## Invariant ID: SB02.stage-runner.barrier-diagnostics

Raw note: Harden barrier timeout diagnostics, cancellation behavior, wait-for-object-motions missing-object behavior, wait-for-render-idle with forever symbols, wait-for-event manual-step isolation, and bounded journal behavior.

Expected behavior: Stage barriers expose blockers and warnings, release stuck timeout barriers with diagnostics, scope manual-step events to the active barrier only, and reset stale runner/journal state on cancellation.

Disallowed shallow implementation: Add diagnostic fields without changing barrier release semantics or leaving global event state able to release later batches.

Failing-first proof: `bundle://proof/SB02/transcripts/stage-runner-audit-failing-first.txt`

Passing proof: `bundle://proof/SB02/transcripts/stage-runner-audit.txt`; `repo://artifacts/webgl-runtime-stage-runner-hardening-v15/stage-runner/stage-runner-proof.json`

Changed source files and hashes: `bundle://proof/SB02/transcripts/changed-file-hashes.txt`

Production assertions: `bundle://proof/SB02/transcripts/source-assertions.txt`

Red-team negative case: The failing-first audit proves a missing object-motion target previously waited on unrelated motions instead of warning and advancing. The passing audit also proves manual-step cannot be pre-signaled into a later batch.

Downstream dependency check: SB03 can depend on the generic stage runner because stage sequencing, cancellation reset, and manual-step isolation are now audit-backed.

Shallow-pass trap: A queue-count or status-only test could pass while manual-step events still leak through global state.

Adversarial negative proof: Failing-first and passing audit cases cover missing object ids, manual-step reuse across batches, timeout of an unsignaled event barrier, and cancel with stale journal entries.

Semantic positive proof: The audit covers the intended ordered stage flow, object-motion waits, render-idle release under animated symbols, explicit event signal release, bounded journal counters, and scheduler integration.

Anti-stub audit: `bundle://proof/SB02/transcripts/anti-stub-audit.txt`
