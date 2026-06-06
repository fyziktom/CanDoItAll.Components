using System.Reflection;
using CanDoItAll.Components.WebGlLib;
using Microsoft.JSInterop;

namespace CanDoItAll.Components.WebGlLib.Tests;

public sealed class WebGlSceneViewExternalImportLifecycleTests
{
    [Fact]
    public async Task Document_import_protects_runtime_scene_from_stale_parameter_update()
    {
        var jsRuntime = new RecordingJsRuntime();
        var view = new TestableWebGlSceneView(jsRuntime);
        var initialScene = Scene("scene.initial", "object.initial", revision: 0);
        var initialOptions = new WebGlRuntimeOptions { RuntimeKey = "initial" };
        var importedDocument = new WebGlSceneDocument
        {
            Scene = Scene("scene.imported", "object.imported", revision: 1),
            RuntimeOptions = new WebGlRuntimeOptions { RuntimeKey = "imported" }
        };
        var staleParameterScene = Scene("scene.initial", "object.initial.stale", revision: 2);
        var staleParameterOptions = new WebGlRuntimeOptions { RuntimeKey = "stale-parent" };

        await view.RenderWithParametersAsync(initialScene, initialOptions, firstRender: true);
        WebGlSceneCommandResult? importResult = await view.ImportSceneDocumentDetailedAsync(importedDocument);
        await view.RenderWithParametersAsync(staleParameterScene, staleParameterOptions, firstRender: false);

        Assert.NotNull(importResult);
        Assert.True(importResult.Success);
        Assert.Single(jsRuntime.Invocations, static invocation => invocation.Identifier == "CanDoItAll.webglScene.create");
        Assert.Single(jsRuntime.Invocations, static invocation => invocation.Identifier == "CanDoItAll.webglScene.importSceneDetailed");
        Assert.DoesNotContain(jsRuntime.Invocations, static invocation => invocation.Identifier == "CanDoItAll.webglScene.update");

        await view.RenderWithParametersAsync(Scene("scene.intentional", "object.intentional", revision: 3), new WebGlRuntimeOptions { RuntimeKey = "intentional" }, firstRender: false);

        Assert.Single(jsRuntime.Invocations, static invocation => invocation.Identifier == "CanDoItAll.webglScene.update");
    }

    [Fact]
    public async Task Large_scene_parameter_lifecycle_uses_revision_and_runtime_key_instead_of_full_payload()
    {
        var jsRuntime = new RecordingJsRuntime();
        var view = new TestableWebGlSceneView(jsRuntime);
        WebGlSceneModel largeScene = LargeScene(objectCount: 1000, revision: 7);
        var runtimeOptions = new WebGlRuntimeOptions
        {
            RuntimeKey = "large-scene-budget-v1",
            RuntimeBudget = WebGlRuntimeBudgetProfiles.Scene1000Plus()
        };

        await view.RenderWithParametersAsync(largeScene, runtimeOptions, firstRender: true);
        largeScene.Objects[0].Color = "#f97316";
        await view.RenderWithParametersAsync(largeScene, runtimeOptions, firstRender: false);

        Assert.DoesNotContain(jsRuntime.Invocations, static invocation => invocation.Identifier == "CanDoItAll.webglScene.update");

        largeScene.Revision++;
        await view.RenderWithParametersAsync(largeScene, runtimeOptions, firstRender: false);

        Assert.Single(jsRuntime.Invocations, static invocation => invocation.Identifier == "CanDoItAll.webglScene.update");
    }

    [Fact]
    public async Task Runtime_stop_methods_call_public_scene_facade()
    {
        var jsRuntime = new RecordingJsRuntime();
        var view = new TestableWebGlSceneView(jsRuntime);

        await view.RenderWithParametersAsync(Scene("scene.stop", "object.stop", revision: 1), new WebGlRuntimeOptions(), firstRender: true);
        WebGlSceneCommandResult? stop = await view.StopRuntimeActivityAsync("pause");
        WebGlSceneCommandResult? cancelStages = await view.CancelCommandStagesAsync("cancel");

        Assert.NotNull(stop);
        Assert.True(stop.Success);
        Assert.Equal("runtime-stop", stop.CommandKind);
        Assert.NotNull(cancelStages);
        Assert.True(cancelStages.Success);
        Assert.Equal("command-stage-cancel", cancelStages.CommandKind);
        Assert.Contains(jsRuntime.Invocations, static invocation => invocation.Identifier == "CanDoItAll.webglScene.stopRuntimeActivity");
        Assert.Contains(jsRuntime.Invocations, static invocation => invocation.Identifier == "CanDoItAll.webglScene.cancelCommandStages");
        Assert.Contains(jsRuntime.Invocations, invocation =>
            invocation.Identifier == "CanDoItAll.webglScene.stopRuntimeActivity" &&
            invocation.Arguments.Length > 1 &&
            invocation.Arguments[1] is string reason &&
            reason == "pause");
    }

    [Fact]
    public async Task Runtime_stop_wait_for_idle_fails_closed_when_idle_proof_times_out()
    {
        var jsRuntime = new RecordingJsRuntime
        {
            IdleResult = new()
            {
                Success = false,
                Idle = false,
                TimedOut = true,
                Reason = "pause",
                TimeoutMs = 25,
                PollIntervalMs = 5,
                ElapsedMs = 26,
                Blockers = ["motion:active:1", "command-stage:queued:1"],
                Diagnostics = new() { ActiveMotionCount = 1, QueuedCommandStageCount = 1 }
            }
        };
        var view = new TestableWebGlSceneView(jsRuntime);

        await view.RenderWithParametersAsync(Scene("scene.stop", "object.stop", revision: 1), new WebGlRuntimeOptions(), firstRender: true);
        WebGlSceneCommandResult? stop = await view.StopRuntimeActivityAsync("pause", waitForIdle: true, timeoutMs: 25, pollIntervalMs: 5);

        Assert.NotNull(stop);
        Assert.False(stop.Success);
        Assert.False(stop.Succeeded);
        Assert.Equal(WebGlSceneCommandLifecycleStates.Failed, stop.LifecycleState);
        Assert.Equal("False", stop.Metadata["runtimeIdle"]);
        Assert.Equal("True", stop.Metadata["runtimeIdleRequired"]);
        Assert.Contains("Runtime idle proof failed", stop.Errors.Single());
        Assert.Contains(jsRuntime.Invocations, static invocation => invocation.Identifier == "CanDoItAll.webglScene.waitForRuntimeIdle");
    }

    [Fact]
    public async Task Apply_command_batch_and_wait_sends_hard_idle_proof_option_by_default()
    {
        var jsRuntime = new RecordingJsRuntime();
        var view = new TestableWebGlSceneView(jsRuntime);

        await view.RenderWithParametersAsync(Scene("scene.batch", "object.batch", revision: 1), new WebGlRuntimeOptions(), firstRender: true);
        WebGlSceneCommandBatchResult? result = await view.ApplyCommandBatchAndWaitAsync(
            new WebGlSceneCommandBatch { BatchId = "batch.proof" },
            timeoutMs: 25,
            pollIntervalMs: 5,
            reason: "SB03-runtime-idle");

        Assert.NotNull(result);
        Invocation invocation = Assert.Single(jsRuntime.Invocations, static item => item.Identifier == "CanDoItAll.webglScene.applyCommandBatchAndWait");
        Assert.Equal("batch.proof", Assert.IsType<WebGlSceneCommandBatch>(invocation.Arguments[1]).BatchId);
        object options = invocation.Arguments[2]!;
        Assert.True(GetProperty<bool>(options, "requireRuntimeIdle"));
        Assert.True(GetProperty<bool>(options, "hardFailOnIdleTimeout"));
        Assert.Equal("SB03-runtime-idle", GetProperty<string>(options, "reason"));
        Assert.Equal(WebGlRuntimeIdlePolicyModes.VisualStrict, GetProperty<string>(options, "policyMode"));
    }


    private static WebGlSceneModel Scene(string sceneId, string objectId, int revision)
        => new()
        {
            SceneId = sceneId,
            Revision = revision,
            Objects =
            {
                new()
                {
                    Id = objectId,
                    Kind = "generic",
                    Position = new WebGlVector3(revision, 0, 0)
                }
            }
        };

    private static WebGlSceneModel LargeScene(int objectCount, int revision)
        => new()
        {
            SceneId = "scene.large.lifecycle",
            Revision = revision,
            UiState = new() { Revision = revision },
            Objects =
            [
                .. Enumerable.Range(0, objectCount).Select(index => new WebGlSceneObject
                {
                    Id = $"object.{index:0000}",
                    Kind = "generic",
                    Position = new WebGlVector3(index % 50, 0, index / 50),
                    Color = "#38bdf8"
                })
            ]
        };

    private sealed class TestableWebGlSceneView : WebGlSceneView
    {
        private readonly IJSRuntime jsRuntime;

        public TestableWebGlSceneView(IJSRuntime jsRuntime)
            => this.jsRuntime = jsRuntime;

        public async Task RenderWithParametersAsync(WebGlSceneModel scene, WebGlRuntimeOptions options, bool firstRender)
        {
            Scene = scene;
            Options = options;
            SetJsRuntime();
            OnParametersSet();
            await OnAfterRenderAsync(firstRender);
        }

        private void SetJsRuntime()
        {
            PropertyInfo? property = typeof(WebGlSceneView).GetProperty("JsRuntime", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            Assert.NotNull(property);
            property.SetValue(this, jsRuntime);
        }
    }

    private sealed class RecordingJsRuntime : IJSRuntime
    {
        public List<Invocation> Invocations { get; } = [];
        public WebGlRuntimeIdleResult IdleResult { get; set; } = new()
        {
            Success = true,
            Idle = true,
            TimedOut = false,
            Reason = "runtime-idle",
            Diagnostics = new()
        };

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
            => InvokeAsync<TValue>(identifier, CancellationToken.None, args);

        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            Invocations.Add(new(identifier, args ?? []));
            object? result = identifier switch
            {
                "CanDoItAll.webglScene.create" => true,
                "CanDoItAll.webglScene.update" => true,
                "CanDoItAll.webglScene.importSceneDetailed" => new WebGlSceneCommandResult
                {
                    Success = true,
                    CommandKind = "import-scene",
                    SceneId = ExtractSceneId(args)
                },
                "CanDoItAll.webglScene.importScene" => true,
                "CanDoItAll.webglScene.stopRuntimeActivity" => new WebGlSceneCommandResult
                {
                    Success = true,
                    CommandKind = "runtime-stop",
                    CommandId = ExtractReason(args)
                },
                "CanDoItAll.webglScene.waitForRuntimeIdle" => IdleResult,
                "CanDoItAll.webglScene.applyCommandBatchAndWait" => new WebGlSceneCommandBatchResult
                {
                    Success = true,
                    CommandKind = "command-batch",
                    CommandId = args is { Length: > 1 } && args[1] is WebGlSceneCommandBatch batch ? batch.BatchId : string.Empty,
                    Settled = true,
                    LifecycleState = WebGlSceneCommandLifecycleStates.Settled
                },
                "CanDoItAll.webglScene.cancelCommandStages" => new WebGlSceneCommandResult
                {
                    Success = true,
                    CommandKind = "command-stage-cancel",
                    CommandId = ExtractReason(args)
                },
                _ => throw new InvalidOperationException($"Unexpected JS invocation '{identifier}'.")
            };

            return ValueTask.FromResult((TValue)result);
        }

        private static string ExtractSceneId(object?[]? args)
            => args is { Length: > 1 } && args[1] is WebGlSceneModel scene ? scene.SceneId : string.Empty;

        private static string ExtractReason(object?[]? args)
            => args is { Length: > 1 } && args[1] is string reason ? reason : string.Empty;
    }

    private static T GetProperty<T>(object instance, string propertyName)
    {
        PropertyInfo? property = instance.GetType().GetProperty(propertyName);
        Assert.NotNull(property);
        return Assert.IsType<T>(property.GetValue(instance));
    }

    private sealed record Invocation(string Identifier, object?[] Arguments);
}
