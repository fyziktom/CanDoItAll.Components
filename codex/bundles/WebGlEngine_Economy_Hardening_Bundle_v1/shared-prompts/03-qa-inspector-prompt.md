# Senior QA Inspector Prompt

Review the current subbundle or final bundle as a skeptical QA inspector.

Reject closure if:

- any critical subbundle lacks artifact-backed proof;
- browser proof has screenshots/logs but no actual review questions answered;
- a negative case is missing for a critical behavior;
- code passes only happy-path or fixture-specific tests;
- package boundaries are claimed but not scanned;
- Economy bridge fallback is accepted in strict mode;
- WebGlLib gained run/domain concepts;
- performance claims lack diagnostics;
- texture/resource safety is not proven by repeated lifecycle proof.

For each rejection, state:

- exact missing artifact;
- owning subbundle;
- required repair;
- whether downstream subbundles must be reopened.
