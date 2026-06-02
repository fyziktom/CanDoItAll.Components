#!/usr/bin/env python3
from pathlib import Path
import argparse, sys

REQUIRED_ROOT = ["README.md","inputs","analysis","requirements","architecture","plan","traceability","shared-prompts","subbundles","reviews"]
REQUIRED_SUB = ["## Status","## Objective","## Covered Inputs","## Prerequisites","## Exact Source References","## Scope","## Dependency Impact","## Validation Depth","## Implementation Steps","## Do Not Do","## Acceptance Checklist","## Proof Required","## Browser Validation Logging","## Progression Gate","## Suggested Agent Prompt"]

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", default="prepared")
    parser.add_argument("--profile", default="initiative")
    args = parser.parse_args()
    root = Path.cwd()
    errors = []
    for item in REQUIRED_ROOT:
        if not (root/item).exists():
            errors.append(f"Missing root item: {item}")
    subs = sorted((root/"subbundles").glob("SB*")) if (root/"subbundles").exists() else []
    if not subs:
        errors.append("No subbundles found")
    for sub in subs:
        readme = sub/"README.md"
        if not readme.exists():
            errors.append(f"Missing README: {sub}")
            continue
        text = readme.read_text(encoding="utf-8")
        for section in REQUIRED_SUB:
            if section not in text:
                errors.append(f"{sub.name} missing section {section}")
    plan = root/"plan/01-phase-plan.md"
    if plan.exists():
        text = plan.read_text(encoding="utf-8")
        for marker in ["## Subbundle Dependency Map","## Critical Subbundles","## Phase Gates","```mermaid"]:
            if marker not in text:
                errors.append(f"Plan missing {marker}")
    for sub in subs:
        sb = sub.name.split("-")[0]
        manifest = root/"proof"/sb/"manifest.md"
        if not manifest.exists():
            errors.append(f"Missing proof manifest for {sb}")
    if errors:
        print("Bundle validation failed:")
        for error in errors:
            print(f"- {error}")
        sys.exit(1)
    print(f"Bundle validation passed for stage={args.stage}, profile={args.profile}, subbundles={len(subs)}")

if __name__ == "__main__":
    main()
