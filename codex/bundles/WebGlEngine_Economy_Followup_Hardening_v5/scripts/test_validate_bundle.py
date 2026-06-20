#!/usr/bin/env python3
from __future__ import annotations

from pathlib import Path
import subprocess
import sys
import tempfile
import unittest


SCRIPT_DIR = Path(__file__).resolve().parent
VALIDATOR = SCRIPT_DIR / "validate_bundle.py"
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


def write_text(path: Path, content: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(content, encoding="utf-8")


def create_fixture_bundle(
    root: Path,
    *,
    completed_sb01: bool = True,
    priority: str = "P1",
    blank_transcript: bool = False,
    screenshot_without_assertions: bool = False,
    missing_failing_first: bool = False,
    stale_package_marker: bool = False,
) -> None:
    for required_dir in REQUIRED_DIRS:
        (root / required_dir).mkdir(parents=True, exist_ok=True)

    for number in range(1, 13):
        is_completed = completed_sb01 and number == 1
        status = "Completed" if is_completed else "Prepared"
        readme = root / "subbundles" / f"sb{number:02d}-fixture" / "README.md"
        write_text(
            readme,
            f"# SB{number:02d} fixture\n\nPriority: {priority if number == 1 else 'P2'}\nStatus: {status}\n",
        )

        proof_dir = root / "proof" / f"SB{number:02d}"
        manifest_status = "completed" if is_completed else "prepared"
        write_text(
            proof_dir / "manifest.md",
            f"# Proof manifest SB{number:02d}\n\nStatus: {manifest_status}\n\n"
            + ("- Local failing-first evidence is required.\n" if is_completed and not missing_failing_first else ""),
        )
        write_text(
            proof_dir / "semantic-invariants.md",
            f"# Semantic invariants SB{number:02d}\n\nStatus: {manifest_status}\n",
        )

    if not completed_sb01:
        return

    transcript_dir = root / "proof" / "SB01" / "transcripts"
    transcript_dir.mkdir(parents=True, exist_ok=True)
    write_text(
        transcript_dir / "passing-proof.txt",
        "" if blank_transcript else "Focused validation passed.\n",
    )
    write_text(
        transcript_dir / "source-assertion-fixture-scan.txt",
        "Source assertion fixture scan passed.\n",
    )
    if not missing_failing_first:
        write_text(
            transcript_dir / "failing-first-fixture-proof.txt",
            "Failing-first fixture proof captured the pre-fix gap.\n",
        )
    if stale_package_marker:
        write_text(
            transcript_dir / "package-proof.txt",
            "The package proof used a stale package feed.\n",
        )

    if screenshot_without_assertions:
        browser_dir = root / "proof" / "SB01" / "browser"
        browser_dir.mkdir(parents=True, exist_ok=True)
        (browser_dir / "proof-after.png").write_bytes(b"\x89PNG\r\nfixture\n")
    else:
        browser_dir = root / "proof" / "SB01" / "browser"
        browser_dir.mkdir(parents=True, exist_ok=True)
        (browser_dir / "proof-after.png").write_bytes(b"\x89PNG\r\nfixture\n")
        write_text(
            browser_dir / "proof-assertions.json",
            '{"assertions":{"fixturePassed":true}}\n',
        )


class ValidateBundleTests(unittest.TestCase):
    def run_validator(self, root: Path, stage: str = "prepared") -> subprocess.CompletedProcess[str]:
        return subprocess.run(
            [
                sys.executable,
                str(VALIDATOR),
                "--root",
                str(root),
                "--stage",
                stage,
                "--profile",
                "initiative",
            ],
            text=True,
            capture_output=True,
            check=False,
        )

    def test_prepared_stage_allows_incomplete_future_subbundles(self) -> None:
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            create_fixture_bundle(root, completed_sb01=False)

            result = self.run_validator(root)

        self.assertEqual(result.returncode, 0, result.stdout + result.stderr)

    def test_rejects_blank_completed_transcripts(self) -> None:
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            create_fixture_bundle(root, blank_transcript=True)

            result = self.run_validator(root)

        self.assertNotEqual(result.returncode, 0)
        self.assertIn("blank transcript", result.stdout.lower())

    def test_rejects_browser_screenshot_without_json_assertions(self) -> None:
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            create_fixture_bundle(root, screenshot_without_assertions=True)

            result = self.run_validator(root)

        self.assertNotEqual(result.returncode, 0)
        self.assertIn("browser screenshots exist without json assertions", result.stdout.lower())

    def test_rejects_critical_completed_proof_without_failing_first(self) -> None:
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            create_fixture_bundle(root, missing_failing_first=True)

            result = self.run_validator(root)

        self.assertNotEqual(result.returncode, 0)
        self.assertIn("critical proof lacks failing-first evidence", result.stdout.lower())

    def test_rejects_stale_package_feed_markers(self) -> None:
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            create_fixture_bundle(root, stale_package_marker=True)

            result = self.run_validator(root)

        self.assertNotEqual(result.returncode, 0)
        self.assertIn("stale package/feed marker", result.stdout.lower())

    def test_accepts_completed_proof_with_integrity_artifacts(self) -> None:
        with tempfile.TemporaryDirectory() as temp_dir:
            root = Path(temp_dir)
            create_fixture_bundle(root)

            result = self.run_validator(root)

        self.assertEqual(result.returncode, 0, result.stdout + result.stderr)


if __name__ == "__main__":
    unittest.main(verbosity=2)
