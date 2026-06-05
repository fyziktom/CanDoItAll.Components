#!/usr/bin/env python3
from pathlib import Path
import sys

root = Path(__file__).resolve().parents[1]
required = [
    'README.md',
    'inputs/raw-user-request.md',
    'inputs/source-references.md',
    'analysis/01_current_state_after_codex.md',
    'analysis/02_primary_weaknesses_and_remediation.md',
    'architecture/01_components_freeze_target.md',
    'architecture/02_freeze_gates.md',
    'architecture/03_no_economy_modification_rule.md',
    'requirements/01_normalized_requirements.md',
    'plan/01_execution_plan.md',
    'traceability/01_requirement_traceability.md',
    'reviews/01_senior_qa_inspector_preflight.md',
    'workflow_definition_template.json',
    'single_file_execution_prompt.md',
]
missing = [p for p in required if not (root / p).is_file()]
subs = sorted((root / 'subbundles').glob('SB*.md'))
if len(subs) != 16:
    missing.append(f'expected 16 subbundles, found {len(subs)}')
for p in subs:
    text = p.read_text(encoding='utf-8')
    for needle in ['## Purpose', '## Acceptance criteria', '## Required proof artifacts']:
        if needle not in text:
            missing.append(f'{p.relative_to(root)} missing {needle}')
if missing:
    print('Bundle validation failed:')
    for item in missing:
        print(' -', item)
    sys.exit(1)
print(f'Bundle validation passed for stage=prepared, subbundles={len(subs)}')
