# Is it possible to run a real test now?

Yes, a real **headless** scenario test is now appropriate.

It should run the full pipeline:

```text
strict input pack
  -> sandbox session load/project
  -> backend simulation
  -> visual frame projection
  -> WebGlRunDocument projection
  -> headless playback controller seek/play-to-end
  -> snapshots
  -> snapshot analysis
  -> artifact export
  -> readiness report
```

However, this is not yet the same as a real **browser WebGL UI test**. Browser runtime mutation still needs a desktop-only apply loop that sends `WebGlRunFrameApplyResult` command batches into the WebGL runtime and verifies runtime diagnostics.

## Recommended state classification

- Ready now: headless real scenario artifact runs.
- Ready after this bundle: desktop browser smoke proof with real WebGL runtime calls.
- Not in scope yet: polished UI, mobile layout, production multi-user persistence, ledger-backed scenario UI.
