import sys
from pathlib import Path

import bpy


def clear_scene() -> None:
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()


def convert_file(input_path: Path, output_path: Path) -> None:
    clear_scene()

    bpy.ops.import_scene.fbx(filepath=str(input_path), use_anim=True)

    output_path.parent.mkdir(parents=True, exist_ok=True)

    bpy.ops.export_scene.gltf(
        filepath=str(output_path),
        export_format="GLB",
        export_apply=True,
        export_animations=True,
        export_yup=True,
        export_materials="EXPORT",
        export_image_format="AUTO",
    )


def get_default_paths() -> tuple[Path, Path]:
    repo_root = Path(__file__).resolve().parent.parent
    models_root = repo_root / "3DModels"
    return models_root / "fbx", models_root / "glb"


def main() -> int:
    args = sys.argv
    script_args = args[args.index("--") + 1 :] if "--" in args else []

    if len(script_args) >= 2:
        input_dir = Path(script_args[0]).resolve()
        output_dir = Path(script_args[1]).resolve()
    elif len(script_args) == 0:
        input_dir, output_dir = get_default_paths()
    else:
        print("Usage: blender -b --python tools/convert_fbx_to_glb.py -- [input_dir output_dir]")
        return 2

    if not input_dir.exists():
        print(f"Input directory does not exist: {input_dir}")
        return 2

    fbx_files = sorted(input_dir.rglob("*.fbx"))
    print(f"Found {len(fbx_files)} FBX files in {input_dir}")

    failures: list[Path] = []

    for fbx_file in fbx_files:
        relative_path = fbx_file.relative_to(input_dir)
        output_file = output_dir / relative_path.with_suffix(".glb")

        print(f"Converting: {fbx_file} -> {output_file}")

        try:
            convert_file(fbx_file, output_file)
        except Exception as ex:
            failures.append(fbx_file)
            print(f"FAILED: {fbx_file}")
            print(ex)

    if failures:
        print(f"Failed to convert {len(failures)} file(s):")
        for failed_file in failures:
            print(f"  {failed_file}")
        return 1

    print(f"Converted {len(fbx_files)} file(s) into {output_dir}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
