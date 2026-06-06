#!/usr/bin/env python3
from pathlib import Path
import sys
root = Path(__file__).resolve().parents[1]
required = [
    'README.md','analysis/01-current-state-after-v14.md','analysis/02-main-weaknesses-and-remediation.md',
    'external-research/01-open-source-webgl-engine-comparison.md','production-line-canary/01-production-line-pressure-test.md',
    'plan/01-phase-plan.md','architecture/01-target-components-rc-architecture.md'
]
errors=[]
for rel in required:
    p=root/rel
    if not p.exists() or p.stat().st_size==0:
        errors.append(f'missing-or-empty:{rel}')
subs=list((root/'subbundles').glob('*/README.md'))
if len(subs)<20:
    errors.append(f'expected-at-least-20-subbundle-readmes:found={len(subs)}')
proofs=list((root/'proof').glob('*/manifest.md'))
if len(proofs)<20:
    errors.append(f'expected-at-least-20-proof-manifests:found={len(proofs)}')
for chk in ['CHECKPOINT-A','CHECKPOINT-B','CHECKPOINT-C','CHECKPOINT-D']:
    if not (root/'proof'/chk/'manifest.md').exists():
        errors.append(f'missing-checkpoint-proof:{chk}')
if errors:
    print('\n'.join(errors))
    sys.exit(1)
print(f'Bundle validation passed for stage=prepared, subbundles={len(subs)}')
