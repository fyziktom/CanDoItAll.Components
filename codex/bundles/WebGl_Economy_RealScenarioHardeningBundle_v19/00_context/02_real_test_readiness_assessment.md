# Real test readiness assessment

## Can we run real scenario tests now?

Yes, but only in staged form.

Recommended order:

1. **Headless data pipeline test**  
   Input pack -> backend materialization -> visual frames -> WebGL run document -> snapshots -> analysis.

2. **Headless executable run-document test**  
   Run document -> WebGlRunPlaybackController -> frame apply result -> command batch -> stage/motion queue semantics.

3. **Browser smoke test on large-screen only**  
   Render initial scene and play a very short action sequence. No mobile/tablet/small-screen work.

4. **Interactive sandbox test**  
   Pause, seek, inspect snapshot, show active/pending actions, export analysis.

The first two are ready to run now. The third needs more proof around runtime execution and GLB asset loading stability.

## What is not yet ready

- A polished visual demo for users.
- Full browser test automation with real GLB assets and reliable visual assertions.
- Runtime snapshot capture from WebGL back into Economy snapshot analysis.
- A stable persistence format for sandbox sessions.
- Multi-backend comparison between SimpleAccounts and Ledger.
