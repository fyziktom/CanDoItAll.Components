namespace CanDoItAll.Components.WebGlLib;

public sealed class WebGlRuntimeIdleResult
{
    public bool Success { get; set; }

    public bool Idle { get; set; }

    public bool TimedOut { get; set; }

    public string Reason { get; set; } = string.Empty;

    public int TimeoutMs { get; set; }

    public int PollIntervalMs { get; set; }

    public int ElapsedMs { get; set; }

    public List<string> Blockers { get; set; } = [];

    public WebGlRuntimeDiagnostics? Diagnostics { get; set; }
}
