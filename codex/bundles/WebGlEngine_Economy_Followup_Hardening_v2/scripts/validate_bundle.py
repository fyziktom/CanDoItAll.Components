#!/usr/bin/env python3
"""Validate the prepared CanDoItAll follow-up bundle structure."""
from __future__ import annotations

import argparse
import sys
from pathlib import Path

REQUIRED_ROOT = [
    "README.md",
    "inputs/raw-user-request.md",
    "analysis/01-current-state-after-codex.md",
    "analysis/02-critical-findings.md",
    "analysis/03-assumptions-risks-reopen-triggers.md",
    "requirements/01-normalized-requirements.md",
    "architecture/01-target-layering.md",
    "plan/01-phase-plan.md",
    "traceability/01-requirement-traceability.md",
    "reviews/00-preparation-self-review.md",
    "reviews/01-execution-report.md",
]

SUBBUNDLE_SECTIONS = [
    "## Status",
    "## Objective",
    "## Covered Inputs",
    "## Prerequisites",
    "## Exact Source References",
    "## Deliverables",
    "## Dependency Impact",
    "## Validation Depth",
    "## Implementation Steps",
    "## Scope Exceptions",
    "## Do Not Do",
    "## Acceptance Checklist",
    "## Proof Required",
    "## Browser Validation Logging",
    "## Progression Gate",
    "## Suggested Agent Prompt",
]

CRITICAL = {"SB01", "SB02", "SB03", "SB04", "SB05", "SB06", "SB08", "SB09", "SB11", "SB12"}


def read(path: Path) -> str:
    return path.read_text(encoding="utf-8")


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", default="prepared")
    parser.add_argument("--profile", default="initiative")
    args = parser.parse_args()
    root = Path.cwd()
    errors: list[str] = []

    for rel in REQUIRED_ROOT:
        if not (root / rel).exists():
            errors.append(f"Missing required file: {rel}")

    plan = root / "plan/01-phase-plan.md"
    if plan.exists():
        text = read(plan)
        for marker in ["## Subbundle Dependency Map", "## Critical Subbundles", "## Phase Gates", "```mermaid"]:
            if marker not in text:
                errors.append(f"Plan missing marker: {marker}")

    subdirs = sorted((root / "subbundles").glob("SB*-*"))
    if len(subdirs) != 12:
        errors.append(f"Expected 12 subbundles, found {len(subdirs)}")

    for subdir in subdirs:
        readme = subdir / "README.md"
        if not readme.exists():
            errors.append(f"Missing subbundle README: {subdir.name}")
            continue
        text = read(readme)
        for section in SUBBUNDLE_SECTIONS:
            if section not in text:
                errors.append(f"{subdir.name} missing section {section}")
        sb = subdir.name.split("-", 1)[0]
        manifest = root / "proof" / sb / "manifest.md"
        if not manifest.exists():
            errors.append(f"{subdir.name} missing proof manifest")
        if sb in CRITICAL and not (root / "proof" / sb / "semantic-invariants.md").exists():
            errors.append(f"{subdir.name} missing semantic invariants")

    if errors:
        print("Bundle validation failed:")
        for error in errors:
            print(f"- {error}")
        return 1

    print(f"Bundle validation passed for stage={args.stage}, profile={args.profile}, subbundles={len(subdirs)}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
