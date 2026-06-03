# SB09 changed file hashes

Command:
Get-FileHash -Algorithm SHA256 for each changed SB09 source, test, and documentation file.

Result:

| SHA256 | File |
| --- | --- |
| 1389ae6b0a99e9e56af1a0ac19f077461886e62c4d87a7576de9652d3f713978 | C:\repositories\CanDoItAll.Components\docs\webgl\run-layer-boundary.md |
| 39e4b0869ba5b4718385f94f6c82b17c3f7ccc62242c4ca7febb6c79d9e728e0 | C:\repositories\CanDoItAll.Components\src\CanDoItAll.Components.WebGlRunLib\Documents\WebGlRunStageOrderingPolicy.cs |
| 4b9df47c66bdabf86252ed837dda2e3f0aede0b46df39da0d81283422213c999 | C:\repositories\CanDoItAll.Components\src\CanDoItAll.Components.WebGlRunLib\Playback\WebGlRunFrameApplyResult.cs |
| b53f78a8f9009de8b44a3ae1aabb7b98533d962c66ed5ecb7c45f622817922ba | C:\repositories\CanDoItAll.Components\src\CanDoItAll.Components.WebGlRunLib\Playback\WebGlRunFrameExecutionValidator.cs |
| e2ddd08ff8533b89a2cf40e8b6e036b279587523dbb2f2a67c7bc78b629fe409 | C:\repositories\CanDoItAll.Components\src\CanDoItAll.Components.WebGlRunLib\Playback\WebGlRunExecutionResultDiagnostics.cs |
| f2e2a7ab1fbe22a56fc55036e378574166ac721222d4d459800f4bf5573d7abe | C:\repositories\CanDoItAll.Components\src\CanDoItAll.Components.WebGlRunLib\Playback\WebGlRunDocumentRunner.cs |
| abcac727d51700aed5de335a69edcee7eb0b77cbc6c1f97f53f5ca5e6df83408 | C:\repositories\CanDoItAll.Components\src\CanDoItAll.Components.WebGlRunLib\WebGlRunPlaybackClock.cs |
| 8d6f0511587117a423dc7883b25c2b7a1550dde5eb594e2a27d0c57470f579fe | C:\repositories\CanDoItAll.Components\src\CanDoItAll.Components.WebGlRunLib\WebGlRunPlaybackController.cs |
| 956713e933dc0f0d9aa101e1e23ed9ce03fbe247b5ad267e9854dc62574e1456 | C:\repositories\CanDoItAll.Components\tests\CanDoItAll.Components.WebGlRunLib.Tests\WebGlRunDocumentRunnerTests.cs |
| 45a11cfb4ca8d791cf40edd64999928634f174e9d3bc60eb0abfd4d0143a6966 | C:\repositories\CanDoItAll.Components\tests\CanDoItAll.Components.WebGlRunLib.Tests\WebGlRunPlaybackControllerTests.cs |
| ef767a1739b6857e5a035e8e2d3867666c230ccdfdb14ed131a2d41571289a7e | C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\EconomyWebGlRunValidator.cs |
| a8a431afec3f4bb5ed34bebab255e68780c78c0b37b553a510b3d40d0d756191 | C:\repositories\CanDoItAll.Economy\src\CanDoItAll.Economy.Simulation.WebGlBridge\EconomyWebGlSnapshotVisualStateBuilder.cs |
| 2f66181f9891b8cd547fbf2717c50e69528d77b06a57a5a3210362830c6d3339 | C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\EconomyWebGlBridgeStrictMappingTests.cs |
| fd426754770507dfc79e98f0864e8bc48806ebd5367f08b38cd1a7a91a3ebe00 | C:\repositories\CanDoItAll.Economy\tests\CanDoItAll.Economy.Tests\EconomyWebGlSnapshotVisualStateBuilderTests.cs |

Semantic assertion:
The hash set covers the new public ordering policy, each production path that adopted it, focused tests, and the boundary documentation update.
