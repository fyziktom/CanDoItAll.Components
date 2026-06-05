#!/usr/bin/env python3
from argparse import ArgumentParser
from pathlib import Path
import sys


def main() -> int:
    parser = ArgumentParser()
    parser.add_argument("--stage", default="prepared", choices=["prepared", "completed"])
    parser.add_argument("--bundle", default=None)
    args = parser.parse_args()

    root = Path(args.bundle).resolve() if args.bundle else Path(__file__).resolve().parents[1]
    errors: list[str] = []

    required = [
        "README.md",
        "analysis/01-current-state-after-v10.md",
        "architecture/01-domain-driver-boundary.md",
        "plan/01-phase-plan.md",
        "traceability/01-requirement-traceability.md",
    ]
    missing = [p for p in required if not (root / p).exists()]
    if missing:
        errors.append(f"Missing required files: {missing}")

    subs = sorted((root / "subbundles").glob("*/README.md"))
    proofs = sorted((root / "proof").glob("SB*/manifest.md"))
    if len(subs) < 18:
        errors.append(f"Expected at least 18 subbundles, found {len(subs)}")
    if len(proofs) < 18:
        errors.append(f"Expected at least 18 proof manifests, found {len(proofs)}")

    if args.stage == "completed":
        validate_completed(root, proofs, errors)

    if errors:
        for error in errors:
            print(error)
        return 1

    print(f"Bundle validation passed for stage={args.stage}, profile=initiative, subbundles={len(subs)}")
    return 0


def validate_completed(root: Path, manifests: list[Path], errors: list[str]) -> None:
    report = root / "reviews" / "01-execution-report.md"
    if not report.exists() or report.stat().st_size == 0:
        errors.append("Missing non-empty reviews/01-execution-report.md")

    for manifest in manifests:
        text = manifest.read_text(encoding="utf-8")
        if "Status: completed" not in text:
            errors.append(f"{manifest.relative_to(root)} is not marked completed")
        if "Status: prepared" in text:
            errors.append(f"{manifest.relative_to(root)} still contains prepared status")

    transcripts = sorted((root / "proof").glob("SB*/transcripts/*"))
    if len(transcripts) < 10:
        errors.append(f"Expected at least 10 transcript artifacts, found {len(transcripts)}")
    for transcript in transcripts:
        if not transcript.is_file():
            continue
        if transcript.stat().st_size == 0:
            errors.append(f"{transcript.relative_to(root)} is empty")

    required_transcripts = [
        "proof/SB01/transcripts/source-state-and-scope.txt",
        "proof/SB02/transcripts/domain-boundary-audit-negative-probe.txt",
        "proof/SB02/transcripts/domain-boundary-audit-webglrunlib.txt",
        "proof/SB03/transcripts/domain-boundary-audit-webgllib.txt",
        "proof/SB03/transcripts/webgllib-runtime-diagnostics-tests.txt",
        "proof/SB04/transcripts/webglrun-validator-tests.txt",
        "proof/SB05/transcripts/economy-tests-build.txt",
        "proof/SB05/transcripts/economy-webgl-bridge-tests.txt",
        "proof/SB10/transcripts/webgl-runtime-audits.txt",
        "proof/SB10/transcripts/webglrun-pause-stop-tests.txt",
        "proof/SB16/transcripts/economy-semantic-readiness-performance-tests.txt",
    ]
    for item in required_transcripts:
        path = root / item
        if not path.exists() or path.stat().st_size == 0:
            errors.append(f"Missing non-empty required transcript {item}")


if __name__ == "__main__":
    sys.exit(main())
