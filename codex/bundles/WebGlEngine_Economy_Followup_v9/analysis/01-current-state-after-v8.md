# Current-state analysis after v8 execution

## Summary verdict

The repositories have moved substantially toward a trustworthy experiment stack. It is now reasonable to use the simulator for exploratory runs, pipeline validation, scenario authoring and visual proof work. It is also becoming reasonable to use the headless runner for strict economic runs when a readiness report passes.

However, the system should not yet automatically claim research-grade conclusions for new scenarios. The remaining risk is no longer one single bug; it is a set of evidence-contract and genericity risks that can still introduce simulator noise or false confidence.

## Previous recommendations status

| Recommendation | Status | Notes |
| --- | --- | --- |
| Runtime stop + idle API | Mostly done | JS and C# APIs exist. Need stronger proof and pause ordering. |
| Pause bug fix | Partial | Current RunPlayback still waits on the C# task before final WebGL stop; visible motion may continue briefly. |
| Command batch settled lifecycle | Mostly done | applyCommandBatchAndWait exists. Need default usage/proof for observer routes. |
| Readiness report bands | Done but needs evidence hardening | Bands exist, but runtime/UI/oracle exercise flags still need artifact-backed validation. |
| ResearchStrict policy | Done | Policy exists and is threaded into loader/backend. |
| Metric/invariant registry | Done but needs no-fallback proof expansion | Registry exists; add broader negative/mutation tests. |
| Behavior expansion profile | Done | Profiles and hashes exist; add lockfile/diff tooling and scenario pack coverage. |
| Golden oracle suite | Partial | Good coded tests exist; external corpus is still needed. |
| Design matrix factor materialization | Mostly done | Materializes effective source and rejects no-effect factors, but binding set is narrow. |
| Third genericity scenario | Missing | No current evidence of a multi-goods exchange/investment/elite scenario pack. |
| Components generic domain leakage | Partial | Boundary guard exists, but production code still hardcodes economy-example forbidden terms. |

## Main risks

1. Browser observer proof can still be self-referential.
2. Pause/stop may not stop visual runtime immediately enough.
3. Readiness flags can overstate proof if not artifact-backed.
4. Generic Components boundary contains economy-specific guard terms.
5. Existing examples do not stress exchange, investment, portfolio and concentration dynamics.
6. Golden oracle proof should live in external files, not only in test code.
7. Factor materialization supports only a limited mutation vocabulary.
8. Browser/WebGL must remain observer evidence, never economic truth.
