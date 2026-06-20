# Preparation self-review

This bundle intentionally remains Components-only. It does not ask Codex to modify Economy.

The key reasoning is that Components must become a stable generic engine before Economy resumes heavy work. The production-line canary is included only to test genericity and future simulator readiness; it must not introduce manufacturing semantics into generic source.

High-risk areas:
- WebGlRunLib package-mode proof may still be false-positive.
- WebGlSceneView refactor can accidentally break lifecycle behavior.
- Runtime idle policy can hide pending work if final render drain is used incorrectly.
- Production-line canary may accidentally introduce domain vocabulary into generic source.
- API approval snapshots can be updated too casually.

Mitigation:
- checkpoint after every cluster of subbundles
- no empty proof
- source/package/API domain-boundary hard gates
- package-mode proof that fails if local project refs are used
- RC validation manifest
