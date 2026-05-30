import argparse
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]


PREPARED_REQUIRED = [
    "README.md",
    "bundle.json",
    "00_context/01_review_findings_components.md",
    "00_context/02_review_findings_economy.md",
    "01_architecture/01_shared_well_readiness_probe.md",
    "01_architecture/02_generic_pipeline_contract.md",
    "01_architecture/03_large_screen_only_policy.md",
    "02_subbundles/SB01_cross_repo_inventory_and_branch_guard.md",
    "02_subbundles/SB15_refactoring_gate_and_closure.md",
    "04_validation/validation_commands.md",
    "04_validation/forbidden_reference_policy.md",
    "07_references/source_references.md",
    "08_shared_well_readiness/shared_well_gap_analysis.md",
    "09_performance/performance_risk_register.md",
    "plan/01-phase-plan.md",
    "traceability/01-requirement-traceability.md",
    "reviews/00-bundle-self-review.md",
    "reviews/01-execution-report.md",
]


COMPLETED_REQUIRED = PREPARED_REQUIRED + [
    f"proof/SB{index:02}/manifest.md"
    for index in range(1, 16)
]


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", choices=["prepared", "completed"], default="prepared")
    args = parser.parse_args()

    required = PREPARED_REQUIRED if args.stage == "prepared" else COMPLETED_REQUIRED
    missing = [path for path in required if not (ROOT / path).exists()]
    if missing:
        for path in missing:
            print(f"missing: {path}")
        return 1

    subbundles = sorted((ROOT / "02_subbundles").glob("SB*.md"))
    if len(subbundles) != 15:
        print(f"expected 15 subbundles, found {len(subbundles)}")
        return 1

    report_text = (ROOT / "reviews" / "01-execution-report.md").read_text(encoding="utf-8")
    if args.stage == "completed" and "Pending" in report_text:
        print("execution report still contains Pending markers")
        return 1

    print(f"WebGl_Economy_SharedWellReadinessBundle_v9 {args.stage} validation passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
