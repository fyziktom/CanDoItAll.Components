# SB10 changed file hashes

Command:
Get-FileHash -Algorithm SHA256 for each changed SB10 source, test, documentation, and browser-proof artifact.

Result:

| SHA256 | File |
| --- | --- |
| a3fa8b7364792e2937d84252011c1d1739c6887155c89d0c1f6b439d59289787 | C:\repositories\CanDoItAll.Components\src\CanDoItAll.Components.WebGlLib\Components\Scene\WebGlSceneView.razor |
| f6450af09739c1a3351e09f87c803b785f46623ed60a7c603c95d29599efe331 | C:\repositories\CanDoItAll.Components\tests\CanDoItAll.Components.WebGlLib.Tests\WebGlSceneViewExternalImportLifecycleTests.cs |
| a81ee76eec073806bdd2c4b2d45c942bcec219500f11049624dd53e987af3d1c | C:\repositories\CanDoItAll.Components\src\CanDoItAll.Components.WebGlLib\README.md |
| 9d562c58ae78b024ecc48a6310f62cee50c6c6a41d8b6cf73de7ed7416c95298 | C:\repositories\CanDoItAll.Components\codex\bundles\WebGlEngine_Economy_Followup_Hardening_v4\proof\SB10\browser\run-playback-after-import-step-rerender.png |

Semantic assertion:
The hash set covers the production lifecycle change, lifecycle regression test, documentation update, and browser screenshot proof.
