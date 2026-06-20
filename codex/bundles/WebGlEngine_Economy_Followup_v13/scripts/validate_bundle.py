#!/usr/bin/env python3
from pathlib import Path
import sys

root = Path(__file__).resolve().parents[1]
required = ["README.md","analysis/01-current-state-after-v12.md","architecture/01-target-architecture.md","requirements/01-normalized-requirements.md","plan/01-phase-plan.md"]
errors=[]
for rel in required:
    p=root/rel
    if not p.exists() or p.stat().st_size == 0:
        errors.append(f"missing-or-empty:{rel}")
subs=sorted((root/"subbundles").glob("*/README.md"))
if len(subs) != 16:
    errors.append(f"expected-16-subbundles-found-{len(subs)}")
for p in subs:
    text=p.read_text(encoding="utf-8")
    if "Required proof" not in text and "Required evidence" not in text:
        errors.append(f"subbundle-missing-proof:{p}")
proofs=sorted((root/"proof_templates").glob("SB*/manifest.md"))
if len(proofs) != 16:
    errors.append(f"expected-16-proof-templates-found-{len(proofs)}")
if errors:
    print("Bundle validation failed:")
    for e in errors:
        print(" -", e)
    sys.exit(1)
print("Bundle validation passed for stage=prepared, subbundles=16")
