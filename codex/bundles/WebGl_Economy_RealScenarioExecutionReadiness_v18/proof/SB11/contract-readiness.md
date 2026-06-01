# SB11 Contract Readiness

The first browser proof is intentionally not built in this bundle.

Readiness outcome:

- The browser proof belongs in `CanDoItAll.Economy`, not Components.
- `CanDoItAll.Economy.SimulationSandbox` now exposes session operations suitable for a large-screen UI: load, project, step, seek, pause, resume, snapshot, analyze, export, import, status, and safe operation results.
- Real probe artifacts now include strict input validation, simulation frames/deltas, visual frames, WebGL run documents, snapshots, analysis JSON, and readiness reports under `artifacts/economy/real-probe/`.
- The large-screen policy remains desktop-only. No small, medium, mobile, or tablet UI work was added.

Minimum UI contract available for the next bundle:

- Select one real experiment input pack.
- Display projection diagnostics and readiness report status.
- Feed the WebGL run document into generic Components WebGL primitives.
- Step, seek, pause, and resume through the session service.
- Show current frame and stage ids from the run document/session status.
- Export the current snapshot and display snapshot analysis text.

Proof:

- `proof/SB05/transcripts/economy-sandbox-session-tests.txt`
- `proof/SB04/transcripts/economy-real-probe-artifact-exporter-tests.txt`
- `proof/SB12/transcripts/economy-required-filtered-tests.txt`
