# Requirement Traceability

| Requirement | Owning subbundle | Source files or artifacts | Required proof |
|---|---|---|---|
| R01 Branch And Boundary Guard | SB01 | `README.md`, `bundle.json`, `04_validation/forbidden_reference_policy.md` | Branch/commit transcript, dependency scan transcript |
| R02 Components Runtime Hardening | SB02 | `00_context/01_current_review_summary.md`, `02_subbundles/SB02_components_runtime_stage_barrier_hardening.md` | WebGlLib tests or audit transcript, anti-stub audit |
| R03 Components Run Document Controller | SB03 | `02_subbundles/SB03_components_executable_run_document_controller.md`, `03_code_skeletons/SnapshotRuntimeAttachment_shape.md` | WebGlRunLib tests, controller source assertion |
| R04 Components Runtime Audit | SB04 | `02_subbundles/SB04_components_js_runtime_size_and_refactor_gate.md`, `04_validation/forbidden_reference_policy.md` | Runtime audit transcript |
| R05 Economy Bridge Strictness | SB05 | `02_subbundles/SB05_economy_bridge_strict_execution_validation.md` | Negative strict validator tests, source assertion |
| R06 Economy Bridge Refactor | SB06 | `02_subbundles/SB06_economy_bridge_projector_refactoring.md` | Projector tests, source scan |
| R07 Renderer-Neutral Mapping | SB07 | `01_architecture/03_genericity_and_domain_leakage.md`, `04_validation/forbidden_reference_policy.md` | Boundary audit and source scan |
| R08 Headless Real Scenario Runner | SB08 | `02_subbundles/SB08_economy_simulation_sandbox_real_test_runner.md`, `08_readiness_probes/` | Generated artifact folder and transcript |
| R09 Snapshot Runtime Attachment | SB09 | `02_subbundles/SB09_economy_snapshot_runtime_state_attachment.md`, `03_code_skeletons/SnapshotRuntimeAttachment_shape.md` | Snapshot tests and hash proof |
| R10 Snapshot Analysis Services | SB10 | `02_subbundles/SB10_economy_snapshot_analysis_service_hardening.md` | Analyzer tests and domain-term audit |
| R11 Backend Selector And Ledger Readiness | SB11 | `02_subbundles/SB11_economy_backend_selector_and_ledger_readiness.md` | Backend selector tests |
| R12 Readiness Report | SB12 | `02_subbundles/SB12_real_scenario_readiness_probe.md`, `08_readiness_probes/` | `readiness-report.json` for required scenarios |
| R13 Large-Screen Browser Smoke Plan | SB13 | `02_subbundles/SB13_large_screen_browser_smoke_plan.md` | Large-screen-only checklist and optional browser artifact |
| R14 Performance Gates | SB14 | `02_subbundles/SB14_performance_and_scalability_gates.md`, `09_performance/performance_risk_register.md` | Performance transcript with counts, elapsed times, thresholds |
| R15 Final Validation And Closure | SB15 | `04_validation/validation_commands.md`, `02_subbundles/SB15_validation_and_closure.md` | Required command transcripts, final validator, raw-note closure |
