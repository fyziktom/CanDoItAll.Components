import argparse
from pathlib import Path
import sys

REQUIRED_ROOT_DIRS = [
    "inputs", "analysis", "requirements", "architecture", "plan", "traceability",
    "shared-prompts", "subbundles", "templates", "reviews", "proof"
]

REQUIRED_SUBBUNDLE_SECTIONS = [
    "## Status",
    "## Objective",
    "## Covered Inputs",
    "## Prerequisites",
    "## Exact Source References",
    "## Deliverables",
    "## Dependency Impact",
    "## Validation Depth",
    "## Implementation Steps",
    "## Do Not Do",
    "## Acceptance Checklist",
    "## Proof Required",
    "## Browser Validation Logging",
    "## Progression Gate",
    "## Suggested Agent Prompt",
]

REQUIRED_PLAN_SECTIONS = [
    "## Subbundle Dependency Map",
    "## Critical Subbundles",
    "## Phase Gates",
]

def fail(msg: str, errors: list[str]) -> None:
    errors.append(msg)

def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", default="prepared")
    parser.add_argument("--profile", default="initiative")
    args = parser.parse_args()

    root = Path.cwd()
    errors: list[str] = []

    for rel in REQUIRED_ROOT_DIRS:
        if not (root / rel).exists():
            fail(f"Missing required root section: {rel}", errors)

    for rel in ["README.md", "plan/01-phase-plan.md", "traceability/01-requirement-traceability.md", "traceability/02-input-coverage-matrix.md"]:
        if not (root / rel).is_file():
            fail(f"Missing required file: {rel}", errors)

    plan = (root / "plan/01-phase-plan.md").read_text(encoding="utf-8") if (root / "plan/01-phase-plan.md").is_file() else ""
    for section in REQUIRED_PLAN_SECTIONS:
        if section not in plan:
            fail(f"Plan missing section: {section}", errors)
    if "```mermaid" not in plan:
        fail("Plan missing mermaid dependency map.", errors)

    subbundle_roots = sorted((root / "subbundles").glob("SB*-*")) if (root / "subbundles").exists() else []
    if not subbundle_roots:
        fail("No subbundles found.", errors)

    for sub in subbundle_roots:
        readme = sub / "README.md"
        if not readme.is_file():
            fail(f"{sub.name} missing README.md", errors)
            continue
        text = readme.read_text(encoding="utf-8")
        for section in REQUIRED_SUBBUNDLE_SECTIONS:
            if section not in text:
                fail(f"{sub.name} missing section {section}", errors)
        sbid = sub.name.split("-")[0]
        manifest = root / "proof" / sbid / "manifest.md"
        if not manifest.is_file():
            fail(f"{sub.name} missing proof manifest placeholder {manifest.relative_to(root)}", errors)
        if "Critical foundation" in text:
            invariants = root / "proof" / sbid / "semantic-invariants.md"
            if not invariants.is_file():
                fail(f"{sub.name} critical but missing semantic invariants {invariants.relative_to(root)}", errors)

    xlsx = root / "CanDoItAll_WebGlEngine_Economy_Hardening_Checklists.xlsx"
    if not xlsx.is_file():
        fail("Missing XLSX checklist workbook.", errors)

    if errors:
        print("Bundle validation failed:")
        for err in errors:
            print(f"- {err}")
        return 1

    print(f"Bundle validation passed for stage={args.stage}, profile={args.profile}, subbundles={len(subbundle_roots)}")
    return 0

if __name__ == "__main__":
    raise SystemExit(main())
