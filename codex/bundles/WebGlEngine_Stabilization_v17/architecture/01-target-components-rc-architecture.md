# Target Components WebGL release-candidate architecture

```text
CanDoItAll.Components.WebGlLib
  WebGlSceneModel / WebGlSceneDocument
  WebGlSceneView
  WebGlScenePatch / WebGlSceneCommandBatch
  WebGlObjectMotionCommand
  Asset catalog, model import options, runtime diagnostics
  JS runtime: window.CanDoItAll.webglScene
  No run documents, no domain events, no simulation semantics

CanDoItAll.Components.WebGlRunLib
  WebGlRunDocument / Timeline / Frame / Stage / Action
  Action plan compiler
  Playback controller / document runner
  Browser apply adapter
  Observer proof
  Domain-driver contract
  No economy, no production-line, no resource accounting

Domain package / future simulator
  Domain model and events
  Domain-specific visual mapping driver
  Domain terms, metrics, or policies
  Can generate WebGlRunDocument or WebGlSceneDocument
```

## Freeze principle

After v17, changes to `WebGlLib` and `WebGlRunLib` should be limited to:
- generic defects
- performance/resource fixes
- security/packaging/static asset issues
- CI/proof hardening
- explicit generic API changes approved by at least two non-equivalent canaries

## Mandatory canaries

1. WebGlLib-only viewer:
   - imports only WebGlLib
   - displays primitive and optional GLB/fallback scene
   - proves static assets and package content

2. WebGlRunLib generic route sample:
   - imports WebGlRunLib package in package mode
   - validates a generic run document
   - runs without domain packages

3. Production-line canary:
   - test/sample/domain-driver fixture
   - validates a very different simulator shape
   - must not add production-line semantics to generic source
