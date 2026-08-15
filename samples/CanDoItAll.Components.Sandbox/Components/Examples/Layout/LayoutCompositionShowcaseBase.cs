using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace CanDoItAll.Components.Sandbox.Components.Examples.Layout;

public abstract class LayoutCompositionShowcaseBase : ComponentBase
{
    protected static readonly IReadOnlyList<string> ModeOptions = ["All modes", "Interval speed", "Harmony", "Sight reading"];
    protected static readonly IReadOnlyList<int> WindowOptions = [7, 14, 30, 90, 180];
    protected const string HeroSurfaceClass = "w-full min-w-0 rounded-[1.75rem] border border-slate-200/80 bg-[linear-gradient(135deg,rgba(255,255,255,0.98)_0%,rgba(244,249,255,0.98)_56%,rgba(220,243,255,0.96)_100%)] p-5 shadow-[0_24px_60px_-34px_rgba(14,116,144,0.3)]";

    protected string selectedMode = ModeOptions[0];
    protected int selectedWindowDays = 30;
    protected DateTimeOffset lastUpdated = new(2026, 4, 4, 11, 9, 0, TimeSpan.FromHours(-4));
    protected string HeroEyebrow => "Practice analytics composition";
    protected string HeroTitle => "Switch the active practice mode or time window without losing scan rhythm.";
    protected string HeroDescription => "Longer explanatory copy should wrap cleanly while the filters, pills, and actions stay aligned.";
    protected string SelectedModeLabel => "All note-identification modes";
    protected string WindowLabel => $"{selectedWindowDays}-day window";
    protected string FreshnessLabel => $"Updated {lastUpdated.ToString("M/d/yyyy h:mm tt", CultureInfo.InvariantCulture)}";
    protected bool ActionsDisabled => false;

    protected Task OnModeChanged(object value) { selectedMode = value?.ToString() ?? selectedMode; return Task.CompletedTask; }
    protected Task OnWindowChanged(object value) { if (value is int days) selectedWindowDays = days; else if (int.TryParse(value?.ToString(), out var parsedDays)) selectedWindowDays = parsedDays; return Task.CompletedTask; }
    protected Task HandleRefreshAsync() { lastUpdated = lastUpdated.AddMinutes(12); return Task.CompletedTask; }
    protected Task HandleResetAsync() { if (!ActionsDisabled) { selectedMode = ModeOptions[0]; selectedWindowDays = 30; lastUpdated = new(2026, 4, 4, 11, 9, 0, TimeSpan.FromHours(-4)); } return Task.CompletedTask; }
}
