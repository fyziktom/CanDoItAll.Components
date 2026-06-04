#!/usr/bin/env python3
from pathlib import Path
import argparse, sys

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", default="prepared")
    parser.add_argument("--profile", default="initiative")
    args = parser.parse_args()
    root = Path.cwd()
    required_dirs = ["inputs","analysis","requirements","architecture","plan","subbundles","proof","traceability","reviews","scripts"]
    missing = [d for d in required_dirs if not (root/d).exists()]
    if missing:
        print("Missing directories: " + ", ".join(missing))
        return 2
    subbundles = sorted((root/"subbundles").glob("SB*_*"))
    if len(subbundles) != 16:
        print(f"Expected 16 subbundles, found {len(subbundles)}")
        return 3
    bad = []
    for sb in subbundles:
        readme = sb/"README.md"
        if not readme.exists() or readme.stat().st_size < 500:
            bad.append(str(readme))
    if bad:
        print("Invalid subbundle README(s): " + ", ".join(bad))
        return 4
    for i in range(1,17):
        p = root/"proof"/f"SB{i:02d}"/"manifest.md"
        if not p.exists() or p.stat().st_size < 50:
            print(f"Missing proof manifest: {p}")
            return 5
    print(f"Bundle validation passed for stage={args.stage}, profile={args.profile}, subbundles=16")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
