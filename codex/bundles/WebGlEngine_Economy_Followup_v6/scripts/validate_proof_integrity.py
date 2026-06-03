from __future__ import annotations

import argparse
from pathlib import Path


PLACEHOLDERS = (
    "Status: pending",
    "To be filled",
    "Minimum required fields",
    "placeholder",
    "TBD",
    "- [ ]",
)


def read_text(path: Path) -> str:
    try:
        return path.read_text(encoding="utf-8")
    except UnicodeDecodeError:
        return path.read_text(encoding="utf-8", errors="replace")


def validate_markdown(path: Path, errors: list[str]) -> None:
    if not path.exists():
        errors.append(f"missing proof file: {path}")
        return

    text = read_text(path)
    if len(text.strip()) < 200:
        errors.append(f"proof file is too small to be substantive: {path}")

    lower = text.lower()
    for placeholder in PLACEHOLDERS:
        if placeholder.lower() in lower:
            errors.append(f"proof file still contains placeholder text '{placeholder}': {path}")

    required_terms = ("Evidence", "Result", "Changed files")
    if path.name == "manifest.md" and not all(term.lower() in lower for term in required_terms):
        errors.append(f"manifest must include Evidence, Result, and Changed files sections: {path}")

    if path.name == "semantic-invariants.md" and "Invariant" not in text:
        errors.append(f"semantic invariant file must describe at least one invariant: {path}")


def validate_artifact(path: Path, errors: list[str]) -> None:
    if path.stat().st_size == 0:
        errors.append(f"empty proof artifact: {path}")
        return

    if path.suffix.lower() in {".txt", ".md", ".json", ".log"}:
        text = read_text(path)
        for placeholder in PLACEHOLDERS:
            if placeholder.lower() in text.lower():
                errors.append(f"proof artifact still contains placeholder text '{placeholder}': {path}")


def validate(bundle_root: Path) -> list[str]:
    errors: list[str] = []
    proof_root = bundle_root / "proof"
    for index in range(1, 15):
        proof_dir = proof_root / f"SB{index:02d}"
        if not proof_dir.exists():
            errors.append(f"missing proof directory: {proof_dir}")
            continue

        validate_markdown(proof_dir / "manifest.md", errors)
        validate_markdown(proof_dir / "semantic-invariants.md", errors)

    for artifact in proof_root.rglob("*"):
        if artifact.is_file():
            validate_artifact(artifact, errors)

    return errors


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--bundle-root", type=Path, default=Path(__file__).resolve().parents[1])
    args = parser.parse_args()
    errors = validate(args.bundle_root.resolve())
    if errors:
        print("Proof integrity validation failed:")
        for error in errors:
            print(f"- {error}")
        return 1

    print("Proof integrity validation passed.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
