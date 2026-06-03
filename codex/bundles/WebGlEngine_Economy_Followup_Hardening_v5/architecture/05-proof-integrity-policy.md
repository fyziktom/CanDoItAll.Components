# Proof integrity policy

Completed proof must fail validation when:

- transcript files are empty or whitespace-only,
- browser proof has screenshots without JSON assertions,
- console logs are missing,
- a P0/P1 subbundle lacks failing-first proof,
- package proof does not use a fresh feed or isolated cache,
- final red-team report does not map every requirement to a proof artifact.
