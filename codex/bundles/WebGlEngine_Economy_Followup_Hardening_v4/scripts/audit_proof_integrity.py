#!/usr/bin/env python3
"""Audit bundle proof artifacts for missing or empty evidence files."""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path


ARTIFACT_RE = re.compile(r"`(bundle://proof/[^`]+|proof/[^`]+)`")
COMPLETED_RE = re.compile(r"^Status:\s*Completed\.?\s*$", re.IGNORECASE | re.MULTILINE)
PROOF_TEXT_SUFFIXES = {
    ".txt",
    ".log",
    ".json",
    ".md",
    ".csv",
    ".xml",
    ".html",
}


def normalize_artifact(root: Path, artifact: str) -> Path:
    if artifact.startswith("bundle://"):
        artifact = artifact.removeprefix("bundle://")

    return root / artifact.replace("/", "\\")


def is_text_proof(path: Path) -> bool:
    return path.suffix.lower() in PROOF_TEXT_SUFFIXES


def manifest_artifacts(root: Path, manifest: Path) -> list[Path]:
    text = manifest.read_text(encoding="utf-8")
    return [normalize_artifact(root, match.group(1)) for match in ARTIFACT_RE.finditer(text)]


def proof_tree_artifacts(proof_root: Path) -> list[Path]:
    if not proof_root.exists():
        return []

    return [
        path
        for path in proof_root.rglob("*")
        if path.is_file() and is_text_proof(path)
    ]


def audit(root: Path, include_prepared: bool, include_uncited: bool) -> tuple[int, list[str]]:
    failures: list[str] = []
    manifests = sorted((root / "proof").glob("SB*/manifest.md"))
    if not manifests:
        failures.append("No proof manifests found under proof/SB*/manifest.md")
        return 1, failures

    for manifest in manifests:
        text = manifest.read_text(encoding="utf-8")
        completed = bool(COMPLETED_RE.search(text))
        if not completed and not include_prepared:
            continue

        for artifact in manifest_artifacts(root, manifest):
            if not artifact.exists():
                failures.append(f"missing manifest artifact: {artifact.relative_to(root)}")
                continue

            if artifact.is_file() and is_text_proof(artifact) and artifact.stat().st_size == 0:
                failures.append(f"empty manifest artifact: {artifact.relative_to(root)}")

    if include_uncited:
        for artifact in proof_tree_artifacts(root / "proof"):
            if artifact.stat().st_size == 0:
                failures.append(f"empty proof-tree artifact: {artifact.relative_to(root)}")

    return (1 if failures else 0), failures


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--bundle-root", default=".", help="Bundle root to audit.")
    parser.add_argument(
        "--include-prepared",
        action="store_true",
        help="Also audit prepared manifests. By default only completed manifests are required.",
    )
    parser.add_argument(
        "--include-uncited",
        action="store_true",
        help="Also fail on empty text/log/json artifacts anywhere under proof/.",
    )
    args = parser.parse_args()

    root = Path(args.bundle_root).resolve()
    exit_code, failures = audit(root, args.include_prepared, args.include_uncited)
    if failures:
        print(f"Proof integrity audit failed for {root}")
        for failure in failures:
            print(f"- {failure}")
        sys.exit(exit_code)

    print(f"Proof integrity audit passed for {root}")


if __name__ == "__main__":
    main()
