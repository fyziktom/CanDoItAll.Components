#!/usr/bin/env python3
import argparse
from pathlib import Path

REQUIRED_DIRS = [
    "inputs", "analysis", "architecture", "requirements", "plan",
    "subbundles", "proof", "traceability", "shared-prompts", "reviews"
]

def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", default="prepared")
    parser.add_argument("--profile", default="initiative")
    args = parser.parse_args()

    root = Path.cwd()
    errors = []

    for directory in REQUIRED_DIRS:
        if not (root / directory).is_dir():
            errors.append(f"Missing directory: {directory}")

    subbundles = sorted((root / "subbundles").glob("SB*")) if (root / "subbundles").is_dir() else []
    if len(subbundles) != 14:
        errors.append(f"Expected 14 subbundles, found {len(subbundles)}")

    for subbundle in subbundles:
        if not (subbundle / "README.md").is_file():
            errors.append(f"Missing subbundle README: {subbundle}")
        proof_manifest = root / "proof" / subbundle.name / "manifest.md"
        if not proof_manifest.is_file():
            errors.append(f"Missing proof manifest: proof/{subbundle.name}/manifest.md")

    required_files = [
        "README.md",
        "analysis/01-current-state-after-v5.md",
        "analysis/02-economic-simulation-readiness.md",
        "analysis/03-risk-register.md",
        "architecture/01-target-architecture.md",
        "requirements/01-normalized-requirements.md",
        "traceability/01-requirement-traceability.md",
        "shared-prompts/implementation-prompt.md",
        "shared-prompts/qa-prompt.md",
        "reviews/00-preparation-self-review.md",
    ]
    for file_name in required_files:
        path = root / file_name
        if not path.is_file() or path.stat().st_size == 0:
            errors.append(f"Missing or empty required file: {file_name}")

    if errors:
        print("Bundle validation failed:")
        for error in errors:
            print(f"- {error}")
        return 1

    print(f"Bundle validation passed for stage={args.stage}, profile={args.profile}, subbundles={len(subbundles)}")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
