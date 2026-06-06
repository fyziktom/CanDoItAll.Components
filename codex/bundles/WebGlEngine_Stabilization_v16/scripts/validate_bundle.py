#!/usr/bin/env python3
from __future__ import annotations

import argparse
from pathlib import Path
import sys

REQUIRED_ROOTS = [
    "README.md",
    "MANIFEST.json",
    "analysis/01-current-state-after-v15.md",
    "analysis/02-external-open-source-benchmark.md",
    "analysis/03-production-line-canary.md",
    "architecture/01-target-components-stabilization-architecture.md",
    "architecture/02-production-line-domain-driver-boundary.md",
    "plan/01-phase-plan.md",
    "requirements/01-normalized-requirements.md",
    "traceability/01-requirement-traceability.md",
    "reviews/00-preparation-self-review.md",
]

def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", default="prepared")
    args = parser.parse_args()

    root = Path(__file__).resolve().parents[1]
    missing = [path for path in REQUIRED_ROOTS if not (root / path).exists()]
    subbundles = sorted((root / "subbundles").glob("*/README.md"))
    manifests = sorted((root / "proof").glob("*/manifest.md"))

    errors: list[str] = []
    if missing:
        errors.append("Missing required root files: " + ", ".join(missing))
    if len(subbundles) < 20:
        errors.append(f"Expected at least 20 subbundle/checkpoint README files, found {len(subbundles)}.")
    if len(manifests) < 20:
        errors.append(f"Expected at least 20 proof manifests, found {len(manifests)}.")

    content = "\n".join(path.read_text(encoding="utf-8") for path in subbundles)
    if "CanDoItAll.Economy" in content and "Repository: `CanDoItAll.Components` only." not in content:
        errors.append("Unexpected Economy reference pattern in subbundle content.")

    if errors:
        for error in errors:
            print(f"ERROR: {error}", file=sys.stderr)
        return 1

    print(f"Bundle validation passed for stage={args.stage}, subbundles={len(subbundles)}")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
