# SB12 Pure Repository Transfer Checklist

## Standard Package Scope

- Include `repo://src/CanDoItAll.Components.Common`.
- Include `repo://src/CanDoItAll.Components.BaseLib`.
- Include `repo://src/CanDoItAll.Components.Charts`.
- Include `repo://src/CanDoItAll.Components.OverlayLib`.
- Include `repo://src/CanDoItAll.Components.Mermaid`.
- Include `repo://Directory.Build.props`, package readmes, Tailwind inputs, generated BaseLib CSS, and static web assets required by SB10/SB12 package verification.
- WebGL/Canvas implementation is excluded from this transfer checklist and must move through a separate WebGL/Canvas follow-up bundle.

## Required Proof To Carry Forward

- Inventory workbook: `bundle://inventories/standard-components-publishing-map.xlsx`.
- Tailwind policy: `repo://docs/standard-components-tailwind-policy.md`.
- Foundation ownership policy: `repo://docs/standard-components-foundation-ownership.md`.
- Compatibility policy: `repo://docs/standard-components-compatibility-policy.md`.
- Public API and package approval tests under `repo://tests/CanDoItAll.Components.BaseLib.Tests`.
- Common contract tests under `repo://tests/CanDoItAll.Components.Common.Tests`.
- SB11 final visual matrix report: `bundle://proof/SB11/data/sb11-visual-matrix.json`.
- SB12 package verification report: `bundle://proof/SB12/data/sb12-package-verification.json`.

## Release Gate Before Publishing

- Run standard builds for Common, BaseLib, Charts, OverlayLib, and Mermaid.
- Run Common and BaseLib test projects in locked approval mode.
- Rebuild Tailwind output from `repo://Tailwind/input.css`.
- Pack only the five standard packages and inspect `.nupkg` contents with `bundle://scripts/verify-sb10-packages.py`.
- Run the full standard sandbox visual matrix before publishing any UI-breaking change.
- Keep compatibility shims until a separate consumer migration/removal proof changes the policy and approvals in the same commit.

