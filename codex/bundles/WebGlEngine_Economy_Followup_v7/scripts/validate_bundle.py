#!/usr/bin/env python3
from pathlib import Path
import argparse
import sys

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", default="prepared")
    parser.add_argument("--profile", default="initiative")
    args = parser.parse_args()

    root = Path.cwd()
    required_dirs = ["inputs","analysis","requirements","architecture","plan","subbundles","proof","traceability","shared-prompts","reviews"]
    missing = [d for d in required_dirs if not (root/d).exists()]
    if missing:
        print(f"Missing directories: {missing}")
        return 1

    subbundles = sorted((root/"subbundles").glob("sb*-*/README.md"))
    if len(subbundles) != 15:
        print(f"Expected 15 subbundles, found {len(subbundles)}")
        return 1

    for readme in subbundles:
        text = readme.read_text(encoding="utf-8")
        for token in ["## Goal", "## Tasks", "## Acceptance criteria", "## Required proof artifacts", "## Gate"]:
            if token not in text:
                print(f"{readme} missing {token}")
                return 1

    for i in range(1,16):
        proof = root/"proof"/f"SB{i:02d}"/"manifest.md"
        if not proof.exists():
            print(f"Missing proof manifest: {proof}")
            return 1

    print(f"Bundle validation passed for stage={args.stage}, profile={args.profile}, subbundles=15")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
