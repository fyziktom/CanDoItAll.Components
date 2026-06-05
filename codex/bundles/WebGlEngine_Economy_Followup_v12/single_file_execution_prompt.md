# Single-file execution prompt for Codex

You are executing `CanDoItAll_WebGlEngine_Economy_Followup_Hardening_Bundle_v12`.

Work sequentially from SB01 to SB19. Do not skip refactor gates. Use both repositories:

- CanDoItAll.Components / webgl-engine
- CanDoItAll.Economy / main

The highest priority is reducing simulator/runtime/projection noise so economic experiments can be trusted. The second priority is domain neutrality: generic Components/WebGlRunLib must not be tailored to Economy or to current examples.

Before each subbundle:
1. Read its README.
2. Confirm current code.
3. Write a short implementation plan.
4. Make changes.
5. Run focused tests/audits.
6. Store proof under `proof/SBxx`.
7. Update reopen items if incomplete.

Never mark a subbundle complete if proof is missing or zero bytes.
