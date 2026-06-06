namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlRuntimeMotionQueueSnapshot
{
    public string ObjectId { get; set; } = string.Empty;

    public List<string> QueuedMotionIds { get; set; } = [];
}

public sealed class WebGlRuntimeCommandStageQueueItem
{
    public string BatchId { get; set; } = string.Empty;

    public string StageId { get; set; } = string.Empty;

    public string BarrierPolicy { get; set; } = string.Empty;

    public double WaitSeconds { get; set; }

    public string BarrierEventId { get; set; } = string.Empty;
}

public sealed class WebGlRuntimeCommandStageResult
{
    public string ResultId { get; set; } = string.Empty;

    public string BatchId { get; set; } = string.Empty;

    public string StageId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string BarrierPolicy { get; set; } = string.Empty;

    public string Error { get; set; } = string.Empty;
}

public sealed class WebGlRuntimeCommandStageJournalEntry
{
    public int Sequence { get; set; }

    public long TimestampMs { get; set; }

    public string EventKind { get; set; } = string.Empty;

    public string BatchId { get; set; } = string.Empty;

    public string StageId { get; set; } = string.Empty;

    public string ResultId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string BarrierPolicy { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
