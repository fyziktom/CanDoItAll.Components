#!/usr/bin/env python3
import argparse
import hashlib
import json
from pathlib import Path
from zipfile import ZipFile


EXPECTED_PACKAGES = {
    "CanDoItAll.Components.Common": {
        "static_assets": [],
    },
    "CanDoItAll.Components.BaseLib": {
        "static_assets": [
            "staticwebassets/css/output.css",
            "staticwebassets/js/fileUpload.js",
            "staticwebassets/assets/identity/avatars/avatar-01.jpg",
        ],
    },
    "CanDoItAll.Components.Charts": {
        "static_assets": [],
    },
    "CanDoItAll.Components.OverlayLib": {
        "static_assets": [
            "staticwebassets/css/overlay-window.css",
            "staticwebassets/js/runtime/overlay-window.js",
        ],
    },
    "CanDoItAll.Components.Mermaid": {
        "static_assets": [
            "staticwebassets/js/mermaidDiagram.js",
            "staticwebassets/js/vendor/mermaid.esm.min.mjs",
        ],
    },
}

FORBIDDEN_SUFFIXES = (".cs", ".razor")
FORBIDDEN_SEGMENTS = ("/bin/", "/obj/", "/.artifacts/", "/.codex-tmp/")


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def normalize_entry(entry: str) -> str:
    return entry.replace("\\", "/")


def inspect_package(package_dir: Path, package_id: str, version: str) -> dict:
    candidates = sorted(package_dir.glob(f"{package_id}.{version}.nupkg"))
    if len(candidates) != 1:
        return {
            "packageId": package_id,
            "passed": False,
            "errors": [f"Expected exactly one package matching {package_id}.{version}.nupkg, found {len(candidates)}."],
        }

    package_path = candidates[0]
    with ZipFile(package_path) as archive:
        entries = sorted(normalize_entry(entry.filename) for entry in archive.infolist() if not entry.is_dir())

    required = [
        f"{package_id}.nuspec",
        "README.md",
        f"lib/net10.0/{package_id}.dll",
    ]
    required.extend(EXPECTED_PACKAGES[package_id]["static_assets"])

    errors: list[str] = []
    for required_entry in required:
        if required_entry not in entries:
            errors.append(f"Missing required package entry: {required_entry}")

    forbidden_entries = [
        entry for entry in entries
        if entry.endswith(FORBIDDEN_SUFFIXES)
        or any(segment in f"/{entry}" for segment in FORBIDDEN_SEGMENTS)
    ]
    if forbidden_entries:
        errors.append(f"Package contains source/build leakage entries: {forbidden_entries}")

    if package_id in ("CanDoItAll.Components.BaseLib", "CanDoItAll.Components.OverlayLib", "CanDoItAll.Components.Mermaid"):
        if not any(entry.startswith("staticwebassets/") for entry in entries):
            errors.append("Expected static web assets but package has none.")

    return {
        "packageId": package_id,
        "file": str(package_path),
        "bytes": package_path.stat().st_size,
        "sha256": sha256(package_path),
        "entryCount": len(entries),
        "sampleEntries": entries[:20],
        "requiredEntries": required,
        "passed": not errors,
        "errors": errors,
    }


def main() -> int:
    parser = argparse.ArgumentParser(description="Verify SB10 standard package outputs.")
    parser.add_argument("--package-dir", required=True, type=Path)
    parser.add_argument("--version", required=True)
    parser.add_argument("--output", required=True, type=Path)
    args = parser.parse_args()

    package_dir = args.package_dir.resolve()
    report = {
        "packageDir": str(package_dir),
        "version": args.version,
        "packages": [
            inspect_package(package_dir, package_id, args.version)
            for package_id in EXPECTED_PACKAGES
        ],
    }
    report["passed"] = all(package["passed"] for package in report["packages"])

    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({
        "output": str(args.output),
        "packages": len(report["packages"]),
        "passed": report["passed"],
        "failed": [package["packageId"] for package in report["packages"] if not package["passed"]],
    }, indent=2))

    return 0 if report["passed"] else 1


if __name__ == "__main__":
    raise SystemExit(main())
