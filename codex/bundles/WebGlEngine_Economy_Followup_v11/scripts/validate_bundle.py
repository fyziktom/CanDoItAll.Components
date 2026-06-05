#!/usr/bin/env python3
from pathlib import Path
import sys
root = Path(__file__).resolve().parents[1]
required = ["README.md", "analysis/01-current-state-after-v10.md", "architecture/01-domain-driver-boundary.md", "plan/01-phase-plan.md", "traceability/01-requirement-traceability.md"]
missing = [p for p in required if not (root/p).exists()]
subs = sorted((root/"subbundles").glob("*/README.md"))
proofs = sorted((root/"proof").glob("SB*/manifest.md"))
if missing:
    print("Missing required files:", missing)
    sys.exit(1)
if len(subs) < 18:
    print(f"Expected at least 18 subbundles, found {len(subs)}")
    sys.exit(1)
if len(proofs) < 18:
    print(f"Expected at least 18 proof manifests, found {len(proofs)}")
    sys.exit(1)
print(f"Bundle validation passed for stage=prepared, profile=initiative, subbundles={len(subs)}")
