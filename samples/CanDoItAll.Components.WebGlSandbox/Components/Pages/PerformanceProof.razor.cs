using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlSandbox.Components.Pages;

public partial class PerformanceProof
{
    private readonly WebGlRuntimeOptions runtimeOptions = new()
    {
        DeterministicMode = true,
        ShowDiagnosticsPanel = true,
        ShowLabels = false,
        ShowSymbols = true,
        AutoFitOnCreate = true,
        RenderMode = WebGlRenderModes.Auto,
        AssetQualityProfile = WebGlAssetQualityProfiles.Primitive,
        RuntimeKey = "performance-proof-100-v1",
        RuntimeBudget = WebGlRuntimeBudgetProfiles.Scene100()
    };

    private WebGlSceneModel scene = CreateScene();
    private WebGlSceneView? sceneView;
    private WebGlSceneCommandBatchResult? latestBatchResult;
    private WebGlRuntimeDiagnostics? latestDiagnostics;
    private WebGlSceneProofSnapshot? latestSnapshot;
    private string statusText = "loading";
    private string statusTone = "info";
    private string statusDetail = "Waiting for runtime.";
    private string runtimeError = string.Empty;
    private bool proofStarted;

    private string BatchCommandCountText => (latestBatchResult?.Metrics.BatchCommandCount ?? latestDiagnostics?.BatchCommandCount ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture);
    private string BatchDurationText => (latestBatchResult?.Metrics.BatchDurationMs ?? latestDiagnostics?.BatchDurationMs ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture);
    private string CoalescedPatchText => (latestBatchResult?.Metrics.CoalescedPatchCount ?? latestDiagnostics?.CoalescedPatchCount ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture);
    private string DroppedMotionText => (latestBatchResult?.Metrics.DroppedDuplicateMotionCount ?? latestDiagnostics?.DroppedDuplicateMotionCount ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture);
    private string StageCountText => (latestBatchResult?.Metrics.StageCount ?? latestDiagnostics?.BatchStageCount ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture);
    private string ObjectCountText => (latestDiagnostics?.ObjectCount ?? latestSnapshot?.ObjectCount ?? scene.Objects.Count).ToString(System.Globalization.CultureInfo.InvariantCulture);
    private string RenderCountText => (latestDiagnostics?.RenderCount ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture);
    private string ActiveMotionCountText => (latestDiagnostics?.ActiveMotionCount ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture);
    private string CacheHitText => (latestDiagnostics?.AssetCacheHitCount ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture);
    private string CacheMissText => (latestDiagnostics?.AssetCacheMissCount ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture);
    private string LinkRebuildText => (latestDiagnostics?.LinkGeometryRebuildCount ?? latestSnapshot?.LinkGeometryRebuildCount ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture);

    private async Task HandleRuntimeReady(WebGlRuntimeReadyEventArgs args)
    {
        latestDiagnostics = args.Diagnostics;
        runtimeError = string.Empty;
        statusText = "ready";
        statusTone = "success";
        statusDetail = $"Runtime ready with {args.Diagnostics.ObjectCount} objects.";
        if (!proofStarted)
        {
            proofStarted = true;
            await RunProofBatchAsync();
        }
    }

    private Task HandleRuntimeError(WebGlRuntimeErrorEventArgs args)
    {
        statusText = "error";
        statusTone = "danger";
        runtimeError = args.Detail;
        statusDetail = "Runtime error.";
        return InvokeAsync(StateHasChanged);
    }

    private async Task RunProofBatchAsync()
    {
        if (sceneView is null)
        {
            return;
        }

        statusText = "running";
        statusTone = "info";
        statusDetail = "Applying 202-command batch.";
        WebGlSceneCommandBatch batch = CreateCommandBatch();
        latestBatchResult = await sceneView.ApplyCommandBatchAsync(batch);
        await Task.Delay(250);
        latestDiagnostics = await sceneView.GetDiagnosticsAsync();
        latestSnapshot = await sceneView.GetProofSnapshotAsync();
        statusText = latestBatchResult?.Success != false ? "proof ready" : "batch error";
        statusTone = latestBatchResult?.Success != false ? "success" : "danger";
        statusDetail = $"Applied {BatchCommandCountText} commands with {StageCountText} queued stage(s).";
        await InvokeAsync(StateHasChanged);
    }

    private async Task CaptureProofAsync()
    {
        if (sceneView is null)
        {
            return;
        }

        latestDiagnostics = await sceneView.GetDiagnosticsAsync();
        latestSnapshot = await sceneView.GetProofSnapshotAsync();
    }

    private static WebGlSceneModel CreateScene()
    {
        List<WebGlSceneObject> objects = [];
        List<WebGlSceneLink> links = [];
        const int objectCount = 100;
        for (var index = 0; index < objectCount; index++)
        {
            int row = index / 10;
            int column = index % 10;
            string id = $"object.{index:000}";
            objects.Add(new()
            {
                Id = id,
                Kind = "proof-object",
                Family = "performance-proof",
                Title = $"Object {index:000}",
                Position = new WebGlVector3((column - 4.5) * 0.9, 0, (row - 4.5) * 0.9),
                Size = new WebGlVector3(0.28, 0.8, 0.28),
                Color = index % 3 == 0 ? "#38bdf8" : index % 3 == 1 ? "#f59e0b" : "#22c55e",
                Symbols =
                [
                    new()
                    {
                        Id = $"symbol.{id}",
                        SemanticKind = index % 2 == 0 ? "status" : "motion",
                        Tooltip = index.ToString("000", System.Globalization.CultureInfo.InvariantCulture),
                        Color = "#e0f2fe",
                        Scale = 0.35
                    }
                ],
                Metadata =
                {
                    ["proofRole"] = "performance-object"
                }
            });

            if (index > 0)
            {
                links.Add(new()
                {
                    Id = $"link.{index - 1:000}.{index:000}",
                    SourceObjectId = $"object.{index - 1:000}",
                    TargetObjectId = id,
                    Kind = "proof-link",
                    Color = "#64748b",
                    Width = 0.75
                });
            }
        }

        return new()
        {
            SceneId = "webgl-performance-proof",
            Title = "WebGL Performance Proof",
            Description = "Desktop-only generic command batch proof scene.",
            Environment = new()
            {
                BackgroundColor = "#111827",
                GroundColor = "#1f2937",
                GridColor = "#64748b",
                GroundSize = 14,
                GridDivisions = 14,
                AmbientLightIntensity = 0.72,
                DirectionalLightIntensity = 1.15,
                FogEnabled = false
            },
            Camera = new()
            {
                ViewMode = WebGlSceneViewModes.Isometric,
                ProjectionMode = WebGlSceneProjectionModes.Perspective,
                Distance = 13,
                Azimuth = -0.76,
                Polar = 0.92,
                Target = new WebGlVector3(0, 0, 0)
            },
            UiState = new()
            {
                ShowGrid = true,
                ShowGround = true,
                ShowSymbols = true,
                DeterministicMode = true,
                ActiveAssetProfile = WebGlAssetQualityProfiles.Primitive
            },
            Interaction = new()
            {
                AllowHover = true,
                AllowClickSelection = true,
                AllowCameraOrbit = true,
                AllowCameraPan = true,
                AllowCameraZoom = true,
                FitViewOnCreate = true
            },
            Objects = objects,
            Links = links,
            Metadata =
            {
                ["proof"] = "sb18-performance",
                ["proofSubbundle"] = "SB18",
                ["desktopOnly"] = "true"
            }
        };
    }

    private static WebGlSceneCommandBatch CreateCommandBatch()
    {
        var firstPatch = new WebGlScenePatch
        {
            SceneId = "webgl-performance-proof",
            BaseRevision = 0,
            NextRevision = 1,
            Metadata =
            {
                ["proof"] = "sb18-performance-color",
                ["proofSubbundle"] = "SB18"
            }
        };
        var secondPatch = new WebGlScenePatch
        {
            SceneId = "webgl-performance-proof",
            BaseRevision = 0,
            NextRevision = 1,
            Metadata =
            {
                ["proof"] = "sb18-performance-size",
                ["proofSubbundle"] = "SB18"
            }
        };

        List<WebGlObjectMotionCommand> motions = [];
        for (var index = 0; index < 100; index++)
        {
            string id = $"object.{index:000}";
            int row = index / 10;
            int column = index % 10;
            firstPatch.ObjectPatches.Add(new()
            {
                ObjectId = id,
                Color = index % 2 == 0 ? "#0ea5e9" : "#f97316"
            });
            secondPatch.ObjectPatches.Add(new()
            {
                ObjectId = id,
                Size = new WebGlVector3(0.32, 0.9, 0.32)
            });
            motions.Add(Motion(id, column, row, 0.25));
            motions.Add(Motion(id, column, row, -0.25));
        }

        return new()
        {
            BatchId = "proof.performance.100-objects.200-actions",
            OrderingMode = BatchOrderingMode.CoalesceIndependent,
            Patches = [firstPatch, secondPatch],
            Motions = motions,
            Metadata =
            {
                ["proof"] = "sb18-performance",
                ["proofSubbundle"] = "SB18",
                ["objectCount"] = "100",
                ["motionCommandCount"] = "200",
                ["desktopOnly"] = "true"
            }
        };
    }

    private static WebGlObjectMotionCommand Motion(string id, int column, int row, double offset)
        => new()
        {
            MotionId = $"motion.{id}.{offset:0.00}",
            ObjectId = id,
            TargetPosition = new WebGlVector3((column - 4.5) * 0.9 + offset, 0, (row - 4.5) * 0.9 - offset),
            DurationSeconds = 0.4,
            Easing = WebGlMotionEasings.EaseInOut,
            QueueMode = WebGlMotionQueueModes.Replace,
            Metadata =
            {
                ["proof"] = "sb18-performance-motion",
                ["proofSubbundle"] = "SB18"
            }
        };
}
