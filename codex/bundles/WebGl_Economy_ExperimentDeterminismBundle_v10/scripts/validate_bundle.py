#!/usr/bin/env python3
from pathlib import Path
import sys

root = Path(__file__).resolve().parents[1]
required = [
    "README.md",
    "05_spreadsheets/implementation_matrix.xlsx",
    "01_architecture/01_experimental_input_pipeline.md",
    "01_architecture/02_generic_simulation_kernel_boundaries.md",
    "01_architecture/03_shared_well_and_farmer_land_gap_analysis.md",
    "04_validation/validation_commands.md",
    "04_validation/forbidden_reference_policy.md",
    "06_prompts/one_shot_codex_prompt.md",
]
missing = [path for path in required if not (root / path).exists()]
if missing:
    print("Missing required bundle files:")
    for item in missing:
        print(f"- {item}")
    sys.exit(1)

subbundles = sorted((root / "02_subbundles").glob("SB*.md"))
if len(subbundles) < 15:
    print(f"Expected at least 15 subbundles, found {len(subbundles)}")
    sys.exit(1)

bad = []
for path in root.rglob("*.md"):
    text = path.read_text(encoding="utf-8", errors="ignore").lower()
    if "git checkout -b" in text and "forbidden" not in text[max(0, text.find("git checkout -b")-200):text.find("git checkout -b")+200]:
        bad.append(str(path.relative_to(root)))
    if "small-screen optimization" in text and "do not" not in text[max(0, text.find("small-screen optimization")-200):text.find("small-screen optimization")+200]:
        bad.append(str(path.relative_to(root)))

if bad:
    print("Potential forbidden instructions:")
    for item in sorted(set(bad)):
        print(f"- {item}")
    sys.exit(1)

print("Bundle validation passed.")
