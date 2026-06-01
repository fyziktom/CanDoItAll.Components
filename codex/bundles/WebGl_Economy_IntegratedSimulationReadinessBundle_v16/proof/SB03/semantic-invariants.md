# SB03 Semantic Invariants

| Invariant ID | Assertion |
|---|---|
| SB03-barrier-policy-semantics | Every supported barrier policy resolves through the canonical stage-barrier module and blocks or releases by its actual condition, not by string presence alone. |
| SB03-motion-queue-semantics | Queued object motions keep the render scheduler active until their object queue and active motion state are drained. |

## Shallow-pass trap

A shallow implementation could accept barrier names but ignore queued motions or unresolved manual/event waits. The audit covers all canonical policies and verifies unresolved event barriers do not wake the render loop.

## Adversarial negative proof

`components-stage-runner-audit.txt` includes the event barrier path where work stays pending without repeated render-loop activation until the event is signaled.

## Semantic positive proof

`components-stage-runner-audit.txt`, `components-motion-queue-audit.txt`, and `components-webglrun-tests.txt` all pass with exit code 0.

## Anti-stub audit

No TypeScript or fixture-specific shortcut was added; barrier behavior is in generic JS/C# WebGL runtime files.

