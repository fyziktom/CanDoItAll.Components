# WebGL External Consumer Quickstart

This guide is for packages that consume the generic WebGL engine without changing Components internals.

## Package References

Use packages for integration proof:

```xml
<PackageReference Include="CanDoItAll.Components.WebGlLib" Version="0.1.0" />
<PackageReference Include="CanDoItAll.Components.WebGlRunLib" Version="0.1.0" />
```

Use only `WebGlLib` when the host needs a scene viewer and does not need run playback.

## Scene Viewer Host

1. Include `WebGlLibHeadAssets` in the document head.
2. Include `WebGlLibBodyAssets` with scene runtime assets in the body.
3. Render `WebGlSceneView` with a generic `WebGlSceneModel`.
4. Validate serialized scene documents with `WebGlSceneDocumentValidator`.
5. Use `GetProofSnapshotAsync`, `GetDiagnosticsAsync`, and `WaitForRuntimeIdleAsync` for browser proof.

## Run Playback Host

1. Map upstream events into a generic `WebGlRunDocument`.
2. Validate with `WebGlRunDocumentValidator`.
3. Use `WebGlRunDocumentRunner` for load, step, seek, pause, cancel, stop, and reset.
4. Use `WebGlRunBrowserApplyAdapter` with `WebGlSceneViewBrowserRuntime` for browser playback.
5. Use `visualStrict` idle mode for screenshot or final-state proof.

## Domain Driver Boundary

A consuming package owns domain vocabulary. Its driver may map local action names to generic `WebGlRunActionKinds`, stamp driver metadata, and preserve opaque source references. Components must not interpret those names.

Keep these rules:

- Put domain terms in the consuming package or under allowed `source.*` metadata.
- Keep generic action ids, stage ids, object ids, and package APIs domain-neutral.
- Treat unknown mapped actions as `Wait` or reject them in the driver before creating a run document.
- Run the domain-boundary hard gates before publishing.

## Local Package Proof

Pack Components with a proof suffix, restore from a local NuGet source with `<clear />`, and build package-mode samples:

```powershell
npm run webgl:validate-rc
```

The command builds packages, restores isolated samples from the fresh package folder, asserts the restored samples used package libraries instead of project references, runs boundary audits, and captures browser observer proof.

## Production-Line Canary

`samples/CanDoItAll.Components.WebGlRunLibGenericSample` validates both a generic route document and a sample-only production-line canary. Production-line terms are intentionally confined to sample/test fixtures and documentation; generic `WebGlLib` and `WebGlRunLib` source must remain domain-neutral.
