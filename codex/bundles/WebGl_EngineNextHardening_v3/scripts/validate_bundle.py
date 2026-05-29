#!/usr/bin/env python3
from __future__ import annotations

import argparse
from pathlib import Path
import sys


ROOT = Path(__file__).resolve().parents[1]
REPO = ROOT.parents[2]
ARTIFACTS = REPO / "artifacts" / "webgl-engine-next-hardening"


PREPARED_REQUIRED = [
    ROOT / "README.md",
    ROOT / "bundle.json",
    ROOT / "01_codex_master_prompt.md",
    ROOT / "03_subbundles" / "SB01_branch_inventory_and_evidence_gate.md",
    ROOT / "03_subbundles" / "SB15_validation_and_final_report.md",
]

COMPLETED_REQUIRED = [
    ARTIFACTS / "inventory" / "current-runtime-inventory.md",
    ARTIFACTS / "inventory" / "current-runtime-line-counts.txt",
    ARTIFACTS / "inventory" / "current-glb-inventory.json",
    ARTIFACTS / "browser" / "browser-validation-summary.md",
    ARTIFACTS / "browser" / "tycoon-village-high-glb.png",
    ARTIFACTS / "browser" / "model-lab-model-high-bounds.png",
    ARTIFACTS / "browser" / "run-playback-step.png",
    ARTIFACTS / "IMPLEMENTATION_REPORT.md",
]


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", choices=["prepared", "completed"], required=True)
    args = parser.parse_args()

    required = list(PREPARED_REQUIRED)
    if args.stage == "completed":
        required.extend(COMPLETED_REQUIRED)

    missing = [path for path in required if not path.exists()]
    if missing:
        for path in missing:
            print(f"missing: {path.relative_to(REPO)}", file=sys.stderr)
        return 1

    print(f"bundle validation passed for stage {args.stage}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
