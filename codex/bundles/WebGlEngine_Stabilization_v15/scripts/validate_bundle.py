#!/usr/bin/env python3
import argparse
import json
from pathlib import Path
import sys

root = Path(__file__).resolve().parents[1]
repo_root = root.parents[2]

parser = argparse.ArgumentParser()
parser.add_argument('--stage', default='prepared', choices=['prepared', 'completed'])
args = parser.parse_args()

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

if args.stage == 'completed':
    completed_artifacts = {
        'SB01': ['changed-file-hashes.txt', 'source-refs.md', 'proof-inventory.md'],
        'SB02': ['external-gap-matrix.md'],
        'SB03': ['api-change-template.md', 'transcripts/focused-dotnet-tests-after-ordinal-approval-update.txt'],
        'SB04': ['js-api-parity-report.md'],
        'CHECKPOINT-A': ['checkpoint-a-report.md'],
        'SB05': ['facade-refactor-report.md'],
        'SB06': ['idle-policy-proof.json', 'transcripts/failing-command-batch-idle-policy.txt', 'transcripts/passing-command-batch-idle-policy.txt'],
        'SB07': ['command-lifecycle-proof.json'],
        'SB08': ['asset-lifecycle-report.md'],
        'CHECKPOINT-B': ['checkpoint-b-report.md'],
        'SB09': ['package-mode-proof.md'],
        'SB10': ['domain-boundary-report.md'],
        'SB11': ['domain-driver-rc-report.md'],
        'SB12': ['production-line-canary-proof.md'],
        'CHECKPOINT-C': ['checkpoint-c-report.md'],
        'SB13': ['interaction-contract-report.md'],
        'SB14': ['large-scene-budget.json'],
        'SB15': ['diagnostics-schema-report.md'],
        'SB16': ['browser/browser-observer-proof.json', 'browser/run-playback.png', 'transcripts/browser-observer-proof-after-reload-fix.txt'],
        'CHECKPOINT-D': ['checkpoint-d-report.md'],
        'SB17': ['consumer-docs-report.md'],
        'SB18': ['rc-validation-report.md', 'transcripts/rc-validation-transcript.txt'],
        'SB19': ['governance-template.md'],
        'SB20': ['final-freeze-signoff.md'],
    }

    for proof_id, rels in completed_artifacts.items():
        manifest = root/'proof'/proof_id/'manifest.md'
        if not manifest.exists() or 'Status: completed' not in manifest.read_text(encoding='utf-8'):
            errors.append(f'proof-not-completed:{proof_id}')
        for rel in rels:
            artifact = root/'proof'/proof_id/rel
            if not artifact.exists() or artifact.stat().st_size == 0:
                errors.append(f'missing-or-empty-proof-artifact:{proof_id}/{rel}')

    execution_report = root/'reviews'/'01-execution-report.md'
    if not execution_report.exists() or execution_report.stat().st_size == 0:
        errors.append('missing-or-empty:reviews/01-execution-report.md')

    browser_proof = root/'proof'/'SB16'/'browser'/'browser-observer-proof.json'
    if browser_proof.exists():
        try:
            browser = json.loads(browser_proof.read_text(encoding='utf-8'))
            assertions = browser.get('assertions', {})
            failed_assertions = [name for name, value in assertions.items() if value is not True]
            if failed_assertions:
                errors.append(f'browser-proof-failed-assertions:{",".join(failed_assertions)}')
            if browser.get('final', {}).get('observer', {}).get('observerProofValid') is not True:
                errors.append('browser-proof-final-observer-invalid')
        except json.JSONDecodeError as exc:
            errors.append(f'browser-proof-invalid-json:{exc}')

    rc_manifest = repo_root/'artifacts'/'webgl-engine-rc-v15'/'artifact-manifest.json'
    if not rc_manifest.exists() or rc_manifest.stat().st_size == 0:
        errors.append('missing-or-empty:artifacts/webgl-engine-rc-v15/artifact-manifest.json')
    else:
        try:
            manifest = json.loads(rc_manifest.read_text(encoding='utf-8'))
            if manifest.get('failed') is not False:
                errors.append('rc-validation-manifest-failed')
            failed_steps = [step.get('name', '(unnamed)') for step in manifest.get('steps', []) if step.get('status') != 'passed']
            if failed_steps:
                errors.append(f'rc-validation-failed-steps:{",".join(failed_steps)}')
        except json.JSONDecodeError as exc:
            errors.append(f'rc-validation-manifest-invalid-json:{exc}')

if errors:
    print('\n'.join(errors))
    sys.exit(1)
print(f'Bundle validation passed for stage={args.stage}, subbundles={len(subs)}')
