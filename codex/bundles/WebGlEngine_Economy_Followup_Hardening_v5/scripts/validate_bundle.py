#!/usr/bin/env python3
from pathlib import Path
import argparse, sys
parser = argparse.ArgumentParser()
parser.add_argument('--stage', default='prepared')
parser.add_argument('--profile', default='initiative')
args = parser.parse_args()
root = Path(__file__).resolve().parents[1]
required_dirs = ['inputs','analysis','requirements','architecture','plan','subbundles','proof','traceability','shared-prompts','reviews']
missing = [d for d in required_dirs if not (root/d).exists()]
if missing:
    print('Missing directories:', ', '.join(missing))
    sys.exit(1)
subs = sorted((root/'subbundles').glob('sb*'))
if len(subs) != 12:
    print(f'Expected 12 subbundles, found {len(subs)}')
    sys.exit(1)
for sub in subs:
    if not (sub/'README.md').exists():
        print(f'Missing README: {sub}')
        sys.exit(1)
for i in range(1,13):
    pd = root/'proof'/f'SB{i:02d}'
    if not (pd/'manifest.md').exists() or not (pd/'semantic-invariants.md').exists():
        print(f'Missing proof placeholders for SB{i:02d}')
        sys.exit(1)
print(f'Bundle validation passed for stage={args.stage}, profile={args.profile}, subbundles={len(subs)}')
