# SB01 — Branch, Inventory, and Evidence Gate

## Goal

Establish a trustworthy baseline before modifying the `webgl-engine` branch.

## Tasks

1. Verify branch:
   ```powershell
   git status --short --branch
   git branch --show-current
   ```
   Stop if not on the expected current branch. Do not create a new branch.

2. Create evidence folder:
   ```text
   artifacts/webgl-engine-next-hardening/
     inventory/
     validation/
     browser/
     reviews/
   ```

3. Generate inventory:
   - list `src/CanDoItAll.Components.WebGlLib/wwwroot/js/runtime/scene/*.js`
   - line counts
   - imports/exports
   - public façade method names
   - C# scene/asset/patch/motion/document contracts
   - sandbox routes
   - external GLB inventory

4. Record current known model issue:
   - models are present and runtime tries to load them,
   - some imported models can be invisible,
   - do not hide this behind primitive fallback; diagnostics must make it visible.

5. Save inventory as:
   ```text
   artifacts/webgl-engine-next-hardening/inventory/current-runtime-inventory.md
   artifacts/webgl-engine-next-hardening/inventory/current-runtime-line-counts.txt
   artifacts/webgl-engine-next-hardening/inventory/current-glb-inventory.json
   ```

## Validation

Run:
```powershell
npm run webgllib:audit-scene-runtime
dotnet build CanDoItAll.Components.slnx
```

## Done criteria

- Branch was not changed.
- Inventory exists.
- Audit result is recorded.
- Existing runtime still builds before any feature work.
