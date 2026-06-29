# SB10 Final Red Team Closure And Transfer Gate

## Status

- `Ready`

## Objective

- Perform final red-team closure for the Canvas/Floating Windows publishing-readiness bundle, reject shallow proof, close every raw note, and prepare the transfer gate.

## Covered Inputs

- RAW01: Reuse the basic-components publishing preparation pattern.
- RAW02: Study the recent bundle system.
- RAW03: Prepare Canvas and floating-window refactor/improvement/hardening/validation work.
- RAW04: Do not do WebGL part yet.
- RAW05: Preserve all functionality.
- RAW06: Make it maintainable, clear, documented, and ready for open source.
- R01-R13.

## Prerequisites

- SB01-SB09 progression gates passed.
- Execution report has subbundle rows, browser analytics rows, raw-note closure rows, and unresolved risk rows filled in.
- Proof manifests and semantic invariants exist for every subbundle.
- Build/test/pack and visual matrix evidence exists or has explicit blocker approval.

## Exact Source References

- bundle://README.md
- bundle://plan/01-phase-plan.md
- bundle://requirements/01-normalized-requirements.md
- bundle://traceability/01-requirement-traceability.md
- bundle://reviews/01-execution-report.md
- bundle://proof
- repo://CanDoItAll.Components.slnx
- repo://src/CanDoItAll.Components.CanvasLib
- repo://src/CanDoItAll.Components.OverlayLib
- repo://src/CanDoItAll.Components.Sandbox
- repo://tests/CanDoItAll.Components.BaseLib.Tests

## Deliverables

- Completed-stage validator transcript.
- Final raw-note closure audit.
- Final proof manifest audit covering every subbundle.
- Fake-proof resistance report.
- Final build/test/pack and visual matrix evidence summary.
- Open-source transfer checklist and follow-up list, with WebGL explicitly separated.
- Bundle status updated to completed only after evidence passes.

## Dependency Impact

- This is the release-readiness decision point for the bundle.
- Weak final closure could pass shallow tests, missing screenshots, package drift, or hidden WebGL scope creep into the open-source preparation stream.

## Validation Depth

- Process-critical closure.
- Completed-stage structural validator, Semantic Adequacy Gate, artifact-backed proof manifest audit, raw-note literal closure, fake-proof resistance, and final transfer readiness.

## Implementation Steps

1. Review every subbundle proof manifest, semantic invariants file, transcript, screenshot set, and execution report row.
2. Run the completed-stage validator and save output to `bundle://reviews/completed-validation.txt`.
3. Audit raw notes RAW01-RAW06 against traceability, subbundle evidence, and final source state.
4. Verify browser analytics include route, viewport, actions, screenshot path, review answer, and reopen decision for every UI phase.
5. Verify build/test/pack and package/API/docs proof from SB09.
6. Run source assertions proving WebGL implementation files were not touched.
7. Create a fake-proof resistance report that lists rejected shallow evidence patterns and why current proof is stronger.
8. Create a transfer checklist with validation commands, docs status, known follow-ups, and explicit WebGL follow-up separation.
9. Update bundle README and execution report to completed only when every gate is honestly satisfied.

## Scope Exceptions

- New implementation changes are allowed only for small closure defects found during audit; otherwise reopen the owning subbundle.
- WebGL work remains a future follow-up, not part of this bundle.

## Do Not Do

- Do not mark the bundle complete with missing screenshots, placeholder proof, unresolved critical findings, or pending validator output.
- Do not hide reopened defects as residual risk.
- Do not edit WebGL files.
- Do not publish packages externally.

## Acceptance Checklist

- Completed-stage validator passes.
- Every raw note has evidence-backed closure.
- Every subbundle has proof manifest and semantic invariants.
- Browser analytics rows are complete for SB05-SB08.
- Package/API/docs readiness proof is complete.
- WebGL exclusion is source-asserted.
- Follow-up list separates WebGL from Canvas/Floating Windows residual items.

## Proof Required

- `bundle://reviews/completed-validation.txt`
- `bundle://proof/SB10/final-proof-audit.md`
- `bundle://proof/SB10/raw-note-closure.md`
- `bundle://proof/SB10/fake-proof-resistance.md`
- `bundle://proof/SB10/webgl-exclusion-source-assertion.txt`
- `bundle://proof/SB10/open-source-transfer-checklist.md`
- Final build/test/pack transcript references.
- Final visual matrix transcript references.
- `bundle://proof/SB10/manifest.md`
- `bundle://proof/SB10/semantic-invariants.md`

## Browser Validation Logging

- N/A for new UI behavior. Audit SB05-SB08 browser analytics and rerun only the route/viewport needed to resolve a disputed screenshot or reopened defect.

## Progression Gate

- Bundle may be marked complete only after completed-stage validation, raw-note closure, proof audit, package/API/docs proof, browser analytics, and WebGL exclusion all pass.
- If any gate fails, reopen the owning subbundle and leave the bundle execution status incomplete.

## Suggested Agent Prompt

```text
Execute SB10 only. Red-team the finished Canvas/Floating Windows bundle, reject shallow proof, run completed-stage validation, close every raw note with evidence, separate WebGL as follow-up, and mark the bundle complete only if all gates pass.
```
