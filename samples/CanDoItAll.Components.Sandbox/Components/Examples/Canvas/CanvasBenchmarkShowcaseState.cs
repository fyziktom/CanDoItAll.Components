namespace CanDoItAll.Components.Sandbox.Components.Examples.Canvas;

/// <summary>
/// Keeps the two benchmark showcase blocks in a page-local, shared interaction.
/// </summary>
public sealed class CanvasBenchmarkShowcaseState
{
    public string SelectedTierKey { get; private set; } = CanvasBenchmarkSamples.DefaultTierKey;

    public bool IsRunning { get; private set; }

    public string? LastMeasuredLabel { get; private set; }

    public CanvasBenchmarkRunResult? Result { get; private set; }

    public event Action? Changed;

    public event Func<Task<bool>>? RunRequested;

    public void SelectTier(string key)
    {
        if (string.Equals(SelectedTierKey, key, StringComparison.Ordinal))
        {
            return;
        }

        SelectedTierKey = key;
        Changed?.Invoke();
    }

    public async Task RunAsync()
    {
        if (IsRunning || RunRequested is null)
        {
            return;
        }

        IsRunning = true;
        Changed?.Invoke();
        try
        {
            if (await RunRequested.Invoke())
            {
                LastMeasuredLabel = $"Measured {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm} UTC";
            }
        }
        finally
        {
            IsRunning = false;
            Changed?.Invoke();
        }
    }

    public void SetResult(CanvasBenchmarkRunResult result)
    {
        Result = result;
        Changed?.Invoke();
    }
}

public sealed class CanvasBenchmarkRunResult
{
    public List<CanvasBenchmarkMeasurement> Tiers { get; set; } = [];
    public string Recommendation { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}

public sealed class CanvasBenchmarkMeasurement
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int NodeCount { get; set; }
    public int LinkCount { get; set; }
    public double RetainedAverageMs { get; set; }
    public double CanvasAverageMs { get; set; }
    public double ImprovementRatio { get; set; }
    public int RetainedDomNodeCount { get; set; }
}
