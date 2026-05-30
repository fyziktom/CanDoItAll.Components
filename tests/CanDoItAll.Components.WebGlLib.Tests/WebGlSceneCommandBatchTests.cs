using CanDoItAll.Components.WebGlLib;
using System.Text.Json;

namespace CanDoItAll.Components.WebGlLib.Tests;

public sealed class WebGlSceneCommandBatchTests
{
    [Fact]
    public void Batch_normalizer_keeps_many_motions_in_one_host_interop_step()
    {
        var batch = new WebGlSceneCommandBatch
        {
            BatchId = "frame.large",
            Motions =
            [
                .. Enumerable.Range(0, 100).Select(index => new WebGlObjectMotionCommand
                {
                    MotionId = $"motion.{index}",
                    ObjectId = $"object.{index}",
                    TargetPosition = new WebGlVector3(index, 0, 0)
                })
            ]
        };

        WebGlSceneCommandBatchNormalizationResult result = WebGlSceneCommandBatchNormalizer.Normalize(batch);

        Assert.Equal(100, result.Batch.Motions.Count);
        Assert.Equal(100, result.Metrics.BatchCommandCount);
        Assert.Equal(100, result.Metrics.CommandCountBeforeNormalization);
        Assert.Equal(100, result.Metrics.CommandCountAfterNormalization);
        Assert.Equal(1, result.Metrics.EstimatedHostInteropCallCount);
        Assert.Equal(99, result.Metrics.InteropCallsAvoided);
        Assert.Equal("1", result.Batch.Metadata["estimatedHostInteropCallCount"]);
    }

    [Fact]
    public void Batch_normalizer_coalesces_object_patches_and_drops_duplicate_motions()
    {
        var batch = new WebGlSceneCommandBatch
        {
            BatchId = "frame.coalesce",
            Patches =
            [
                new() { ObjectPatches = [new() { ObjectId = "object.a", Position = new WebGlVector3(1, 0, 0) }] },
                new() { ObjectPatches = [new() { ObjectId = "object.a", Color = "#22c55e" }] }
            ],
            Motions =
            [
                new() { MotionId = "motion.a.1", ObjectId = "object.a", TargetPosition = new WebGlVector3(1, 0, 0) },
                new() { MotionId = "motion.a.2", ObjectId = "object.a", TargetPosition = new WebGlVector3(2, 0, 0) }
            ]
        };

        WebGlSceneCommandBatchNormalizationResult result = WebGlSceneCommandBatchNormalizer.Normalize(batch);

        WebGlSceneObjectPatch objectPatch = result.Batch.Patches.Single().ObjectPatches.Single();
        Assert.Equal(new WebGlVector3(1, 0, 0), objectPatch.Position);
        Assert.Equal("#22c55e", objectPatch.Color);
        Assert.Single(result.Batch.Motions);
        Assert.Equal(1, result.Metrics.CoalescedPatchCount);
        Assert.Equal(1, result.Metrics.DroppedDuplicateMotionCount);
        Assert.Equal(4, result.Metrics.CommandCountBeforeNormalization);
        Assert.Equal(2, result.Metrics.CommandCountAfterNormalization);
        Assert.Contains(result.Warnings, warning => warning.Contains("Duplicate motion", StringComparison.Ordinal));
    }

    [Fact]
    public void Batch_normalizer_preserves_sequential_duplicate_motions()
    {
        var batch = new WebGlSceneCommandBatch
        {
            BatchId = "frame.sequence",
            OrderingMode = BatchOrderingMode.Sequential,
            Motions =
            [
                new() { MotionId = "motion.a.out", ObjectId = "object.a", TargetPosition = new WebGlVector3(1, 0, 0) },
                new() { MotionId = "motion.a.back", ObjectId = "object.a", TargetPosition = new WebGlVector3(0, 0, 0) }
            ]
        };

        WebGlSceneCommandBatchNormalizationResult result = WebGlSceneCommandBatchNormalizer.Normalize(batch);

        Assert.Equal(2, result.Batch.Motions.Count);
        Assert.Equal(0, result.Metrics.DroppedDuplicateMotionCount);
        Assert.Equal(1, result.Metrics.PreservedOrderedDuplicateMotionCount);
        Assert.Empty(result.Warnings);
        Assert.Equal("Sequential", result.Batch.Metadata["orderingMode"]);
    }

    [Fact]
    public void Batch_normalizer_does_not_coalesce_add_remove_patch_sets()
    {
        var batch = new WebGlSceneCommandBatch
        {
            BatchId = "frame.ordered-patches",
            Patches =
            [
                new() { RemoveObjectIds = ["object.a"] },
                new() { ObjectPatches = [new() { ObjectId = "object.a", Color = "#22c55e" }] }
            ]
        };

        WebGlSceneCommandBatchNormalizationResult result = WebGlSceneCommandBatchNormalizer.Normalize(batch);

        Assert.Equal(2, result.Batch.Patches.Count);
        Assert.Equal(0, result.Metrics.CoalescedPatchCount);
        Assert.Contains(result.Warnings, warning => warning.Contains("ordered semantics", StringComparison.Ordinal));
    }

    [Fact]
    public void Batch_normalizer_preserves_stage_boundaries_and_duplicate_motions_across_stages()
    {
        var batch = new WebGlSceneCommandBatch
        {
            BatchId = "frame.staged",
            OrderingMode = BatchOrderingMode.Sequential,
            Stages =
            [
                new()
                {
                    StageId = "move.out",
                    OrderingMode = BatchOrderingMode.Sequential,
                    Motions =
                    [
                        new() { MotionId = "motion.out", ObjectId = "actor", TargetPosition = new WebGlVector3(4, 0, 0) }
                    ]
                },
                new()
                {
                    StageId = "show.symbol",
                    OrderingMode = BatchOrderingMode.Sequential,
                    Patches =
                    [
                        new() { ObjectPatches = [new() { ObjectId = "actor", Symbols = [new() { Id = "admin", SemanticKind = "document" }] }] }
                    ]
                },
                new()
                {
                    StageId = "move.back",
                    OrderingMode = BatchOrderingMode.Sequential,
                    Motions =
                    [
                        new() { MotionId = "motion.back", ObjectId = "actor", TargetPosition = new WebGlVector3(0, 0, 0) }
                    ]
                }
            ]
        };

        WebGlSceneCommandBatchNormalizationResult result = WebGlSceneCommandBatchNormalizer.Normalize(batch);

        Assert.Equal(3, result.Batch.Stages.Count);
        Assert.Empty(result.Batch.Motions);
        Assert.Equal(0, result.Metrics.DroppedDuplicateMotionCount);
        Assert.Equal(0, result.Metrics.PreservedOrderedDuplicateMotionCount);
        Assert.Equal([1, 0, 1], result.Batch.Stages.Select(stage => stage.Motions.Count).ToArray());
        Assert.Equal("move.out", result.Batch.Stages[0].StageId);
        Assert.Equal("move.back", result.Batch.Stages[2].StageId);
    }

    [Fact]
    public void Batch_normalizer_respects_explicit_batching_policy()
    {
        var batch = new WebGlSceneCommandBatch
        {
            BatchId = "frame.policy",
            BatchingPolicy = WebGlSceneBatchingPolicies.PreserveOrder,
            Motions =
            [
                new() { MotionId = "motion.a.1", ObjectId = "object.a", TargetPosition = new WebGlVector3(1, 0, 0) },
                new() { MotionId = "motion.a.2", ObjectId = "object.a", TargetPosition = new WebGlVector3(2, 0, 0) }
            ]
        };

        WebGlSceneCommandBatchNormalizationResult result = WebGlSceneCommandBatchNormalizer.Normalize(batch);

        Assert.Equal(2, result.Batch.Motions.Count);
        Assert.Equal(WebGlSceneBatchingPolicies.PreserveOrder, result.Batch.BatchingPolicy);
        Assert.Equal(1, result.Metrics.PreservedOrderedDuplicateMotionCount);
        Assert.Equal("1", result.Batch.Metadata["preservedOrderedDuplicateMotionCount"]);
    }

    [Theory]
    [InlineData("coalesce-patch-duplicate-motion.json")]
    [InlineData("ordered-stages.json")]
    public void Batch_normalizer_matches_shared_fixture_expectations(string fixtureFile)
    {
        using JsonDocument document = JsonDocument.Parse(File.ReadAllText(Path.Combine("..", "..", "..", "..", "..", "tools", "webgllib", "command-batch-fixtures", fixtureFile)));
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };
        WebGlSceneCommandBatch batch = document.RootElement.GetProperty("input").Deserialize<WebGlSceneCommandBatch>(options)!;
        JsonElement expected = document.RootElement.GetProperty("expected");

        WebGlSceneCommandBatchNormalizationResult result = WebGlSceneCommandBatchNormalizer.Normalize(batch);

        Assert.Equal(expected.GetProperty("batchCommandCount").GetInt32(), result.Metrics.BatchCommandCount);
        Assert.Equal(expected.GetProperty("commandCountBeforeNormalization").GetInt32(), result.Metrics.CommandCountBeforeNormalization);
        Assert.Equal(expected.GetProperty("commandCountAfterNormalization").GetInt32(), result.Metrics.CommandCountAfterNormalization);
        Assert.Equal(expected.GetProperty("stageCount").GetInt32(), result.Metrics.StageCount);
        Assert.Equal(expected.GetProperty("patchCount").GetInt32(), result.Batch.Patches.Count);
        Assert.Equal(expected.GetProperty("motionCount").GetInt32(), result.Batch.Motions.Count);
        Assert.Equal(expected.GetProperty("coalescedPatchCount").GetInt32(), result.Metrics.CoalescedPatchCount);
        Assert.Equal(expected.GetProperty("droppedDuplicateMotionCount").GetInt32(), result.Metrics.DroppedDuplicateMotionCount);
        Assert.Equal(expected.GetProperty("preservedOrderedDuplicateMotionCount").GetInt32(), result.Metrics.PreservedOrderedDuplicateMotionCount);
        Assert.Equal(expected.GetProperty("interopCallsAvoided").GetInt32(), result.Metrics.InteropCallsAvoided);
        Assert.Equal(
            expected.GetProperty("stageSummaries").EnumerateArray().Select(item => item.GetProperty("stageId").GetString()).ToArray(),
            result.Batch.Stages.Select(stage => stage.StageId).ToArray());
    }
}
