# WebGL API Change Request Template

Use this template before changing any frozen WebGL engine surface.

## Summary

- Proposed change:
- Affected package:
- Surface: C# public API, JS facade, action kind, package content, driver manifest, documentation
- Compatibility impact: additive, behavioral, breaking, removal

## Generic Justification

- Consumer-neutral problem:
- Why existing generic APIs are insufficient:
- Why this does not encode a consuming domain:

## Approval Updates

- Public API snapshot:
- JS API manifest:
- Action-kind snapshot:
- Package-content snapshot:
- Driver-manifest snapshot:
- Migration note:

## Required Proof

Run the focused proof first, then the full RC command:

```powershell
npm run webgl:validate-rc
```

Attach the transcript, package-mode proof, browser observer JSON, and any failing-first evidence.
