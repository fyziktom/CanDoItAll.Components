# SB12 Residual Risk And Follow-Up Separation

## Closed For This Bundle

- Standard component packages build, test, pack, and pass package archive verification.
- Standard sandbox coverage is split from deferred Canvas/WebGL scope.
- Standard routes passed final visual matrix proof with no failed checks or console errors.
- AppComponents parked basic duplicates were reduced with consumer build proof and useful Button behavior ported.
- Tailwind-owned styling policy and generated CSS are aligned for the standard component base.

## Residual Risks

| Risk | Disposition | Required follow-up |
|---|---|---|
| WebGL/Canvas components were intentionally out of implementation scope. | Accepted and separated. | WebGL/Canvas follow-up bundle must inventory, harden, visually test, package-check, and transfer those projects separately. |
| BaseLib compatibility shims remain published. | Accepted for compatibility. | A consumer migration/removal bundle must update policy, approvals, public API proof, and downstream app proof before removal. |
| Pure repository transfer may change paths or package metadata. | Managed by checklist. | Re-run SB10/SB12 build/test/pack/package verification after transfer. |
| Visual regressions may appear after future component changes. | Managed by matrix verifier. | Keep `bundle://scripts/verify-sb11-visual-matrix.mjs` or its pure-repo equivalent as a release gate. |

## Explicit Follow-Up Scope

The next bundle should be titled WebGL/Canvas follow-up bundle and should not reuse this standard-component closure as proof for WebGL/Canvas internals. It must produce its own inventory, sandbox coverage, Playwright screenshots, package proof, and transfer checklist.

