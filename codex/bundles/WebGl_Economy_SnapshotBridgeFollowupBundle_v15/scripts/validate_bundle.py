#!/usr/bin/env python3
from __future__ import annotations

import argparse
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
CRITICAL_SUBBUNDLES = {
    "SB03",
    "SB04",
    "SB05",
    "SB06",
    "SB08",
    "SB09",
    "SB10",
    "SB12",
    "SB13",
    "SB14",
    "SB15",
}

REQUIRED_PREPARED = [
    "README.md",
    "bundle.json",
    "inputs/00-original-request.md",
    "inputs/01-source-artifacts.md",
    "inputs/02-structured-input.md",
    "analysis/01-current-state.md",
    "analysis/02-assumptions-and-risks.md",
    "requirements/01-normalized-requirements.md",
    "architecture/01-target-solution.md",
    "plan/01-phase-plan.md",
    "traceability/01-requirement-traceability.md",
    "traceability/01_requirement_map.md",
    "reviews/00-bundle-self-review.md",
    "reviews/01-execution-report.md",
    "00_context/01_current_review_findings.md",
    "00_context/02_bridge_and_snapshot_gap_analysis.md",
    "01_architecture/01_target_layering.md",
    "01_architecture/02_snapshot_architecture.md",
    "01_architecture/03_generic_probe_cases.md",
    "04_validation/validation_commands.md",
    "04_validation/forbidden_reference_policy.md",
    "07_references/source_references.md",
    "10_workflow/workflow_alignment.md",
]

REPORT_SECTIONS = [
    "## Status",
    "## Subbundle Gate Results",
    "## Browser Validation Analytics",
    "## Analytics Review",
    "## Raw Note Closure",
]

SUBBUNDLE_SECTIONS = [
    "## Status",
    "## Goal",
    "## Prerequisites",
    "## Exact Source References",
    "## Dependency Impact",
    "## Validation Depth",
    "## Acceptance Checklist",
    "## Proof Required",
    "## Progression Gate",
]

PLAN_SECTIONS = [
    "## Execution Order",
    "## Subbundle Dependency Map",
    "```mermaid",
    "## Critical Subbundles",
    "## Phase Gates",
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

    readme = read_if_exists(ROOT / "README.md")
    for required in [
        "## Validation Summary",
        "Bundle readiness gate:",
        "Execution status:",
        "Final closure gate:",
        "Browser validation analytics:",
    ]:
        if required not in readme:
            errors.append(f"README.md missing {required}")

    plan = read_if_exists(ROOT / "plan/01-phase-plan.md")
    for required in PLAN_SECTIONS:
        if required not in plan:
            errors.append(f"plan/01-phase-plan.md missing {required}")

    report = read_if_exists(ROOT / "reviews/01-execution-report.md")
    for required in REPORT_SECTIONS:
        if required not in report:
            errors.append(f"reviews/01-execution-report.md missing {required}")

    subbundles = sorted((ROOT / "02_subbundles").glob("SB*.md"))
    if len(subbundles) != 16:
        errors.append(f"Expected 16 subbundles, found {len(subbundles)}")

    for path in subbundles:
        text = read_if_exists(path)
        for required in SUBBUNDLE_SECTIONS:
            if required not in text:
                errors.append(f"{relative(path)} missing {required}")

    scan_forbidden_instruction_text(errors)


def check_completed(errors: list[str]) -> None:
    report = read_if_exists(ROOT / "reviews/01-execution-report.md")
    for marker in ["| Not started |", "| In progress |", "| Pending |"]:
        if marker in report:
            errors.append(f"Execution report still contains {marker.strip()} rows.")

    traceability = read_if_exists(ROOT / "traceability/01-requirement-traceability.md")
    if "| Pending |" in traceability:
        errors.append("Requirement traceability still contains Pending rows.")

    for index in range(1, 17):
        subbundle_id = f"SB{index:02d}"
        proof_root = ROOT / "proof" / subbundle_id
        manifest = proof_root / "manifest.md"
        if not manifest.exists():
            errors.append(f"Missing proof manifest: {relative(manifest)}")
            continue

        manifest_text = read_if_exists(manifest)
        if "Status: Completed" not in manifest_text and "Status: Blocked" not in manifest_text:
            errors.append(f"{relative(manifest)} must be Completed or Blocked at final closure")
        if "bundle://" not in manifest_text and "repo://" not in manifest_text:
            errors.append(f"{relative(manifest)} lacks portable proof references")

        if subbundle_id in CRITICAL_SUBBUNDLES:
            semantic = proof_root / "semantic-invariants.md"
            if not semantic.exists():
                errors.append(f"Missing semantic invariant contract: {relative(semantic)}")
                continue

            semantic_text = read_if_exists(semantic)
            for required in [
                "Invariant ID",
                "Shallow-pass trap",
                "Adversarial negative proof",
                "Semantic positive proof",
                "Anti-stub audit",
            ]:
                if required not in semantic_text:
                    errors.append(f"{relative(semantic)} missing {required}")

    final_red_team = ROOT / "proof/SB16/final-fake-proof-resistance.md"
    if not final_red_team.exists():
        errors.append(f"Missing final fake-proof resistance artifact: {relative(final_red_team)}")


def scan_forbidden_instruction_text(errors: list[str]) -> None:
    for path in ROOT.rglob("*.md"):
        text = read_if_exists(path).lower()
        for command in ["git checkout -b", "git switch -c", "git branch <new>"]:
            if command in text and "do not" not in surrounding(text, command):
                errors.append(f"{relative(path)} contains unguarded branch creation text")
        for term in ["small-screen optimization", "mobile optimization", "tablet optimization"]:
            context = surrounding(text, term)
            if term in text and not any(marker in context for marker in ["do not", "large-screen", "desktop"]):
                errors.append(f"{relative(path)} may add forbidden non-desktop WebGL work")


def read_if_exists(path: Path) -> str:
    if not path.exists():
        return ""
    return path.read_text(encoding="utf-8", errors="replace")


def surrounding(text: str, needle: str) -> str:
    index = text.find(needle)
    if index < 0:
        return ""
    return text[max(0, index - 240): index + 240]


def relative(path: Path) -> str:
    return str(path.relative_to(ROOT)).replace("\\", "/")


if __name__ == "__main__":
    raise SystemExit(main())
