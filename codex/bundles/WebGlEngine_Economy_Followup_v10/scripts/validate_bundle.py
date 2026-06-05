#!/usr/bin/env python3
from pathlib import Path
import sys

root = Path(__file__).resolve().parents[1]
required = ['README.md','MANIFEST.json','analysis/01-current-state-after-v9.md','analysis/02-weaknesses-and-remediation.md','architecture/01-target-architecture.md','requirements/01-normalized-requirements.md','traceability/01-requirement-traceability.md']
errors=[]
for rel in required:
    p=root/rel
    if not p.exists() or p.stat().st_size==0:
        errors.append(f'missing-or-empty:{rel}')
subbundles=list((root/'subbundles').glob('*/README.md'))
if len(subbundles)<18:
    errors.append(f'expected-at-least-18-subbundles-found-{len(subbundles)}')
proofs=list((root/'proof').glob('SB*/manifest.md'))
if len(proofs)<18:
    errors.append(f'expected-at-least-18-proof-manifests-found-{len(proofs)}')
if errors:
    print('Bundle validation failed')
    for e in errors:
        print(e)
    sys.exit(1)
print(f'Bundle validation passed for stage=prepared, profile=initiative, subbundles={len(subbundles)}')
