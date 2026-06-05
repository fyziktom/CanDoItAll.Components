#!/usr/bin/env python3
from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]

REQUIRED_TOP_LEVEL = [
    "README.md",
    "MANIFEST.json",
    "analysis/01-current-state-after-v9.md",
    "analysis/02-weaknesses-and-remediation.md",
    "architecture/01-target-architecture.md",
    "requirements/01-normalized-requirements.md",
    "traceability/01-requirement-traceability.md",
]

CLOSED_STATUSES = {"pass", "passed", "complete", "completed"}
TEXT_EXTENSIONS = {".cs", ".css", ".html", ".js", ".json", ".md", ".ps1", ".py", ".txt", ".yml", ".yaml"}
IMAGE_EXTENSIONS = {".gif", ".jpeg", ".jpg", ".png", ".webp"}
MACHINE_EVIDENCE_RE = re.compile(r"(assert|diagnostic|json|snapshot|state)", re.IGNORECASE)
SKIPPED_SCAN_RE = re.compile(
    r"\b(scan skipped|skipped scan|scan was not run|scan not run|proof was not run|proof not run|placeholder proof|todo proof)\b",
    re.IGNORECASE,
)
STATUS_RE = re.compile(r"^Status:\s*(?P<status>[A-Za-z-]+)\s*$", re.IGNORECASE | re.MULTILINE)
CODE_SPAN_RE = re.compile(r"`([^`]+)`")


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate the WebGlEngine Economy follow-up bundle.")
    parser.add_argument("--stage", default="prepared", help="Validation stage label for reporting.")
    parser.add_argument("--profile", default="initiative", help="Bundle profile label for reporting.")
    args = parser.parse_args()

    errors: list[str] = []
    warnings: list[str] = []

    validate_top_level(errors)
    subbundles = list((ROOT / "subbundles").glob("*/README.md"))
    if len(subbundles) < 18:
        errors.append(f"expected-at-least-18-subbundles-found-{len(subbundles)}")

    manifests = sorted((ROOT / "proof").glob("SB*/manifest.md"))
    if len(manifests) < 18:
        errors.append(f"expected-at-least-18-proof-manifests-found-{len(manifests)}")

    closed_manifest_count = 0
    artifact_count = 0
    screenshot_count = 0
    for manifest in manifests:
        result = validate_proof_manifest(manifest, errors, warnings)
        if result.closed:
            closed_manifest_count += 1
            artifact_count += result.artifact_count
            screenshot_count += result.screenshot_count

    if errors:
        print("Bundle validation failed")
        for error in errors:
            print(error)
        if warnings:
            print("Warnings:")
            for warning in warnings:
                print(warning)
        return 1

    print(
        "Bundle validation passed "
        f"for stage={args.stage}, profile={args.profile}, "
        f"subbundles={len(subbundles)}, closedProofManifests={closed_manifest_count}, "
        f"checkedArtifacts={artifact_count}, checkedScreenshots={screenshot_count}"
    )
    if warnings:
        print("Warnings:")
        for warning in warnings:
            print(warning)
    return 0


def validate_top_level(errors: list[str]) -> None:
    for rel in REQUIRED_TOP_LEVEL:
        path = ROOT / rel
        if not path.exists() or path.stat().st_size == 0:
            errors.append(f"missing-or-empty:{rel}")


class ProofManifestResult:
    def __init__(self, *, closed: bool, artifact_count: int, screenshot_count: int) -> None:
        self.closed = closed
        self.artifact_count = artifact_count
        self.screenshot_count = screenshot_count


def validate_proof_manifest(manifest: Path, errors: list[str], warnings: list[str]) -> ProofManifestResult:
    rel_manifest = manifest.relative_to(ROOT).as_posix()
    if not manifest.exists() or manifest.stat().st_size == 0:
        errors.append(f"missing-or-empty:{rel_manifest}")
        return ProofManifestResult(closed=False, artifact_count=0, screenshot_count=0)

    text = read_text(manifest)
    status_match = STATUS_RE.search(text)
    status = status_match.group("status").lower() if status_match else "unknown"
    closed = status in CLOSED_STATUSES
    if not closed:
        return ProofManifestResult(closed=False, artifact_count=0, screenshot_count=0)

    refs = extract_artifact_refs(text)
    if not refs:
        errors.append(f"proof-manifest-artifacts-missing:{rel_manifest}")
        return ProofManifestResult(closed=True, artifact_count=0, screenshot_count=0)

    resolved_artifacts: list[Path] = []
    screenshot_artifacts: list[Path] = []
    machine_evidence_artifacts: list[Path] = []
    for ref in refs:
        resolved = resolve_artifact_ref(ref, manifest.parent)
        if resolved is None:
            continue

        rel_resolved = to_bundle_rel(resolved)
        if not resolved.exists():
            errors.append(f"proof-artifact-missing:{rel_manifest}:{ref}")
            continue

        if resolved.stat().st_size == 0:
            errors.append(f"proof-artifact-empty:{rel_manifest}:{rel_resolved}")
            continue

        resolved_artifacts.append(resolved)
        suffix = resolved.suffix.lower()
        if suffix in TEXT_EXTENSIONS:
            content = read_text(resolved)
            if not content.strip():
                errors.append(f"proof-artifact-blank:{rel_manifest}:{rel_resolved}")
            if "scan" in resolved.name.lower() and SKIPPED_SCAN_RE.search(content):
                errors.append(f"proof-scan-skipped-or-placeholder:{rel_manifest}:{rel_resolved}")

        if suffix in IMAGE_EXTENSIONS:
            screenshot_artifacts.append(resolved)
        elif suffix in TEXT_EXTENSIONS and MACHINE_EVIDENCE_RE.search(resolved.name):
            machine_evidence_artifacts.append(resolved)

    if screenshot_artifacts:
        if not machine_evidence_artifacts:
            errors.append(f"screenshot-without-machine-readable-assertions:{rel_manifest}")
        else:
            newest_evidence_mtime = max(item.stat().st_mtime for item in machine_evidence_artifacts)
            for screenshot in screenshot_artifacts:
                if screenshot.stat().st_mtime + 300 < newest_evidence_mtime:
                    errors.append(
                        "stale-screenshot:"
                        f"{rel_manifest}:{to_bundle_rel(screenshot)} older-than-newest-machine-evidence"
                    )

    if not resolved_artifacts:
        warnings.append(f"proof-manifest-has-no-resolved-bundle-artifacts:{rel_manifest}")

    return ProofManifestResult(
        closed=True,
        artifact_count=len(resolved_artifacts),
        screenshot_count=len(screenshot_artifacts),
    )


def extract_artifact_refs(text: str) -> list[str]:
    refs: list[str] = []
    in_artifacts = False
    for line in text.splitlines():
        stripped = line.strip()
        lower = stripped.lower()
        if lower in {"## artifacts", "artifacts attached:"}:
            in_artifacts = True
            continue

        if in_artifacts:
            if stripped.startswith("## "):
                break
            if stripped and not stripped.startswith("-"):
                break
            if not stripped:
                continue

            for value in CODE_SPAN_RE.findall(stripped):
                value = value.strip()
                if is_artifact_ref(value):
                    refs.append(value)

    return refs


def is_artifact_ref(value: str) -> bool:
    if value.startswith("repo://"):
        return False
    if value.startswith("bundle://"):
        return True
    suffix = Path(value).suffix.lower()
    return suffix in TEXT_EXTENSIONS or suffix in IMAGE_EXTENSIONS


def resolve_artifact_ref(value: str, proof_dir: Path) -> Path | None:
    if value.startswith("repo://"):
        return None
    if value.startswith("bundle://"):
        return ROOT / value.removeprefix("bundle://")

    path = Path(value)
    if path.is_absolute():
        return path
    if "/" in value or "\\" in value:
        return ROOT / path
    return proof_dir / path


def read_text(path: Path) -> str:
    return path.read_text(encoding="utf-8", errors="replace")


def to_bundle_rel(path: Path) -> str:
    try:
        return path.resolve().relative_to(ROOT.resolve()).as_posix()
    except ValueError:
        return str(path)


if __name__ == "__main__":
    sys.exit(main())
