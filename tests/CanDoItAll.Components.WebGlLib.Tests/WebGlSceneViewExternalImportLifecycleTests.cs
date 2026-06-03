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
                _ => throw new InvalidOperationException($"Unexpected JS invocation '{identifier}'.")
            };

            return ValueTask.FromResult((TValue)result);
        }

        private static string ExtractSceneId(object?[]? args)
            => args is { Length: > 1 } && args[1] is WebGlSceneModel scene ? scene.SceneId : string.Empty;
    }

    private sealed record Invocation(string Identifier, object?[] Arguments);
}
