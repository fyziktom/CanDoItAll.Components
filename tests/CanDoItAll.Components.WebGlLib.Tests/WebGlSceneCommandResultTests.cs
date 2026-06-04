using System.Text.Json;
using CanDoItAll.Components.WebGlLib;

namespace CanDoItAll.Components.WebGlLib.Tests;

public sealed class WebGlSceneCommandResultTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void Command_result_round_trips_success_and_diagnostics()
    {
        var result = new WebGlSceneCommandResult
        {
            CommandId = "patch:1",
            CommandKind = "patch",
            SceneId = "scene",
            Success = true,
            Revision = 7,
            AffectedObjectIds = ["object.a", "object.b"],
            AffectedLinkIds = ["link.a"],
            Diagnostics =
            {
                ["renderCount"] = "3",
                ["failedCommandCount"] = "0"
            },
            Metadata =
            {
                ["source"] = "unit-test"
            }
        };

        var json = JsonSerializer.Serialize(result, JsonOptions);
        var roundTripped = JsonSerializer.Deserialize<WebGlSceneCommandResult>(json, JsonOptions);

        Assert.NotNull(roundTripped);
        Assert.True(roundTripped.Success);
        Assert.True(roundTripped.Succeeded);
        Assert.Equal("patch:1", roundTripped.CommandId);
        Assert.Equal("patch", roundTripped.CommandKind);
        Assert.Equal(["object.a", "object.b"], roundTripped.AffectedObjectIds);
        Assert.Equal("3", roundTripped.Diagnostics["renderCount"]);
        Assert.Equal("unit-test", roundTripped.Metadata["source"]);
    }

    [Fact]
    public void Command_result_round_trips_failed_alias_from_javascript_shape()
    {
        const string json = """
            {
              "commandId": "motion-cancel:missing",
              "success": false,
              "succeeded": false,
              "sceneId": "scene",
              "commandKind": "motion-cancel",
              "revision": 2,
              "errors": [ "Motion 'missing' was not found." ],
              "warnings": [],
              "affectedObjectIds": [],
              "affectedLinkIds": [],
              "diagnostics": { "failedCommandCount": "1" },
              "metadata": {}
            }
            """;

        var result = JsonSerializer.Deserialize<WebGlSceneCommandResult>(json, JsonOptions);

        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, error => error.Contains("not found", StringComparison.Ordinal));
        Assert.Equal("1", result.Diagnostics["failedCommandCount"]);
    }

    [Fact]
    public void Command_result_round_trips_lifecycle_state_from_javascript_shape()
    {
        const string json = """
            {
              "commandId": "run-frame:4",
              "success": true,
              "succeeded": true,
              "settled": false,
              "lifecycleState": "scheduled",
              "sceneId": "scene",
              "commandKind": "command-batch",
              "revision": 2,
              "errors": [],
              "warnings": [],
              "affectedObjectIds": [],
              "affectedLinkIds": [],
              "diagnostics": { "activeMotionCount": "24" },
              "metadata": { "lifecycleState": "scheduled", "settled": "false" }
            }
            """;

        var result = JsonSerializer.Deserialize<WebGlSceneCommandResult>(json, JsonOptions);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.False(result.Settled);
        Assert.Equal(WebGlSceneCommandLifecycleStates.Scheduled, result.LifecycleState);
        Assert.Equal("scheduled", result.Metadata["lifecycleState"]);
    }
}
