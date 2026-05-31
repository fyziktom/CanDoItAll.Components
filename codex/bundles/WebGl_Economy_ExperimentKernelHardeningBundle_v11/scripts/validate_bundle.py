#!/usr/bin/env python3
from pathlib import Path
import argparse
import sys


ROOT = Path(__file__).resolve().parents[1]


REQUIRED_PREPARED = [
    "README.md",
    "bundle.json",
    "00_context/01_review_summary.md",
    "00_context/02_shared_well_probe.md",
    "00_context/03_farmer_land_probe.md",
    "01_architecture/01_target_architecture.md",
    "01_architecture/02_vernon_smith_input_discipline.md",
    "01_architecture/03_large_screen_policy.md",
    "04_validation/validation_commands.md",
    "04_validation/forbidden_reference_policy.md",
    "06_prompts/one_shot_codex_prompt.md",
    "07_references/source_references.md",
    "10_workflow/candoitall_workflow_alignment.md",
    "plan/01-phase-plan.md",
    "requirements/01-normalized-requirements.md",
    "traceability/01-raw-note-closure.md",
    "analysis/02-assumptions-and-risks.md",
    "reviews/00-bundle-self-review.md",
    "reviews/01-execution-report.md",
]


REQUIRED_EXECUTION_REPORT_SECTIONS = [
    "## Status",
    "## Subbundle Gate Results",
    "## Browser Validation Analytics",
    "## Analytics Review",
    "## Raw Note Closure",
]


REQUIRED_SUBBUNDLE_SECTIONS = [
    "## Goal",
    "## Entry gate",
    "## Implementation notes",
    "## Required proof",
    "## Closure checklist",
]


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", choices=["prepared", "completed"], default="prepared")
    args = parser.parse_args()

    errors: list[str] = []
    check_prepared(errors)
    if args.stage == "completed":
        check_completed(errors)

    if errors:
        print("Bundle validation failed:")
        for error in errors:
            print(f"- {error}")
        return 1

    print(f"Bundle validation passed for stage '{args.stage}'.")
    return 0


def check_prepared(errors: list[str]) -> None:
    for relative in REQUIRED_PREPARED:
        if not (ROOT / relative).exists():
            errors.append(f"Missing required file: {relative}")

    subbundles = sorted((ROOT / "02_subbundles").glob("SB*.md"))
    if len(subbundles) != 20:
        errors.append(f"Expected 20 subbundles, found {len(subbundles)}")

    for path in subbundles:
        text = read(path)
        for section in REQUIRED_SUBBUNDLE_SECTIONS:
            if section not in text:
                errors.append(f"{rel(path)} missing section {section}")
        if "Priority: Critical" in text and "Semantic Adequacy Gate" not in text:
            errors.append(f"{rel(path)} missing Semantic Adequacy Gate text for critical subbundle")

    report = ROOT / "reviews/01-execution-report.md"
    if report.exists():
        text = read(report)
        for section in REQUIRED_EXECUTION_REPORT_SECTIONS:
            if section not in text:
                errors.append(f"reviews/01-execution-report.md missing section {section}")

    plan = ROOT / "plan/01-phase-plan.md"
    if plan.exists():
        text = read(plan)
        for required in ["## Subbundle Dependency Map", "```mermaid", "## Critical Subbundles", "## Phase Gates"]:
            if required not in text:
                errors.append(f"plan/01-phase-plan.md missing {required}")

    risks = ROOT / "analysis/02-assumptions-and-risks.md"
    if risks.exists():
        text = read(risks)
        for required in ["## Critical Path Risks", "## Validation Risks", "## Reopen Triggers"]:
            if required not in text:
                errors.append(f"analysis/02-assumptions-and-risks.md missing {required}")

    scan_forbidden_instruction_text(errors)


def check_completed(errors: list[str]) -> None:
    report = ROOT / "reviews/01-execution-report.md"
    text = read(report) if report.exists() else ""
    if "| Ready |" in text or "| In progress |" in text or "| Pending |" in text:
        errors.append("Execution report still contains Ready, In progress, or Pending rows.")

    for manifest in sorted((ROOT / "proof").glob("SB*/manifest.md")):
        text = read(manifest)
        if "Status: Completed" not in text and "Status: Not implemented" not in text and "Status: Blocked" not in text:
            errors.append(f"{rel(manifest)} must be Completed, Not implemented, or Blocked at final closure")
        if "bundle://" not in text and "repo://" not in text:
            errors.append(f"{rel(manifest)} lacks portable proof references")

        semantic = manifest.parent / "semantic-invariants.md"
        if not semantic.exists():
            errors.append(f"Missing semantic invariant contract beside {rel(manifest)}")


def scan_forbidden_instruction_text(errors: list[str]) -> None:
    for path in ROOT.rglob("*.md"):
        text = read(path).lower()
        if "git checkout -b" in text and "do not" not in surrounding(text, "git checkout -b"):
            errors.append(f"{rel(path)} contains unguarded branch creation text")
        if "git switch -c" in text and "do not" not in surrounding(text, "git switch -c"):
            errors.append(f"{rel(path)} contains unguarded branch creation text")
        for term in ["small-screen optimization", "mobile optimization", "tablet optimization"]:
            if term in text and not any(marker in surrounding(text, term) for marker in ["do not", "large-screen", "desktop"]):
                errors.append(f"{rel(path)} may add forbidden non-desktop WebGL work")


def surrounding(text: str, needle: str) -> str:
    index = text.find(needle)
    if index < 0:
        return ""
    return text[max(0, index - 240): index + 240]


def read(path: Path) -> str:
    return path.read_text(encoding="utf-8", errors="ignore")


def rel(path: Path) -> str:
    return str(path.relative_to(ROOT)).replace("\\", "/")


if __name__ == "__main__":
    sys.exit(main())
