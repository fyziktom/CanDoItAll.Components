# Semantic invariants SB02

## Invariants

- SB02-I01: Generic Components public run-action vocabulary must use domain-neutral names for flow visuals.
- SB02-I02: Generic validation must reject unsupported domain-owned action names instead of falling back to wait.
- SB02-I03: Proof artifacts must be non-empty and source-backed.

## Semantic Adequacy Gate

- Shallow-pass trap: keeping the old generic action name while only changing Economy tests.
- Adversarial negative proof: `bundle://proof/SB02/source-scan-components-no-resource-transfer.txt` verifies no `ResourceTransferVisual` or `resource-transfer-visual` remains in Components `src`, `tests`, or `samples`.
- Semantic positive proof: `bundle://proof/SB02/components-webglrun-phase-a-test.txt` includes `Batch_compiler_accepts_generic_directed_flow_visual_as_driver_mapped_noop`, proving the new generic action is accepted and traceable.
- Anti-stub audit: `bundle://proof/SB02/anti-stub-scan.txt`.
- Changed source hashes: `bundle://proof/SB02/changed-file-hashes.txt`.

## Production Behavior Artifact Matrix

| Invariant | Producer | Consumer | Lifecycle | Negative proof |
|---|---|---|---|---|
| SB02-I01 | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Actions/WebGlRunActionKinds.cs` | `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/Actions/WebGlRunActionNormalizer.cs`; `repo://CanDoItAll.Components/src/CanDoItAll.Components.WebGlRunLib/WebGlRunActionCompiler.cs` | `DirectedFlowVisual` is declared, validated, and compiled as a generic directed-flow visual. | `bundle://proof/SB02/source-scan-components-no-resource-transfer.txt` |
