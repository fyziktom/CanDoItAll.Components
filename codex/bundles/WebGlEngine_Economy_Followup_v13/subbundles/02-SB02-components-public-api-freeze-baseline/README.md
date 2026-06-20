# SB02 - Components public API and action-kind freeze baseline

Create an approval-test baseline for generic Components public API.

Scope:
- WebGlLib public C# types, JS public `CanDoItAll.webglScene` surface, NuGet package content.
- WebGlRunLib public C# types, `WebGlRunActionKinds.All`, domain-driver contracts.
- Make approval snapshots deterministic and human-reviewable.

Rules:
- No new Economy-specific terms.
- No public API removal without a migration note.
- New API after freeze must fail approval unless intentionally updated.

Required proof:
- approval snapshots committed,
- failing-first proof with a deliberate API/action-kind change,
- passing proof after reverting,
- package content diff.

