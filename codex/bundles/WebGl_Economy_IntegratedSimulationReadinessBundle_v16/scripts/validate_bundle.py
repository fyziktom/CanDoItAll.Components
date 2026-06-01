#!/usr/bin/env python3
from __future__ import annotations

import argparse
import re
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
SUBBUNDLE_IDS = [f"SB{index:02d}" for index in range(1, 16)]
CRITICAL_SUBBUNDLES = {
    "SB03",
    "SB04",
    "SB05",
    "SB06",
    "SB07",
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
    "FILE_INDEX.md",
    "inputs/00-original-request.md",
    "requirements/01-normalized-requirements.md",
    "plan/01-phase-plan.md",
    "traceability/01-requirement-traceability.md",
    "reviews/00-bundle-self-review.md",
    "reviews/01-execution-report.md",
    "00_context/01_current_review_findings.md",
    "00_context/02_connection_readiness_gap_analysis.md",
    "01_architecture/01_target_layering.md",
    "01_architecture/02_snapshot_analysis_architecture.md",
    "01_architecture/03_generic_probe_model.md",
    "01_architecture/04_webgl_js_runtime_policy.md",
    "04_validation/validation_commands.md",
    "04_validation/forbidden_reference_policy.md",
    "07_references/source_references.md",
    "10_workflow/workflow_alignment.md",
]

REPORT_SECTIONS = [
    "## Status",
    "## Subbundle Gate Results",
    "## Browser Validation Analytics",
    "## Command Transcript Index",
    "## Raw Note Closure",
    "## Follow-ups And Blockers",
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
    for relative_path in REQUIRED_PREPARED:
        if not (ROOT / relative_path).exists():
            errors.append(f"Missing required file: {relative_path}")

    readme = read(ROOT / "README.md")
    for marker in [
        "## Validation Summary",
        "Bundle readiness gate:",
        "Execution status:",
        "Final closure gate:",
        "Browser validation analytics:",
    ]:
        if marker not in readme:
            errors.append(f"README.md missing {marker}")

    plan = read(ROOT / "plan/01-phase-plan.md")
    for marker in PLAN_SECTIONS:
        if marker not in plan:
            errors.append(f"plan/01-phase-plan.md missing {marker}")

    report = read(ROOT / "reviews/01-execution-report.md")
    for marker in REPORT_SECTIONS:
        if marker not in report:
            errors.append(f"reviews/01-execution-report.md missing {marker}")

    subbundles = sorted((ROOT / "02_subbundles").glob("SB*.md"))
    if len(subbundles) != len(SUBBUNDLE_IDS):
        errors.append(f"Expected {len(SUBBUNDLE_IDS)} subbundles, found {len(subbundles)}")

    for subbundle_id in SUBBUNDLE_IDS:
        subbundle = find_subbundle(subbundle_id)
        if subbundle is None:
            errors.append(f"Missing subbundle file for {subbundle_id}")
            continue

        text = read(subbundle)
        for marker in ["# "]:
            if marker not in text:
                errors.append(f"{relative(subbundle)} missing {marker}")
        if "## Goal" not in text and "## Required closure artifacts" not in text:
            errors.append(f"{relative(subbundle)} missing ## Goal or closure artifact section")
        if "## Required actions" not in text and "## Required closure artifacts" not in text and "## Required probes" not in text and "## Required test flow" not in text and "## Probe" not in text:
            errors.append(f"{relative(subbundle)} missing required-action style section")

        manifest = ROOT / "proof" / subbundle_id / "manifest.md"
        if not manifest.exists():
            errors.append(f"Missing prepared proof manifest for {subbundle_id}")

    traceability = read(ROOT / "traceability/01-requirement-traceability.md")
    for subbundle_id in SUBBUNDLE_IDS:
        if subbundle_id not in traceability:
            errors.append(f"Traceability map missing {subbundle_id}")

    scan_forbidden_instruction_text(errors)


def check_completed(errors: list[str]) -> None:
    report = read(ROOT / "reviews/01-execution-report.md")
    pending_markers = ["| Not started |", "| In progress |", "| Pending |", "| Prepared |"]
    for marker in pending_markers:
        if marker in report:
            errors.append(f"Execution report still contains {marker.strip()} rows")

    for subbundle_id in SUBBUNDLE_IDS:
        proof_root = ROOT / "proof" / subbundle_id
        manifest = proof_root / "manifest.md"
        manifest_text = read(manifest)
        if "Status: Completed" not in manifest_text and "Status: Blocked" not in manifest_text:
            errors.append(f"{relative(manifest)} must be Completed or Blocked")
        if "repo://" not in manifest_text and "bundle://" not in manifest_text:
            errors.append(f"{relative(manifest)} lacks portable proof references")

        for proof_ref in extract_bundle_refs(manifest_text):
            target = ROOT / proof_ref
            if not target.exists():
                errors.append(f"{relative(manifest)} references missing bundle artifact {proof_ref}")

        if subbundle_id in CRITICAL_SUBBUNDLES:
            semantic = proof_root / "semantic-invariants.md"
            if not semantic.exists():
                errors.append(f"Missing semantic invariant contract: {relative(semantic)}")
                continue

            semantic_text = read(semantic)
            for marker in [
                "Invariant ID",
                "Shallow-pass trap",
                "Adversarial negative proof",
                "Semantic positive proof",
                "Anti-stub audit",
            ]:
                if marker not in semantic_text:
                    errors.append(f"{relative(semantic)} missing {marker}")

    final_red_team = ROOT / "proof" / "SB15" / "final-fake-proof-resistance.md"
    if not final_red_team.exists():
        errors.append(f"Missing final fake-proof resistance artifact: {relative(final_red_team)}")


def scan_forbidden_instruction_text(errors: list[str]) -> None:
    for path in ROOT.rglob("*.md"):
        text = read(path).lower()
        for command in ["git checkout -b", "git switch -c", "git branch <new>"]:
            if command in text and "do not" not in surrounding(text, command):
                errors.append(f"{relative(path)} contains unguarded branch creation text")
        for term in ["small-screen optimization", "mobile optimization", "tablet optimization"]:
            context = surrounding(text, term)
            if term in text and not any(marker in context for marker in ["do not", "large-screen", "desktop", "forbidden", "out of scope", "no webgl", "must not"]):
                errors.append(f"{relative(path)} may add forbidden non-desktop WebGL work")


def extract_bundle_refs(text: str) -> list[str]:
    refs = []
    for match in re.finditer(r"bundle://([^\s)]+)", text):
        refs.append(match.group(1).strip().rstrip("`.,;"))
    return refs


def find_subbundle(subbundle_id: str) -> Path | None:
    matches = sorted((ROOT / "02_subbundles").glob(f"{subbundle_id}_*.md"))
    return matches[0] if matches else None


def read(path: Path) -> str:
    if not path.exists():
        return ""
    return path.read_text(encoding="utf-8", errors="replace")


def surrounding(text: str, needle: str) -> str:
    index = text.find(needle)
    if index < 0:
        return ""
    return text[max(0, index - 500): index + 500]


def relative(path: Path) -> str:
    return str(path.relative_to(ROOT)).replace("\\", "/")


if __name__ == "__main__":
    raise SystemExit(main())
