# Single-file execution prompt for Codex

You are working only in `fyziktom/CanDoItAll.Components` on branch `webgl-engine`.

Execute `codex/bundles/components-webgl-engine-stabilization-followup-v14` sequentially. Do not edit `CanDoItAll.Economy`.

Your goal is to stabilize and freeze the generic WebGL/Run engine so future work can move primarily to domain repositories such as Economy.

Rules:

1. Implement subbundles in order.
2. After SB08, stop and perform a refactor/QA gate before proceeding.
3. Preserve public APIs unless a subbundle explicitly asks for an approved API change.
4. Every public API or JS API change must update an approval/baseline artifact.
5. Never add Economy, market, ledger, buyer, seller, investor, equity, production-line, machine, work-order or similar domain semantics to generic Components.
6. If a domain-specific mapping is needed, use `IWebGlRunDomainMappingDriver` or document it as a consumer-domain responsibility.
7. Every subbundle must produce proof artifacts and non-empty transcripts, or explicitly justify why a transcript is intentionally empty.
8. Final output must include a Components release-candidate freeze manifest and a list of deferred issues.
