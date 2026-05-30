using CanDoItAll.Components.BaseLib;
using CanDoItAll.Components.WebGlLib;
using CanDoItAll.Components.WebGlRunLib;
using CanDoItAll.Components.WebGlSandbox;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.WebGlSandbox.Components.Pages;

public partial class RunPlayback
{
    private readonly WebGlRuntimeOptions runtimeOptions = new()
    {
        DeterministicMode = true,
        RenderMode = WebGlRenderModes.Auto,
        AssetQualityProfile = WebGlAssetQualityProfiles.Primitive,
        ShowLabels = false,
        ShowSymbols = true
    };
    private readonly WebGlRunDocument runDocument;
    private readonly WebGlRunPlaybackController playbackController;
    private WebGlSceneModel scene = CreateScene();
    private WebGlSceneView? sceneView;
    private WebGlSceneProofSnapshot? latestSnapshot;
    private string statusText = "Ready.";

    public RunPlayback()
    {
        runDocument = CreateRunDocument();
        playbackController = new WebGlRunPlaybackController(runDocument);
    }

    private WebGlRunPlaybackState PlaybackState => playbackController.State;
    private string PlaybackStatus => PlaybackState.IsPlaying ? "playing" : "paused";
    private string PlaybackTone => PlaybackState.IsPlaying ? "success" : "info";

    private async Task PlayAsync()
    {
        if (PlaybackState.IsPlaying)
        {
            return;
        }

        var playResult = await playbackController.PlayToEndAsync(new SceneFrameApplier(this));
        if (!playResult.Success)
        {
            statusText = string.Join(" ", playResult.Errors);
        }
    }

    private async Task PauseAsync()
    {
        await playbackController.ApplyDetailedAsync(new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Pause });
        statusText = "Paused.";
    }

    private async Task StepAsync()
    {
        var result = await playbackController.ApplyDetailedAsync(new WebGlRunPlaybackCommand { Kind = WebGlRunPlaybackCommandKinds.Next });
        await ApplyPlaybackResultAsync(result);
    }

    private Task SeekStartAsync() => SeekAsync(0);

    private async Task SeekAsync(long frameIndex)
    {
        scene = CreateScene();
        if (sceneView is not null)
        {
            await sceneView.ImportSceneAsync(scene);
        }

        var result = await playbackController.ApplyDetailedAsync(new WebGlRunPlaybackCommand
        {
            Kind = WebGlRunPlaybackCommandKinds.Seek,
            TargetFrameIndex = frameIndex
        });
        await ApplyPlaybackResultAsync(result);
    }

    private async Task ApplyPlaybackResultAsync(WebGlRunPlaybackResult result)
    {
        if (!result.Success)
        {
            statusText = string.Join(" ", result.Errors);
            return;
        }

        foreach (var frame in result.FramesToApply)
        {
            await ApplyFrameAsync(WebGlRunFrameApplyResult.FromFrame(frame));
        }
    }

    private async Task ApplyFrameAsync(WebGlRunFrameApplyResult frame)
    {
        if (sceneView is null)
        {
            return;
        }

        await sceneView.ApplyCommandBatchAsync(frame.CommandBatch);

        statusText = $"Applied generic frame {frame.FrameIndex}.";
        await CaptureProofAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task CaptureProofAsync()
        => latestSnapshot = sceneView is null ? null : await sceneView.GetProofSnapshotAsync();

    private Task HandleMotionCompleted(WebGlSceneCommandResult result)
    {
        statusText = $"Motion completed: {result.CommandId}.";
        return InvokeAsync(StateHasChanged);
    }

    private static WebGlRunDocument CreateRunDocument()
    {
        var compiler = new WebGlRunActionCompiler();
        return new WebGlRunDocument
        {
            RunId = new("generic-run-demo"),
            InitialScene = new WebGlSceneDocument { Scene = CreateScene() },
            Timeline = compiler.Compile(CreateActionPlan()),
            Metadata =
            {
                ["boundary"] = "generic-webgl-runlib",
                ["domain"] = "generic"
            }
        };
    }

    private static WebGlRunActionPlan CreateActionPlan()
        => new()
        {
            FrameRate = 1,
            ObjectBindings =
            [
                new WebGlRunObjectBinding { ObjectId = "object.runner", Position = new WebGlVector3(-3, 0, 0), AnchorPosition = new WebGlVector3(-3, 0, 0) },
                new WebGlRunObjectBinding { ObjectId = "object.goal", Position = new WebGlVector3(3, 0, 0), AnchorPosition = new WebGlVector3(3, 0, 0) }
            ],
            Actions =
            [
                Action("run.move.target", WebGlRunActionKinds.MoveToObject, 0, "object.runner", "object.goal"),
                Action("run.symbol.show", WebGlRunActionKinds.ShowSymbol, 1, "object.runner", parameters: new()
                {
                    ["symbolKind"] = "generic-status",
                    ["color"] = "#facc15",
                    ["tooltip"] = "Generic status"
                }),
                Action("run.pose.work", WebGlRunActionKinds.SetPose, 1, "object.runner", parameters: new()
                {
                    ["poseKey"] = "active"
                }),
                Action("run.return.anchor", WebGlRunActionKinds.ReturnToAnchor, 2, "object.runner"),
                Action("run.symbol.hide", WebGlRunActionKinds.HideSymbol, 3, "object.runner"),
                Action("run.pose.restore", WebGlRunActionKinds.SetPose, 3, "object.runner", parameters: new()
                {
                    ["poseKey"] = "neutral"
                })
            ]
        };

    private static WebGlRunAction Action(
        string id,
        string kind,
        double startsAtSeconds,
        string subjectObjectId,
        string targetObjectId = "",
        Dictionary<string, string>? parameters = null)
        => new()
        {
            ActionId = id,
            ActionKind = kind,
            SubjectObjectId = subjectObjectId,
            TargetObjectId = targetObjectId,
            StartsAtSeconds = startsAtSeconds,
            DurationSeconds = 0.32,
            Parameters = parameters ?? []
        };

    private sealed class SceneFrameApplier(RunPlayback owner) : IWebGlRunFrameApplier
    {
        public async ValueTask ApplyAsync(WebGlRunFrameApplyResult frame, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await owner.ApplyFrameAsync(frame);
        }
    }

    private static WebGlSceneModel CreateScene()
        => new()
        {
            SceneId = "generic-run-playback",
            Title = "Generic Run Playback",
            AssetCatalog = WebGlSandboxAssetCatalogFactory.Create(),
            Environment = new WebGlSceneEnvironment
            {
                BackgroundColor = "#101827",
                GroundColor = "#253349",
                GridColor = "#94a3b8",
                GroundSize = 14,
                GridDivisions = 14,
                FogEnabled = false
            },
            Camera = new WebGlSceneCamera
            {
                ViewMode = WebGlSceneViewModes.Isometric,
                Distance = 13,
                Target = new WebGlVector3(0, 0.6, 0)
            },
            UiState = new WebGlSceneUiState
            {
                ShowLabels = false,
                ShowSymbols = true,
                ActiveAssetProfile = WebGlAssetQualityProfiles.Primitive
            },
            Objects =
            [
                new WebGlSceneObject
                {
                    Id = "object.runner",
                    Kind = "marker",
                    Family = "generic-run",
                    Title = "Runner",
                    AssetId = "asset.symbol.marker.default",
                    Position = new WebGlVector3(-3, 0, 0),
                    Size = new WebGlVector3(0.8, 0.8, 0.8),
                    Color = "#38bdf8",
                    IsSelectable = true
                },
                new WebGlSceneObject
                {
                    Id = "object.goal",
                    Kind = "marker",
                    Family = "generic-run",
                    Title = "Goal",
                    AssetId = "asset.symbol.ready.default",
                    Position = new WebGlVector3(3, 0, 0),
                    Size = new WebGlVector3(0.8, 0.8, 0.8),
                    Color = "#22c55e"
                }
            ],
            Links =
            [
                new WebGlSceneLink
                {
                    Id = "link.path",
                    SourceObjectId = "object.runner",
                    TargetObjectId = "object.goal",
                    Kind = "timeline-path"
                }
            ],
            Metadata =
            {
                ["demo"] = "run-playback",
                ["domain"] = "generic"
            }
        };
}
