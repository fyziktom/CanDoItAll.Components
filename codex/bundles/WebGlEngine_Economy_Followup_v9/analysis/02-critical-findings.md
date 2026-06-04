# Critical findings and repair strategy

## F01 — Pause/Stop order is still not ideal: RunPlayback cancels and waits for the C# playback task before issuing WebGL StopRuntimeActivityAsync, so visible scene motion may continue during the wait window.

**Severity:** P0  
**Area:** Components playback/runtime  
**Repo:** CanDoItAll.Components

**Evidence:** RunPlayback.razor.cs StopPlaybackAsync: waits taskToStop.WaitAsync(...) before sceneView.StopRuntimeActivityAsync(...).

**Repair:** Stop JS/WebGL runtime first in a best-effort non-blocking path, then cancel/drain C# playback task, then perform final StopRuntimeActivityAsync(waitForIdle:true) and assert idle diagnostics.

## F02 — RunPlayback observer diagnostics compare runDocument with itself rather than a browser-exported/loaded document hash, so observer proof can become self-referential.

**Severity:** P0  
**Area:** Browser observer proof  
**Repo:** CanDoItAll.Components

**Evidence:** DiagnosticsJson uses WebGlRunObserverProof.Compare(runDocument, runDocument, BuildObserverSnapshot()).

**Repair:** Export browser scene/run observer state and compare expected document hash, loaded scene hash, completed stage ids, final object positions and idle diagnostics from real browser runtime.

## F03 — Readiness can still be elevated by boolean flags such as BrowserRuntimeExercised, UIExercised and OracleProofExercised unless those flags are backed by required artifacts.

**Severity:** P0  
**Area:** Readiness evidence contract  
**Repo:** CanDoItAll.Economy

**Evidence:** BuildExplicitBand accepts bool exercised and lists of errors/warnings; ResearchReady checks exercised/valid bands and zero warnings.

**Repair:** Require artifact citations, artifact hashes and artifact-set validation for runtime/UI/oracle bands before they can count as exercised.

## F04 — Components generic boundary policy contains hardcoded economy-domain examples in ForbiddenDomainTerms. This is useful as a guard, but it also couples generic package code to current domain examples.

**Severity:** P1  
**Area:** Generic boundary  
**Repo:** CanDoItAll.Components

**Evidence:** WebGlRunGenericBoundaryPolicy.ForbiddenDomainTerms includes economy, ledger, market, account, buyer, seller, price, vernon.

**Repair:** Move domain-term lists into configurable audit options/test fixtures. Keep only structural rules in generic Components production code.

## F05 — Runtime idle blocker collection treats scheduled render-loop handles as blockers. A final scheduled render after stop can produce false idle timeouts if not stabilized with a two-tick idle rule.

**Severity:** P1  
**Area:** Runtime idle semantics  
**Repo:** CanDoItAll.Components

**Evidence:** collectRuntimeIdleBlockers adds render-loop:scheduled when state.animationHandle or diagnostics.isRenderLoopActive is set.

**Repair:** Define settled idle as two consecutive idle probes after final render drain, or separate render scheduled from semantic motion/stage work.

## F06 — Design factor materialization is now real, but supported bindings are still narrow: event magnitude, initial store quantity, scenario metadata, behavior profile and seed.

**Severity:** P1  
**Area:** Experiment design  
**Repo:** CanDoItAll.Economy

**Evidence:** ApplyFactorBinding supports scheduled-event-magnitude, resource-quantity, scenario-metadata, behavior-profile and seed.

**Repair:** Add generic JSON-pointer/scenario-patch factor bindings with safe allowlist, plus typed bindings for policies, fees, capacities, relationship strengths and event schedules.

## F07 — Golden oracle coverage exists, but it is still test-code-centric. A research stack should use external corpus files that do not move with implementation refactors.

**Severity:** P1  
**Area:** Golden oracles  
**Repo:** CanDoItAll.Economy

**Evidence:** SimulationEconomicTrustHardeningTests constructs golden cases in code.

**Repair:** Create tests/Fixtures/GoldenOracles/*.json and an oracle runner that produces path-addressed diffs and hash-chain evidence.

## F08 — Existing runtime examples still appear centered on shared-well and farmer-land. No evidence found for a third, structurally different exchange/investment/elite-formation scenario.

**Severity:** P1  
**Area:** Scenario coverage  
**Repo:** CanDoItAll.Economy

**Evidence:** Scenario catalog tests assert shared-well and farmer-land; searches for monopoly/elite/investment/exchange scenario terms did not find a current scenario pack.

**Repair:** Add a third scenario pack for multi-goods exchange, investment flows and concentration/elite formation metrics.

## F09 — The proposed third scenario will stress gaps that existing examples do not: multi-asset portfolios, exchange order matching, investment/loan/equity-like flows, wealth concentration and policy shocks.

**Severity:** P1  
**Area:** Third scenario genericity  
**Repo:** CanDoItAll.Economy + Components

**Evidence:** Current event/metric registries cover several primitives, but not all exchange/investment abstractions as first-class generic scenario concepts.

**Repair:** Implement through generic event/action primitives first; only add new generic core types if the scenario cannot be represented without domain leakage.

## F10 — Headless runner and manifest path is strong, but manifest comparability must explicitly classify volatile vs deterministic artifacts and include observer proofs when browser claims are made.

**Severity:** P2  
**Area:** Headless runner and manifests  
**Repo:** CanDoItAll.Economy

**Evidence:** Headless runner emits artifacts and manifest; docs describe manifest hashes and approved volatile readiness-report hash.

**Repair:** Require manifest diff gates in CI and attach browser observer artifacts only as observer evidence, never as economic truth.
