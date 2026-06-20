# WebGL Post-Freeze Change Governance

The WebGL engine is treated as a release-candidate substrate. Changes after freeze must be generic, reviewed, and proven against the same gates that protect package consumers.

## Allowed Reasons

- Fix a correctness bug in generic scene rendering, command execution, idle detection, resource disposal, package metadata, or documented public behavior.
- Add an additive generic capability that multiple consumer types can use without domain vocabulary.
- Tighten validation, diagnostics, or proof artifacts without widening public semantics.
- Update dependencies or packaging when package-mode proof remains green.

## Disallowed Reasons

- Add a field, action kind, API method, or package dependency for one consuming domain.
- Let a sample or bridge requirement bypass the domain driver boundary.
- Update approval snapshots without an API change request and migration note.
- Suppress audit findings with broad allowlists or non-expiring exceptions.

## Versioning

- Additive compatible changes may remain on the current minor line with a proof suffix while validating.
- Behavioral or breaking changes need an explicit migration note and version-bump decision.
- Package-mode proof must use a unique proof suffix and isolated restore cache.

## Required Review Packet

Every post-freeze change should include:

- Completed `docs/webgl/api-change-request-template.md` when a frozen surface changes.
- Focused failing-first proof when feasible.
- Updated approval snapshots with rationale.
- Domain-boundary hard-gate transcript.
- Package-mode proof transcript.
- Browser observer proof for runtime-visible behavior.
