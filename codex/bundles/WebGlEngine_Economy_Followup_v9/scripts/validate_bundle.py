#!/usr/bin/env python3
"""Validate the prepared follow-up bundle structure."""
from __future__ import annotations
from pathlib import Path
import sys

root = Path(__file__).resolve().parents[1]
required = [
    'README.md',
    'inputs/raw-user-request.md',
    'inputs/repository-source-references.md',
    'analysis/01-current-state-after-v8.md',
    'analysis/02-critical-findings.md',
    'architecture/01-target-architecture.md',
    'architecture/02-third-scenario-exchange-investment-elite.md',
    'plan/01-phase-plan.md',
    'reviews/01-senior-qa-inspector-final-check.md',
    'traceability/01-requirement-traceability.md',
    'CanDoItAll_WebGlEngine_Economy_Followup_v9_Checklists.xlsx',
]
missing = [path for path in required if not (root / path).exists()]
subbundles = sorted((root / 'subbundles').glob('SB*_*/README.md'))
if missing:
    print('Missing required files:')
    for path in missing:
        print(f' - {path}')
    sys.exit(1)
if len(subbundles) != 17:
    print(f'Expected 17 subbundle README files, found {len(subbundles)}')
    sys.exit(2)
empty = [str(path.relative_to(root)) for path in root.rglob('*') if path.is_file() and path.stat().st_size == 0]
if empty:
    print('Empty files are not allowed in prepared bundle:')
    for path in empty:
        print(f' - {path}')
    sys.exit(3)
print('Bundle validation passed for stage=prepared, profile=initiative, subbundles=17')
