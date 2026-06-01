# Missing before full simulation + visualization join

The code now appears ready for headless real scenario tests. Before a visual join, harden these missing pieces:

1. **Executable playback contract**
   - A `WebGlRunDocument` must have a deterministic way to produce command batches per frame/stage.
   - The result must be inspectable without a browser.

2. **Browser runtime proof**
   - Execute at least one short generic sequence in a large-screen browser:
     - move actor to object,
     - wait for object motion,
     - apply pose/symbol,
     - return to anchor,
     - pause and export runtime diagnostics.

3. **Bridge strictness**
   - No unresolved subject/target object should pass strict mode.
   - No missing asset/pose/symbol should pass strict mode unless an explicit no-op fallback is allowed.
   - Diagnostic fallback object should be permissive/dev-only.

4. **Snapshot runtime attachment**
   - Economy snapshot already has visual state, but browser runtime state must be attachable:
     - current frame index,
     - active/queued stage ids,
     - active/queued motion ids,
     - command journal tail,
     - render diagnostics.

5. **Scenario-session persistence**
   - Persist sandbox session state:
     - input pack refs/hashes,
     - current step,
     - selected snapshot id,
     - exported run document hash,
     - optional UI-only state outside deterministic simulation hash.

6. **Performance proof**
   - Headless test with at least:
     - 100 actors,
     - 500 stores/resources,
     - 1000 events,
     - 500 visual actions,
     - 1000 links/symbols.
