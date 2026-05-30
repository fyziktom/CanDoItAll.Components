# One-shot Codex prompt

You are working in two already-cloned repositories:

- `CanDoItAll.Components`
- `CanDoItAll.Economy`

Do not create a new branch. Work in the currently checked-out branch in each repo.

Implement the follow-up hardening bundle v8. Keep WebGL desktop/large-screen only. Do not add mobile/tablet/small-screen/medium-screen optimization work for WebGL.

Important boundaries:

- Components stays generic and must not reference Economy.
- Economy stays independent from Components/WebGL.
- Shared-well is an acceptance scenario only; generic engine code must not contain well/water-specific behavior except sample data/tests/docs.

Execution order:

1. Inventory and branch guard.
2. Components ordered action stages and batch semantics.
3. Components action normalizer and target/anchor resolver.
4. Components runtime performance hardening.
5. Economy scenario normalizer and event taxonomy.
6. Economy behavior expansion and shared-well readiness proof.
7. Economy visual action mapper hardening.
8. Performance proofs and closure report.

All source code comments must be in English.
