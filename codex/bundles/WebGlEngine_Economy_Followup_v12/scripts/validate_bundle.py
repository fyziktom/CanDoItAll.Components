#!/usr/bin/env python3
from pathlib import Path
import argparse
import sys

REQUIRED_ROOT = [
    "README.md",
    "analysis/01-current-state-after-v11.md",
    "analysis/02-main-weaknesses-and-remediation.md",
    "architecture/01-target-architecture.md",
    "workflow_definition_template.json",
    "inputs/repository-source-references.md",
]

def fail(msg):
    print(f"ERROR: {msg}", file=sys.stderr)
    raise SystemExit(2)

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", default="prepared", choices=["prepared", "completed"])
    parser.add_argument("--root", default=".")
    args = parser.parse_args()
    root = Path(args.root).resolve()
    for rel in REQUIRED_ROOT:
        path = root / rel
        if not path.exists() or path.stat().st_size == 0:
            fail(f"missing or empty required file: {rel}")
    subbundles = sorted((root / "subbundles").glob("*/README.md"))
    if len(subbundles) < 10:
        fail(f"expected at least 10 subbundles, found {len(subbundles)}")
    proof_manifests = sorted((root / "proof").glob("SB*/manifest.md"))
    if len(proof_manifests) != len(subbundles):
        fail(f"proof manifest count {len(proof_manifests)} does not match subbundle count {len(subbundles)}")
    if args.stage == "completed":
        empties = [p for p in (root / "proof").rglob("*") if p.is_file() and p.stat().st_size == 0]
        if empties:
            fail("zero-byte proof files: " + ", ".join(str(p.relative_to(root)) for p in empties[:25]))
    print(f"Bundle validation passed for stage={args.stage}, subbundles={len(subbundles)}")

if __name__ == "__main__":
    main()
