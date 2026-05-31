#!/usr/bin/env python3
from pathlib import Path
import argparse
import sys


ROOT = Path(__file__).resolve().parents[1]
CRITICAL_SUBBUNDLES = {"SB02", "SB03", "SB05", "SB07", "SB08", "SB10", "SB13", "SB14"}


REQUIRED_PREPARED = [
    "README.md",
    "bundle.json",
    "00_context/01_current_review_summary.md",
    "00_context/02_js_runtime_findings.md",
    "00_context/03_simulation_snapshot_gap.md",
    "01_architecture/01_target_layering.md",
    "01_architecture/02_bridge_pipeline.md",
    "01_architecture/03_snapshot_architecture.md",
    "01_architecture/04_genericity_probe_examples.md",
    "03_code_skeletons/SimulationRunSnapshotContracts.cs.md",
    "03_code_skeletons/EconomyWebGlSnapshotBridge_shape.cs.md",
    "03_code_skeletons/WebGlStageRunnerDiagnostics_shape.js.md",
    "04_validation/validation_commands.md",
    "04_validation/forbidden_reference_policy.md",
    "05_spreadsheets/implementation_matrix.xlsx",
    "06_prompts/one_shot_codex_prompt.md",
    "07_references/source_references.md",
    "08_readiness_probes/shared_resource_probe.md",
    "08_readiness_probes/finite_resource_probe.md",
    "09_performance/performance_risk_register.md",
    "10_workflow/workflow_alignment.md",
    "requirements/01-normalized-requirements.md",
    "plan/01-phase-plan.md",
    "analysis/02-assumptions-and-risks.md",
    "traceability/01-raw-note-closure.md",
    "reviews/00-bundle-self-review.md",
    "reviews/01-execution-report.md",
]


REQUIRED_REPORT_SECTIONS = [
    "## Status",
    "## Subbundle Gate Results",
    "## Browser Validation Analytics",
    "## Analytics Review",
    "## Raw Note Closure",
]


REQUIRED_SUBBUNDLE_SECTIONS = [
    "## Status",
    "## Prerequisites",
    "## Validation Depth",
    "## Progression Gate",
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
    if len(subbundles) != 15:
        errors.append(f"Expected 15 subbundles, found {len(subbundles)}")

    for path in subbundles:
        text = read(path)
        if not text.startswith("# SB"):
            errors.append(f"{rel(path)} must start with an SB heading")
        for section in REQUIRED_SUBBUNDLE_SECTIONS:
            if section not in text:
                errors.append(f"{rel(path)} missing section {section}")
        if "Acceptance:" not in text and "Validation:" not in text:
            errors.append(f"{rel(path)} missing acceptance or validation guidance")

    report = ROOT / "reviews/01-execution-report.md"
    if report.exists():
        text = read(report)
        for section in REQUIRED_REPORT_SECTIONS:
            if section not in text:
                errors.append(f"reviews/01-execution-report.md missing section {section}")

    plan = ROOT / "plan/01-phase-plan.md"
    if plan.exists():
        text = read(plan)
        for required in ["## Subbundle Dependency Map", "```mermaid", "## Critical Subbundles", "## Phase Gates"]:
            if required not in text:
                errors.append(f"plan/01-phase-plan.md missing {required}")

    traceability = ROOT / "traceability/01-raw-note-closure.md"
    if traceability.exists():
        text = read(traceability)
        for required in ["Raw note ID", "Normalized requirements", "Owning subbundle", "Planned proof", "Closure"]:
            if required not in text:
                errors.append(f"traceability/01-raw-note-closure.md missing {required}")

    scan_forbidden_instruction_text(errors)


def check_completed(errors: list[str]) -> None:
    report = ROOT / "reviews/01-execution-report.md"
    report_text = read(report) if report.exists() else ""
    for marker in ["| Ready |", "| In progress |", "| Pending |"]:
        if marker in report_text:
            errors.append(f"Execution report still contains {marker.strip()} rows.")

    traceability = ROOT / "traceability/01-raw-note-closure.md"
    traceability_text = read(traceability) if traceability.exists() else ""
    if "| Pending |" in traceability_text:
        errors.append("Raw note closure still contains Pending rows.")

    for index in range(1, 16):
        subbundle_id = f"SB{index:02d}"
        proof_root = ROOT / "proof" / subbundle_id
        manifest = proof_root / "manifest.md"
        if not manifest.exists():
            errors.append(f"Missing proof manifest: {rel(manifest)}")
            continue
        manifest_text = read(manifest)
        if "Status: Completed" not in manifest_text and "Status: Blocked" not in manifest_text:
            errors.append(f"{rel(manifest)} must be Completed or Blocked at final closure")
        if "bundle://" not in manifest_text and "repo://" not in manifest_text:
            errors.append(f"{rel(manifest)} lacks portable proof references")
        if subbundle_id in CRITICAL_SUBBUNDLES:
            semantic = proof_root / "semantic-invariants.md"
            if not semantic.exists():
                errors.append(f"Missing semantic invariant contract: {rel(semantic)}")
            else:
                semantic_text = read(semantic)
                for required in ["Shallow-pass trap", "Adversarial negative proof", "Semantic positive proof", "Anti-stub audit"]:
                    if required not in semantic_text:
                        errors.append(f"{rel(semantic)} missing {required}")

    final_red_team = ROOT / "proof/SB15/final-fake-proof-resistance.md"
    if not final_red_team.exists():
        errors.append(f"Missing final fake-proof resistance artifact: {rel(final_red_team)}")


def scan_forbidden_instruction_text(errors: list[str]) -> None:
    for path in ROOT.rglob("*.md"):
        text = read(path).lower()
        for command in ["git checkout -b", "git switch -c", "git branch <new>"]:
            if command in text and "do not" not in surrounding(text, command):
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
