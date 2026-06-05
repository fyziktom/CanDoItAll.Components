# Reopen items

None after v12 execution.

Notes:

- The multi-goods CLI proof intentionally uses `--no-oracle`, so it proves `headless-valid` and does not claim `researchReady=true`.
- A first attempt to run the multi-goods CLI without `--no-build` hit a transient `ResolvePackageAssets` failure in sibling Components project assets. The command was rerun with `--no-build` after the targeted validation build, generated the requested artifacts, and passed.
