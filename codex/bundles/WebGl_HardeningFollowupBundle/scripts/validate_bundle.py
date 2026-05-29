import argparse
from pathlib import Path


ROOT = Path(__file__).resolve().parents[1]
REPO = ROOT.parents[2]


REQUIRED = [
    "artifacts/webgl-scene-hardening/01_INVENTORY.md",
    "artifacts/webgl-scene-hardening/ASSET_VARIANT_INVENTORY.md",
    "artifacts/webgl-scene-hardening/IMPLEMENTATION_REPORT.md",
    "artifacts/webgl-scene-hardening/RUN_LAYER_BOUNDARY.md",
    "artifacts/webgl-scene-hardening/VALIDATION.md",
    "artifacts/webgl-scene-hardening/browser-summary.json",
    "artifacts/webgl-scene-hardening/browser-final-proof.json",
    "artifacts/webgl-scene-hardening/browser-final-canvas.png",
    "artifacts/webgl-scene-hardening/browser-console.log",
    "codex/bundles/WebGl_HardeningFollowupBundle/proof/SB09/manifest.md",
    "codex/bundles/WebGl_HardeningFollowupBundle/proof/SB09/semantic-invariants.md",
]


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--stage", choices=["prepared", "completed"], default="completed")
    args = parser.parse_args()

    missing = [path for path in REQUIRED if not (REPO / path).exists()]
    if missing:
        for path in missing:
            print(f"missing: {path}")
        return 1

    print(f"WebGl_HardeningFollowupBundle {args.stage} validation passed with {len(REQUIRED)} required artifact(s).")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
