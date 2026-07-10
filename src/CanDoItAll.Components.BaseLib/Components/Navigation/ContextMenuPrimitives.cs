namespace CanDoItAll.Components.BaseLib;

public sealed record ContextMenuItem
{
    public required string Id { get; init; }

    public required string Text { get; init; }

    public string Icon { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public bool Disabled { get; init; }

    public bool Danger { get; init; }

    public bool SeparatorBefore { get; init; }
}
