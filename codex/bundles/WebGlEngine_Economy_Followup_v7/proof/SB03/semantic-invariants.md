# Semantic invariants - SB03

- This subbundle must preserve generic/domain boundaries.
- Proof must include at least one negative/failing-first check where applicable.
- Artifacts must not be empty placeholders.
- Components remains domain-neutral: command lifecycle states describe render-runtime scheduling only and do not add Economy semantics.
- A successful staged command batch is never ambiguous: normal apply reports `scheduled` when runtime blockers remain, while settled proof apply reports `settled` only after idle blockers clear.
- Browser proof must assert zero active motions, queued motions, queued command stages, and stage barriers after the settled path.
- Failing-first proof is retained in `proof/SB03/transcripts/command-lifecycle-tests-failing-first.txt`; closure proof is retained in `proof/SB03/transcripts/command-lifecycle-tests.txt`.
