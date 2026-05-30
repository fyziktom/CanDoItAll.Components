import argparse
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]


PREPARED_REQUIRED = [
    "README.md",
    "bundle.json",
    "00_context/01_current_review_summary.md",
    "00_context/02_components_findings.md",
    "00_context/03_economy_findings.md",
    "01_architecture/01_target_pipeline.md",
    "01_architecture/02_shared_well_readiness_analysis.md",
    "01_architecture/03_large_screen_only_policy.md",
    "02_subbundles/SB01_cross_repo_inventory_branch_large_screen_guard.md",
    "02_subbundles/SB20_refactoring_gate_and_closure.md",
    "04_validation/validation_commands.md",
    "plan/01-phase-plan.md",
    "traceability/01-requirement-traceability.md",
    "reviews/00-bundle-self-review.md",
    "reviews/01-execution-report.md",
]


COMPLETED_REQUIRED = PREPARED_REQUIRED + [
    "proof/SB01/manifest.md",
    "proof/SB02/manifest.md",
    "proof/SB03/manifest.md",
    "proof/SB04/manifest.md",
    "proof/SB05/manifest.md",
    "proof/SB06/manifest.md",
    "proof/SB07/manifest.md",
    "proof/SB08/manifest.md",
    "proof/SB09/manifest.md",
    "proof/SB10/manifest.md",
    "proof/SB11/manifest.md",
    "proof/SB12/manifest.md",
    "proof/SB13/manifest.md",
    "proof/SB14/manifest.md",
    "proof/SB15/manifest.md",
    "proof/SB16/manifest.md",
    "proof/SB17/manifest.md",
    "proof/SB18/manifest.md",
    "proof/SB19/manifest.md",
    "proof/SB20/manifest.md",
]


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", choices=["prepared", "completed"], default="prepared")
    args = parser.parse_args()

    required = PREPARED_REQUIRED if args.stage == "prepared" else COMPLETED_REQUIRED
    missing = [path for path in required if not (ROOT / path).exists()]

    subbundles = sorted((ROOT / "02_subbundles").glob("SB*.md"))
    if len(subbundles) != 20:
        print(f"expected 20 subbundles, found {len(subbundles)}")
        return 1

    if missing:
        for path in missing:
            print(f"missing: {path}")
        return 1

    report = ROOT / "reviews" / "01-execution-report.md"
    report_text = report.read_text(encoding="utf-8")
    if args.stage == "completed" and "Pending" in report_text:
        print("execution report still contains Pending markers")
        return 1

    print(f"WebGl_Economy_SharedWellHardeningBundle_v8 {args.stage} validation passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

