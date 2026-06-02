#!/usr/bin/env python3
import argparse
from pathlib import Path
import sys

REQUIRED_ROOT = [
    "README.md",
    "inputs/raw-user-request.md",
    "analysis/01-current-state-after-v3.md",
    "requirements/01-normalized-requirements.md",
    "architecture/01-target-playback-contract.md",
    "plan/01-phase-plan.md",
    "traceability/01-requirement-traceability.md",
    "reviews/01-preparation-self-review.md",
]

def fail(message: str) -> None:
    print(f"ERROR: {message}")
    sys.exit(1)

def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", default="prepared")
    parser.add_argument("--profile", default="initiative")
    args = parser.parse_args()

    root = Path.cwd()
    for rel in REQUIRED_ROOT:
        path = root / rel
        if not path.exists() or not path.read_text(encoding="utf-8").strip():
            fail(f"Missing or empty required file: {rel}")

    subbundles = sorted((root / "subbundles").glob("SB*-*/README.md"))
    if len(subbundles) != 12:
        fail(f"Expected 12 subbundle README files, found {len(subbundles)}")

    for readme in subbundles:
        text = readme.read_text(encoding="utf-8")
        for section in ["## Objective", "## Scope", "## Required proof", "## Refactor gate"]:
            if section not in text:
                fail(f"{readme} missing section {section}")

    for index in range(1, 13):
        sb = f"SB{index:02d}"
        manifest = root / "proof" / sb / "manifest.md"
        invariants = root / "proof" / sb / "semantic-invariants.md"
        if not manifest.exists() or not manifest.read_text(encoding="utf-8").strip():
            fail(f"Missing proof manifest for {sb}")
        if not invariants.exists() or not invariants.read_text(encoding="utf-8").strip():
            fail(f"Missing semantic invariants for {sb}")

    print(f"Bundle validation passed for stage={args.stage}, profile={args.profile}, subbundles=12")

if __name__ == "__main__":
    main()
