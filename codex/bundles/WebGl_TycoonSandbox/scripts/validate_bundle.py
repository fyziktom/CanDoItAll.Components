#!/usr/bin/env python3
"""Lightweight structural validator for the WebGl_TycoonSandbox bundle."""

from __future__ import annotations

import argparse
import pathlib
import sys


ROOT = pathlib.Path(__file__).resolve().parents[1]


PREPARED_REQUIRED = [
    "README.md",
    "bundle.json",
    "00_context_and_repo_observations.md",
    "01_codex_master_prompt.md",
    "02_architecture_target.md",
    "05_runtime_js_design.md",
    "06_validation_checklist.md",
    "07_done_criteria.md",
    "plan/01-phase-plan.md",
    "traceability/01-requirements-map.md",
    "reviews/00-bundle-self-review.md",
    "reviews/01-execution-report.md",
]


SUBBUNDLES = [
    "SB01_inventory_and_guardrails.md",
    "SB02_scene_contracts.md",
    "SB03_asset_catalog_services.md",
    "SB04_symbol_system.md",
    "SB05_interaction_services.md",
    "SB06_interop_runtime.md",
    "SB07_webgl_sandbox_project.md",
    "SB08_tycoon_village_demo.md",
    "SB09_validation_docs_hardening.md",
]


COMPLETED_REQUIRED = [
    "proof/SB02/manifest.md",
    "proof/SB02/semantic-invariants.md",
    "proof/SB03/manifest.md",
    "proof/SB03/semantic-invariants.md",
    "proof/SB06/manifest.md",
    "proof/SB06/semantic-invariants.md",
    "proof/SB08/manifest.md",
    "proof/SB08/semantic-invariants.md",
    "proof/SB09/manifest.md",
    "proof/SB09/semantic-invariants.md",
]


def fail(message: str) -> int:
    print(f"FAIL: {message}", file=sys.stderr)
    return 1


def check_files(paths: list[str]) -> list[str]:
    return [path for path in paths if not (ROOT / path).is_file()]


def check_subbundles() -> list[str]:
    return [
        f"03_subbundles/{name}"
        for name in SUBBUNDLES
        if not (ROOT / "03_subbundles" / name).is_file()
    ]


def check_prepared() -> int:
    missing = check_files(PREPARED_REQUIRED) + check_subbundles()
    if missing:
        return fail("prepared bundle missing required files: " + ", ".join(missing))

    plan_text = (ROOT / "plan/01-phase-plan.md").read_text(encoding="utf-8")
    if "flowchart" not in plan_text or "Critical foundation" not in plan_text:
        return fail("phase plan must include dependency map and critical foundation labels")

    trace_text = (ROOT / "traceability/01-requirements-map.md").read_text(encoding="utf-8")
    if "SB08" not in trace_text or "Browser" not in trace_text:
        return fail("traceability must map browser-visible village proof")

    print("PASS: prepared bundle structure is ready.")
    return 0


def check_completed() -> int:
    result = check_prepared()
    if result:
        return result

    missing = check_files(COMPLETED_REQUIRED)
    if missing:
        return fail("completed bundle missing proof files: " + ", ".join(missing))

    report = (ROOT / "reviews/01-execution-report.md").read_text(encoding="utf-8")
    if "Pending" in report:
        return fail("execution report still contains Pending markers")

    proof_text = "\n".join((ROOT / path).read_text(encoding="utf-8") for path in COMPLETED_REQUIRED)
    required_terms = ["repo://", "bundle://", "anti-stub", "failing-first", "passing"]
    missing_terms = [term for term in required_terms if term not in proof_text]
    if missing_terms:
        return fail("proof files missing required terms: " + ", ".join(missing_terms))

    print("PASS: completed bundle structure and proof files are present.")
    return 0


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", choices=["prepared", "completed"], required=True)
    args = parser.parse_args()
    return check_prepared() if args.stage == "prepared" else check_completed()


if __name__ == "__main__":
    raise SystemExit(main())

