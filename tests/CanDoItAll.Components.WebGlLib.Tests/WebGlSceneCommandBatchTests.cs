using CanDoItAll.Components.WebGlLib;

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
        Assert.Equal(1, result.Metrics.EstimatedHostInteropCallCount);
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
}
