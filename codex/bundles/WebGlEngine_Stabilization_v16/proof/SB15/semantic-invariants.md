# Semantic invariants - SB15

- Components-only scope is preserved.
- Generic source, public API approvals, and package content remain free of Economy and production-line domain terms unless an explicit metadata-backed allowlist applies.
- Allowlists are not silent exceptions; each entry must declare owner, reason, and expiry.
- Bundle/docs allowances do not weaken generic source/package/API hard gates.
- WebGlLib does not reference WebGlRunLib; WebGlRunLib does not reference domain packages.

## Production Behavior Artifact Matrix

| Behavior | Producer | Consumer | Lifecycle | Proof |
|---|---|---|---|---|
| Source hard gate | `domain-boundary-auditor.cjs` | RC wrapper | release-candidate validation | passed row in `repo://artifacts/webgl-engine-rc-v16/validation-summary.md` |
| Public/package hard gates | same auditor | RC wrapper | release-candidate validation | passed rows in same summary |
