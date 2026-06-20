#!/usr/bin/env python3
from __future__ import annotations

from dataclasses import dataclass
from pathlib import Path
import argparse
import json
import re
import sys


REQUIRED_DIRS = [
    "inputs",
    "analysis",
    "requirements",
    "architecture",
    "plan",
    "subbundles",
    "proof",
    "traceability",
    "shared-prompts",
    "reviews",
]

BROWSER_SCREENSHOT_EXTENSIONS = {".png", ".jpg", ".jpeg", ".webp"}
CRITICAL_PRIORITIES = {"p0", "p1"}
STALE_PACKAGE_FEED_MARKERS = [
    "stale package feed",
    "package feed is stale",
    "stale feed",
    "nu1301",
    "unable to load the service index",
]


@dataclass(frozen=True)
class SubbundleProof:
    number: int
    subbundle_dir: Path
    proof_dir: Path
    readme_text: str
    manifest_text: str
    semantic_text: str
    priority: str
    readme_status: str
    manifest_status: str

    @property
    def is_completed(self) -> bool:
        return (
            self.readme_status.lower() == "completed"
            or self.manifest_status.lower() == "completed"
        )

    @property
    def is_critical(self) -> bool:
        return self.priority.lower() in CRITICAL_PRIORITIES


def read_text(path: Path) -> str:
    return path.read_text(encoding="utf-8-sig", errors="replace")


def parse_field(text: str, field_name: str) -> str:
    match = re.search(
        rf"(?im)^\s*{re.escape(field_name)}\s*:\s*(?P<value>.+?)\s*$",
        text,
    )
    return match.group("value").strip() if match else ""


def rel(path: Path, root: Path) -> str:
    try:
        return path.relative_to(root).as_posix()
    except ValueError:
        return path.as_posix()


def has_nonblank_content(path: Path) -> bool:
    if path.stat().st_size == 0:
        return False
    return bool(read_text(path).strip())


def validate_base_structure(root: Path) -> tuple[list[str], dict[int, Path]]:
    errors: list[str] = []
    missing = [name for name in REQUIRED_DIRS if not (root / name).exists()]
    if missing:
        errors.append("Missing directories: " + ", ".join(missing))

    subbundle_root = root / "subbundles"
    subbundles: dict[int, Path] = {}
    if subbundle_root.exists():
        for subbundle_dir in sorted(subbundle_root.glob("sb*")):
            if not subbundle_dir.is_dir():
                continue
            match = re.match(r"sb(?P<number>\d{2})", subbundle_dir.name, re.IGNORECASE)
            if not match:
                continue
            number = int(match.group("number"))
            subbundles[number] = subbundle_dir

    if len(subbundles) != 12:
        errors.append(f"Expected 12 subbundles, found {len(subbundles)}")

    for number in range(1, 13):
        subbundle_dir = subbundles.get(number)
        if subbundle_dir is None:
            errors.append(f"Missing subbundle sb{number:02d}")
            continue
        if not (subbundle_dir / "README.md").exists():
            errors.append(f"Missing README: {rel(subbundle_dir, root)}")

        proof_dir = root / "proof" / f"SB{number:02d}"
        if not (proof_dir / "manifest.md").exists():
            errors.append(f"Missing proof manifest for SB{number:02d}")
        if not (proof_dir / "semantic-invariants.md").exists():
            errors.append(f"Missing semantic invariants for SB{number:02d}")

    return errors, subbundles


def load_subbundle_proofs(root: Path, subbundles: dict[int, Path]) -> tuple[list[str], list[SubbundleProof]]:
    errors: list[str] = []
    proofs: list[SubbundleProof] = []

    for number in range(1, 13):
        subbundle_dir = subbundles.get(number)
        if subbundle_dir is None:
            continue

        readme_path = subbundle_dir / "README.md"
        proof_dir = root / "proof" / f"SB{number:02d}"
        manifest_path = proof_dir / "manifest.md"
        semantic_path = proof_dir / "semantic-invariants.md"
        if not readme_path.exists() or not manifest_path.exists() or not semantic_path.exists():
            continue

        readme_text = read_text(readme_path)
        manifest_text = read_text(manifest_path)
        semantic_text = read_text(semantic_path)
        proof = SubbundleProof(
            number=number,
            subbundle_dir=subbundle_dir,
            proof_dir=proof_dir,
            readme_text=readme_text,
            manifest_text=manifest_text,
            semantic_text=semantic_text,
            priority=parse_field(readme_text, "Priority"),
            readme_status=parse_field(readme_text, "Status"),
            manifest_status=parse_field(manifest_text, "Status"),
        )
        proofs.append(proof)

        if not proof.priority:
            errors.append(f"Missing Priority field: {rel(readme_path, root)}")

    return errors, proofs


def validate_completed_proof(root: Path, proof: SubbundleProof) -> list[str]:
    errors: list[str] = []
    label = f"SB{proof.number:02d}"

    if proof.manifest_status.lower() != "completed":
        errors.append(f"{label} completed subbundle has proof manifest status '{proof.manifest_status or 'missing'}'")

    placeholder_markers = ["fill during execution", "status: prepared"]
    proof_text = (proof.manifest_text + "\n" + proof.semantic_text).lower()
    for marker in placeholder_markers:
        if marker in proof_text:
            errors.append(f"{label} completed proof still contains placeholder marker '{marker}'")

    transcript_dir = proof.proof_dir / "transcripts"
    transcript_files = sorted(transcript_dir.glob("*.txt")) if transcript_dir.exists() else []
    if not transcript_files:
        errors.append(f"{label} completed proof has no transcript files")

    for transcript in transcript_files:
        if not has_nonblank_content(transcript):
            errors.append(f"{label} blank transcript: {rel(transcript, root)}")

    transcript_names = [file.name.lower() for file in transcript_files]
    joined_proof_text = (
        proof.manifest_text
        + "\n"
        + proof.semantic_text
        + "\n"
        + "\n".join(transcript_names)
    ).lower()

    if proof.is_critical and "failing-first" not in joined_proof_text:
        errors.append(f"{label} critical proof lacks failing-first evidence")

    if transcript_files and not any("source-assertion" in name for name in transcript_names):
        errors.append(f"{label} completed proof lacks a source-assertion transcript")

    browser_dir = proof.proof_dir / "browser"
    if browser_dir.exists():
        browser_files = [file for file in sorted(browser_dir.iterdir()) if file.is_file()]
        screenshots = [
            file for file in browser_files
            if file.suffix.lower() in BROWSER_SCREENSHOT_EXTENSIONS
        ]
        assertion_json_files = [
            file for file in browser_files
            if file.suffix.lower() == ".json" and "assertion" in file.name.lower()
        ]

        for browser_file in browser_files:
            if browser_file.stat().st_size == 0:
                errors.append(f"{label} empty browser proof artifact: {rel(browser_file, root)}")

        if screenshots and not assertion_json_files:
            errors.append(f"{label} browser screenshots exist without JSON assertions")

        for assertion_file in assertion_json_files:
            try:
                assertion_payload = json.loads(read_text(assertion_file))
            except json.JSONDecodeError as exc:
                errors.append(f"{label} invalid browser assertion JSON {rel(assertion_file, root)}: {exc}")
                continue
            if not isinstance(assertion_payload, dict) or "assertions" not in assertion_payload:
                errors.append(f"{label} browser assertion JSON lacks an assertions object: {rel(assertion_file, root)}")

    for transcript in transcript_files:
        content = read_text(transcript).lower()
        for marker in STALE_PACKAGE_FEED_MARKERS:
            if marker in content:
                errors.append(f"{label} stale package/feed marker '{marker}' in {rel(transcript, root)}")

    return errors


def validate_bundle(root: Path, stage: str, profile: str) -> list[str]:
    del profile
    root = root.resolve()
    errors, subbundles = validate_base_structure(root)
    load_errors, proofs = load_subbundle_proofs(root, subbundles)
    errors.extend(load_errors)

    if stage.lower() == "completed":
        for proof in proofs:
            if not proof.is_completed:
                errors.append(f"SB{proof.number:02d} is not completed")

    for proof in proofs:
        if proof.is_completed:
            errors.extend(validate_completed_proof(root, proof))

    return errors


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", default="prepared")
    parser.add_argument("--profile", default="initiative")
    parser.add_argument(
        "--root",
        type=Path,
        default=Path(__file__).resolve().parents[1],
        help="Bundle root. Defaults to the parent of scripts/.",
    )
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    errors = validate_bundle(args.root, args.stage, args.profile)
    if errors:
        print("Bundle validation failed:")
        for error in errors:
            print(f"- {error}")
        return 1

    subbundle_count = len(list((args.root / "subbundles").glob("sb*")))
    print(
        f"Bundle validation passed for stage={args.stage}, "
        f"profile={args.profile}, subbundles={subbundle_count}"
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())
