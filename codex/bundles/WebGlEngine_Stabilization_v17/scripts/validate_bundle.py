import sys
from pathlib import Path

root = Path(__file__).resolve().parents[1]
required = ["README.md", "plan/01-subbundle-index.md", "reviews/00-preparation-self-review.md"]
missing = [p for p in required if not (root / p).exists()]
subbundles = sorted((root / "subbundles").glob("*/README.md"))
proofs = sorted((root / "proof").glob("*/manifest.md"))

if missing:
    print("Missing required files:")
    for item in missing:
        print(f" - {item}")
    sys.exit(1)

if len(subbundles) < 20:
    print(f"Expected at least 20 subbundle/checkpoint README files, found {len(subbundles)}")
    sys.exit(1)

if len(proofs) < len(subbundles):
    print(f"Expected proof manifests for all subbundles/checkpoints, found {len(proofs)} vs {len(subbundles)}")
    sys.exit(1)

print(f"Bundle validation passed for stage=prepared, subbundles={len(subbundles)}")
